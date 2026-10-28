using System.Text.Json.Serialization;

namespace MarketDataIngestion;

//records


public record BookLevel(decimal price, decimal quantity);


public record ExchangeInfoResponse(List<SymbolInfo> symbols);

public record SymbolInfo(string symbol, SymbolStatus status);
public record AveragePrice(int intervalMins, decimal price, int closeTime);
public record AveragePriceStream(string eventType, string EventTime, string symbol, int avgPriceInterval, decimal averagePrice, int lastTradeTime);
public record BookTicker(string symbol, decimal bidPrice, decimal bidQty, decimal askPrice, decimal askQty);
public record TwentyFourHoursPrice(string symbol, decimal priceChange, decimal priceChangePErcent, decimal weightedAvgPrice,
decimal lastQty, decimal bidQty, decimal askPrice, decimal openPrice, decimal highPrice, decimal lowPrice, decimal volume, decimal quoteVolume, 
long openTime, long closetime, int firstId, int lastId, int count);


public sealed record KlineDto(
	[property: JsonPropertyName("e")] string EventType,
	[property: JsonPropertyName("E")] long EventTime,
	[property: JsonPropertyName("s")] string Symbol, 
	[property: JsonPropertyName("k")] KlinePayloadDto Kline);

public sealed record KlinePayloadDto(
    [property: JsonPropertyName("t")] long StartTime,
    [property: JsonPropertyName("T")] long CloseTime,
    [property: JsonPropertyName("s")] string Symbol,
    [property: JsonPropertyName("i")] string Interval,
    [property: JsonPropertyName("o")] decimal Open,
    [property: JsonPropertyName("c")] decimal Close,
    [property: JsonPropertyName("h")] decimal High,
    [property: JsonPropertyName("l")] decimal Low,
    [property: JsonPropertyName("v")] decimal Volume,
    [property: JsonPropertyName("n")] int NumberOfTrades,
    [property: JsonPropertyName("x")] bool IsClosed,
    [property: JsonPropertyName("q")] decimal QuoteVolume);


public sealed record IndividualSymbolStream(
    [property: JsonPropertyName("e")] string EventType,
    [property: JsonPropertyName("E")] long EventTime,
    [property: JsonPropertyName("s")] string Symbol,
    [property: JsonPropertyName("p")] decimal PriceChange,
    [property: JsonPropertyName("P")] decimal PriceChangePercent,
    [property: JsonPropertyName("w")] decimal WeightedAvgPrice,
    [property: JsonPropertyName("x")] decimal FirstTradePrice,
    [property: JsonPropertyName("c")] decimal LastPrice,
    [property: JsonPropertyName("Q")] decimal LastQuantity,
    [property: JsonPropertyName("b")] decimal BestBidPrice,
    [property: JsonPropertyName("B")] decimal BestBidQuantity,
    [property: JsonPropertyName("a")] decimal BestAskPrice,
    [property: JsonPropertyName("A")] decimal BestAskQuantity,
    [property: JsonPropertyName("o")] decimal OpenPrice,
    [property: JsonPropertyName("h")] decimal HighPrice,
    [property: JsonPropertyName("l")] decimal LowPrice,
    [property: JsonPropertyName("v")] decimal TotalTradedBaseVolume,
    [property: JsonPropertyName("q")] decimal TotalTradedQuoteVolume,
    [property: JsonPropertyName("O")] long StatsOpenTime,
    [property: JsonPropertyName("C")] long StatsCloseTime,
    [property: JsonPropertyName("F")] long FirstTradeId,
    [property: JsonPropertyName("L")] long LastTradeId,
    [property: JsonPropertyName("n")] int TotalNumberOfTrades);

public sealed record WindowTicker(
    [property: JsonPropertyName("e")] string EventType,
    [property: JsonPropertyName("E")] long EventTime,
    [property: JsonPropertyName("s")] string Symbol,
    [property: JsonPropertyName("p")] decimal PriceChange,
    [property: JsonPropertyName("P")] decimal PriceChangePercent,
    [property: JsonPropertyName("o")] decimal OpenPrice,
    [property: JsonPropertyName("h")] decimal HighPrice,
    [property: JsonPropertyName("l")] decimal LowPrice,
    [property: JsonPropertyName("c")] decimal LastPrice,
    [property: JsonPropertyName("w")] decimal WeightedAvgPrice,
    [property: JsonPropertyName("v")] decimal TotalTradedBaseVolume,
    [property: JsonPropertyName("q")] decimal TotalTradedQuoteVolume,
    [property: JsonPropertyName("O")] long StatsOpenTime,
    [property: JsonPropertyName("C")] long StatsCloseTime,
    [property: JsonPropertyName("F")] long FirstTradeId,
    [property: JsonPropertyName("L")] long LastTradeId,
    [property: JsonPropertyName("n")] int TotalNumberOfTrades);



public sealed record BookTickerStream(
    [property: JsonPropertyName("u")] long UpdateId,
    [property: JsonPropertyName("s")] string Symbol,
    [property: JsonPropertyName("b")] decimal BestBidPrice,
    [property: JsonPropertyName("B")] decimal BestBidQty,
    [property: JsonPropertyName("a")] decimal BestAskPrice,
    [property: JsonPropertyName("A")] decimal BestAskQty);



//classes
//
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

public class MultipleBookerTicker

{
	public List<BookTicker> ManyBookTickers {get;set;} = [];
}

public class MultipleDailyResponse
{
    public List<TwentyFourHoursPrice> DailyPricesAll {get;set;} = [];
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

public class OrderBookStreamResponse
{
	public string EventType {get;set;}
	public int EventTyme {get;set;}
	public string Symbol {get;set;}
	public int FirstUpdateId {get;set;}
	public int FinalUpdateId {get;set;}

	List<BookLevel> Bids {get; set;}
	List<BookLevel> Asks {get;set;}
	
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
}




public enum SymbolStatus
{
	TRADING,
	HALT,
	BREAK
}

public enum KlineInterval
{
    OneSecond,
    OneMinute,
    ThreeMinutes,
    FiveMinutes,
    FifteenMinutes,
    ThirtyMinutes,
    OneHour,
    TwoHours,
    FourHours,
    SixHours,
    EightHours,
    TwelveHours,
    OneDay,
    ThreeDays,
    OneWeek,
    OneMonth
}


public static class IntervalConverter
{
	public static string ToBinance(KlineInterval interval) => interval switch
	{
		KlineInterval.OneSecond =>"1s",
		KlineInterval.OneMinute      => "1m",
        KlineInterval.ThreeMinutes   => "3m",
        KlineInterval.FiveMinutes    => "5m",
        KlineInterval.FifteenMinutes => "15m",
        KlineInterval.ThirtyMinutes  => "30m",
        KlineInterval.OneHour        => "1h",
        KlineInterval.TwoHours       => "2h",
        KlineInterval.FourHours      => "4h",
        KlineInterval.SixHours       => "6h",
        KlineInterval.EightHours     => "8h",
        KlineInterval.TwelveHours    => "12h",
        KlineInterval.OneDay         => "1d",
        KlineInterval.ThreeDays      => "3d",
        KlineInterval.OneWeek        => "1w",
        KlineInterval.OneMonth       => "1M",
		_ => throw new ArgumentOutOfRangeException(nameof(interval))
	};
}