using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;


namespace MarketDataIngestion;

public interface IBinanceAPi
{
    
}

public class BinanceApi : IBinanceApi
{
    private readonly BinanceApiSettings _apiSettings;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly HttpClient _httpClient;

    public BinanceApi(HttpClient client, IOptions<BinanceApiSettings> apiSettings)
    {
        
        _apiSettings = apiSettings.Value; 
        _httpClient = client;
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters = {
                new TraderConvertor(),
                new JsonStringEnumConverter()
            }
        };
        
    }

    public async Task<List<string>> GetSymbolAsync(CancellationToken ct = default)
    {
        var url = $"{_apiSettings.EndPoint}exchangeInfo";
        var info = await _httpClient.GetFromJsonAsync<ExchangeInfo>(url, _jsonOptions, ct);

        var result = new List<string>();
        if (info is null)
            return result;

        foreach (var symbol in info.Symbols)
        {
            if (symbol.Status == SymbolStatus.TRADING)
                result.Add(symbol.Symbol);
        }
        return result;
    }
}