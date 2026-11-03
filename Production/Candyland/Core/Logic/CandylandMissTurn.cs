namespace Candyland.Core.Logic;
[SingletonGame]
public class CandylandMissTurn(IToast toast) : IMissTurnClass<CandylandPlayerItem>
{
    public Task PlayerMissTurnAsync(CandylandPlayerItem player)
    {
        toast.ShowInfoToast($"{player.NickName} missed the turn for falling in a pit");
        return Task.CompletedTask;
    }
}