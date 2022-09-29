﻿namespace GolfCardGame.Core.CustomPiles;
public class HiddenCards : BasicMultiplePilesCP<RegularSimpleCard>
{
    readonly GolfCardGameGameContainer _gameContainer;
    public HiddenCards(GolfCardGameGameContainer gameContainer) : base(gameContainer.Command)
    {
        _gameContainer = gameContainer;
        Rows = 1;
        Columns = 2;
        HasFrame = true;
        Style = EnumMultiplePilesStyleList.SingleCard;
        HasText = true;
        LoadBoard();
        IsEnabled = false;
        if (PileList!.Count != 2)
        {
            throw new CustomBasicException("Must have 2 piles");
        }
        PileClickedAsync = HiddenCards_PileClickedAsync;
        int x = 0;
        PileList.ForEach(thisPile =>
        {
            x++;
            thisPile.Text = $"Card {x}";
        });
    }
    public void ChangeCard(int pile, RegularSimpleCard newCard)
    {
        var thisPile = PileList![pile];
        thisPile.ThisObject = newCard;
        thisPile.IsEnabled = false;
    }
    public void RevealCards()
    {
        PileList!.ForEach(thisPile => thisPile.ThisObject.IsUnknown = false);
    }
    public override void ClearBoard()
    {
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetSelf();
        if (_gameContainer.SingleInfo.TempSets.Count != 2)
        {
            throw new CustomBasicException("Needs 2 cards for the hidden cards");
        }
        int x = 0;
        _gameContainer.SingleInfo.TempSets.ForEach(thisCard =>
        {
            var thisPile = PileList![x];
            thisPile.ThisObject = thisCard;
            thisPile.ThisObject.IsUnknown = true;
            thisPile.IsEnabled = true;
            x++;
        });
    }
    private async Task HiddenCards_PileClickedAsync(int index, BasicPileInfo<RegularSimpleCard> thisPile)
    {
        if (_gameContainer.ChangeUnknownAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the change unknown async.  Rethink");
        }
        await _gameContainer.ChangeUnknownAsync(index);
    }
}