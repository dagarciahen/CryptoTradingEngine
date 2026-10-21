namespace Contracts;

public class Order
{
    public long OrderId {get;}
    public decimal Price {get;}
    public OrderAction Action {get;}
    public long TimeStamp {get;}
    public long Quantity {get;}
    public string Uid {get;}


    public Order(long orderId, decimal price, OrderAction action, long quantity, string uid, long timestamp)
    {
        OrderId = orderId;
        Price = price;
        Action = action;
        Quantity = quantity;
        Uid = uid;
        TimeStamp = timestamp;
    }

    public override string ToString() => $"[{OrderId} Action: {Action}, Price {Price}, Quantity: {Quantity}]";
    public override int GetHashCode()
{
    var hash = new HashCode();
    hash.Add(OrderId);
    hash.Add(Action);
    hash.Add(Price);
    hash.Add(Quantity);
    hash.Add(Uid);

    return hash.ToHashCode();
}

public override bool Equals(object? obj)
    {
        if ( obj is not Order other)
            return false;
        
        return OrderId == other.OrderId
        && Action == other.Action
        && Price == other.Price
        && Quantity == other.Quantity
        && Uid == other.Uid;
    }

}

