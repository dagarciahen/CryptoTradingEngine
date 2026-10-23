namespace MarketDataIngestion;

public class BinanceStreamApiSettings
{
    private readonly string Api = "BinanceStreamApi";
    public string BinanceStreamApiUrl {get; set;} = null!;
    public string EndPoint {get;} = null!;
    public int MaxAttempts = 10;
    

}