﻿namespace Opetong.Core.Logic;
public class RummySet : SetInfo<EnumSuitList, EnumRegularColorList, RegularRummyCard, SavedSet>
{
    private int _player;
    private readonly OpetongGameContainer _gameContainer;
    public RummySet(OpetongGameContainer gameContainer) : base(gameContainer.Command)
    {
        _gameContainer = gameContainer;
    }
    public override void LoadSet(SavedSet current)
    {
        _player = current.Player;
        HandList.ReplaceRange(current.CardList);
    }
    public override SavedSet SavedSet()
    {
        SavedSet output = new();
        output.CardList = HandList.ToRegularDeckDict();
        output.Player = _player;
        return output;
    }
    private int GetBaseRate
    {
        get
        {
            var newList = HandList.ToRegularDeckDict();
            bool rets;
            rets = _gameContainer.Rummys!.IsNewRummy(newList, HandList.Count, EnumRummyType.Sets);
            if (rets)
            {
                return 10;
            }
            bool suits;
            bool runs;
            suits = _gameContainer.Rummys.IsNewRummy(newList, HandList.Count, EnumRummyType.Colors);
            runs = _gameContainer.Rummys.IsNewRummy(newList, HandList.Count, EnumRummyType.Runs);
            if (suits && runs)
            {
                return 10;
            }
            return 5;
        }
    }
    public void CreateNewSet(IDeckDict<RegularRummyCard> thisCol)
    {
        if (thisCol.Count < 2 || thisCol.Count > 4)
        {
            throw new CustomBasicException("A set must have 2 to 4 cards");
        }
        _player = _gameContainer.WhoTurn;
        thisCol.ForEach(thisCard =>
        {
            thisCard.Drew = false;
            thisCard.IsSelected = false;
        });
        HandList.ReplaceRange(thisCol);
    }
    public int CalculateScore(int whichPlayer)
    {
        if (whichPlayer != _player)
        {
            return 0;
        }
        int bases = GetBaseRate;
        return bases * HandList.Count;
    }
}