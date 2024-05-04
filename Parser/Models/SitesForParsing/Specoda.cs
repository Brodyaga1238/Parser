namespace Parser.Models.SitesForParsing;

public class Specoda: ISite
{
    public  string SitemapUrl { get; } ="https://specoda.ru/index.php?route=extension/feed/yandex_sitemap";
    public  string CategoryXpath { get;  }="//li[@itemprop='itemListElement'][last()-1]";
    public  string NameXpath { get;} ="/html/body/div[position()>0]/div[1]/div/div[1]/div[1]/div[1]/h1/span";///html/body/div[1]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div[1]/p[1]/span
    public  string DescriptionXpath { get; } ="//div[@class=\"col-md-6\"]";
    public  string PriceXpath { get; }="/html/body/div[position()>0]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div[1]/p[1]/span";
    public  string StockXpath { get; }="/html/body/div[position()>0]/div[2]/div[2]/div[2]/div[3]/table/tbody/tr[position()>0]/td[position() mod 3 = 0]";
    public  string CharacteristicsXpathPar { get;  }="/html/body/div[position()>0]/div[1]/div/div[1]/div[2]/ul/li[1]";
    public  string CharacteristicsXpathParHelp { get;  }="/html/body/div[position()>0]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div[2]/div/div[2]/div";
    public  string CharacteristicsXpath { get; } = "/html/body/div[position()>0]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div[2]/div/div[1]/div/p/span";
    public string CharacteristicsXpathHelp { get; } = "/html/body/div[position()>0]/div[1]/div/div[1]/div[1]/div[2]/div[2]/div[2]/div/div[1]/div/p/text()";
    public string CharacteristicsColOrEqu { get; } = "/html/body/div[position()>0]/div[2]/div[2]/div[1]/div[3]/div";
    public string CharacteristicsColOrEquHelp { get; } = "/html/body/div[position()>0]/div[2]/div[2]/div[1]/div[3]/ul";
    public string CharacteristicsColOrEqu2 { get; } = "/html/body/div[position()>0]/div[2]/div[2]/div[1]/div[4]/div";
    public string CharacteristicsColOrEquHelp2 { get; } = "/html/body/div[position()>0]/div[2]/div[2]/div[1]/div[4]/ul";

    public string IsProductPattern { get; } =@"https:\/\/specoda\.ru\/.*\.html$"; //";
    public string ImageUrl { get; }
    public string BaseUrl { get; } = "https://www.f-tk.ru";
}