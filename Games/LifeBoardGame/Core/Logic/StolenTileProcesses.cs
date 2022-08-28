namespace LifeBoardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class StolenTileProcesses : IStolenTileProcesses
{
    private readonly LifeBoardGameVMData _model;
    private readonly LifeBoardGameGameContainer _gameContainer;
    public StolenTileProcesses(LifeBoardGameVMData model, LifeBoardGameGameContainer gameContainer)
    {
        _model = model;
        _gameContainer = gameContainer;
    }
    public async Task ComputerStealTileAsync()
    {
        int firstItem = _model!.PlayerPicker!.ItemToChoose();
        _model.PlayerChosen = _model.PlayerPicker.GetText(firstItem); //i think
        await TilesStolenAsync(_model.PlayerChosen);
    }
    public void LoadOtherPlayerTiles()
    {
        _model.Instructions = "Choose a player to steal a life tile from";
        var tempList = _gameContainer.PlayerList!.AllPlayersExceptForCurrent();
        tempList.RemoveAllOnly(items => items.LastMove == EnumFinal.CountrySideAcres || items.TilesCollected == 0);
        var finList = tempList.Select(items => items.NickName).ToBasicList();
        _gameContainer.AddPlayerChoices(finList);
    }
    public async Task TilesStolenAsync(string player)
    {
        if (_gameContainer.CanSendMessage())
        {
            await _gameContainer.Network!.SendAllAsync("stole", player);
        }
        _model.PlayerPicker.ShowOnlyOneSelectedItem(player);
        if (_gameContainer.Test.NoAnimations == false)
        {
            await _gameContainer.Delay.DelaySeconds(1);
        }
        var thisPlayer = _gameContainer.PlayerList![player];
        thisPlayer.TilesCollected--;
        LifeBoardGameGameContainer.ObtainLife(_gameContainer.SingleInfo!);
        if (_gameContainer.SaveRoot!.WasMarried)
        {
            _gameContainer.GameStatus = EnumWhatStatus.NeedToSpin;
        }
        else
        {
            _gameContainer.GameStatus = EnumWhatStatus.NeedToEndTurn;
        }
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
}