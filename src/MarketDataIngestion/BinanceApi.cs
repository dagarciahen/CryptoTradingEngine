using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using MarketDataIngestion;


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
		_apiSettings = _apiSettings.Value;
		_httpClient = client;
		_jsonOptions = new  JsonSerializerOptions(JsonSerializerOptions.Web)
		{
			Converters = new JsonStringEnumConverter()
		}
	};

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



public async Task<List<string>> GetAllSymbolsAsync(CancellatonToken ct = default)
{

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
	foreach(var bid in bidRes)
	{
		bids.Add(new BookLevel(decimal.Parse(bid[0]), decimal.Parse(bid[1])));
	}
	var asks = new List<BookLevel>();
	foreach(var ask in askRes)
	{
		asks.Add(new BookLevel(decimal.Parse(bid[0]), decimal.Parse(bid[1])));
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
}






