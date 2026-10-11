namespace Rook.Core.Logic;
[SingletonGame]
[AutoReset]
public class NestProcesses(RookVMData model,
    RookGameContainer gameContainer
        ) : INestProcesses
{
    async Task INestProcesses.ProcessNestAsync(DeckRegularDict<RookCardInformation> list)
    {
        if (list.Count != 5)
        {
            throw new CustomBasicException("The nest must contain exactly 5 cards");
        }
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!))
        {
            await gameContainer.Network!.SendAllAsync("nestlist", list);
        }
        gameContainer.SaveRoot!.NestList.ReplaceRange(list);
        if (gameContainer.PlayerList!.Count < 4)
        {
            var newCol = model!.Deck1!.DrawSeveralCards(3);
            gameContainer.SaveRoot.NestList.AddRange(newCol);
        }
        gameContainer.SingleInfo!.MainHandList.RemoveSelectedItems(list);
        gameContainer.SingleInfo.MainHandList.UnhighlightObjects();
        gameContainer.StartingStatus!.Invoke();
        if (gameContainer.PlayerList!.Count < 3)
        {
            gameContainer.SaveRoot.GameStatus = EnumStatusList.Normal;
        }
        else
        {
            gameContainer.SaveRoot.GameStatus = EnumStatusList.ChooseTrump;
        }
        model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectOneOnly;
        //if (_gameContainer.PlayerList!.Count == 3)
        //{
        //    if (_gameContainer.SaveRoot.WonSoFar == 1)
        //    {
        //        _gameContainer.SingleInfo = _gameContainer.PlayerList![3];
        //    }
        //    else if (_gameContainer.SaveRoot.WonSoFar == 2)
        //    {
        //        _gameContainer.SingleInfo = _gameContainer.PlayerList![1];
        //    }
        //    else
        //    {
        //        _gameContainer.SingleInfo = _gameContainer.PlayerList![2];
        //    }
        //    _gameContainer.WhoTurn = _gameContainer.SingleInfo.Id;
        //}
        if (gameContainer.PlayerList.Count == 2)
        {
            if (gameContainer.SaveRoot.WonSoFar == 1)
            {
                gameContainer.WhoTurn = 2;
            }
            else
            {
                gameContainer.WhoTurn = 1;
            }
        }
        //else if (_gameContainer.PlayerList.Count == 4)
        //{
        //    _gameContainer.WhoTurn = _gameContainer.WhoStarts; //(?) well see if this is correct or not
        //    //throw new CustomBasicException("Rethink figuring out who goes first when its 4 players");
        //}
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        if (gameContainer.PlayerList.Count == 2)
        {
            gameContainer.SaveRoot.DummyPlay = true;
        }
        if (gameContainer.PlayerList.Count < 4)
        {
            await gameContainer.StartNewTrickAsync!.Invoke();
        }
        else
        {
            await gameContainer.ContinueTurnAsync!.Invoke();
            //await _gameContainer.StartNewTurnAsync!.Invoke(); //i think.
        }
    }
}