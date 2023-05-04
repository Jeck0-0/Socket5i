using System;
using System.Collections.Generic;
using System.Linq;

namespace Socket_5I
{
    public class Rubrica
    {
        public List<Contatto> Contatti;

        public Rubrica() 
        {
            Contatti = new List<Contatto>();
        }


        public Contatto GetContatto(string ip, int porta)
        {
            return Contatti.FirstOrDefault(x => x.Ip == ip && x.Port == porta);
        }
        public Contatto GetContatto(string nome)
        {
            return Contatti.FirstOrDefault(x => x.Nome == nome);
        }

        public void AggiungiContatto(string ip, int porta, string nome)
        {
            //se esiste gia' mando un'Exception
            if (Contatti.Any(x => x.Ip == ip && x.Port == porta && x.Nome == nome))
                throw new Exception("Contatto gia' esistente");
            
            //se esiste gia' uno con stesso ip e porta modifico il nome
            Contatto modifica = GetContatto(ip, porta);
            if (modifica != null)
            {
                modifica.Nome = nome;
                return;
            }

            //se esiste gia' uno con lo stesso nome modifico ip e porta
            modifica = GetContatto(nome);
            if(modifica != null)
            {
                modifica.Ip = ip;
                modifica.Port = porta;
                return;
            }

            //altrimenti lo aggiungo
            Contatti.Add(new Contatto(ip, porta, nome));
        }
    }
}
