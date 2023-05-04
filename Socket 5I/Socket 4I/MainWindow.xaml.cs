using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Socket_5I
{
    public partial class MainWindow : Window
    {
        private Socket m_socket = null;

        private Rubrica m_rubrica = null;
        private Contatto m_me = null;

        public MainWindow()
        {
            InitializeComponent();

            //Imposto la porta ad un valore casuale
            Random random = new Random();
            int port = random.Next(11000, 12000);
            
            //Inizializzazione della rubrica e del mio contatto
            m_rubrica = new Rubrica();
            m_me = new Contatto("127.0.0.1", port, "You");

            //Creazione socket
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            //Imposto il local endpoint e lo collego alla socket
            IPAddress local_address = IPAddress.Any;
            IPEndPoint local_endpoint = new IPEndPoint(local_address, port);
            m_socket.Bind(local_endpoint);

            //Avvio della Task che si occupa di ricevere messaggi
            Task.Run(WaitForMessage);

            lbl_Port.Content = m_me.ToString();
        }

        //Thread separato per attendere nuovi messaggi
        private Task WaitForMessage()
        {
            byte[] buffer = new byte[512];
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                int rec = m_socket.ReceiveFrom(buffer, ref remoteEndPoint);
                
                string msg = Encoding.UTF8.GetString(buffer, 0, rec);
                string ip = ((IPEndPoint)remoteEndPoint).Address.ToString();
                int porta = ((IPEndPoint)remoteEndPoint).Port;

                //Dispatcher.Invoke serve per modificare l'UI da un
                //thread secondario (non si potrebbe altrimenti)
                lbl_Port.Dispatcher.Invoke(() => RiceviMessaggio(ip, porta, msg));
            }
        }

        

        private void AggiungiContatto(string nome, string ip, int porta)
        {
            m_rubrica.AggiungiContatto(ip, porta, nome);
            AggiornaUI();
        }

        private void RiceviMessaggio(string ip, int porta, string msg)
        {
            try
            { 
                Contatto c = m_rubrica.GetContatto(ip, porta);

                //se proviene da un ip nuovo lo aggiungo ai contatti
                if (c == null)                 {
                    AggiungiContatto(ip + porta, ip, porta);
                    c = m_rubrica.GetContatto(ip + porta);
                }

                //aggiungo il messaggio ricevuto e aggiorno la UI
                c.AggiungiMessaggio(c, msg);
                AggiornaUI();
            }
            catch 
            {
                MessageBox.Show("Invalid message received");
            }
        }

        private void InviaMessaggio(Contatto contatto, string msg)
        {
            //definisco l'endpoint a cui mandare il messaggio
            IPAddress remote_address = IPAddress.Parse(contatto.Ip);
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, contatto.Port);
            byte[] messaggio = Encoding.UTF8.GetBytes(msg);

            //lo mando
            m_socket.SendTo(messaggio, remote_endpoint);

            //e visualizzo il messaggio mandato
            contatto.AggiungiMessaggio(m_me, msg);
            AggiornaUI();
        }


        private void AggiornaUI()
        {
            Contatto selectedContact = lbx_Rubrica.SelectedItem as Contatto;

            AggiornaChatUI(selectedContact);

            lbx_Rubrica.Items.Clear();
            m_rubrica.Contatti.ForEach(x => lbx_Rubrica.Items.Add(x));
            lbx_Rubrica.SelectedItem = selectedContact;
        }

        private void AggiornaChatUI(Contatto contatto)
        {
            lbx_Messaggi.Items.Clear();
            if (contatto != null) 
                contatto.Chat.ForEach(x => lbx_Messaggi.Items.Add(x));
        }


        //aggiungi contatto
        private void btnAggiungi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nome = txb_Nome.Text;
                if (string.IsNullOrEmpty(nome))
                    throw new Exception("Nome non valido");

                string portaTxt = txt_Porta.Text;
                if (!int.TryParse(portaTxt, out int porta))
                    throw new Exception("Porta non valida");

                string ip = txb_Ip.Text;
                if (string.IsNullOrEmpty(ip))
                    throw new Exception("Ip non valido");

                AggiungiContatto(nome, ip, porta);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //manda messaggio a tutti i contatti
        private void btnBroadcast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = txt_Messaggio.Text;
                if (string.IsNullOrEmpty(msg))
                    throw new Exception("scrivi qualcosa da inviare");

                foreach (Contatto c in m_rubrica.Contatti)
                    InviaMessaggio(c, msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //invia messaggio al contatto selezionato
        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string msg = txt_Messaggio.Text;
                if (string.IsNullOrEmpty(msg))
                    throw new Exception("scrivi qualcosa da inviare");

                Contatto contatto = lbx_Rubrica.SelectedItem as Contatto;
                if (contatto == null)
                    throw new Exception("Nessun contatto selezionato");

                InviaMessaggio(contatto, msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //aggiorna chat e informazioni del contatto selezionato
        private void lbx_Rubrica_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Contatto selectedContact = lbx_Rubrica.SelectedItem as Contatto;
            if (selectedContact != null)
            {
                lbl_Selected.Content = selectedContact;
                AggiornaChatUI(selectedContact);
                txt_Porta.Text = selectedContact.Port.ToString();
                txb_Ip.Text = selectedContact.Ip;
                txb_Nome.Text = selectedContact.Nome;
            }
            else
            {
                lbl_Selected.Content = string.Empty;
            }
        }
    }
}
