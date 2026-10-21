using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;


namespace MarketDataIngestion;

public interface IBinanceStreamClient
{
    
}

public class BinanceStreamClient : IBinanceStreamClient
{
    private readonly BinanceApiSettings _apiSettings;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Socket _socket;

    public BinanceStreamClient(IOptions<BinanceApiSettings> apiSettings)
    {
        
        _apiSettings = apiSettings.Value; 
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters = {new TraderConvertor()}
        }
        
    }
}