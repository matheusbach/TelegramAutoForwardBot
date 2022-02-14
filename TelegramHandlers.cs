using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CypherBot
{
	public class TelegramHandlers
	{
		public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			var ErrorMessage = exception switch
			{
				ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => exception.ToString()
			};

			Console.WriteLine(ErrorMessage);
			return Task.CompletedTask;
		}

		public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			var handler = update.Type switch
			{
				UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
				UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),

				_ => UnknownUpdateHandlerAsync(botClient, update)
			};

			try
			{
				await handler;
			}
			catch (Exception exception)
			{
				await HandleErrorAsync(botClient, exception, cancellationToken);
			}
		}

		private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
		{
			Console.WriteLine($"Telegram Bot: {message.Type} chatId: {message.Chat.Id} messageId: {message.MessageId}" + (message.Type == MessageType.Text ? " text: " + message.Text : null));

			if (message.Type != MessageType.Document && message.Type != MessageType.Photo || message.Chat.Id != Settings.sourceChatId) { return; }

			Task<Message> action = ForwardMessage(botClient, message);

			if (action == null) { return; }

			Message sentMessage = await action;
			Console.WriteLine($"Mensagem encaminada com ID: {sentMessage.MessageId}");

			static async Task<Message> ForwardMessage(ITelegramBotClient botClient, Message message)
			{
				return await botClient.ForwardMessageAsync(chatId: Settings.destinationChatId, fromChatId: message.Chat.Id, messageId: message.MessageId);
			}
		}

		private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
		{
			Console.WriteLine($"Unknown update type: {update.Type}");
			return Task.CompletedTask;
		}
	}
}