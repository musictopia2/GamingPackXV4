namespace BladesOfSteel.Core.Logic;
[SingletonGame]
[AutoReset]
public class ComputerAI(BladesOfSteelGameContainer gameContainer, BladesOfSteelVMData model)
{
    private DeckRegularDict<RegularSimpleCard> CardsForFirstDefense()
    {
        if (gameContainer.GetDefenseStage == null)
        {
            throw new CustomBasicException("Nobody is handling get defense stage for computer ai.  Rethink");
        }
        var thisList = gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumRegularColorList.Black);
        var firstItem = thisList.OrderByDescending(items => gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => temps.Value.Value)).Take(1).Single();
        return [.. firstItem];
    }
    private DeckRegularDict<RegularSimpleCard> CardsToAddDefense()
    {
        if (gameContainer.GetDefenseStage == null)
        {
            throw new CustomBasicException("Nobody is handling get defense stage for computer ai.  Rethink");
        }
        if (gameContainer.SingleInfo!.DefenseList.Count == 0)
        {
            throw new CustomBasicException("Should have used the CardsForFirstDefense because 0 cards left");
        }
        int maxs = 3 - gameContainer.SingleInfo.DefenseList.Count;
        var thisList = gameContainer.SingleInfo.MainHandList.PossibleCombinations(EnumRegularColorList.Black, maxs);
        if (thisList.Count == 0)
        {
            throw new CustomBasicException("Must be at least one combination.  Otherwise; would be attacking");
        }
        var firstItem = thisList.OrderByDescending(items => gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => temps.Value.Value)).Take(1).Single();
        return [.. firstItem];
    }
    private bool NeedsToRemoveDefenseCards()
    {
        if (gameContainer.GetDefenseStage == null)
        {
            throw new CustomBasicException("Nobody is handling get defense stage for computer ai.  Rethink");
        }
        var handCombo = gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumRegularColorList.Black);
        var boardCombo = gameContainer.SingleInfo.DefenseList.PossibleCombinations(EnumRegularColorList.Black);
        var handList = handCombo.OrderByDescending(items => gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => temps.Value.Value)).Take(1).Single();
        var boardList = boardCombo.OrderByDescending(items => gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => temps.Value.Value)).Take(1).Single();
        var handLevel = gameContainer.GetDefenseStage(handList);
        var boardLevel = gameContainer.GetDefenseStage(boardList);
        return handLevel > boardLevel || handList.Sum(items => items.Value.Value) > boardList.Sum(items => items.Value.Value);
    }
    private DeckRegularDict<RegularSimpleCard> CardsForAttack()
    {
        if (gameContainer.GetAttackStage == null)
        {
            throw new CustomBasicException("Nobody is handling get attack stage for computer ai.  Rethink");
        }
        var possibleList = gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumRegularColorList.Red);
        var thisItem = possibleList.OrderByDescending(items => gameContainer.GetAttackStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).Single(); //hopefully using distinct count works.
        return [.. thisItem];
    }
    public (EnumFirstStep firstStep, DeckRegularDict<RegularSimpleCard> cardList) GetFirstMove()
    {
        if (gameContainer.SingleInfo!.MainHandList.Count != 6)
        {
            throw new CustomBasicException("Must have exactly 6 cards in hand when figuring out the first move");
        }
        bool exists = gameContainer.SingleInfo.MainHandList.Any(items => items.Color == EnumRegularColorList.Black);
        if (gameContainer.SingleInfo.DefenseList.Count == 0 && exists == true)
        {
            return (EnumFirstStep.PlayDefense, CardsForFirstDefense());
        }
        int counts = gameContainer.SingleInfo.MainHandList.Count(items => items.Color == EnumRegularColorList.Red);
        if (counts > 1)
        {
            return (EnumFirstStep.PlayAttack, CardsForAttack());
        }
        if (NeedsToRemoveDefenseCards() == true)
        {
            return (EnumFirstStep.PlayDefense, CardsForFirstDefense());
        }
        if (gameContainer.SingleInfo.DefenseList.Count == 3)
        {
            return (EnumFirstStep.ThrowAwayAllCards, null!);
        }
        return (EnumFirstStep.PlayDefense, CardsToAddDefense());
    }
    public (EnumDefenseStep DefenseStep, DeckRegularDict<RegularSimpleCard> CardList) CardsForDefense()
    {
        if (gameContainer.GetDefenseStage == null)
        {
            throw new CustomBasicException("Nobody is handling get defense stage for computer ai.  Rethink");
        }
        var possibleList = gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumRegularColorList.Black);
        possibleList.KeepConditionalItems(items => model.MainDefense1!.CanAddDefenseCards(items) == true);
        var newList = gameContainer.SingleInfo.DefenseList.PossibleCombinations(EnumRegularColorList.Black);
        newList.KeepConditionalItems(items => model.MainDefense1!.CanAddDefenseCards(items) == true);
        if (possibleList.Count == 0 && newList.Count == 0)
        {
            return (EnumDefenseStep.Pass, null!);
        }
        var handList = possibleList.OrderByDescending(items => gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).SingleOrDefault();
        var tempDefenseList = newList.OrderByDescending(items => gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).SingleOrDefault();
        if (possibleList.Count == 0)
        {
            return (EnumDefenseStep.Board, new DeckRegularDict<RegularSimpleCard>(tempDefenseList!));
        }
        if (newList.Count == 0)
        {
            return (EnumDefenseStep.Hand, new DeckRegularDict<RegularSimpleCard>(handList!));
        }
        var handLevel = gameContainer.GetDefenseStage(handList!);
        var boardLevel = gameContainer.GetDefenseStage(tempDefenseList!);
        if (handLevel < boardLevel || handList!.Sum(items => items.Value.Value) <= tempDefenseList!.Sum(items => items.Value.Value))
        {
            return (EnumDefenseStep.Hand, handList!.ToRegularDeckDict());
        }
        return (EnumDefenseStep.Board, tempDefenseList!.ToRegularDeckDict());
    }
}
