namespace PickelCardGame.Core.Logic;
[SingletonGame]
public class PickelDelegates
{
    public Func<Task>? LoadBiddingAsync { get; set; }
    public Func<Task>? CloseBiddingAsync { get; set; }
}