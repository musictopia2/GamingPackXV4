﻿namespace BladesOfSteel.Core.Logic;
[SingletonGame]
[AutoReset]
public class ComputerAI
{
    private readonly BladesOfSteelGameContainer _gameContainer;
    private readonly BladesOfSteelVMData _model;
    public ComputerAI(BladesOfSteelGameContainer gameContainer, BladesOfSteelVMData model)
    {
        _gameContainer = gameContainer;
        _model = model;
    }
    private DeckRegularDict<RegularSimpleCard> CardsForFirstDefense()
    {
        if (_gameContainer.GetDefenseStage == null)
        {
            throw new CustomBasicException("Nobody is handling get defense stage for computer ai.  Rethink");
        }
        var thisList = _gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumRegularColorList.Black);
        var firstItem = thisList.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => temps.Value.Value)).Take(1).Single();
        return new DeckRegularDict<RegularSimpleCard>(firstItem);
    }
    private DeckRegularDict<RegularSimpleCard> CardsToAddDefense()
    {
        if (_gameContainer.GetDefenseStage == null)
        {
            throw new CustomBasicException("Nobody is handling get defense stage for computer ai.  Rethink");
        }
        if (_gameContainer.SingleInfo!.DefenseList.Count == 0)
        {
            throw new CustomBasicException("Should have used the CardsForFirstDefense because 0 cards left");
        }
        int maxs = 3 - _gameContainer.SingleInfo.DefenseList.Count;
        var thisList = _gameContainer.SingleInfo.MainHandList.PossibleCombinations(EnumRegularColorList.Black, maxs);
        if (thisList.Count == 0)
        {
            throw new CustomBasicException("Must be at least one combination.  Otherwise; would be attacking");
        }
        var firstItem = thisList.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => temps.Value.Value)).Take(1).Single();
        return new DeckRegularDict<RegularSimpleCard>(firstItem);
    }
    private bool NeedsToRemoveDefenseCards()
    {
        if (_gameContainer.GetDefenseStage == null)
        {
            throw new CustomBasicException("Nobody is handling get defense stage for computer ai.  Rethink");
        }
        var handCombo = _gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumRegularColorList.Black);
        var boardCombo = _gameContainer.SingleInfo.DefenseList.PossibleCombinations(EnumRegularColorList.Black);
        var handList = handCombo.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => temps.Value.Value)).Take(1).Single();
        var boardList = boardCombo.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.Sum(temps => temps.Value.Value)).Take(1).Single();
        var handLevel = _gameContainer.GetDefenseStage(handList);
        var boardLevel = _gameContainer.GetDefenseStage(boardList);
        return handLevel > boardLevel || handList.Sum(items => items.Value.Value) > boardList.Sum(items => items.Value.Value);
    }
    private DeckRegularDict<RegularSimpleCard> CardsForAttack()
    {
        if (_gameContainer.GetAttackStage == null)
        {
            throw new CustomBasicException("Nobody is handling get attack stage for computer ai.  Rethink");
        }
        var possibleList = _gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumRegularColorList.Red);
        var thisItem = possibleList.OrderByDescending(items => _gameContainer.GetAttackStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).Single(); //hopefully using distinct count works.
        return new DeckRegularDict<RegularSimpleCard>(thisItem);
    }
    public (EnumFirstStep firstStep, DeckRegularDict<RegularSimpleCard> cardList) GetFirstMove()
    {
        if (_gameContainer.SingleInfo!.MainHandList.Count != 6)
        {
            throw new CustomBasicException("Must have exactly 6 cards in hand when figuring out the first move");
        }
        bool exists = _gameContainer.SingleInfo.MainHandList.Any(items => items.Color == EnumRegularColorList.Black);
        if (_gameContainer.SingleInfo.DefenseList.Count == 0 && exists == true)
        {
            return (EnumFirstStep.PlayDefense, CardsForFirstDefense());
        }
        int counts = _gameContainer.SingleInfo.MainHandList.Count(items => items.Color == EnumRegularColorList.Red);
        if (counts > 1)
        {
            return (EnumFirstStep.PlayAttack, CardsForAttack());
        }
        if (NeedsToRemoveDefenseCards() == true)
        {
            return (EnumFirstStep.PlayDefense, CardsForFirstDefense());
        }
        if (_gameContainer.SingleInfo.DefenseList.Count == 3)
        {
            return (EnumFirstStep.ThrowAwayAllCards, null!);
        }
        return (EnumFirstStep.PlayDefense, CardsToAddDefense());
    }
    public (EnumDefenseStep DefenseStep, DeckRegularDict<RegularSimpleCard> CardList) CardsForDefense()
    {
        if (_gameContainer.GetDefenseStage == null)
        {
            throw new CustomBasicException("Nobody is handling get defense stage for computer ai.  Rethink");
        }
        var possibleList = _gameContainer.SingleInfo!.MainHandList.PossibleCombinations(EnumRegularColorList.Black);
        possibleList.KeepConditionalItems(items => _model.MainDefense1!.CanAddDefenseCards(items) == true);
        var newList = _gameContainer.SingleInfo.DefenseList.PossibleCombinations(EnumRegularColorList.Black);
        newList.KeepConditionalItems(items => _model.MainDefense1!.CanAddDefenseCards(items) == true);
        if (possibleList.Count == 0 && newList.Count == 0)
        {
            return (EnumDefenseStep.Pass, null!);
        }
        var handList = possibleList.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).SingleOrDefault();
        var tempDefenseList = newList.OrderByDescending(items => _gameContainer.GetDefenseStage(items)).ThenByDescending(items => items.DistinctCount(temps => temps.Value)).Take(1).SingleOrDefault();
        if (possibleList.Count == 0)
        {
            return (EnumDefenseStep.Board, new DeckRegularDict<RegularSimpleCard>(tempDefenseList!));
        }
        if (newList.Count == 0)
        {
            return (EnumDefenseStep.Hand, new DeckRegularDict<RegularSimpleCard>(handList!));
        }
        var handLevel = _gameContainer.GetDefenseStage(handList!);
        var boardLevel = _gameContainer.GetDefenseStage(tempDefenseList!);
        if (handLevel < boardLevel || handList!.Sum(items => items.Value.Value) <= tempDefenseList!.Sum(items => items.Value.Value))
        {
            return (EnumDefenseStep.Hand, handList!.ToRegularDeckDict());
        }
        return (EnumDefenseStep.Board, tempDefenseList!.ToRegularDeckDict());
    }
}
