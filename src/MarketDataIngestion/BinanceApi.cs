using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using MarketDataIngestion;
using System.Globalization;


namespace MarketDataIngetsion;

public class BinanceApi
{
	private readonly HttpClient _httpClient;
	private readonly BinanceApiSettings _apiSettings;
	private readonly JsonSerializerOptions _jsonOptions;
	private readonly HashSet<string> _validSymbols = new();
	private readonly int _limitentriesbook = 100;

	public BinanceApi(HttpClient client, IOptions<BinanceApiSettings> apiSettings)
	{
		_apiSettings = apiSettings.Value;
		_httpClient = client;
		_jsonOptions = new  JsonSerializerOptions(JsonSerializerOptions.Web);
	}

	public async Task<bool> ValidSymbol(string symbol, CancellationToken ct = default)
	{
		var url = $"{_apiSettings.EndPoint}/exchangeInfo";
		if (_validSymbols.Contains(symbol))
			return true;
		var response = await _httpClient.GetFromJsonAsync<ExchangeInfoResponse>(url, _jsonOptions, ct);
		if (response is null)
			return false;
		foreach(var s in response.symbols)
		{
			if (s.symbol == symbol && s.status == SymbolStatus.TRADING)
			{
				_validSymbols.Add(symbol);
				return true;
			}
		}
		return false;
	}



public async Task<OrderBook> GetOrderBookAsync(string symbol, int limitentriesbook, CancellationToken ct = default)
{
	var url = $"{_apiSettings.EndPoint}/depth?symbol={symbol}&limit={limitentriesbook}&symbolStatus=TRADING";
	var response = await _httpClient.GetFromJsonAsync<OrderBookResponse>(url, _jsonOptions, ct);
	var orderBook = new OrderBook();
	int LastUpdateId = response.LastUpdateId;
	List<List<string>> bidsRes = response.Bids;
	List<List<string>> asksRes = response.Asks;
	var bids = new List<BookLevel>();
	foreach(var bid in bidsRes)
	{
		bids.Add(new BookLevel(decimal.Parse(bid[0]), decimal.Parse(bid[1])));
	}
	var asks = new List<BookLevel>();
	foreach(var ask in asksRes)
	{
		asks.Add(new BookLevel(decimal.Parse(ask[0]), decimal.Parse(ask[1])));
	}
	orderBook.LastUpdateId = LastUpdateId;
	orderBook.Bids = bids;
	orderBook.Asks = asks;

	return orderBook;
	

}

public async Task<AveragePrice> GetAveragePriceAsync(string symbol, CancellationToken ct = default)
{
	var url = $"{_apiSettings.EndPoint}/avgPrice?symbol={symbol}";
	var response = await _httpClient.GetFromJsonAsync<AveragePriceResponse>(url, _jsonOptions, ct);
	var averagePrice = new AveragePrice(response.Mins, int.Parse(response.Price), response.CloseTime);
	return averagePrice;
	
}
public async Task<List<KlinePayloadDto>> GetKlineAsync(string symbol, KlineInterval interval, DateTime? startTime, DateTime? endTime, int limit, CancellationToken ct = default)
	{
		long startime = startTime.HasValue ? new DateTimeOffset(startTime.Value).ToUnixTimeMilliseconds() : 0;
		long endtime = endTime.HasValue ? new DateTimeOffset(endTime.Value).ToUnixTimeMilliseconds() : 0;

		var url = $"{_apiSettings.EndPoint}/api/v3/klines?symbol={symbol}&interval={IntervalConverter.ToBinance(interval)}";
		var response = await _httpClient.GetFromJsonAsync<List<List<JsonElement>>>(url, _jsonOptions, ct);
		var result = new List<KlinePayloadDto>();
		foreach( var k in response)
			result.Add(new KlinePayloadDto(
				StartTime: k[0].GetInt64(),
				CloseTime: k[6].GetInt64(),
				Symbol: symbol,
				Interval: IntervalConverter.ToBinance(interval),
				Open: decimal.Parse(k[1].GetString()!, CultureInfo.InvariantCulture),
				Close: decimal.Parse(k[4].GetString()!, CultureInfo.InvariantCulture),
				High: decimal.Parse(k[2].GetString()!, CultureInfo.InvariantCulture),
				Low: decimal.Parse(k[3].GetString()!, CultureInfo.InvariantCulture),
				Volume: decimal.Parse(k[5].GetString()!, CultureInfo.InvariantCulture),
				NumberOfTrades: k[8].GetInt32(),
				IsClosed: true,
				QuoteVolume:decimal.Parse(k[7].GetString()!, CultureInfo.InvariantCulture)
				));
		return result;

	}

public async Task<BookTicker> GetBookTickerAsync(string symbol, CancellationToken ct = default)
	{
		var url = $"{_apiSettings.EndPoint}/api/v3/ticker/bookTicker?symbol={symbol}";
		return await _httpClient.GetFromJsonAsync<BookTicker>(url, _jsonOptions, ct);
		
	}
public async Task<List<BookTicker>> GetBookTickersAsyncs(IEnumerable<string> symbols, SymbolStatus symbolStatus, CancellationToken ct = default)
	{
		var parts = new List<string>();
		foreach (var s in symbols)
		{
			parts.Add("\"" + s + "\"");
		}
		var jsonArray = "[" + string.Join(",", parts)+ "]";
		var encoded = jsonArray.Replace("[", "%5B").Replace("\"", "%22").Replace("]", "%5D");
		var url = $"{_apiSettings.EndPoint}/api/v3/ticker/bookTicker?=symbols={encoded}";
		return await _httpClient.GetFromJsonAsync<List<BookTicker>>(url, _jsonOptions, ct);
		
	}
public async Task<TwentyFourHoursPrice> GetDailyPrice(string symbol, CancellationToken ct = default)
	{
		
		var url = $"{_apiSettings.EndPoint}/api/v3/ticker/24h?symbol={symbol}";
		return await _httpClient.GetFromJsonAsync<TwentyFourHoursPrice>(url, _jsonOptions, ct);
		
	}

public async Task<List<TwentyFourHoursPrice>> GetMultipleDailyPricesAsync(IEnumerable<string> symbols, CancellationToken ct = default)
	{
		
		var jsonArray = JsonSerializer.Serialize(symbols);
		string encoded = Uri.EscapeDataString(jsonArray);
		var url = $"{_apiSettings.EndPoint}/api/v3/ticker/bookTicker?=symbols={encoded}";
		return await _httpClient.GetFromJsonAsync<List<TwentyFourHoursPrice>>(url, _jsonOptions, ct);

	}


}






