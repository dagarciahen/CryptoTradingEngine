namespace MarketDataIngestion;

//records


public record BookLevel(decimal price, decimal quantity);


public record ExchangeInfoResponse(List<SymbolInfo> symbols);

public record SymbolInfo(string symbol, SymbolStatus status);
public record AveragePrice(int intervalMins, decimal price, int closeTime);
public record AveragePriceStream(string eventType, string EventTime, string symbol, int avgPriceInterval, decimal averagePrice, int lastTradeTime);

//classes
public class BinanceStreamResult
{
    public string Id {get; set;}
    public int Status {get;set;}
    public List<Trade> Result {get;set;}
    public List<RateLimit> RateLimits {get; set;}
 
}
public class Trade
{
    public string Id {get;set;}
    public string Price {get;set;}
    public string Qty {get;set;}
    public int Time {get;set;}
    public bool IsBuyerMarker {get;set;}
    public bool IsBestMatch {get;set;}
}

public class RateLimit
{
    public string RateLimitType {get;set;}
    public string Interval {get;set;}
    public string IntervalNum {get;set;}
    public int Limit {get;set;}
    public int Count {get;set;}

}

public class OrderBook
{
	public int LastUpdateId {get;set;}
	public List<BookLevel> Bids {get;set;} = [];
	public List<BookLevel> Asks {get;set;} = [];
}

////responses models
public class AveragePriceResponse
{
	public int Mins {get;set;}
	public string Price {get;set;}
	public int CloseTime {get;set;}
}

public class OrderBookResponse
{
	public int LastUpdateId {get;set;}
	public List<List<string>> Bids {get;set;} = [];
	public List<List<string>> Asks {get;set;} = [];
}

public class StreamMessageResponse<T>
{
	public string Stream {get; set;}
	public T Data {get; set;}
}

public class OrderStreamResponse
{
	public string EventType {get;set;}
	public int EventTime {get;set;}
	public string Symbol {get;set;}
	public string FirstIdEvent {get;set;}




public enum SymbolStatus
{
	TRADING,
	HALT,
	BREAK
}


