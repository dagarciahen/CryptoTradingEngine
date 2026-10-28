using System;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;
using System.Net.Sockets;

namespace MarketDataIngestion;

public class BinanceStreamClient
{
	private readonly BinanceStreamApiSettings _streamSettings;
	public BinanceStreamClient(IOptions<BinanceStreamApiSettings> streamSettings)
	{
		_streamSettings = streamSettings.Value;
	}
//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-10.0&source=recommendations

	public async Task GetTradesAsync(string symbol)
	{
		var uri = new Uri($"{_streamSettings.EndPoint}/stream?streams={symbol.ToLower()}@trade");
		using var _socket = new ClientWebSocket();
		//1 trade 200 bytes
		byte[] buffer = new byte[4096];
		WebSocketReceiveResult receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		while (!receiveResult.CloseStatus.HasValue)
		{
			string json = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
			var message = JsonSerializer.Deserialize<StreamMessageResponse<Trade>>(json);
			if (message is null || message.Data is null)
				continue;
			Trade trade = message.Data;
			
			receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		}
		await _socket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
	}

	public async Task AveragePriceStreamAsync(string symbol, CancellationToken ct = default)
	{
		var uri = new Uri($"{_streamSettings.EndPoint}/ws/{symbol.ToLower()}@avgPrice");
		using var _socket = new ClientWebSocket();
		byte[] buffer = new byte[4096];
		WebSocketReceiveResult receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		while (_socket.State == WebSocketState.Open)
		{
			string json = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
			var message = JsonSerializer.Deserialize<AveragePriceResponse>(json);
			if (message is null)
				continue;
			receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		}
			await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", ct);
	}
		
	public async Task OrderBookStremAsync(string symbol, CancellationToken ct = default)
	{
	var uri = new Uri($"{_streamSettings.EndPoint}/ws/{symbol.ToLower()}@depth");
	using var _socket = new ClientWebSocket();
	await _socket.ConnectAsync(uri, ct);
	byte[] buffer = new byte[4096];
	WebSocketReceiveResult receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
	while (_socket.State == WebSocketState.Open)
	{
		string json = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
		var message = JsonSerializer.Deserialize<OrderBookStreamResponse>(json);
		if (message is null)
			continue;
		receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
	}
	
		await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", ct);
	}
	public async Task KlineStreamAsync(string symbol, string interval, CancellationToken ct = default)
	{
		var uri = new Uri($"{_streamSettings.EndPoint}/ws/{symbol.ToLower()}@kline_{interval.ToString()}");
		using var _socket = new ClientWebSocket();
	await _socket.ConnectAsync(uri, ct);
		byte[] buffer = new byte[4096];
		WebSocketReceiveResult receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		while (_socket.State == WebSocketState.Open)
		{
		string json = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
		var message = JsonSerializer.Deserialize<KlineDto>(json);
		if (message is null)
			continue;
		receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		}
		await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", ct);

	}

	public async Task SymbolTickerStreamAsync(string symbol, CancellationToken ct = default)
	{
		var uri = new Uri($"{_streamSettings.EndPoint}/ws/{symbol.ToLower()}@ticker");
		using var _socket = new ClientWebSocket();
	await _socket.ConnectAsync(uri, ct);
		byte[] buffer = new byte[4096];
		WebSocketReceiveResult receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		while (_socket.State == WebSocketState.Open)
		{
			string json = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
			var message = JsonSerializer.Deserialize<IndividualSymbolStream>(json);
			if (message is null)
				continue;
			receiveResult = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
		}
		await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", ct);
	}
}
