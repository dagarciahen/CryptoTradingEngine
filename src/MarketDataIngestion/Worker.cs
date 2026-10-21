using Contracts.Events;
using Contracts.Interfaces;
using Infrastructure;

namespace MarketDataIngestion
{
    // Orchestrates the pipeline: read trades from Binance -> map to domain event ->
    // publish to RabbitMQ. Reconnection lives here: if the stream ends or throws, we
    // wait and start a new stream. The Worker knows nothing about JSON or websockets.
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ILivePricePublisher _publisher;
        private readonly IRabbitMQInfrastructure _rabbitmq;
        private readonly IBinanceStreamClient _streamClient;

        public Worker(
            ILogger<Worker> logger,
            ILivePricePublisher publisher,
            IRabbitMQInfrastructure rabbitmq,
            IBinanceStreamClient streamClient)
        {
            _logger = logger;
            _publisher = publisher;
            _rabbitmq = rabbitmq;
            _streamClient = streamClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _rabbitmq.InitializeAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await foreach (var trade in _streamClient.StreamTradesAsync(stoppingToken))
                    {
                        var priceEvent = MapToEvent(trade);
                        await _publisher.PublishAsync(priceEvent);

                        _logger.LogInformation(
                            "{Symbol} {Price} qty {Quantity} @ {Timestamp:HH:mm:ss.fff}",
                            priceEvent.Symbol, priceEvent.Price, priceEvent.Quantity, priceEvent.Timestamp);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Connection with Binance lost. Reconnecting in 5s");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        private static LivePriceUpdatedEvent MapToEvent(BinanceTrade trade) => new()
        {
            Symbol = trade.Symbol,
            Price = trade.Price,
            Quantity = trade.Quantity,
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(trade.TradeTime).UtcDateTime
        };
    }
}
          