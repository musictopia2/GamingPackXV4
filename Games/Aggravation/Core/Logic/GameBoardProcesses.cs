﻿namespace Aggravation.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses
{
    private readonly AggravationGameContainer _gameContainer;
    private readonly AggravationVMData _model;
    private readonly GameBoardGraphicsCP _graphicsCP;
    public GameBoardProcesses(AggravationGameContainer gameContainer, AggravationVMData model, GameBoardGraphicsCP graphicsCP)
    {
        _gameContainer = gameContainer;
        _model = model;
        _graphicsCP = graphicsCP;
        LoadBoard();
        _gameContainer.IsValidMove = IsValidMove;
    }

    private Dictionary<int, SpaceInfo>? _spaceList;
    private int PreviousPiece
    {
        get
        {
            return _gameContainer.SaveRoot!.PreviousSpace;
        }
        set
        {
            _gameContainer.SaveRoot!.PreviousSpace = value;
        }
    }
    private BasicList<MoveInfo> MoveList
    {
        get
        {
            return _gameContainer.SaveRoot!.MoveList;
        }
        set
        {
            _gameContainer.SaveRoot!.MoveList = value;
        }
    }
    private EnumColorChoice OurColor
    {
        get
        {
            return _gameContainer.SaveRoot!.OurColor;
        }
        set
        {
            _gameContainer.SaveRoot!.OurColor = value;
        }
    }
    public void LoadSavedGame()
    {
        foreach (var thisSpace in _spaceList!.Values)
        {
            thisSpace.Player = 0;
        }
        _gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.PieceList.ForEach(thisPiece =>
            {
                var thisSpace = _spaceList.Values.Single(items => items.Index == thisPiece);
                thisSpace.Player = thisPlayer.Id;
            });
        });
    }
    internal void SendTurn()
    {
        _gameContainer.Aggregator.Publish(new NewTurnEventModel()); //hopefully this works here too.
    }
    private void LoadBoard()
    {
        _gameContainer.Animates.LongestTravelTime = 150;
        PopulateSpaces();
    }
    public void RepaintBoard()
    {
        _gameContainer.Aggregator.RepaintBoard();
    }
    private void PopulateSpaces()
    {
        if (_spaceList != null)
        {
            return;
        }
        _spaceList = new Dictionary<int, SpaceInfo>();
        int x;
        int y;
        int q = 0;
        int z = 1;
        SpaceInfo thisSpace;
        for (x = 1; x <= 4; x++)
        {
            for (y = 1; y <= 4; y++)
            {
                q += 1;
                thisSpace = new ();
                thisSpace.ColorOwner = EnumColorChoice.FromValue(z);
                thisSpace.Index = q;
                thisSpace.WhatBoard = EnumBoardStatus.IsStart;
                thisSpace.SpaceNumber = y;
                _spaceList.Add(q, thisSpace);
            }
            z += 1;
            if (z > 4)
            {
                z = 1;
            }
        }
        y = 0;
        for (x = 17; x <= 64; x++)
        {
            y += 1;
            thisSpace = new ();
            thisSpace.Index = x;
            if (x == 17)
            {
                thisSpace.ColorOwner = EnumColorChoice.Blue;
            }
            else if (x == 29)
            {
                thisSpace.ColorOwner = EnumColorChoice.Green;
            }
            else if (x == 41)
            {
                thisSpace.ColorOwner = EnumColorChoice.Red;
            }
            else if (x == 53)
            {
                thisSpace.ColorOwner = EnumColorChoice.Yellow;
            }
            else
            {
                thisSpace.ColorOwner = EnumColorChoice.None;
            }
            thisSpace.WhatBoard = EnumBoardStatus.OnBoard;
            thisSpace.SpaceNumber = y;
            _spaceList.Add(x, thisSpace);
        }
        q = 64;
        z = 1;
        for (x = 1; x <= 4; x++)
        {
            for (y = 1; y <= 4; y++)
            {
                q += 1;
                thisSpace = new ();
                thisSpace.ColorOwner = EnumColorChoice.FromValue(z);
                thisSpace.SpaceNumber = y;
                thisSpace.Index = q;
                thisSpace.WhatBoard = EnumBoardStatus.IsHome;
                _spaceList.Add(q, thisSpace);
            }
            z += 1;
        }
        thisSpace = new ();
        thisSpace.SpaceNumber = 1;
        thisSpace.Index = 81;
        thisSpace.WhatBoard = EnumBoardStatus.OnCenter;
        _spaceList.Add(81, thisSpace);
    }
    public void ClearBoard()
    {
        _gameContainer.PlayerList!.ForEach(thisPlayer => thisPlayer.PieceList.Clear());
        foreach (var thisSpace in _spaceList!.Values)
        {
            if (thisSpace.ColorOwner != EnumColorChoice.None && thisSpace.WhatBoard == EnumBoardStatus.IsStart)
            {
                thisSpace.Player = FindPlayer(thisSpace.ColorOwner);
                if (thisSpace.Player > 0)
                {
                    var tempPlayer = _gameContainer.PlayerList[thisSpace.Player];
                    tempPlayer.PieceList.Add(thisSpace.Index);
                }
            }
            else
            {
                thisSpace.Player = 0;
            }
        }
        RepaintBoard();
    }
    private int FindPlayer(EnumColorChoice thisColor)
    {
        var thisPlayer = _gameContainer.PlayerList!.SingleOrDefault(items => items.Color == thisColor);
        if (thisPlayer == null)
        {
            return 0;
        }
        return thisPlayer.Id;
    }
    public void StartTurn()
    {
        _gameContainer.SaveRoot!.DiceNumber = 0;
        OurColor = _gameContainer.SingleInfo!.Color;
        PreviousPiece = 0;
        _gameContainer.MovePlayer = 0;
        _model.Cup!.CanShowDice = false;
        SendTurn();
    }
    public bool IsValidMove(int index)
    {
        if (_gameContainer.Command.IsExecuting)
        {
            return false;
        }
        if (_gameContainer.SaveRoot.DiceNumber == 0)
        {
            return false; //because you have to roll.
        }
        if (PreviousPiece == 0)
        {
            return MoveList.Any(items => items.SpaceFrom == index);
        }
        return MoveList.Any(items => items.SpaceTo == index);
    }
    private async Task NoMovesEndTurnAsync()
    {
        _gameContainer.SaveRoot!.Instructions = "No moves.  End turn.";
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelaySeconds(.5);
        }
        await _gameContainer.EndTurnAsync!.Invoke();
    }
    private async Task DoAnotherTurnAsync()
    {
        _gameContainer.SaveRoot!.Instructions = "Will get another turn for rolling a 6";
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelaySeconds(.5);
        }
        await _gameContainer.StartNewTurnAsync!.Invoke();
    }
    private async Task DoEndTurnAsync()
    {
        _gameContainer.SaveRoot!.Instructions = "Move was made successfully";
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelaySeconds(.5);
        }
        await _gameContainer.EndTurnAsync!.Invoke();
    }
    private int WhenToGoHome
    {
        get
        {
            int nums;
            if (OurColor == EnumColorChoice.Blue)
            {
                nums = 64;
            }
            else if (OurColor == EnumColorChoice.Green)
            {
                nums = 28;
            }
            else if (OurColor == EnumColorChoice.Red)
            {
                nums = 40;
            }
            else if (OurColor == EnumColorChoice.Yellow)
            {
                nums = 52;
            }
            else
            {
                throw new CustomBasicException("Need a color for the player");
            }
            return _spaceList!.Values.Single(items => items.Index == nums).SpaceNumber;
        }
    }
    private int FindStartSpace(int whatPlayer)
    {
        var tempPlayer = _gameContainer.PlayerList![whatPlayer];
        var thisCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.IsStart && items.ColorOwner == tempPlayer.Color && items.Player == 0).ToBasicList();
        if (thisCol.Count == 0)
        {
            throw new CustomBasicException("Could not find the start space");
        }
        return thisCol.First().Index;
    }
    private bool IsGameOver => _spaceList!.Values.Count(items => items.Player == _gameContainer.WhoTurn && items.WhatBoard == EnumBoardStatus.IsHome) == 4;
    private async Task BackToStartAsync(int oldSpace, int oldPlayer)
    {
        int thisStart = FindStartSpace(oldPlayer);
        _gameContainer.PlayerGoingBack = oldPlayer;
        _gameContainer.Animates.LocationFrom = _graphicsCP.SpaceList[oldSpace].Location;
        _gameContainer.Animates.LocationTo = _graphicsCP.SpaceList[thisStart].Location;
        var tempPlayer = _gameContainer.PlayerList![oldPlayer];
        tempPlayer.PieceList.RemoveSpecificItem(oldSpace);
        await _gameContainer.Animates.DoAnimateAsync();
        _gameContainer.PlayerGoingBack = 0;
        var thisSpace = _spaceList![thisStart];
        thisSpace.Player = oldPlayer;
        tempPlayer.PieceList.Add(thisStart);
        if (tempPlayer.PieceList.Count != 4)
        {
            throw new CustomBasicException("After going back to start, needs 4 pieces");
        }
        RepaintBoard();
    }
    private int CalculateSafety(SpaceInfo thisSpace, int lefts, bool willStart)
    {
        int newNum;
        if (willStart == false)
        {
            if (thisSpace.WhatBoard != EnumBoardStatus.IsHome)
            {
                newNum = lefts + 1;
            }
            else
            {
                newNum = thisSpace.SpaceNumber + lefts;
            }
        }
        else
        {
            newNum = lefts;
        }

        if (newNum == 0)
        {
            throw new CustomBasicException("Cannot be 0");
        }
        if (newNum > 4)
        {
            return 0;
        }
        var newCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.IsHome && items.ColorOwner == OurColor).ToBasicList();
        if (newCol.Count != 4)
        {
            throw new CustomBasicException("Need to have 4 home spaces");
        }
        var thisCol = newCol.Where(items => items.SpaceNumber < newNum).ToBasicList();
        if (thisSpace.WhatBoard == EnumBoardStatus.IsHome)
        {
            thisCol.KeepConditionalItems(items => items.SpaceNumber > thisSpace.SpaceNumber);
        }
        if (thisCol.Any(items => items.Player == _gameContainer.WhoTurn))
        {
            return 0;
        }
        return newCol.First(items => items.SpaceNumber == newNum).Index;
    }
    private int CalculateNewSpace(SpaceInfo thisSpace, int numberToUse)
    {
        BasicList<SpaceInfo> newCol;
        if (thisSpace.WhatBoard == EnumBoardStatus.IsStart)
        {
            newCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.OnBoard && items.ColorOwner != EnumColorChoice.None).ToBasicList();
            if (newCol.Count != 4)
            {
                throw new CustomBasicException("Need to have 4 start spaces");
            }
            return newCol.Single(items => items.ColorOwner == OurColor).Index;
        }
        if (thisSpace.WhatBoard == EnumBoardStatus.IsHome)
        {
            return CalculateSafety(thisSpace, numberToUse, false);
        }
        if (thisSpace.WhatBoard == EnumBoardStatus.OnCenter && _gameContainer.SaveRoot!.DiceNumber > 1)
        {
            return 0;
        }
        SpaceInfo nextSpace;
        if (thisSpace.WhatBoard == EnumBoardStatus.OnCenter)
        {
            int nums;
            if (OurColor == EnumColorChoice.Blue)
            {
                nums = 58;
            }
            else if (OurColor == EnumColorChoice.Green)
            {
                nums = 22;
            }
            else if (OurColor == EnumColorChoice.Red)
            {
                nums = 34;
            }
            else if (OurColor == EnumColorChoice.Yellow)
            {
                nums = 46;
            }
            else
            {
                throw new CustomBasicException("No color for the player");
            }
            nextSpace = _spaceList!.Values.Single(items => items.Index == nums);
            if (nextSpace.Player == _gameContainer.WhoTurn)
            {
                return 0;
            }
            return nextSpace.Index;
        }
        int toHome = WhenToGoHome;
        if (toHome == thisSpace.SpaceNumber)
        {
            return CalculateSafety(thisSpace, numberToUse, true);
        }
        int newNum = thisSpace.SpaceNumber;
        var thisCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.OnBoard).ToBasicList();
        for (int x = 1; x <= numberToUse; x++)
        {
            newNum++;
            if (newNum == 49)
            {
                newNum = 1;
            }
            if (newNum == toHome + 1 || newNum == 1 & toHome == 48)
            {
                return CalculateSafety(thisSpace, numberToUse - x, false);
            }
            nextSpace = thisCol.Single(items => items.SpaceNumber == newNum);
            if (nextSpace.Player == _gameContainer.WhoTurn)
            {
                return 0; //you can't jump your own space.
            }
        }
        nextSpace = thisCol.Single(items => items.SpaceNumber == newNum);
        return nextSpace.Index;
    }
    public async Task GetValidMovesAsync()
    {
        if (_gameContainer.Test.ImmediatelyEndGame)
        {
            await _gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        if (_gameContainer.SaveRoot!.DiceNumber == 0)
        {
            return;
        }
        MoveList = new();
        BasicList<int> firstCol = _gameContainer.SingleInfo!.PieceList.ToBasicList();
        if (firstCol.Count != 4)
        {
            throw new CustomBasicException("The player must have 4 pieces");
        }
        if (_gameContainer.SaveRoot.DiceNumber < 0 || _gameContainer.SaveRoot.DiceNumber > 6)
        {
            throw new CustomBasicException("The dice has to be from 1 to 6");
        }
        BasicList<int> thisCol = new();
        if (_gameContainer.SaveRoot.DiceNumber != 1 && _gameContainer.SaveRoot.DiceNumber != 6)
        {
            firstCol.ForEach(thisInt =>
            {
                if (!_spaceList!.Values.Any(items => items.Index == thisInt && items.WhatBoard == EnumBoardStatus.IsStart))
                {
                    thisCol.Add(thisInt);
                }
            });
        }
        else
        {
            thisCol = firstCol;
        }
        int newPlayer;
        MoveInfo thisMove;
        thisCol.ForEach(thisInt =>
        {
            var thisSpace = _spaceList![thisInt];
            var nextSpace = CalculateNewSpace(thisSpace, _gameContainer.SaveRoot.DiceNumber);
            if (nextSpace > 0)
            {
                var newSpace = _spaceList[nextSpace];
                if (newSpace.ColorOwner != EnumColorChoice.None)
                {
                    newPlayer = FindPlayer(newSpace.ColorOwner);
                }
                else
                {
                    newPlayer = 0;
                }
                if (newSpace.Player != thisSpace.Player && newSpace.ColorOwner == EnumColorChoice.None || newSpace.Player == 0 || newSpace.ColorOwner != EnumColorChoice.None && newSpace.Player != newPlayer)
                {
                    thisMove = new ();
                    thisMove.SpaceFrom = thisSpace.Index;
                    thisMove.SpaceTo = newSpace.Index;
                    MoveList.Add(thisMove);
                }
                if (newSpace.Index == 23 && OurColor == EnumColorChoice.Blue || newSpace.Index == 35 && OurColor == EnumColorChoice.Green || newSpace.Index == 47 && OurColor == EnumColorChoice.Red || newSpace.Index == 59 && OurColor == EnumColorChoice.Yellow)
                {
                    newSpace = _spaceList[81];
                    thisMove = new ();
                    thisMove.SpaceFrom = thisSpace.Index;
                    thisMove.SpaceTo = 81;
                    MoveList.Add(thisMove);
                }
            }
        });
        if (MoveList.Count == 0)
        {
            if (_gameContainer.SaveRoot.DiceNumber == 6)
            {
                _gameContainer.SaveRoot.Instructions = "No moves.  Will get another turn.";
                if (_gameContainer.Test!.NoAnimations == false)
                {
                    await _gameContainer.Delay!.DelaySeconds(.5);
                }
                await _gameContainer.StartNewTurnAsync!.Invoke();
                return;
            }
            await NoMovesEndTurnAsync();
            return;
        }
        int ourID = 0;
        if (_gameContainer.BasicData!.MultiPlayer)
        {
            ourID = _gameContainer.PlayerList!.GetSelf().Id;
        }
        if (_gameContainer.BasicData.MultiPlayer == false || _gameContainer.WhoTurn != ourID)
        {
            _gameContainer.SaveRoot.Instructions = $"Waiting for {_gameContainer.SingleInfo.NickName} to make a move";
        }
        else
        {
            _gameContainer.SaveRoot.Instructions = "Choose a piece to move";
        }
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task MakeMoveAsync(int space)
    {
        if (!MoveList.Any(items => items.SpaceFrom == space) && space == 0)
        {
            throw new CustomBasicException($"Space {space} was not found on the movelist");
        }
        var thisSpace = _spaceList![space];
        _gameContainer.MovePlayer = 0;
        if (PreviousPiece == 0)
        {
            int counts = MoveList.Count(items => items.SpaceFrom == space);
            if (counts == 2)
            {
                PreviousPiece = space;
                _gameContainer.SaveRoot!.Instructions = $"Decide to place piece in the center or move {_gameContainer.SaveRoot.DiceNumber} spaces";
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
        }
        int tempPiece = PreviousPiece;
        PreviousPiece = 0;
        RepaintBoard();
        MoveInfo thisMove;
        if (tempPiece == 0)
        {
            thisMove = MoveList.Single(items => items.SpaceFrom == space);
        }
        else
        {
            thisMove = MoveList.Single(items => items.SpaceTo == space);
            thisSpace = _spaceList[tempPiece];
        }
        var newSpace = _spaceList[thisMove.SpaceTo];
        thisSpace.Player = 0;
        int oldPlayer = newSpace.Player;
        _gameContainer.SingleInfo!.PieceList.RemoveSpecificItem(thisMove.SpaceFrom);
        if (thisSpace.WhatBoard == EnumBoardStatus.IsStart)
        {
            _gameContainer.SingleInfo.PieceList.Add(thisMove.SpaceTo);
            if (newSpace.Player > 0)
            {
                var otherPlayer = _gameContainer.PlayerList![oldPlayer];
                newSpace.Player = _gameContainer.WhoTurn;
                await BackToStartAsync(thisMove.SpaceTo, oldPlayer);
            }
            else
            {
                newSpace.Player = _gameContainer.WhoTurn;
                RepaintBoard();
            }
            if (_gameContainer.SaveRoot!.DiceNumber < 6)
            {
                await DoEndTurnAsync();
            }
            else
            {
                await DoAnotherTurnAsync();
            }
            return;
        }
        RepaintBoard();
        int nextMove;
        for (int x = 1; x <= _gameContainer.SaveRoot!.DiceNumber; x++)
        {
            if (x == _gameContainer.SaveRoot.DiceNumber && space == 81 && tempPiece > 0)
            {
                nextMove = 81;
            }
            else
            {
                nextMove = CalculateNewSpace(thisSpace, x);
            }
            _gameContainer.MovePlayer = nextMove;
            RepaintBoard();
            if (_gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(.1);
            }
        }
        _gameContainer.MovePlayer = 0;
        _gameContainer.SingleInfo.PieceList.Add(newSpace.Index);
        RepaintBoard();
        if (newSpace.Player != 0)
        {
            newSpace.Player = _gameContainer.WhoTurn;
            await BackToStartAsync(newSpace.Index, oldPlayer);
        }
        else
        {
            newSpace.Player = _gameContainer.WhoTurn;
        }
        if (IsGameOver || _gameContainer.Test.ImmediatelyEndGame)
        {
            await _gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        if (_gameContainer.SaveRoot.DiceNumber == 6)
        {
            await DoAnotherTurnAsync();
            return;
        }
        await DoEndTurnAsync();
    }
}
