namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.Extensions;
public static class BasicGameProcesses
{
    public static async Task ProtectedGameOverNextAsync<P>(this IBasicGameProcesses<P> game)
        where P : class, IPlayerItem, new()
    {
        game.CurrentMod.NormalTurn = "None";
        if (game.BasicData!.MultiPlayer == true && game.BasicData.Client == true)
        {
            CommandContainer command = Resolver!.Resolve<CommandContainer>();
            command.ManuelFinish = true;
            command.IsExecuting = true;
            InProgressHelpers.MoveInProgress = false;
            game.Network!.IsEnabled = true;
            return;
        }
        await game.SendGameOverAsync(game.Error);
    }
    public static async Task RoundOverNextAsync<P>(this IBasicGameProcesses<P> game)
        where P : class, IPlayerItem, new()
    {
        CommandContainer command = Resolver!.Resolve<CommandContainer>();
        if (game.BasicData!.MultiPlayer == false || game.BasicData.Client == false)
        {
            game.CurrentMod.Status = "Goto the next round";
            await game.Aggregator.PublishAsync(new RoundOverEventModel());
        }
        else
        {

            game.CurrentMod.Status = "Waiting for host to goto the next round";
            game.Network!.IsEnabled = true;
            command.ManuelFinish = true;
            command.IsExecuting = true;
            InProgressHelpers.MoveInProgress = false;
        }
        command.UpdateAll();
    }
    public static void ShowConnected<P>(this IBasicGameProcesses<P> game)
        where P : class, IPlayerItem, new()
    {
        game.CurrentMod.Status = "Connected";
    }
}