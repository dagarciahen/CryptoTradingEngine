namespace StrategyMarker;

public enum SignalAction
{
    Hold,
    Buy,
    Sell
}

public sealed record Signal(
        string Symbol,
        SignalAction Action,
        decimal Price,
        DateTime TimesTamp);