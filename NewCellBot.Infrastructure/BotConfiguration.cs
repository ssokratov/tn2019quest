namespace NewCellBot.Infrastructure
{
    public class BotConfiguration
    {
        public string BotToken { get; set; }

        public string Socks5Host { get; set; }

        public int Socks5Port { get; set; }
        public string Socks5Login { get; set; }
        public string Socks5Password { get; set; }
    }
}