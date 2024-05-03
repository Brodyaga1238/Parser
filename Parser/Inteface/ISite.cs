namespace Parser;

public interface ISite
{
    string SitemapUrl { get; }
    string CategoryXpath { get; }
    string NameXpath { get;} 
    string DescriptionXpath { get;} 
    string PriceXpath { get;}
    string StockXpath { get;}
    string CharacteristicsXpathPar { get;}
    string CharacteristicsXpathParHelp { get;}
    string CharacteristicsXpath{ get;}
    string CharacteristicsXpathHelp{ get;}
    string CharacteristicsColOrEqu { get; } 
    string CharacteristicsColOrEquHelp { get; } 
    string CharacteristicsColOrEqu2 { get; } 
    string CharacteristicsColOrEquHelp2 { get; } 
    string IsProductPattern { get;}
}