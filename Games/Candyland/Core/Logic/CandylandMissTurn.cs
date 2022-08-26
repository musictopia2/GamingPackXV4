namespace Candyland.Core.Logic;
[SingletonGame]
public class CandylandMissTurn : IMissTurnClass<CandylandPlayerItem>
{
    private readonly IToast _toast;
    public CandylandMissTurn(IToast toast)
    {
        _toast = toast;
    }
    public Task PlayerMissTurnAsync(CandylandPlayerItem player)
    {
        _toast.ShowInfoToast($"{player.NickName} missed the turn for falling in a pit");
        return Task.CompletedTask;
    }
}