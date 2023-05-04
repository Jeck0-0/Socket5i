namespace Socket_5I
{
    public class Messaggio
    {
        public readonly Contatto Sender;
        public readonly string Msg;

        public Messaggio(Contatto sender, string msg)
        {
            Sender = sender;
            Msg = msg;
        }

        public override string ToString()
        {
            return $"{Sender.Nome}: {Msg}";
        }
    }
}
