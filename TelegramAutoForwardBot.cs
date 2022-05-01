using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace CypherBot
{
    internal static class TelegramAutoForwardBot
    {
        public static CultureInfo ci = new CultureInfo("en-US");
        public static TelegramBotClient botClient = new TelegramBotClient(Settings.telegramBotToken);

        private static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;
            CultureInfo.DefaultThreadCurrentUICulture = ci;
            CultureInfo.CurrentCulture = ci;
            CultureInfo.CurrentUICulture = ci;

            User me = botClient.GetMeAsync().Result;

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
            botClient.StartReceiving(TelegramHandlers.HandleUpdateAsync,
                               TelegramHandlers.HandleErrorAsync,
                               receiverOptions,
                               cts.Token);

            Console.WriteLine($"Telegram Bot start listening for @{me.Username}");

            while (true)
            {
                Console.Read();
            }
        }
    }
}
