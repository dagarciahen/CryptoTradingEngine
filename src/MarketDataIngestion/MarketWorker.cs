namespace MarketDataIngestion;


public class MarkerWoker
{
	private readonly BinanceApi _binanceApi = new();
	private readonly BinanceStreamClient _streamClient = new();
	private readonly BinanceStreamApiSettings _settings = new();

	public async Task RunAsync(CancellationToken ct)
	{
		string symbol = ReadSymbolFromSettings();
		await ValidateSymbolAsync(symbol);
		await StartMarketDataStreamAsync(symbol, ct);
	}

	private string ReadSymbolFromSettings()
	{
		return _settings.Symbol;
	}
	private async Task ValidateSymbolAsync(string symbol)
	{
		var symbols = await _binanceApi.GetAllSymbolsAsync();
		if (!symbols.Contains(symbol))
		{
			throw new Exception($"Invalid symbol: {symbol}");
		}
	}
	private async Task StartMarketDataStreamAsync(string symbol, CancellationToken.None)
	{
		await _streamClient.GetTradesAsync(symbol);
	}
}

}



