﻿namespace GoFish.Core.Logic;
[SingletonGame]
[AutoReset]
public class AskProcesses : IAskProcesses
{
    private readonly GoFishGameContainer _gameContainer;
    private readonly GoFishVMData _model;
    private readonly IToast _toast;
    public AskProcesses(GoFishGameContainer gameContainer, GoFishVMData model, IToast toast)
    {
        _gameContainer = gameContainer;
        _model = model;
        _toast = toast;
    }
    void IAskProcesses.LoadAskList()
    {
        if (_gameContainer.LoadAskScreenAsync != null)
        {
            _gameContainer.LoadAskScreenAsync.Invoke();
        }
        if (_gameContainer!.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.AskList.LoadFromHandCardValues(_gameContainer.SingleInfo);
        }
        else
        {
            _model.AskList.LoadEntireList();
        }
        _model.CardYouAsked = EnumRegularCardValueList.None;
        _model.AskList.UnselectAll();
    }
    async Task IAskProcesses.NumberToAskAsync(EnumRegularCardValueList asked)
    {
        _gameContainer.SaveRoot!.NumberAsked = true;
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            _model!.AskList!.SelectSpecificItem(asked);
        }
        GoFishPlayerItem otherPlayer = GetPlayer();
        await NumberToAskAsync(asked, otherPlayer);
    }
    private async Task NumberToAskAsync(EnumRegularCardValueList whichOne, GoFishPlayerItem otherPlayer)
    {
        if (otherPlayer.MainHandList.Count == 0)
        {
            await _gameContainer.ContinueTurnAsync!();
            return;
        }
        _model!.AskList!.SelectSpecificItem(whichOne);
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelaySeconds(1);
        }
        DeckRegularDict<RegularSimpleCard> thisList;
        if (_gameContainer.Test.AllowAnyMove == false || otherPlayer.MainHandList.Count != 1)
        {
            thisList = otherPlayer.MainHandList.Where(items => items.Value == whichOne).ToRegularDeckDict();
        }
        else
        {
            thisList = otherPlayer.MainHandList.ToRegularDeckDict();
        }
        if (thisList.Count == 0)
        {
            if (_model.Deck1!.IsEndOfDeck() == false)
            {
                if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                {
                    _toast.ShowInfoToast("Go Fish");
                }
                _gameContainer.LeftToDraw = 0;
                _gameContainer.PlayerDraws = 0;
                await _gameContainer.DrawAsync!.Invoke();
                return;
            }
            if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _toast.ShowInfoToast("No more cards left to draw.  Therefore; will have to end your turn");
            }
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (otherPlayer.PlayerCategory == EnumPlayerCategory.Self && _gameContainer.Test.NoAnimations == false)
        {
            thisList.ForEach(items =>
            {
                _model.PlayerHand1!.SelectOneFromDeck(items.Deck);
            });
            await _gameContainer.Delay!.DelaySeconds(1); //so you can see what you have to get rid of.
        }
        thisList.ForEach(items =>
        {
            otherPlayer.MainHandList.RemoveObjectByDeck(items.Deck);
            if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                items.Drew = true;
            }
            _gameContainer.SingleInfo.MainHandList.Add(items);
        });
        if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            _gameContainer.SortAfterDrawing!.Invoke(); //i think.
        if (otherPlayer.MainHandList.Count == 0)
        {
            int cards = _model.Deck1!.CardsLeft();
            if (cards < 5 && cards > 0)
            {
                _gameContainer.LeftToDraw = cards;
                _gameContainer.PlayerDraws = otherPlayer.Id;
                await _gameContainer.DrawAsync!.Invoke();
                return;
            }
            else if (cards > 0)
            {
                _gameContainer.LeftToDraw = 5;
                _gameContainer.PlayerDraws = otherPlayer.Id;
                await _gameContainer.DrawAsync!.Invoke();
                return;
            }
        }
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    private GoFishPlayerItem GetPlayer()
    {
        if (_gameContainer.PlayerList!.Count > 2)
        {
            throw new CustomBasicException("Since there are more than 2 players; needs to know the nick name");
        }
        int nums;
        GoFishPlayerItem tempPlayer;
        if (_gameContainer.WhoTurn == 1)
        {
            nums = 2;
        }
        else
        {
            nums = 1;
        }
        _gameContainer.SaveRoot!.PlayOrder.OtherTurn = nums;
        tempPlayer = _gameContainer.PlayerList![nums]; //one based now
        return tempPlayer;
    }
}