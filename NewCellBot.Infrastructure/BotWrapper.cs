using Microsoft.Extensions.Options;
using MihaZupan;
using Telegram.Bot;

namespace NewCellBot.Infrastructure
{
    public class BotWrapper
    {
        private readonly BotConfiguration _config;

        public BotWrapper(IOptions<BotConfiguration> config)
        {
            _config = config.Value;
            // use proxy if configured in appsettings.*.json
            Client = string.IsNullOrEmpty(_config.Socks5Host)
                ? new TelegramBotClient(_config.BotToken)
                : new TelegramBotClient(
                    _config.BotToken,
                    new HttpToSocks5Proxy(
                        _config.Socks5Host,
                        _config.Socks5Port,
                        _config.Socks5Login,
                        _config.Socks5Password));
        }

        public TelegramBotClient Client { get; }
    }
}