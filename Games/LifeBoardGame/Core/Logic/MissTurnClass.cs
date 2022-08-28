namespace LifeBoardGame.Core.Logic;
[SingletonGame]
public class MissTurnClass : IMissTurnClass<LifeBoardGamePlayerItem>
{
    private readonly IToast _toast;
    public MissTurnClass(IToast toast)
    {
        _toast = toast;
    }
    Task IMissTurnClass<LifeBoardGamePlayerItem>.PlayerMissTurnAsync(LifeBoardGamePlayerItem player)
    {
        _toast.ShowInfoToast($"{player.NickName} has lost this turn");
        return Task.CompletedTask;
    }
}