﻿namespace Rook.Core.Logic;
[SingletonGame]
[AutoReset]
public class NestProcesses : INestProcesses
{
    private readonly RookVMData _model;
    private readonly RookGameContainer _gameContainer;
    public NestProcesses(RookVMData model,
        RookGameContainer gameContainer
        )
    {
        _model = model;
        _gameContainer = gameContainer;
    }
    async Task INestProcesses.ProcessNestAsync(DeckRegularDict<RookCardInformation> list)
    {
        if (list.Count != 5)
        {
            throw new CustomBasicException("The nest must contain exactly 5 cards");
        }
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
        {
            await _gameContainer.Network!.SendAllAsync("nestlist", list);
        }
        _gameContainer.SaveRoot!.NestList.ReplaceRange(list);
        if (_gameContainer.PlayerList!.Count < 4)
        {
            var newCol = _model!.Deck1!.DrawSeveralCards(3);
            _gameContainer.SaveRoot.NestList.AddRange(newCol);
        }
        _gameContainer.SingleInfo!.MainHandList.RemoveSelectedItems(list);
        _gameContainer.SingleInfo.MainHandList.UnhighlightObjects();
        _gameContainer.StartingStatus!.Invoke();
        if (_gameContainer.PlayerList!.Count < 3)
        {
            _gameContainer.SaveRoot.GameStatus = EnumStatusList.Normal;
        }
        else
        {
            _gameContainer.SaveRoot.GameStatus = EnumStatusList.ChooseTrump;
        }
        _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectOneOnly;
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
        if (_gameContainer.PlayerList.Count == 2)
        {
            if (_gameContainer.SaveRoot.WonSoFar == 1)
            {
                _gameContainer.WhoTurn = 2;
            }
            else
            {
                _gameContainer.WhoTurn = 1;
            }
        }
        //else if (_gameContainer.PlayerList.Count == 4)
        //{
        //    _gameContainer.WhoTurn = _gameContainer.WhoStarts; //(?) well see if this is correct or not
        //    //throw new CustomBasicException("Rethink figuring out who goes first when its 4 players");
        //}
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
        if (_gameContainer.PlayerList.Count == 2)
        {
            _gameContainer.SaveRoot.DummyPlay = true;
        }
        if (_gameContainer.PlayerList.Count < 4)
        {
            await _gameContainer.StartNewTrickAsync!.Invoke();
        }
        else
        {
            await _gameContainer.ContinueTurnAsync!.Invoke();
            //await _gameContainer.StartNewTurnAsync!.Invoke(); //i think.
        }
    }
}