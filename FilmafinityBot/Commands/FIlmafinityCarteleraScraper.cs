using System;
using System.Collections.Generic;
using IronWebScraper;
using Telegram.Bot.Types;

public class FilmaffinityCarteleraScraper : WebScraper
{
    private const string titleName = "div.movie-poster";
    private int maxCarteleraFilms = 6;

    private const string linkFilmafinity = "https://www.filmaffinity.com/es/cat_new_th_es.html";

    private Action<List<Film>, Update> callBack;

    private Update update;

    public void Awake(Update update, Action<List<Film>, Update> action)
    {
        this.update = update;
        callBack += action;
        Start();
    }

    public override void Init()
    {
        this.LoggingLevel = WebScraper.LogLevel.All;
        this.Request(linkFilmafinity, Parse);
    }

    public override void Parse(Response response)
    {
        List<Film> films = new List<Film>();

        HtmlNode[] titles = response.Css(titleName);

        int lenght = titles.Length > maxCarteleraFilms ? maxCarteleraFilms : titles.Length;

        for (int i = 0; i < lenght; i++)
        {
            Film newFilm = new Film
            {
                title = titles[i].TextContentClean,
                link = titles[i].Css("a")[0].Attributes["href"]
            };

            if (newFilm.title.Contains("estreno"))
                newFilm.title = newFilm.title.Replace("estreno", (i + 1).ToString());

            films.Insert(films.Count, newFilm);
        }

        if (callBack != null)
            callBack.Invoke(films, update);
    }
}