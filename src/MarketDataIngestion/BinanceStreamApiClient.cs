using System;
using System.Net;
using System.Net.Socket;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;

namespace MarketDataIngestion;

public class BinanceStreamClient
{
	private readonly BinanceStreamApiSettings _streamSettings;
	private readonly ClientWebSocket _socket = new();
	public BinanceStreamClient(IOptions<BinanceStreamApiSettings> streamSettings)
	{
		_streamSettings = streamSettings.Value;
	}
//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-10.0&source=recommendations

	public async Task GetTradesAsync(string symbol)
	{
		var uri = new Uri($"{_streamSettings.EndPoint}/stream?streams={symbol.ToLower()}@trade");
		await _socket.ConnectAsync(uri, CancellationToken.None);
		byte[] buffer = new byte[4096];
		WebSocketReceiveResult receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		while (!receiveResult.CloseStatus.HasValue)
		{
			string json = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
			var message = JsonSerializer.Deserialize<StreamMessageResponse<Trade>>(json);
			if (message is null || message.?Data is null)
				continue;
			Trade trade = message.Data;
			
			receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		}
		await _socket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
	}

	public async Task AveragePriceStreamAsync(string symbol)
	{
		var uri = new Uri($"{_streamSettings.EndPoint}/ws/{symbol.ToLower()}@avgPrice");
		await _socket.ConnectAsync(uri, CancellationToken.None);
		byte[] buffer = new byte[4096];
		var receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		await _socket.CloseAsync(receiveResult.
