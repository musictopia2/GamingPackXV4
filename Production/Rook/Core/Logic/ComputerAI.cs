namespace Rook.Core.Logic;
public static class ComputerAI
{
    public static async Task CardToBidAsync(RookVMData model, IBidProcesses processes)
    {
        if (await processes.CanPassAsync() == false)
        {
            model.BidChosen = model.Bid1!.NumberToChoose();
            return;
        }
        model.BidChosen = model.Bid1!.NumberToChoose(false);
    }
    public static void ColorToCall(RookVMData model)
    {
        var thisColor = model.Color1!.ItemToChoose();
        model.ColorChosen = thisColor;
    }
    public static int CardToPlay(RookMainGameClass mainGame, RookVMData model)
    {
        DeckRegularDict<RookCardInformation> newList;
        var firstList = model.GetCurrentHandList();
        newList = firstList.Where(x => mainGame.IsValidMove(x.Deck)).ToRegularDeckDict();
        if (newList.Count == 0)
        {
            throw new CustomBasicException("There must be at least one card it can play for computer player");
        }
        return newList.GetRandomItem().Deck;
    }
    public static DeckRegularDict<RookCardInformation> CardsToRemove(RookMainGameClass mainGame)
    {
        return mainGame.SingleInfo!.MainHandList.GetRandomList(false, 5).ToRegularDeckDict();
    }
}