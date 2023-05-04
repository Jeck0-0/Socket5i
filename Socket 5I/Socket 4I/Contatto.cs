using System.Collections.Generic;

namespace Socket_5I
{
    public class Contatto
    {
        public string Ip;
        public int Port;
        public string Nome;
        public List<Messaggio> Chat;

        public Contatto(string ip, int port, string nome)
        {
            Ip = ip;
            Port = port;
            Nome = nome;
            Chat = new List<Messaggio>();
        }

        public void AggiungiMessaggio(Contatto sender, string msg)
        {
            Chat.Add(new Messaggio(sender, msg));
        }

        public override string ToString()
        {
            return $"[{Ip}:{Port}] {Nome} ({Chat.Count})";
        }
    }
}
