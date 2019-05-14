using System;
using System.Collections.Generic;
using IronWebScraper;
using Telegram.Bot.Types;

public class FilmaffinitySearchScraper : WebScraper
{
  private const string titleName = "div.mc-title";
  private const string linkName = "div.mc-poster";
  private const string calificationName = "div.mr-rating";

  private const string linkFilmafinity = "https://www.filmaffinity.com/es/search.php?stype=title&stext=";

  private string filmToSearch;
  private Action<List<Film>, Update> callBack;

  private Update update;

  public void Awake(string film, Update update, Action<List<Film>, Update> action)
  {
    filmToSearch = film;
    this.update = update;
    callBack += action;
    Start();
  }

  public override void Init()
  {
    this.LoggingLevel = WebScraper.LogLevel.All;
    this.Request(linkFilmafinity + filmToSearch, Parse);
  }

  public override void Parse(Response response)
  {
    List<Film> films = new List<Film>();

    if (string.Equals(response.FinalUrl, linkFilmafinity + filmToSearch) == true)
    {
      HtmlNode[] titles = response.Css(titleName);
      HtmlNode[] IDs = response.Css(linkName);
      HtmlNode[] califications = response.Css(calificationName);

      int lenght = titles.Length > 5 ? 5 : titles.Length;

      for (int i = 0; i < lenght; i++)
      {
        string[] divideCalification = { "-", "-" };

        if (califications[i].TextContentClean.Equals("--") == false)
          divideCalification = califications[i].TextContentClean.Split(" ");

        Film newFilm = new Film
        {
          title = titles[i].TextContentClean,
          link = IDs[i].Css("a")[0].Attributes["href"],
          rating = divideCalification[0],
          reviews = divideCalification[1]
        };

        films.Insert(films.Count, newFilm);
      }
    }
    else
    {
      HtmlNode[] titles = response.Css("#main-title");
      HtmlNode[] califications = response.Css("#rat-avg-container");

      string[] divideCalification = { "-", "-" };

      if (califications[0].TextContentClean.Equals("--") == false)
        divideCalification = califications[0].TextContentClean.Split(" ");

      Film newFilm = new Film
      {
        title = titles[0].TextContentClean,
        link = response.FinalUrl,
        rating = divideCalification[0],
        reviews = divideCalification[1]
      };

      films.Insert(films.Count, newFilm);
    }

    if (callBack != null)
      callBack.Invoke(films, update);
  }
}