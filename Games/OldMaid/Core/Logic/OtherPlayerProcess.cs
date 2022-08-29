﻿namespace OldMaid.Core.Logic;
[SingletonGame]
[AutoReset]
public class OtherPlayerProcess : IOtherPlayerProcess
{
    private readonly OldMaidGameContainer _gameContainer;
    private readonly OldMaidVMData _model;
    public OtherPlayerProcess(OldMaidGameContainer gameContainer, OldMaidVMData model)
    {
        _gameContainer = gameContainer;
        _model = model;
    }
    public void SortOtherCards()
    {
        DeckRegularDict<RegularSimpleCard> output = new();
        _gameContainer!.OtherPlayer!.MainHandList.ForEach(thisCard =>
        {
            RegularSimpleCard newCard = new();
            newCard.Populate(thisCard.Deck);
            newCard.IsUnknown = true;
            output.Add(newCard);
        });
        output.ShuffleList();
        _model.OpponentCards1!.HandList.ReplaceRange(output);
    }
    async Task IOtherPlayerProcess.SelectCardAsync(int deck)
    {
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
        {
            await _gameContainer.Network!.SendAllAsync("cardselected", deck);
        }
        if (_gameContainer!.OtherPlayer!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.PlayerHand1!.SelectOneFromDeck(deck);
            _gameContainer.Command.UpdateAll();
            if (_gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(.75);
            }
        }
        _gameContainer.OtherPlayer.MainHandList.RemoveObjectByDeck(deck);
        var thisCard = _gameContainer.DeckList!.GetSpecificItem(deck);
        thisCard.IsUnknown = false;
        thisCard.IsSelected = false;
        SortOtherCards();
        _gameContainer.SingleInfo!.MainHandList.Add(thisCard);
        if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            thisCard.Drew = true;
            if (_gameContainer.SortCards == null)
            {
                throw new CustomBasicException("Nobody is handling sort cards.  Rethink");
            }
            _gameContainer.SortCards.Invoke();
        }
        _gameContainer.SaveRoot!.AlreadyChoseOne = true;
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
}