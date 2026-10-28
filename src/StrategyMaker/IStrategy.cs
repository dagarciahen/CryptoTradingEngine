public interface IStrategy
{
    string Name {get;}
    
    Signal OnCandle(Can