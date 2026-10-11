namespace Rook.Core.Logic;
[SingletonGame]
[AutoReset]
public class TrumpProcesses(RookVMData model, RookGameContainer gameContainer) : ITrumpProcesses
{
    public async Task ProcessTrumpAsync()
    {
        gameContainer.SaveRoot!.TrumpSuit = model!.ColorChosen;
        if (gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            model.Color1!.SelectSpecificItem(model.ColorChosen);
        }
        if (gameContainer.SingleInfo.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("colorselected", model.ColorChosen);
        }
        if (gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(1);
        }
        ResetTrumps();
        if (gameContainer.PlayerList!.Count < 3)
        {
            gameContainer.SaveRoot.GameStatus = EnumStatusList.SelectNest;
            gameContainer.SingleInfo.MainHandList.AddRange(gameContainer.SaveRoot.NestList);
            if (gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
                gameContainer.SortCards!.Invoke();
            }
        }
        else
        {
            gameContainer.SaveRoot.GameStatus = EnumStatusList.Normal;
        }
        if (gameContainer.PlayerList!.Count == 2)
        {
            model.Dummy1!.MakeAllKnown();
            model.Dummy1.HandList.Sort();
        }
        //if (_gameContainer.PlayerList.Count < 3)
        //{
        //    _model.Status = "Choose the 5 cards to get rid of";
        //}
        else
        {
            gameContainer.WhoTurn = gameContainer.WhoStarts; //iffy.
        }
        await gameContainer.StartNewTurnAsync!.Invoke();
    }
    public void ResetTrumps()
    {
        model.ColorChosen = EnumColorTypes.None; //i think
        model.Color1!.UnselectAll();
    }
}