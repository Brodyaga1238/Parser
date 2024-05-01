namespace Parser;

public interface ISite
{
    string SitemapUrl { get; }
    string CategoryXpath { get; }
    string NameXpath { get;} 
    string DescriptionXpath { get;} 
    string PriceXpath { get;}
    string StockXpath { get;}
    string CharacteristicsXpath { get;}
    string IsProductPattern { get;}
}