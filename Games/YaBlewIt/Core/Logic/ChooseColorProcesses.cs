namespace YaBlewIt.Core.Logic;
[SingletonGame]
[AutoReset]
public class ChooseColorProcesses(YaBlewItGameContainer gameContainer) : IChooseColorProcesses, IChoosePieceNM, ISerializable
{
    async Task IChoosePieceNM.ChoosePieceReceivedAsync(string data)
    {
        EnumColors color = await js1.DeserializeObjectAsync<EnumColors>(data);
        await ColorChosenAsync(color);
    }
    private async Task ColorChosenAsync(EnumColors color)
    {
        gameContainer.SaveRoot.ProtectedColors.Add(color);
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!) == true)
        {
            await gameContainer.Network!.SendAllAsync(mm1.ChosenPiece, color);
        }
        gameContainer.SaveRoot.GameStatus = EnumGameStatus.ResolveFire;
        await gameContainer.ContinueTurnAsync!.Invoke(); //hopefully its this simple.
    }
    async Task IChooseColorProcesses.ColorChosenAsync(EnumColors color)
    {
        await ColorChosenAsync(color);
    }
}