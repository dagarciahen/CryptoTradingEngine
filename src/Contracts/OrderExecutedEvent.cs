namespace Contracts;

public record OrderExecutedEvent(long OrderID, string Uid, decimal Quantity, Coin coin, OrderType orderType, DateTime ExecutedAt);
