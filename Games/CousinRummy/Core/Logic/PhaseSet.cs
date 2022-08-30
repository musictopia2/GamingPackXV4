﻿namespace CousinRummy.Core.Logic;
public class PhaseSet : SetInfo<EnumSuitList, EnumRegularColorList, RegularRummyCard, SavedSet>
{
    private readonly CousinRummyGameContainer _gameContainer;
    public PhaseSet(CousinRummyGameContainer gameContainer) : base(gameContainer.Command)
    {
        _gameContainer = gameContainer;
    }
    public override void LoadSet(SavedSet payLoad)
    {
        HandList.ReplaceRange(payLoad.CardList);
    }
    public override SavedSet SavedSet()
    {
        SavedSet output = new();
        output.CardList = HandList.ToRegularDeckDict();
        return output;
    }
    public void AddCard(RegularRummyCard thisCard)
    {
        UpdateCard(thisCard);
        HandList.Add(thisCard);
    }
    private void UpdateCard(RegularRummyCard thisCard)
    {
        thisCard.IsSelected = false;
        thisCard.Drew = false;
        thisCard.Player = _gameContainer.WhoTurn;
    }
    public void CreateSet(IDeckDict<RegularRummyCard> thisList)
    {
        thisList.ForEach(thisCard => UpdateCard(thisCard));
        HandList.ReplaceRange(thisList);
    }
    public int PointsReceived(int player)
    {
        if (_gameContainer.ModifyCards == null)
        {
            throw new CustomBasicException("Nobody is handling modify cards.  Rethink");
        }
        _gameContainer.ModifyCards.Invoke(HandList);
        return HandList.Where(items => items.Player == player).Sum(items => items.Points);
    }
    public bool CanExpand(RegularRummyCard thisCard)
    {
        if (thisCard.IsObjectWild == true)
            return true;
        EnumRegularCardValueList numberNeeded = HandList.First(items => items.IsObjectWild == false).Value;
        return thisCard.Value == numberNeeded;
    }
}
