namespace MarketDataIngestion;

public class BinanceApiSettings
{
    public static readonly string Api = "BinanceApi";
    public string BinanceAPiUrl {get; set;} = null!;
    public string EndPoint {get;set;} = null!;
    public int MaxAttempts {get;set;} = 100;
    public string[] Symbols {get; set;} = [];



}