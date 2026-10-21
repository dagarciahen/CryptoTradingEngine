namespace Contracts;

public record OrderCreatedEvent(long OrderID, string Uid, decimal Quantity, Coin coin, DateTime CreatedAt, OrderType orderType);
