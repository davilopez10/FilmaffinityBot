using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

public class Film
{
    public string title;
    public string link;
    public string rating;
    public string reviews;
}

public class FilmafinityCommand
{
    private TelegramBotClient botClient;

    public void SearchFilm(TelegramBotClient botClient,string film, Update update)
    {
        this.botClient = botClient;

        film = film.Replace(" ", "+");
        var scraper = new FilmaffinitySearchScraper();
        scraper.Awake(film, update, OnFinishSearch);
    }

    public void GetCartelera(TelegramBotClient botClient, Update update)
    {
        this.botClient = botClient;

        var scraper = new FilmaffinityCarteleraScraper();
        scraper.Awake(update, OnGetCartelera);
    }

    private async void OnFinishSearch(List<Film> filmsFound, Update update)
    {
        if (filmsFound.Count > 0)
        {
            var results = new InlineQueryResultBase[filmsFound.Count];

            for (int i = 0; i < filmsFound.Count; i++)
            {
                InputTextMessageContent aux = new InputTextMessageContent(filmsFound[i].title);

                //@TODO color to rating
                // float rating = float.Parse(filmsFound[i].rating);
                //string emojiRating = rating > 6.9f ?TelegramEmoji.Emojis.Symbols.Other_Symbol.: rating > 4.9 ?:;

                results[i] = new InlineQueryResultArticle(i.ToString(),filmsFound[i].title, aux);

                results[i].ReplyMarkup = new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(filmsFound[i].rating, filmsFound[i].link));
            }

            await botClient.AnswerInlineQueryAsync(update.InlineQuery.Id, results);
        }
    }

    private async void OnGetCartelera(List<Film> films, Update update)
    {
        for (int i = 0; i < films.Count; i++)
        {
            await botClient.SendTextMessageAsync(
                chatId: update.Message.Chat,
                text: string.Format("{0}", films[i].title),
                replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                    "Open", films[i].link
                ))
            );
        }
    }
}