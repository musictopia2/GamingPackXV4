namespace CrazyEights.Core.Logic;
[SingletonGame]
[AutoReset]
public class SuitProcesses(CrazyEightsGameContainer gameContainer, CrazyEightsVMData model) : ISuitProcesses, IChoosePieceNM, ISerializable
{
    public async Task ChoosePieceReceivedAsync(string data)
    {
        EnumSuitList Suit = await js1.DeserializeObjectAsync<EnumSuitList>(data);
        await SuitChosenAsync(Suit);
    }
    public async Task SuitChosenAsync(EnumSuitList chosen)
    {
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        if (gameContainer.SingleInfo.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync(mm1.ChosenPiece, chosen);
        }
        gameContainer.SaveRoot!.CurrentSuit = chosen;
        gameContainer.SaveRoot.ChooseSuit = false;
        var thisCard = model!.Pile1!.CurrentCard;
        thisCard.DisplaySuit = chosen;
        await gameContainer.EndTurnAsync!.Invoke();
    }
}