using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramEmoji;

public class SpoilerCommand
{
    public async Task Execute(TelegramBotClient botClient, Update update)
    {
        InputTextMessageContent aux = new InputTextMessageContent(
         Emojis.Symbols.Other_Symbol.double_exclamation_mark +
         Emojis.Symbols.Other_Symbol.double_exclamation_mark +
         Emojis.Symbols.Other_Symbol.double_exclamation_mark +
         Emojis.Symbols.Other_Symbol.double_exclamation_mark +
         Emojis.Symbols.Other_Symbol.double_exclamation_mark +
         Emojis.Symbols.Other_Symbol.double_exclamation_mark +
         Emojis.Symbols.Other_Symbol.double_exclamation_mark
         );

        var articleResult = new InlineQueryResultArticle("A", "Spoiler", aux);

        articleResult.ThumbUrl = "https://cdn.pixabay.com/photo/2012/04/12/22/25/warning-sign-30915_960_720.png";

        var results = new InlineQueryResultBase[] { articleResult };

        results[0].ReplyMarkup = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Spoiler", update.InlineQuery.Query.Remove(0, 9)));

        await botClient.AnswerInlineQueryAsync(update.InlineQuery.Id, results);
    }

}
