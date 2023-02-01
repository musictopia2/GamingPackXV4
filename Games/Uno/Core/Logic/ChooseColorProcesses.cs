namespace Uno.Core.Logic;
[SingletonGame]
[AutoReset]
public class ChooseColorProcesses : IChooseColorProcesses, IChoosePieceNM, ISerializable
{
    private readonly UnoGameContainer _gameContainer;
    private readonly UnoVMData _model;
    private readonly UnoColorsDelegates _delegates;
    public ChooseColorProcesses(UnoGameContainer gameContainer, UnoVMData model, UnoColorsDelegates delegates)
    {
        _gameContainer = gameContainer;
        _model = model;
        _delegates = delegates;
    }
    async Task IChoosePieceNM.ChoosePieceReceivedAsync(string data)
    {
        EnumColorTypes color = await js1.DeserializeObjectAsync<EnumColorTypes>(data);
        await ColorChosenAsync(color);
    }
    private async Task ColorChosenAsync(EnumColorTypes color)
    {
        _gameContainer.SaveRoot!.GameStatus = EnumGameStatus.NormalPlay;
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
        {
            await _gameContainer.Network!.SendAllAsync(mm1.ChosenPiece, color);
        }
        if (color == EnumColorTypes.ZOther)
        {
            throw new CustomBasicException("Cannot choose wild as a color");
        }
        _gameContainer.SaveRoot.CurrentColor = color;
        UnoCardInformation thisCard = _model.Pile1.CurrentCard;
        thisCard.Color = color;
        _gameContainer.Command.UpdateAll();
        if (_delegates.CloseColorAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the closing of colors.  Rethink");
        }
        await _delegates.CloseColorAsync.Invoke();
        await _gameContainer.EndTurnAsync!.Invoke();
    }
    async Task IChooseColorProcesses.ColorChosenAsync(EnumColorTypes color)
    {
        await ColorChosenAsync(color);
    }
}