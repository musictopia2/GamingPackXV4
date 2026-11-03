namespace LifeBoardGame.Core.Logic;
[SingletonGame]
public class MissTurnClass(IToast toast) : IMissTurnClass<LifeBoardGamePlayerItem>
{
    Task IMissTurnClass<LifeBoardGamePlayerItem>.PlayerMissTurnAsync(LifeBoardGamePlayerItem player)
    {
        toast.ShowInfoToast($"{player.NickName} has lost this turn");
        return Task.CompletedTask;
    }
}