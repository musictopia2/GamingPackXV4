namespace YaBlewIt.Core.Logic;
[SingletonGame]
[AutoReset]
public class ChooseColorProcesses : IChooseColorProcesses, IChoosePieceNM, ISerializable
{
    private readonly YaBlewItGameContainer _gameContainer;
    public ChooseColorProcesses(YaBlewItGameContainer gameContainer)
    {
        _gameContainer = gameContainer;
    }
    async Task IChoosePieceNM.ChoosePieceReceivedAsync(string data)
    {
        EnumColors color = await js1.DeserializeObjectAsync<EnumColors>(data);
        await ColorChosenAsync(color);
    }
    private async Task ColorChosenAsync(EnumColors color)
    {
        _gameContainer.SaveRoot.ProtectedColors.Add(color);
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
        {
            await _gameContainer.Network!.SendAllAsync(mm1.ChosenPiece, color);
        }
        _gameContainer.SaveRoot.GameStatus = EnumGameStatus.ResolveFire;
        await _gameContainer.ContinueTurnAsync!.Invoke(); //hopefully its this simple.
    }
    async Task IChooseColorProcesses.ColorChosenAsync(EnumColors color)
    {
        await ColorChosenAsync(color);
    }
}