using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramEmoji;

namespace FilmafinityBot
{
    public static class BotCallbackFunction
    {
        [FunctionName("BotCallbackFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            string botkey = Environment.GetEnvironmentVariable("BOT_KEY");

            var botClient = new TelegramBotClient(botkey);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var update = JsonConvert.DeserializeObject<Update>(requestBody);

            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                    //Spoiler Callback
                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, update.CallbackQuery.Data, true, null, 0);

                    break;

                case UpdateType.Message:
                    if (update.Message.Text.StartsWith("!cartelera"))
                    {
                        FilmafinityCommand fmCommand = new FilmafinityCommand();
                        fmCommand.GetCartelera(botClient, update);
                    }
                    break;

                case UpdateType.InlineQuery:
                    if (update.InlineQuery.Query.StartsWith("!spoiler "))
                    {
                        SpoilerCommand spoilerCommand = new SpoilerCommand();
                        await spoilerCommand.Execute(botClient, update);
                    }
                    else if (update.InlineQuery.Query.StartsWith("!rating ") && update.InlineQuery.Query.Length > 14)
                    {
                        FilmafinityCommand fmCommand = new FilmafinityCommand();
                        string message = update.InlineQuery.Query.Remove(0, 8);
                        fmCommand.SearchFilm(botClient, message, update);
                    }

                    break;
            }

            return new OkResult();
        }
    }
}