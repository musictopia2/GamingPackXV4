﻿namespace Rook.Core.Logic;
[SingletonGame]
[AutoReset]
public class BidProcesses : IBidProcesses
{
    private readonly RookVMData _model;
    private readonly RookGameContainer _gameContainer;
    public BidProcesses(RookVMData model, RookGameContainer gameContainer)
    {
        _model = model;
        _gameContainer = gameContainer;
    }
    public async Task BeginBiddingAsync()
    {
        _gameContainer.ShowedOnce = true;
        if (_gameContainer.SaveRoot!.HighestBidder == 0)
        {
            throw new CustomBasicException("The highest bidder cannot be 0");
        }
        PopulateBids();
        _model.CanPass = await CanPassAsync();
        await _gameContainer.StartNewTurnAsync!.Invoke();
    }
    private void PopulateBids()
    {
        _model.BidChosen = -1;
        if (_gameContainer!.SaveRoot!.HighestBidder == 1)
        {
            throw new CustomBasicException("The highest bid cannot be 0");
        }
        if (_gameContainer.PlayerList!.Count < 4)
        {
            _model.Bid1!.LoadNormalNumberRangeValues(_gameContainer.SaveRoot.HighestBidder + 5, 100, 5);
        }
        else
        {
            _model.Bid1!.LoadNormalNumberRangeValues(_gameContainer.SaveRoot.HighestBidder + 5, 120, 5);
        }
    }
    public async Task<bool> CanPassAsync()
    {
        var temps = await _gameContainer.PlayerList!.CalculateWhoTurnAsync();
        _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
        if (temps == _gameContainer.WhoStarts && _gameContainer.SaveRoot!.WonSoFar == 0)
        {
            return false;
        }
        return true;
    }
    public async Task PassBidAsync()
    {
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
        {
            await _gameContainer.Network!.SendAllAsync("pass");
        }
        _gameContainer.SingleInfo!.Pass = true;
        await ContinueBidProcessAsync();
    }
    public async Task ProcessBidAsync()
    {
        if (_model!.BidChosen == -1)
        {
            throw new CustomBasicException("The bid amount cannot be -1");
        }
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            _model.Bid1!.SelectNumberValue(_model.BidChosen);
        }
        if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
        {
            await _gameContainer.Network!.SendAllAsync("bid", _model.BidChosen);
        }
        _gameContainer.SaveRoot!.WonSoFar = _gameContainer.WhoTurn;
        _gameContainer.SingleInfo.BidAmount = _model.BidChosen;
        _gameContainer.SaveRoot.HighestBidder = _model.BidChosen;
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelaySeconds(1);
        }
        _gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.Id != _gameContainer.WhoTurn)
            {
                thisPlayer.BidAmount = 0; //because somebody else won it.
            }
        });
        ResetBids();
        await ContinueBidProcessAsync();
    }
    private async Task ContinueBidProcessAsync()
    {
        if (_gameContainer.SaveRoot!.HighestBidder == 100 && _gameContainer.PlayerList!.Count < 4)
        {
            await EndBiddingAsync();
            return;
        }
        if (_gameContainer.SaveRoot!.HighestBidder == 120)
        {
            await EndBiddingAsync();
            return;
        }
        if (_gameContainer.SaveRoot.WonSoFar > 0)
        {
            if (_gameContainer.PlayerList!.Count(items => items.Pass == false) == 1) //maybe should have been false (?)
            {
                await EndBiddingAsync();
                return;
            }
        }
        int olds = _gameContainer.WhoTurn;
        do
        {
            _gameContainer.WhoTurn = await _gameContainer.PlayerList!.CalculateWhoTurnAsync();
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            if (_gameContainer.SingleInfo.Pass == false)
            {
                break;
            }
        } while (true);
        if (_gameContainer.WhoTurn == olds)
        {
            throw new CustomBasicException("Cannot be the same player again");
        }
        _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
        _model!.CanPass = await CanPassAsync();
        await BeginBiddingAsync();
    }
    private async Task EndBiddingAsync()
    {
        _model!.CanPass = false;
        _gameContainer.WhoTurn = _gameContainer.SaveRoot!.WonSoFar;
        _model.TrickArea1!.NewRound(); //i think.
        if (_gameContainer.PlayerList!.Count < 3)
        {
            _gameContainer.SaveRoot.GameStatus = EnumStatusList.ChooseTrump;
        }
        else
        {
            _gameContainer.SaveRoot.GameStatus = EnumStatusList.SelectNest;
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            _gameContainer.SingleInfo!.MainHandList.AddRange(_gameContainer.SaveRoot.NestList);
            if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
                _gameContainer.SortCards!.Invoke();
            }
        }
        _gameContainer.AfterBidding?.Invoke();
        await _gameContainer.StartNewTurnAsync!.Invoke();
    }
    private void ResetBids()
    {
        _model.BidChosen = -1;
        _model.Bid1!.UnselectAll();
    }
}
