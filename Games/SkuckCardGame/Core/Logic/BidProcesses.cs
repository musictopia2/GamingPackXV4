namespace SkuckCardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class BidProcesses : IBidProcesses
{
    private readonly SkuckCardGameVMData _model;
    private readonly SkuckCardGameGameContainer _gameContainer;
    public BidProcesses(SkuckCardGameVMData model, SkuckCardGameGameContainer gameContainer)
    {
        _model = model;
        _gameContainer = gameContainer;
    }
    private bool HasException()
    {
        if (_gameContainer.Test!.DoubleCheck == true)
        {
            return true;
        }
        int diffs = _gameContainer.PlayerList!.First().StrengthHand - _gameContainer.PlayerList!.Last().StrengthHand;
        if (Math.Abs(diffs) >= 12)
        {
            return true;
        }
        return false;
    }
    async Task IBidProcesses.ProcessBidAmountAsync()
    {
        if (_model!.BidAmount == -1)
        {
            throw new CustomBasicException("Did not choose a bid amount");
        }
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
        SkuckCardGamePlayerItem player = _gameContainer.SingleInfo!;
        //var thisPlayer = _gameContainer.PlayerList![id];
        player.BidAmount = _model.BidAmount;
        int whoStarts = _gameContainer.WhoStarts;
        player.MainHandList.UnhighlightObjects();
        _gameContainer.WhoTurn = await _gameContainer.PlayerList.CalculateWhoTurnAsync();
        if (_gameContainer.WhoTurn != whoStarts)
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }


        //if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
        //{
        //    thisPlayer.BidVisible = true;
        //    _gameContainer.SaveRoot!.WhatStatus = EnumStatusList.WaitForOtherPlayers;
        //    if (_gameContainer.BasicData!.MultiPlayer == false)
        //    {
        //        await _gameContainer.ComputerTurnAsync!.Invoke();
        //        return;
        //    }
        //    _gameContainer!.Command!.ManuelFinish = true; //because you can't change your mind.
        //    _gameContainer.Network!.IsEnabled = true; //wait for other players.
        //    _gameContainer.Command.UpdateAll();
        //    return;
        //}
        if (HasException() == true)
        {
            _gameContainer!.Command!.ManuelFinish = true;
            _gameContainer.SaveRoot!.WhatStatus = EnumStatusList.ChoosePlay;
            _gameContainer.Command.IsExecuting = true; //make sure its executing no matter what as well.
            if (_gameContainer.PlayerList.First().StrengthHand > _gameContainer.PlayerList.Last().StrengthHand)
            {
                _gameContainer.WhoTurn = 2;
            }
            else
            {
                _gameContainer.WhoTurn = 1; //to double check.
            }
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            await _gameContainer.StartNewTurnAsync!.Invoke();
            return;
        }
        _gameContainer.SaveRoot!.WhatStatus = EnumStatusList.NormalPlay;
        if (_gameContainer.WhoTurn == 1)
        {
            _gameContainer.WhoTurn = 2;
        }
        else
        {
            _gameContainer.WhoTurn = 1;
        }
        _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
        await _gameContainer.StartNewTrickAsync!.Invoke();
    }
}