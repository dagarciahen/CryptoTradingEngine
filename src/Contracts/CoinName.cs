namespace Contracts;


public readonly record struct CoinName
{
    public string Value {get;}
    public CoinName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Coin name can not be empty", nameof(value));
        Value = value.Trim().ToUpperInvariant();
    }
    public override string ToString() => Value;
}