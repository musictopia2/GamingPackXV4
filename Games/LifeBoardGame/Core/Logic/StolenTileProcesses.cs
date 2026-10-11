namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class StolenTileProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer) : IStolenTileProcesses
{
    public async Task ComputerStealTileAsync()
    {
        int firstItem = model!.PlayerPicker!.ItemToChoose();
        model.PlayerChosen = model.PlayerPicker.GetText(firstItem); //i think
        await TilesStolenAsync(model.PlayerChosen);
    }
    public void LoadOtherPlayerTiles()
    {
        model.Instructions = "Choose a player to steal a life tile from";
        var tempList = gameContainer.PlayerList!.AllPlayersExceptForCurrent();
        tempList.RemoveAllOnly(items => items.LastMove == EnumFinal.CountrySideAcres || items.TilesCollected == 0);
        var finList = tempList.Select(items => items.NickName).ToBasicList();
        gameContainer.AddPlayerChoices(finList);
    }
    public async Task TilesStolenAsync(string player)
    {
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendAllAsync("stole", player);
        }
        model.PlayerPicker.ShowOnlyOneSelectedItem(player);
        if (gameContainer.Test.NoAnimations == false)
        {
            await gameContainer.Delay.DelaySeconds(1);
        }
        var thisPlayer = gameContainer.PlayerList![player];
        thisPlayer.TilesCollected--;
        LifeBoardGameGameContainer.ObtainLife(gameContainer.SingleInfo!);
        if (gameContainer.SaveRoot!.WasMarried)
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
        }
        else
        {
            gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
}