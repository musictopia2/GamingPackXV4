namespace SkuckCardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class BidProcesses(SkuckCardGameVMData model, SkuckCardGameGameContainer gameContainer) : IBidProcesses
{
    private bool HasException()
    {
        if (gameContainer.Test!.DoubleCheck == true)
        {
            return true;
        }
        int diffs = gameContainer.PlayerList!.First().StrengthHand - gameContainer.PlayerList!.Last().StrengthHand;
        if (Math.Abs(diffs) >= 12)
        {
            return true;
        }
        return false;
    }
    async Task IBidProcesses.ProcessBidAmountAsync()
    {
        if (model!.BidAmount == -1)
        {
            throw new CustomBasicException("Did not choose a bid amount");
        }
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        SkuckCardGamePlayerItem player = gameContainer.SingleInfo!;
        //var thisPlayer = _gameContainer.PlayerList![id];
        player.BidAmount = model.BidAmount;
        int whoStarts = gameContainer.WhoStarts;
        player.MainHandList.UnhighlightObjects();
        gameContainer.WhoTurn = await gameContainer.PlayerList.CalculateWhoTurnAsync();
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        if (gameContainer.SingleInfo.BidAmount == 0)
        {
            
            await gameContainer.ContinueTurnAsync!.Invoke();
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
            gameContainer!.Command!.ManuelFinish = true;
            gameContainer.SaveRoot!.WhatStatus = EnumStatusList.ChoosePlay;
            gameContainer.Command.IsExecuting = true; //make sure its executing no matter what as well.
            if (gameContainer.PlayerList.First().StrengthHand > gameContainer.PlayerList.Last().StrengthHand)
            {
                gameContainer.WhoTurn = 2;
            }
            else
            {
                gameContainer.WhoTurn = 1; //to double check.
            }
            gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
            await gameContainer.StartNewTurnAsync!.Invoke();
            return;
        }
        gameContainer.SaveRoot!.WhatStatus = EnumStatusList.NormalPlay;
        if (gameContainer.WhoTurn == 1)
        {
            gameContainer.WhoTurn = 2;
        }
        else
        {
            gameContainer.WhoTurn = 1;
        }
        //_gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
        //if (_gameContainer.SingleInfo.BidAmount == 0)
        //{
        //    _gameContainer.SaveRoot!.WhatStatus = EnumStatusList.ChooseBid;
        //    await _gameContainer.StartNewTurnAsync!.Invoke();
        //    return;
        //}
        await gameContainer.StartNewTrickAsync!.Invoke();
    }
}