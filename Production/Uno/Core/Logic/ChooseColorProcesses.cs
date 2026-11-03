namespace Uno.Core.Logic;
[SingletonGame]
[AutoReset]
public class ChooseColorProcesses(UnoGameContainer gameContainer, UnoVMData model, UnoColorsDelegates delegates) : IChooseColorProcesses, IChoosePieceNM, ISerializable
{
    async Task IChoosePieceNM.ChoosePieceReceivedAsync(string data)
    {
        EnumColorTypes color = await js1.DeserializeObjectAsync<EnumColorTypes>(data);
        await ColorChosenAsync(color);
    }
    private async Task ColorChosenAsync(EnumColorTypes color)
    {
        gameContainer.SaveRoot!.GameStatus = EnumGameStatus.NormalPlay;
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!) == true)
        {
            await gameContainer.Network!.SendAllAsync(mm1.ChosenPiece, color);
        }
        if (color == EnumColorTypes.ZOther)
        {
            throw new CustomBasicException("Cannot choose wild as a color");
        }
        gameContainer.SaveRoot.CurrentColor = color;
        UnoCardInformation thisCard = model.Pile1.CurrentCard;
        thisCard.Color = color;
        if (delegates.CloseColorAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the closing of colors.  Rethink");
        }
        await delegates.CloseColorAsync.Invoke();
        await gameContainer.EndTurnAsync!.Invoke();
    }
    async Task IChooseColorProcesses.ColorChosenAsync(EnumColorTypes color)
    {
        await ColorChosenAsync(color);
    }
}