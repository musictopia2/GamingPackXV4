﻿namespace Sorry.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses
{
    private readonly SorryGameContainer _gameContainer;
    private readonly GameBoardGraphicsCP _graphicsBoard;
    private readonly SorryVMData _model;
    private Dictionary<int, SpaceInfo>? _spaceList;
    public GameBoardProcesses(SorryGameContainer gameContainer, GameBoardGraphicsCP graphicsBoard, SorryVMData model)
    {
        _gameContainer = gameContainer;
        _graphicsBoard = graphicsBoard;
        _model = model;
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
    private int PreviousPiece
    {
        get
        {
            return _gameContainer.SaveRoot!.PreviousPiece;
        }
        set
        {
            _gameContainer.SaveRoot!.PreviousPiece = value;
        }
    }
    public void StartTurn()
    {
        OurColor = _gameContainer.SingleInfo!.Color;
        _gameContainer.MovePlayer = 0;
        _gameContainer.SaveRoot!.MovesMade = 0;
        _gameContainer.SaveRoot.SpacesLeft = 0;
        MoveList = new();
        _gameContainer.SaveRoot.HighlightList = new();
        PreviousPiece = 0;
        _gameContainer.SaveRoot.DidDraw = false;
        _gameContainer.SaveRoot.CurrentCard = new CardInfo();
        _gameContainer.SaveRoot.PreviousSplit = 0;
        OurColor = _gameContainer.SingleInfo.Color;
        _gameContainer.RepaintBoard();
    }
    public void LoadBoard()
    {
        _gameContainer.Animates.LongestTravelTime = 200;
        PopulateSpaces();
    }
    private void PopulateSpaces()
    {
        if (_spaceList != null)
        {
            return;
        }
        _spaceList = new System.Collections.Generic.Dictionary<int, SpaceInfo>();
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
                thisSpace.SpaceDesc = EnumSpaceType.None;
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
        for (x = 17; x <= 76; x++)
        {
            y += 1;
            thisSpace = new ();
            thisSpace.Index = x;
            thisSpace.WhatBoard = EnumBoardStatus.OnBoard;
            thisSpace.SpaceNumber = y;
            if (x == 17)
            {
                thisSpace.ColorOwner = EnumColorChoice.Blue;
                thisSpace.SpaceDesc = EnumSpaceType.OnStart;
            }
            else if (x == 32)
            {
                thisSpace.ColorOwner = EnumColorChoice.Yellow;
                thisSpace.SpaceDesc = EnumSpaceType.OnStart;
            }
            else if (x == 47)
            {
                thisSpace.ColorOwner = EnumColorChoice.Green;
                thisSpace.SpaceDesc = EnumSpaceType.OnStart;
            }
            else if (x == 62)
            {
                thisSpace.ColorOwner = EnumColorChoice.Red;
                thisSpace.SpaceDesc = EnumSpaceType.OnStart;
            }
            else
            {
                if (x == 74)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Blue;
                    thisSpace.SpaceDesc = EnumSpaceType.StartSlide;
                }
                else if (x > 74)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Blue;
                    thisSpace.SpaceDesc = EnumSpaceType.ContinueSlide;
                }
                else if (x == 22)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Blue;
                    thisSpace.SpaceDesc = EnumSpaceType.StartSlide;
                }
                else if (x == 26)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Blue;
                    thisSpace.SpaceDesc = EnumSpaceType.EndSlide;
                }
                else if (x > 22 && x < 26)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Blue;
                    thisSpace.SpaceDesc = EnumSpaceType.ContinueSlide;
                }
                else if (x == 29)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Yellow;
                    thisSpace.SpaceDesc = EnumSpaceType.StartSlide;
                }
                else if (x > 29 && x < 32)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Yellow;
                    thisSpace.SpaceDesc = EnumSpaceType.ContinueSlide;
                }
                else if (x == 37)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Yellow;
                    thisSpace.SpaceDesc = EnumSpaceType.StartSlide;
                }
                else if (x == 41)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Yellow;
                    thisSpace.SpaceDesc = EnumSpaceType.EndSlide;
                }
                else if (x > 37 && x < 41)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Yellow;
                    thisSpace.SpaceDesc = EnumSpaceType.ContinueSlide;
                }
                else if (x == 44)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Green;
                    thisSpace.SpaceDesc = EnumSpaceType.StartSlide;
                }
                else if (x > 44 && x < 47)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Green;
                    thisSpace.SpaceDesc = EnumSpaceType.ContinueSlide;
                }
                else if (x == 52)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Green;
                    thisSpace.SpaceDesc = EnumSpaceType.StartSlide;
                }
                else if (x == 56)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Green;
                    thisSpace.SpaceDesc = EnumSpaceType.EndSlide;
                }
                else if (x > 52 && x < 56)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Green;
                    thisSpace.SpaceDesc = EnumSpaceType.ContinueSlide;
                }
                else if (x == 59)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Red;
                    thisSpace.SpaceDesc = EnumSpaceType.StartSlide;
                }
                else if (x > 59 && x < 62)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Red;
                    thisSpace.SpaceDesc = EnumSpaceType.ContinueSlide;
                }
                else if (x == 67)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Red;
                    thisSpace.SpaceDesc = EnumSpaceType.StartSlide;
                }
                else if (x == 71)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Red;
                    thisSpace.SpaceDesc = EnumSpaceType.EndSlide;
                }
                else if (x > 67 && x < 71)
                {
                    thisSpace.ColorOwner = EnumColorChoice.Red;
                    thisSpace.SpaceDesc = EnumSpaceType.ContinueSlide;
                }
            }
            _spaceList.Add(x, thisSpace);
        }
        z = 1;
        q = 76;
        for (x = 1; x <= 4; x++)
        {
            for (y = 1; y <= 5; y++)
            {
                q += 1;
                thisSpace = new ();
                thisSpace.ColorOwner = EnumColorChoice.FromValue(z);
                thisSpace.SpaceNumber = y;
                thisSpace.Index = q;
                thisSpace.WhatBoard = EnumBoardStatus.IsSafety;
                thisSpace.SpaceDesc = EnumSpaceType.None;
                _spaceList.Add(q, thisSpace);
            }
            z += 1;
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
    public bool IsValidMove(int index)
    {
        if (PreviousPiece == index)
        {
            return true;
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
    private async Task DoEndTurnAsync()
    {
        _gameContainer.SaveRoot!.Instructions = "Move was made successfully";
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelaySeconds(.5);
        }
        await _gameContainer.EndTurnAsync!.Invoke();
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
    private void ErrorCheckPlayers()
    {
        _gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.HowManyHomePieces + thisPlayer.PieceList.Count != 4)
            {
                throw new CustomBasicException("Must always have 4 pieces");
            }
            thisPlayer.PieceList.ForEach(thisPiece =>
            {
                var tempList = _spaceList!.Values.Where(items => items.Player == thisPlayer.Id).ToBasicList();
                var thisSpace = _spaceList.Values.Single(items => items.Index == thisPiece);
                if (thisSpace.Player != thisPlayer.Id)
                {
                    throw new CustomBasicException($"Player {thisPlayer.NickName} has piece {thisPiece} for UI but the View Model shows its not there.  They have {thisPlayer.HowManyHomePieces} at home.  Total pieces not at home is {thisPlayer.PieceList.Count}");
                }
            });
        });
        //i can also risk doing the error checking to check the spacelist as well.
        var tempList = _spaceList!.Values.Where(item => item.Player != 0).ToBasicList();
        tempList.ForEach(item =>
        {
            SorryPlayerItem player = _gameContainer.PlayerList[item.Player];
            if (player.PieceList.Any(piece => piece == item.Index) == false)
            {
                throw new CustomBasicException("Player does not have the piece that is in the space list.  Possible slide problem.  Rethink");
            }
        });
    }
    public void ClearBoard()
    {
        _gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.PieceList.Clear();
            thisPlayer.HowManyHomePieces = 0;
        });
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
        ErrorCheckPlayers();
        _gameContainer.RepaintBoard();
    }
    private bool IsGameOver
    {
        get
        {
            if (_gameContainer.SingleInfo!.HowManyHomePieces == 4)
            {
                return true;
            }
            if (_gameContainer.SingleInfo.HowManyHomePieces > 4)
            {
                throw new CustomBasicException("Can never have more than 4 pieces at home.  Rethink");
            }
            return false;
        }
    }
    private bool OtherPlayersOnBoard => _spaceList!.Values.Any(items => items.WhatBoard == EnumBoardStatus.OnBoard && items.Player > 0 && items.Player != _gameContainer.WhoTurn);
    private BasicList<SpaceInfo> OpponentSpaces()
    {
        return _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.OnBoard && items.Player > 0 && items.Player != _gameContainer.WhoTurn).ToBasicList();
    }
    private int CalculateSafety(SpaceInfo thisSpace, int lefts, bool willStart) //well see.
    {
        int newNum;
        if (willStart == false)
        {
            if (thisSpace.WhatBoard != EnumBoardStatus.IsSafety)
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
        if (newNum == 0 && lefts > 0)
        {
            throw new CustomBasicException("Cannot be 0");
        }
        if (newNum < 0 && lefts > 0)
        {
            throw new CustomBasicException($"Cannot be {lefts} because its not a backwards move");
        }
        if (newNum <= 0)
        {
            newNum = Math.Abs(newNum);
            return OutOfSafetySpace(newNum);
        }
        if (newNum == 6)
        {
            return 100; //which means completely home.
        }
        if (newNum > 5)
        {
            return 0;
        }
        var newCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.IsSafety && items.ColorOwner == OurColor).ToBasicList();
        if (newCol.Count != 5)
        {
            throw new CustomBasicException("Need to have 5 safety spaces");
        }
        return newCol.First(items => items.SpaceNumber == newNum).Index;
    }
    private int OutOfSafetySpace(int lefts)
    {
        int whens = WhenToGoToSafety;
        var thisSpace = _spaceList!.Values.First(items => items.SpaceNumber == whens);
        whens = thisSpace.Index;
        for (int x = 1; x <= lefts; x++)
        {
            whens--;
        }
        return whens;
    }
    private int WhenToGoToSafety
    {
        get
        {
            int nums;
            if (OurColor == EnumColorChoice.Blue)
            {
                nums = 75;
            }
            else if (OurColor == EnumColorChoice.Green)
            {
                nums = 45;
            }
            else if (OurColor == EnumColorChoice.Red)
            {
                nums = 60;
            }
            else if (OurColor == EnumColorChoice.Yellow)
            {
                nums = 30;
            }
            else
            {
                throw new CustomBasicException("Need a color for the player");
            }
            return _spaceList![nums].SpaceNumber;
        }
    }
    private bool HasPieceOnStart(int spaceNumber)
    {
        var thisSpace = _spaceList![spaceNumber];
        return thisSpace.WhatBoard == EnumBoardStatus.IsStart;
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
    private async Task BackToStartAsync(int oldSpace, int oldPlayer)
    {
        int thisStart = FindStartSpace(oldPlayer);
        _gameContainer.PlayerGoingBack = oldPlayer;
        _gameContainer.Animates.LocationFrom = _graphicsBoard.LocationOfSpace(oldSpace);
        _gameContainer.Animates.LocationTo = _graphicsBoard.LocationOfSpace(thisStart);
        var tempPlayer = _gameContainer.PlayerList![oldPlayer];
        tempPlayer.PieceList.RemoveSpecificItem(oldSpace);
        _gameContainer.RepaintBoard();
        await Task.Delay(10);
        await _gameContainer.Animates.DoAnimateAsync();
        _gameContainer.PlayerGoingBack = 0;
        var thisSpace = _spaceList![thisStart];
        thisSpace.Player = oldPlayer;
        tempPlayer.PieceList.Add(thisStart); //you are now at start.
        if (tempPlayer.PieceList.Count + tempPlayer.HowManyHomePieces != 4)
        {
            throw new CustomBasicException("After going back to start, needs 4 pieces");
        }
        _gameContainer.RepaintBoard();
    }
    private async Task SlideAnimationAsync(int startSpace, SpaceInfo endSpace, int slidePlayer)
    {
        var playerObj = _gameContainer.PlayerList![slidePlayer];
        playerObj.PieceList.RemoveSpecificItem(startSpace);
        SpaceInfo oldSpace = _spaceList![startSpace];
        if (oldSpace.Player == 0)
        {
            throw new CustomBasicException($"Must be a player on space {startSpace}");
        }
        int tempPlayer = oldSpace.Player;
        oldSpace.Player = 0;
        int newPlayer = endSpace.Player;
        _gameContainer.PlayerGoingBack = slidePlayer;
        _gameContainer.Animates.LocationFrom = _graphicsBoard.LocationOfSpace(oldSpace.Index);
        _gameContainer.Animates.LocationTo = _graphicsBoard.LocationOfSpace(endSpace.Index);
        _gameContainer.RepaintBoard();
        await _gameContainer.Animates.DoAnimateAsync();
        _gameContainer.PlayerGoingBack = 0;
        playerObj.PieceList.Add(endSpace.Index);
        if (newPlayer != 0)
        {
            await BackToStartAsync(endSpace.Index, newPlayer);
        }
        endSpace.Player = tempPlayer;
        _gameContainer.RepaintBoard();
    }
    private int CalculateNewSpace(SpaceInfo thisSpace, int numberToUse, bool fromValids)
    {
        BasicList<SpaceInfo> newCol;
        if (thisSpace.WhatBoard == EnumBoardStatus.IsStart)
        {
            newCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.OnBoard && items.ColorOwner != EnumColorChoice.None && items.SpaceDesc == EnumSpaceType.OnStart).ToBasicList();
            if (newCol.Count != 4)
            {
                throw new CustomBasicException("Need to have 4 start spaces");
            }
            return newCol.Single(items => items.ColorOwner == OurColor).Index;
        }
        if (thisSpace.WhatBoard == EnumBoardStatus.IsSafety)
        {
            return CalculateSafety(thisSpace, numberToUse, false);
        }
        SpaceInfo nextSpace;
        int toSafety = WhenToGoToSafety;
        if (WhenToGoToSafety == thisSpace.Index)
        {
            return CalculateSafety(thisSpace, numberToUse, true);
        }
        int newNum = thisSpace.SpaceNumber;
        var thisCol = _spaceList!.Values.Where(items => items.WhatBoard == EnumBoardStatus.OnBoard).ToBasicList();
        int finalNums;
        bool isBackwards;
        if (numberToUse > 0)
        {
            isBackwards = false;
        }
        else
        {
            isBackwards = true;
        }
        finalNums = Math.Abs(numberToUse);
        for (int x = 1; x <= finalNums; x++)
        {
            if (isBackwards == false)
            {
                newNum++;
            }
            else
            {
                newNum--;
            }
            if (newNum == 61 && isBackwards == false)
            {
                newNum = 1;
            }
            else if (newNum == 0 && isBackwards)
            {
                newNum = 60;
            }
            if (isBackwards == false && (newNum == toSafety + 1 || newNum == 76 && toSafety == 75))
            {
                return CalculateSafety(thisSpace, numberToUse - x, false);
            }
            nextSpace = thisCol.Single(items => items.SpaceNumber == newNum);
            if (nextSpace.Player == _gameContainer.WhoTurn && x == finalNums && fromValids)
            {
                return 0;
            }
        }
        nextSpace = thisCol.Single(items => items.SpaceNumber == newNum);
        return nextSpace.Index;
    }
    private BasicList<SpaceInfo> OnBoardList()
    {
        return _spaceList!.Values.Where(items => items.WhatBoard != EnumBoardStatus.IsStart && items.Player == _gameContainer.WhoTurn).ToBasicList();
    }
    private async Task AnimateTradeAsync(int spaceFrom, int spaceTo, int oldPlayer, int newPlayer, bool removeOld = true)
    {
        _gameContainer.PlayerGoingBack = oldPlayer;
        _gameContainer.Animates.LocationFrom = _graphicsBoard.LocationOfSpace(spaceFrom);
        _gameContainer.Animates.LocationTo = _graphicsBoard.LocationOfSpace(spaceTo);
        SorryPlayerItem playerObj;
        if (removeOld)
        {
            var oldSpace = _spaceList![spaceFrom];
            if (oldSpace.Player == 0)
            {
                throw new CustomBasicException("No player there");
            }
            playerObj = _gameContainer.PlayerList![oldPlayer];
            playerObj.PieceList.RemoveSpecificItem(oldSpace.Index);
            oldSpace.Player = 0;
        }
        _gameContainer.RepaintBoard();
        await _gameContainer.Animates.DoAnimateAsync();
        _gameContainer.PlayerGoingBack = 0;
        var newSpace = _spaceList![spaceTo];
        newSpace.Player = newPlayer;
        playerObj = _gameContainer.PlayerList![newPlayer];
        playerObj.PieceList.Add(newSpace.Index);
    }
    private async Task MoveSlideAsync(SpaceInfo newSpace, int newPlayer)
    {
        var tempPlayer = _gameContainer.PlayerList![newPlayer];
        _gameContainer.RepaintBoard();
        if (newSpace.SpaceDesc != EnumSpaceType.StartSlide)
        {
            return;
        }
        if (newSpace.ColorOwner == tempPlayer.Color)
        {
            return;
        }
        BasicList<SpaceInfo> slideList = new();
        SpaceInfo tempSpace;
        int x = newSpace.Index;
        do
        {
            x++;
            if (x == 77)
            {
                x = 17;
            }
            tempSpace = _spaceList![x];
            slideList.Add(tempSpace);
            if (tempSpace.SpaceDesc != EnumSpaceType.ContinueSlide)
            {
                break;
            }
        } while (true);
        tempSpace = slideList.Last();
        await SlideAnimationAsync(newSpace.Index, tempSpace, newPlayer);
        slideList.RemoveLastItem();
        await slideList.ForEachAsync(async finSpace =>
        {
            if (finSpace.Player != 0)
            {
                await BackToStartAsync(finSpace.Index, finSpace.Player);
                finSpace.Player = 0;
            }
        });
        _gameContainer.RepaintBoard();
    }
    private async Task MoveSlideAsync(SpaceInfo newSpace)
    {
        await MoveSlideAsync(newSpace, _gameContainer.WhoTurn);
    }
    private async Task TradePlacesAsync(int spaceFrom, int spaceTo)
    {
        PreviousPiece = 0;
        SpaceInfo lastSpace = _spaceList![spaceFrom];
        SpaceInfo nextSpace = _spaceList[spaceTo];
        int lastPlayer = lastSpace.Player;
        int nextPlayer = nextSpace.Player;
        await AnimateTradeAsync(lastSpace.Index, spaceTo, lastPlayer, lastPlayer);
        var thisPlayer = _gameContainer.PlayerList![nextPlayer];
        thisPlayer.PieceList.RemoveSpecificItem(spaceTo);
        _gameContainer.RepaintBoard();
        if (_gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelayMilli(200);
        }
        await AnimateTradeAsync(nextSpace.Index, spaceFrom, nextPlayer, nextPlayer, false);
        await MoveSlideAsync(lastSpace);
        await MoveSlideAsync(nextSpace, lastPlayer);
        await EndMoveAsync();
    }
    private async Task SorryPlayerAsync(MoveInfo thisMove)
    {
        var thisSpace = _spaceList![thisMove.SpaceTo];
        int player = thisSpace.Player;
        await AnimateTradeAsync(thisMove.SpaceFrom, thisMove.SpaceTo, _gameContainer.WhoTurn, _gameContainer.WhoTurn);
        await BackToStartAsync(thisMove.SpaceTo, player);
        PreviousPiece = 0;
        await MoveSlideAsync(thisSpace);
        await EndMoveAsync();
    }
    private BasicList<MoveInfo> MoveListWithPiece(int space)
    {
        return MoveList.Where(items => items.SpaceFrom == space).ToBasicList();
    }
    private MoveInfo GetMove(int spaceFrom, int spaceTo)
    {
        if (spaceFrom == 0 && spaceTo == 0)
        {
            throw new CustomBasicException("Spacefrom or spaceto must be filled out; not blank");
        }
        foreach (var thisMove in MoveList)
        {
            if (spaceFrom > 0 && spaceTo > 0)
            {
                if (thisMove.SpaceFrom == spaceFrom && thisMove.SpaceTo == spaceTo)
                {
                    return thisMove;
                }
            }
            else
            {
                if (spaceFrom > 0 && thisMove.SpaceFrom == spaceFrom)
                {
                    return thisMove;
                }
                if (spaceTo > 0 && thisMove.SpaceTo == spaceTo)
                {
                    return thisMove;
                }
            }
        }
        throw new CustomBasicException("Move not found.  Rethink");
    }
    private async Task EndMoveAsync()
    {
        _gameContainer.SaveRoot!.SpacesLeft = 0;
        _gameContainer.RepaintBoard();
        ErrorCheckPlayers();
        if (_gameContainer.StartNewTurnAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the startnewturn.  Rethink");
        }
        if (IsGameOver || _gameContainer.Test.ImmediatelyEndGame)
        {
            await _gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        if (_gameContainer.SaveRoot!.CurrentCard!.AnotherTurn)
        {
            _gameContainer.SaveRoot.Instructions = "Will get another turn";
            if (_gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(.5);
            }
            await _gameContainer.StartNewTurnAsync();
            return;
        }
        await DoEndTurnAsync();
    }
    private async Task ResumeMoveAsync(int howMany, SpaceInfo thisSpace, MoveInfo thisMove)
    {
        SpaceInfo newSpace;
        if (thisMove.SpaceTo < 100)
        {
            newSpace = _spaceList![thisMove.SpaceTo];
        }
        else
        {
            newSpace = new SpaceInfo();
        }
        _gameContainer.SingleInfo!.PieceList.RemoveSpecificItem(thisMove.SpaceFrom);
        thisSpace.Player = 0;
        if (thisSpace.WhatBoard == EnumBoardStatus.IsStart)
        {
            if (newSpace.Player != 0)
            {
                await BackToStartAsync(newSpace.Index, newSpace.Player);
            }
            newSpace.Player = _gameContainer.WhoTurn;
            _gameContainer.SingleInfo.PieceList.Add(newSpace.Index);
            _gameContainer.RepaintBoard();
            return;
        }
        bool isBackwards = howMany < 0;
        _gameContainer.RepaintBoard();
        await Task.Delay(10);
        int newNum;
        int nextMove;
        for (int x = 1; x <= Math.Abs(howMany); x++)
        {
            if (isBackwards)
            {
                newNum = x * -1;
            }
            else
            {
                newNum = x;
            }
            nextMove = CalculateNewSpace(thisSpace, newNum, false);
            _gameContainer.MovePlayer = nextMove;
            _gameContainer.RepaintBoard();
            if (_gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(.1);
            }
        }
        _gameContainer.MovePlayer = 0;
        if (newSpace.Player != 0)
        {
            _gameContainer.SingleInfo.PieceList.Add(newSpace.Index);
            await BackToStartAsync(newSpace.Index, newSpace.Player);
            newSpace.Player = _gameContainer.WhoTurn;
        }
        else
        {
            newSpace.Player = _gameContainer.WhoTurn;
            if (newSpace.Index > 0)
            {
                _gameContainer.SingleInfo.PieceList.Add(newSpace.Index);
            }
            else
            {
                _gameContainer.SingleInfo.HowManyHomePieces++;
            }
        }
        if (newSpace.Index > 0)
        {
            await MoveSlideAsync(newSpace);
        }
    }
    private async Task LastMovesAsync(MoveInfo thisMove)
    {
        int howMany = 7 - thisMove.NumberUsed;
        MoveList = new();
        var tempList = OnBoardList();
        int nextSpace1;
        SpaceInfo newSpace;
        int newPlayer;
        MoveInfo output;
        tempList.ForEach(thisSpace =>
        {
            nextSpace1 = CalculateNewSpace(thisSpace, howMany, true);
            if (nextSpace1 > 0)
            {
                if (nextSpace1 < 100)
                {
                    newSpace = _spaceList![nextSpace1];
                    if (newSpace.ColorOwner != EnumColorChoice.None)
                    {
                        newPlayer = FindPlayer(newSpace.ColorOwner);
                    }
                    else
                    {
                        newPlayer = 0;
                    }
                }
                else
                {
                    newSpace = new ();
                    newPlayer = 0;
                }
                if (newSpace.Player != thisSpace.Player || newSpace.Player == 0)
                {
                    if (thisSpace.Index != _gameContainer!.SaveRoot!.PreviousSplit)
                    {
                        output = new ();
                        output.SpaceFrom = thisSpace.Index;
                        if (newSpace.Index > 0)
                        {
                            output.SpaceTo = newSpace.Index;
                        }
                        else
                        {
                            output.SpaceTo = 100;
                        }
                        output.NumberUsed = howMany;
                        MoveList.Add(output);
                    }
                }
            }
        });
        if (MoveList.Count == 0)
        {
            throw new CustomBasicException("There are no moves for part 2 of the split.  Find out what happened");
        }
        _gameContainer!.SaveRoot!.SpacesLeft = howMany;
        _gameContainer.RepaintBoard();
        int ourID = 0;
        if (_gameContainer.BasicData!.MultiPlayer)
        {
            ourID = _gameContainer.PlayerList!.GetSelf().Id;
        }
        if (_gameContainer.BasicData.MultiPlayer == false || _gameContainer.WhoTurn != ourID)
        {
            _gameContainer.SaveRoot.Instructions = $"Waiting for {_gameContainer.SingleInfo!.NickName} to make a move";
        }
        else
        {
            _gameContainer.SaveRoot.Instructions = "Choose a piece to move";
        }
        ErrorCheckPlayers();
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    private int DistanceToHome(SpaceInfo thisSpace)
    {
        int y;
        if (thisSpace.WhatBoard == EnumBoardStatus.IsSafety)
        {
            if (thisSpace.ColorOwner == OurColor)
            {
                y = 5;
                for (int i = 1; 1 <= 5; i++)
                {
                    if (thisSpace.SpaceNumber == i)
                    {
                        return y;
                    }
                    y--;
                }
                throw new CustomBasicException("The space number has to be from 1 to 5.  Rethink");
            }
            throw new CustomBasicException("The color does not match the current player");
        }
        if (thisSpace.WhatBoard == EnumBoardStatus.IsStart)
        {
            throw new CustomBasicException("Sorry; the player is at start.  Therefore; cannot calculate the distance to home");
        }
        int toSafety = WhenToGoToSafety;
        if (toSafety == thisSpace.SpaceNumber)
        {
            return 6;
        }
        int distanceToSafety;
        int z = thisSpace.SpaceNumber;
        int x = 0;
        do
        {
            x++;
            z++;
            if (z == 61)
            {
                z = 1;
            }
            if (z == toSafety)
            {
                distanceToSafety = x;
                break;
            }

        } while (true);
        return distanceToSafety + 6;
    }
    private static BasicList<PointF> GetComboList(int maxs)
    {
        if (maxs < 2)
        {
            return new(); //needs at least 2.
        }
        BasicList<PointF> output = new();
        int x;
        int y;
        for (x = 1; x <= maxs; x++)
        {
            for (y = x + 1; y <= maxs; y++)
            {
                var thisPoint = new PointF(x, y);
                output.Add(thisPoint);
            }
        }
        return output;
    }
    private void GetValidSplits(IBasicList<SpaceInfo> thisCol)
    {
        var comboList = GetComboList(thisCol.Count);
        SpaceInfo firstSpace;
        SpaceInfo secondSpace;
        int newLeft1;
        int newLeft2;
        BasicList<PointF> newCombos = new();
        comboList.ForEach(thisPoint =>
        {
            firstSpace = thisCol[(int)thisPoint.X - 1];
            secondSpace = thisCol[(int)thisPoint.Y - 1];
            newLeft1 = DistanceToHome(firstSpace);
            newLeft2 = DistanceToHome(secondSpace);
            if (newLeft1 + newLeft2 >= 7)
            {
                newCombos.Add(thisPoint);
            }
        });
        if (newCombos.Count == 0)
        {
            return;
        }
        int x;
        int nextSpace1;
        int nextSpace2;
        SpaceInfo lastSpace1;
        SpaceInfo lastSpace2;
        int newPlayer1;
        int newPlayer2;
        MoveInfo thisMove;
        int y;
        foreach (var thisPoint in newCombos)
        {
            firstSpace = thisCol[(int)thisPoint.X - 1];
            secondSpace = thisCol[(int)thisPoint.Y - 1];
            y = 0;
            for (x = 6; x >= 1; x += -1)
            {
                y++;
                nextSpace1 = CalculateNewSpace(firstSpace, x, true);
                nextSpace2 = CalculateNewSpace(secondSpace, y, true);
                if (nextSpace1 > 0 || nextSpace2 > 0)
                {
                    if (nextSpace1 < 100 && nextSpace1 > 0)
                    {
                        lastSpace1 = _spaceList![nextSpace1];
                        if (lastSpace1.ColorOwner != EnumColorChoice.None)
                        {
                            newPlayer1 = FindPlayer(lastSpace1.ColorOwner);
                        }
                        else
                        {
                            newPlayer1 = 0;
                        }
                    }
                    else
                    {
                        lastSpace1 = new SpaceInfo();
                        newPlayer1 = 0;
                    }
                    if (nextSpace2 < 100 && nextSpace2 > 0)
                    {
                        lastSpace2 = _spaceList![nextSpace2];
                        if (lastSpace2.ColorOwner != EnumColorChoice.None)
                        {
                            newPlayer2 = FindPlayer(lastSpace2.ColorOwner);
                        }
                        else
                        {
                            newPlayer2 = 0;
                        }
                    }
                    else
                    {
                        lastSpace2 = new ();
                        newPlayer2 = 0;
                    }
                    if (lastSpace1.Player == firstSpace.Player)
                    {
                        nextSpace1 = 0;
                    }
                    if (lastSpace2.Player == secondSpace.Player)
                    {
                        nextSpace2 = 0;
                    }
                    if (nextSpace1 > 0)
                    {
                        thisMove = new ();
                        thisMove.SpaceFrom = firstSpace.Index;
                        thisMove.NumberUsed = x;
                        thisMove.SpaceTo = nextSpace1;
                        MoveList.Add(thisMove);
                    }
                    if (nextSpace2 > 0)
                    {
                        thisMove = new ();
                        thisMove.SpaceFrom = secondSpace.Index;
                        thisMove.NumberUsed = y;
                        thisMove.SpaceTo = nextSpace2;
                        MoveList.Add(thisMove);
                    }
                    if (nextSpace2 == 100 && newCombos.Count == 1)
                    {
                        return;
                    }
                }
            }
        }
    }
    public bool HasRequiredMove
    {
        get
        {
            if (!_gameContainer.SaveRoot!.DidDraw)
            {
                return true; //you have not drawn yet
            }
            return MoveList.Any(items => items.IsOptional == false);
        }
    }
    public void ShowDraw()
    {
        _gameContainer.SaveRoot!.DidDraw = true;
        _model.CardDetails = _gameContainer.SaveRoot.CurrentCard!.Details;
        _gameContainer.RepaintBoard();
    }
    public bool CanGoHome(EnumColorChoice color)
    {
        if (OurColor != color)
        {
            return false;
        }
        if (MoveList.Any(items => items.SpaceTo == 100))
        {
            return PreviousPiece > 0;
        }
        return false;
    }
    private CardInfo CurrentCard => _gameContainer.SaveRoot!.CurrentCard!;
    public async Task GetValidMovesAsync()
    {
        if (CurrentCard.Deck == 0)
        {
            return; //because you did not draw yet.
        }
        MoveList = new();
        BasicList<int> firstCol = _gameContainer.SingleInfo!.PieceList.ToBasicList();
        if (firstCol.Count > 4)
        {
            throw new CustomBasicException("The player must have 4 pieces at the most");
        }
        BasicList<int> thisCol = new();
        if (CurrentCard.CanTakeFromStart || CurrentCard.Sorry && OtherPlayersOnBoard)
        {
            thisCol = firstCol;
        }
        else
        {
            firstCol.ForEach(thisInt =>
            {
                if (HasPieceOnStart(thisInt) == false)
                {
                    thisCol.Add(thisInt);
                }
            });
        }
        BasicList<SpaceInfo> tempList;
        if (CurrentCard.Sorry || CurrentCard.Trade)
        {
            tempList = OpponentSpaces();
        }
        else
        {
            tempList = new();
        }
        SpaceInfo thisSpace;
        MoveInfo thisMove;
        int howMany;
        int nextSpace1;
        int nextSpace2;
        int newPlayer;
        SpaceInfo newSpace;
        for (int x = 1; x <= thisCol.Count; x++)
        {
            if (thisCol[x - 1] == 0)
            {
                throw new CustomBasicException("The piece cannot be 0.  Find out what happened");
            }
            thisSpace = _spaceList![thisCol[x - 1]];
            if (CurrentCard.Sorry)
            {
                tempList.ForEach(tempSpace =>
                {
                    thisMove = new MoveInfo();
                    if (thisSpace.WhatBoard == EnumBoardStatus.IsStart)
                    {
                        thisMove.SpaceFrom = thisSpace.Index;
                        thisMove.SpaceTo = tempSpace.Index;
                        MoveList.Add(thisMove);
                    }
                });
            }
            else
            {
                if (CurrentCard.SplitMove)
                {
                    howMany = 7;
                }
                else if (CurrentCard.SpacesForward == 0)
                {
                    howMany = CurrentCard.SpacesBackwards * -1;
                }
                else
                {
                    howMany = CurrentCard.SpacesForward;
                }
                nextSpace1 = CalculateNewSpace(thisSpace, howMany, true);
                if (nextSpace1 > 0)
                {
                    if (nextSpace1 < 100)
                    {
                        newSpace = _spaceList[nextSpace1];
                        if (newSpace.ColorOwner != EnumColorChoice.None)
                        {
                            newPlayer = FindPlayer(newSpace.ColorOwner);
                        }
                        else
                        {
                            newPlayer = 0;
                        }
                        if (newSpace.Player != thisSpace.Player || newSpace.Player == 0)
                        {
                            thisMove = new ();
                            thisMove.SpaceFrom = thisSpace.Index;
                            thisMove.SpaceTo = newSpace.Index;
                            if (howMany < 0)
                            {
                                thisMove.IsBackwards = true;
                            }
                            MoveList.Add(thisMove);
                        }
                    }
                    else
                    {
                        thisMove = new ();
                        thisMove.SpaceFrom = thisSpace.Index;
                        thisMove.SpaceTo = 100;
                        MoveList.Add(thisMove);
                    }
                }
                if (CurrentCard.SpacesBackwards > 0 && CurrentCard.SpacesForward > 0)
                {
                    howMany = CurrentCard.SpacesBackwards * -1;
                    nextSpace2 = CalculateNewSpace(thisSpace, howMany, true);
                    if (nextSpace2 > 0)
                    {
                        newSpace = _spaceList[nextSpace2];
                        if (newSpace.ColorOwner != EnumColorChoice.None)
                        {
                            newPlayer = FindPlayer(newSpace.ColorOwner);
                        }
                        else
                        {
                            newPlayer = 0;
                        }
                        if (newSpace.Player != thisSpace.Player || newSpace.Player == 0)
                        {
                            thisMove = new ();
                            thisMove.SpaceFrom = thisSpace.Index;
                            thisMove.SpaceTo = newSpace.Index;
                            thisMove.IsBackwards = true;
                            MoveList.Add(thisMove);
                        }
                    }
                }
                if (CurrentCard.Trade && thisSpace.WhatBoard == EnumBoardStatus.OnBoard)
                {
                    tempList.ForConditionalItems(items => items.WhatBoard == EnumBoardStatus.OnBoard, finalSpace =>
                    {
                        thisMove = new ();
                        thisMove.SpaceFrom = thisSpace.Index;
                        thisMove.SpaceTo = finalSpace.Index;
                        thisMove.IsOptional = true;
                        MoveList.Add(thisMove);
                    });
                }
            }
        }
        if (CurrentCard.SplitMove)
        {
            var boardList = OnBoardList();
            if (boardList.Count > 1)
            {
                GetValidSplits(boardList);
            }
        }
        if (MoveList.Count == 0)
        {
            if (_gameContainer.StartNewTurnAsync == null)
            {
                throw new CustomBasicException("Nobody is handling the start new turn.  Rethink");
            }
            ErrorCheckPlayers();
            if (CurrentCard.AnotherTurn)
            {
                _gameContainer.SaveRoot!.Instructions = "No moves.  Will get another turn.";
                if (_gameContainer.Test!.NoAnimations == false)
                {
                    await _gameContainer.Delay!.DelaySeconds(.5);
                }
                await _gameContainer.StartNewTurnAsync(); //i think
                return;
            }
            await NoMovesEndTurnAsync();
            return;
        }
        await OurContinueTurnAsync();
    }
    private async Task OurContinueTurnAsync()
    {
        int ourID = 0;
        if (_gameContainer.BasicData!.MultiPlayer)
        {
            ourID = _gameContainer.PlayerList!.GetSelf().Id;
        }
        if (_gameContainer.BasicData.MultiPlayer == false || _gameContainer.WhoTurn != ourID)
        {
            _gameContainer.SaveRoot!.Instructions = $"Waiting for {_gameContainer.SingleInfo!.NickName} to make a move";
        }
        else
        {
            _gameContainer.SaveRoot!.Instructions = "Choose a piece to move";
        }
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task MakeMoveAsync(int Space)
    {
        var tempList1 = MoveList;
        _gameContainer.SaveRoot!.HighlightList = new();
        SpaceInfo thisSpace;
        if (PreviousPiece == Space) //means changing your mind.
        {
            PreviousPiece = 0;
            _gameContainer.RepaintBoard();
            await OurContinueTurnAsync();
            return;
        }
        if (Space < 100)
        {
            thisSpace = _spaceList![Space];
        }
        else
        {
            thisSpace = new SpaceInfo();
        }
        MoveInfo thisMove;
        BasicList<MoveInfo> tempList;
        if (PreviousPiece == 0)
        {
            tempList = MoveListWithPiece(Space);
            if (tempList.Count > 1)
            {
                PreviousPiece = Space;
                _gameContainer.SaveRoot.HighlightList.Add(PreviousPiece);
                _gameContainer.SaveRoot.HighlightList.AddRange(tempList.Select(items => items.SpaceTo));
                _gameContainer.RepaintBoard();
                _gameContainer.SaveRoot.Instructions = "This is a multiple part move.  Continue making your move";
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            if (tempList.Count == 0)
            {
                throw new CustomBasicException("There are no moves.  Find out what happened");
            }
            thisMove = tempList.First();
            if (thisMove.IsOptional)
            {
                PreviousPiece = Space;
                Space = thisMove.SpaceTo;
            }
        }
        if (PreviousPiece == 0)
        {
            thisMove = GetMove(Space, 0);
        }
        else
        {
            thisMove = GetMove(PreviousPiece, Space);
            thisSpace = _spaceList![PreviousPiece]; //i think this was missing.
        }
        if (CurrentCard.Sorry)
        {
            await SorryPlayerAsync(thisMove);
            return;
        }
        if (CurrentCard.Trade && thisMove.IsOptional)
        {
            await TradePlacesAsync(Space, PreviousPiece);
            return;
        }
        PreviousPiece = 0;
        if (thisMove.IsBackwards)
        {
            await ResumeMoveAsync(CurrentCard.SpacesBackwards * -1, thisSpace, thisMove);
            await EndMoveAsync();
            return;
        }
        if (thisMove.NumberUsed == 0)
        {
            await ResumeMoveAsync(CurrentCard.SpacesForward, thisSpace, thisMove);
            await EndMoveAsync();
            return;
        }
        _gameContainer.SaveRoot.MovesMade++;
        await ResumeMoveAsync(thisMove.NumberUsed, thisSpace, thisMove);
        if (_gameContainer.SaveRoot.MovesMade == 2)
        {
            await EndMoveAsync();
            return;
        }
        if (_gameContainer.SaveRoot.MovesMade > 2)
        {
            throw new CustomBasicException("Only 2 moves can be made.  This means there is a problem.  Find out what happened");
        }
        _gameContainer.SaveRoot.PreviousSplit = thisMove.SpaceTo;
        await LastMovesAsync(thisMove);
    }
}