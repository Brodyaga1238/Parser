namespace Parser.Models.SitesForParsing;

public class Fackel : ISite
{
    public string SitemapUrl { get; } ="https://f-tk.ru/sitemap.xml";
    public  string CategoryXpath { get;  }="/html/body/div[position()>0]/div[1]/nav/div/ul/li[5]";
    public  string NameXpath { get;} ="//h1";
    public  string DescriptionXpath { get; } ="//html/body/div[position()>0]/div[2]/div[3]/div/div[2]/div/div[1]/div";
    public  string PriceXpath { get; }="//html/body/div[position()>0]/div[2]/div[2]/div[2]/div[2]/div[1]/div/b";
    public  string StockXpath { get; }="/html/body/div[position()>0]/div[2]/div[2]/div[2]/div[3]/table/tbody/tr[position()>0]/td[position() mod 3 = 0]";
    public  string CharacteristicsXpath { get;  }="/html/body/div[position()>0]/div[2]/div[2]/div[2]/div[3]/table/tbody/tr[position()>0]/td[1]";
    public  string IsProductPattern { get; }=@"https://www\.f-tk\.ru/catalog/item-" + @"\d+/";
    
}