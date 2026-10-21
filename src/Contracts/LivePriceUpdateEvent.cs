namespace Contracts;

public class LivePriceUpdateEvent
{
    public Coin Coin {get;}
    public int TimeStamp {get;set;}
    public decimal Quantity {get;set;}
}