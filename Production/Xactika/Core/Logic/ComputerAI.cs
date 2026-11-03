namespace Xactika.Core.Logic;
public static class ComputerAI
{
    public static int HowManyToBid(XactikaVMData model)
    {
        return model.Bid1.NumberToChoose();
    }
    public static int CardToPlay(XactikaMainGameClass mainGame)
    {
        var newList = mainGame!.SingleInfo!.MainHandList.Where(items => mainGame.IsValidMove(items.Deck)).ToRegularDeckDict();
        return newList.GetRandomItem().Deck;
    }
    public static EnumShapes GetShapeChosen()
    {
        BasicList<int> tempList = Enumerable.Range(1, 4).ToBasicList();
        int output = tempList.GetRandomItem();
        return EnumShapes.FromValue(output);
    }
}