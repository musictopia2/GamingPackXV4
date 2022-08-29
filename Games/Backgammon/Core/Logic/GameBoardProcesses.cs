namespace Backgammon.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses
{
    private readonly BackgammonGameContainer _gameContainer;
    private readonly GameBoardGraphicsCP _graphicsBoard;
    private readonly BackgammonVMData _model;
    public GameBoardProcesses(BackgammonGameContainer gameContainer, GameBoardGraphicsCP graphicsBoard, BackgammonVMData model)
    {
        _gameContainer = gameContainer;
        _graphicsBoard = graphicsBoard;
        _model = model;
    }
    #region "Reverse Processes"
    internal static int GetReversedID(int oldID)
    {
        if (oldID == 0)
        {
            return 27;
        }
        if (oldID == 25)
        {
            return 26;
        }
        if (oldID == 26)
        {
            return 25;
        }
        if (oldID == 27)
        {
            return 0;
        }
        return 25 - oldID;
    }
    private bool NeedsReversed(int playerConsidered)
    {
        var tempPlayer = _gameContainer.PlayerList![playerConsidered];
        if (tempPlayer.PlayerCategory == EnumPlayerCategory.Computer)
        {
            return true;
        }
        if (_gameContainer.BasicData!.MultiPlayer)
        {
            return tempPlayer.PlayerCategory == EnumPlayerCategory.OtherHuman;
        }
        if (_gameContainer.PlayerList.Any(items => items.PlayerCategory == EnumPlayerCategory.Computer))
        {
            return false;
        }
        return _gameContainer.WhoTurn != playerConsidered;
    }
    private TriangleClass GetTriangle(int player, int yourID, bool autoPopulate = true)
    {
        bool rets = NeedsReversed(player);
        int newID;
        if (rets == true)
        {
            newID = GetReversedID(yourID);
        }
        else
        {
            newID = yourID;
        }
        var thisTriangle = _gameContainer.TriangleList[newID];
        if (autoPopulate)
        {
            thisTriangle.PlayerOwns = player;
        }
        return thisTriangle;
    }
    #endregion
    private void PopulateDiceValues()
    {
        var thisList = _model.Cup!.DiceList;
        if (thisList.Count != 2)
        {
            throw new CustomBasicException("There should be just 2 dice values");
        }
        _gameContainer.FirstDiceValue = thisList.First().Value;
        _gameContainer.SecondDiceValue = thisList.Last().Value;
    }
    private void ClearTriangles()
    {
        foreach (var thisTriangle in _gameContainer.TriangleList.Values)
        {
            thisTriangle.NumberOfTiles = 0;
            thisTriangle.PlayerOwns = 0;
            thisTriangle.Locations.Clear();
        }
    }
    private void PopulateTriangles()
    {
        TriangleClass thisTriangle;
        _gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.CurrentTurnData!.PiecesAtHome > 0)
            {
                thisTriangle = GetTriangle(thisPlayer.Id, 25);
                thisTriangle.NumberOfTiles = thisPlayer.CurrentTurnData.PiecesAtHome;
            }
            if (thisPlayer.CurrentTurnData.PiecesAtStart > 0)
            {
                thisTriangle = GetTriangle(thisPlayer.Id, 0);
                thisTriangle.NumberOfTiles = thisPlayer.CurrentTurnData.PiecesAtStart;
            }
            var thisList = thisPlayer.CurrentTurnData.PiecesOnBoard!.GroupBy(items => items).ToBasicList();
            thisList.ForEach(thisIndex =>
            {
                var ourIndex = thisIndex.Key;
                var count = thisIndex.Count();
                thisTriangle = GetTriangle(thisPlayer.Id, ourIndex);
                thisTriangle.NumberOfTiles = count;
            });
        });
    }
    private static int SpaceNumberForRoll(BasicList<int> thisCol, int whatNum, bool willReverse)
    {
        int thisNum;
        if (willReverse)
        {
            thisNum = GetReversedID(whatNum);
        }
        else
        {
            thisNum = whatNum;
        }
        if (thisCol.Any(items => items == thisNum))
        {
            return thisNum;
        }
        return 0;
    }
    private static int SpaceAboveRoll(BasicList<int> thisCol, int whatNum, bool willReverse)
    {
        int thisNum;
        if (willReverse)
        {
            thisNum = GetReversedID(whatNum);
        }
        else
        {
            thisNum = whatNum;
        }
        int finalValue = 0;
        int diffs = 50;
        int oldDiffs = 0;
        thisCol.ForEach(thisIndex =>
        {
            if (thisIndex > thisNum && willReverse == false)
            {
                oldDiffs = thisIndex - thisNum;
            }
            else if (thisIndex < thisNum && willReverse)
            {
                oldDiffs = thisNum - thisIndex;
            }
            if (oldDiffs < diffs)
            {
                diffs = oldDiffs;
                finalValue = thisIndex;
            }
        });
        return finalValue;
    }
    private static int OneToStack(int whatNum, BasicList<int> thisCol, bool willReverse)
    {
        var thisNum = SpaceNumberForRoll(thisCol, whatNum, willReverse);
        if (thisNum > 0)
        {
            return thisNum;
        }
        return SpaceAboveRoll(thisCol, whatNum, willReverse);
    }
    private EnumStatusType CalculateStatus(int index)
    {
        var thisSpace = _gameContainer.TriangleList[index];
        if (thisSpace.PlayerOwns == 0)
        {
            return EnumStatusType.CompletelyOpen;
        }
        if (thisSpace.NumberOfTiles == 1)
        {
            if (thisSpace.PlayerOwns == _gameContainer.WhoTurn)
            {
                return EnumStatusType.PlayerHasOne;
            }
            else
            {
                return EnumStatusType.KnockOtherPlayer;
            }
        }
        if (thisSpace.PlayerOwns == _gameContainer.WhoTurn)
        {
            return EnumStatusType.PlayerOwns;
        }
        return EnumStatusType.Closed;
    }
    private static int NumberToMove(int whatNum, bool isReversed)
    {
        if (isReversed == false)
        {
            return whatNum;
        }
        return whatNum * -1;
    }
    private BasicList<int> ListSpacesFrom() => _gameContainer.TriangleList.Where(items => items.Value.PlayerOwns == _gameContainer.WhoTurn && items.Key > 0 && items.Key < 25).Select(items => items.Key).OrderBy(items => items).ToBasicList();
    private bool CanStackUp(bool isReversed)
    {
        var thisList = ListSpacesFrom();
        if (isReversed == false)
        {
            return thisList.First() >= 19;
        }
        return thisList.Last() <= 6;
    }
    private PointF CalculateDiffs(int spaceNumber)
    {
        if (spaceNumber == 0)
        {
            return new PointF(30, 0);
        }
        if (spaceNumber == 27)
        {
            return new PointF(-30, 0);
        }
        if (spaceNumber == 25 || spaceNumber == 26)
        {
            return new PointF(0, -11);
        }
        var thisTriangle = _gameContainer.TriangleList[spaceNumber];
        int values;
        if (thisTriangle.NumberOfTiles > 5)
        {
            values = 15;
        }
        else
        {
            values = 28;
        }
        if (spaceNumber > 0 & spaceNumber < 13)
        {
            return new PointF(0, values);
        }
        else
        {
            return new PointF(0, values * -1);
        }
    }
    private static PointF CalculateFirstLocation(int spaceNumber, RectangleF tempRect)
    {
        var diffRight = new PointF(30, 0);
        var diffBottom = new PointF(0, 30);
        if (spaceNumber == 25 || spaceNumber == 26)
        {
            return new PointF(tempRect.Location.X, tempRect.Bottom - diffBottom.Y);
        }
        if (spaceNumber == 0)
        {
            return new PointF(tempRect.Location.X + 3, tempRect.Location.Y);
        }
        if (spaceNumber == 27)
        {
            return new PointF(tempRect.Right - diffRight.X, tempRect.Location.Y);
        }
        if (spaceNumber > 0 && spaceNumber < 13)
        {
            return new PointF(tempRect.Location.X + 3, tempRect.Location.Y);
        }
        else if (spaceNumber > 12 && spaceNumber < 25)
        {
            return new PointF(tempRect.Location.X + 3, tempRect.Bottom - diffBottom.Y);
        }
        else
        {
            throw new CustomBasicException("Cannot find the space");
        }
    }
    private PointF CalculateNewLocation(int spaceNumber)
    {
        var tempPoint = CalculateDiffs(spaceNumber);
        var diffx = tempPoint.X;
        var diffy = tempPoint.Y;
        var thisTriangle = _gameContainer.TriangleList[spaceNumber];
        if (thisTriangle.NumberOfTiles == 0)
        {
            var tempRect = _graphicsBoard.GetRectangleSpace(spaceNumber);
            return CalculateFirstLocation(spaceNumber, tempRect);
        }
        var thisLocation = thisTriangle.Locations.Last();
        tempPoint = thisLocation;
        tempPoint.X += diffx;
        tempPoint.Y += diffy;
        return tempPoint;
    }
    private BasicList<MoveInfo> MovesWithSpecificNumber(int value)
    {
        bool willReverse = _gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self;
        bool stacks;
        bool fromStart;
        if (_gameContainer.SingleInfo.CurrentTurnData!.PiecesAtStart > 0)
        {
            stacks = false;
            fromStart = true;
        }
        else
        {
            fromStart = false;
            stacks = CanStackUp(willReverse);
        }
        if (fromStart == true && stacks == true)
        {
            throw new CustomBasicException("Can't stack if from start");
        }
        EnumStatusType thisResult;
        MoveInfo thisMove;
        BasicList<MoveInfo> thisList = new();
        if (fromStart)
        {
            if (willReverse == false)
            {
                thisResult = CalculateStatus(value);
            }
            else
            {
                thisResult = CalculateStatus(GetReversedID(value));
            }
            if (thisResult == EnumStatusType.Closed)
            {
                return new(); //because no moves can be made
            }
            thisMove = new()
            {
                DiceNumber = value,
                Results = thisResult
            };
            if (willReverse == false)
            {
                thisMove.SpaceFrom = 0;
                thisMove.SpaceTo = value;
            }
            else
            {
                thisMove.SpaceFrom = 27;
                thisMove.SpaceTo = GetReversedID(value);
            }
            return new() { thisMove };
        }
        var fromList = ListSpacesFrom();
        if (fromList.Count == 0)
        {
            return new(); //because there was none.
        }
        int nextStart = 0;
        if (stacks)
        {
            nextStart = OneToStack(value, fromList, willReverse);
        }
        var newNum = NumberToMove(value, willReverse);
        int endMove;
        fromList.ForEach(thisIndex =>
        {
            endMove = thisIndex + newNum;
            if (endMove <= 0)
            {
                endMove = 26;
            }
            if (endMove > 24)
            {
                if (stacks && nextStart == thisIndex)
                {
                    thisMove = new()
                    {
                        Results = EnumStatusType.Stackup,
                        SpaceFrom = thisIndex,
                        DiceNumber = value
                    };
                    if (willReverse)
                    {
                        thisMove.SpaceTo = 26;
                    }
                    else
                    {
                        thisMove.SpaceTo = 25;
                    }
                    thisList.Add(thisMove);
                }
            }
            else
            {
                thisResult = CalculateStatus(endMove);
                if (thisResult != EnumStatusType.Closed)
                {
                    thisMove = new()
                    {
                        Results = thisResult,
                        SpaceFrom = thisIndex,
                        DiceNumber = value,
                        SpaceTo = endMove
                    };
                    thisList.Add(thisMove);
                }
            }
        });
        return thisList;
    }
    private void RepositionPieces()
    {
        var offsetSize = new PointF(3, 3);
        foreach (var thisTriangle in _gameContainer.TriangleList.Values)
        {
            thisTriangle.Locations.Clear();
            if (thisTriangle.NumberOfTiles > 0)
            {
                int index = _gameContainer.TriangleList.GetKey(thisTriangle);
                var thisRect = _graphicsBoard.GetRectangleSpace(index);
                var tempPoint = CalculateDiffs(index);
                var thisLocation = CalculateFirstLocation(index, thisRect);
                var diffx = tempPoint.X;
                var diffy = tempPoint.Y;
                if (index > 0 && index < 25)
                {
                    thisLocation.X += offsetSize.X;
                }
                thisTriangle.NumberOfTiles.Times(x =>
                {
                    thisTriangle.Locations.Add(thisLocation);
                    thisLocation = new PointF(thisLocation.X + diffx, thisLocation.Y + diffy);
                });
            }
        }
        _gameContainer.RefreshPieces = true; //maybe here (?)
    }
    private async Task BackToStartAsync(int player, int index, int oldPosition)
    {
        var thisPlayer = _gameContainer.PlayerList![player];
        thisPlayer.CurrentTurnData!.PiecesOnBoard!.RemoveSpecificItem(oldPosition);
        thisPlayer.CurrentTurnData.PiecesAtStart++;
        _gameContainer.Animates!.LocationTo = CalculateNewLocation(index);
        RepositionPieces();
        int tempTurn = _gameContainer.WhoTurn;
        _gameContainer.WhoTurn = tempTurn;
        await _gameContainer.Animates.DoAnimateAsync();
        _gameContainer.WhoTurn = tempTurn;
        var thisTriangle = _gameContainer.TriangleList[index];
        thisTriangle.PlayerOwns = player;
        thisTriangle.NumberOfTiles += thisTriangle.NumberOfTiles;
        PopulateTriangles();
    }
    private void PopulateMoves()
    {
        _gameContainer.MoveList.Clear();
        ClearTriangles();
        PopulateTriangles();
        bool rets = _gameContainer.HadDoubles();
        BasicList<int> thisList = new();
        if (rets == true)
        {
            thisList.Add(_gameContainer.FirstDiceValue);
        }
        else
        {
            if (_gameContainer.SaveRoot!.NumberUsed != _gameContainer.FirstDiceValue)
            {
                thisList.Add(_gameContainer.FirstDiceValue);
            }
            if (_gameContainer.SaveRoot.NumberUsed != _gameContainer.SecondDiceValue)
            {
                thisList.Add(_gameContainer.SecondDiceValue);
            }
        }
        if (thisList.Count == 0)
        {
            throw new CustomBasicException("Should not populate moves because all used up.  If I can ignore, take this out");
        }
        thisList.ForEach(thisValue =>
        {
            var tempList = MovesWithSpecificNumber(thisValue);
            _gameContainer.MoveList.AddRange(tempList);
        });
        RepositionPieces();
    }
    private void LoadTriangles()
    {
        _gameContainer.TriangleList = new System.Collections.Generic.Dictionary<int, TriangleClass>();
        for (int x = 0; x <= 27; x++)
        {
            var thisTriangle = new TriangleClass();
            _gameContainer.TriangleList.Add(x, thisTriangle);
        }
    }
    public void ClearBoard()
    {
        _gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentTurnData = new()
            {
                PiecesOnBoard = new() { 1, 1, 12, 12, 12, 12, 12, 17, 17, 17, 19, 19, 19, 19, 19 }
            };
        });
        LoadTriangles();
        _gameContainer.MoveList = new();
        ClearTriangles();
        PopulateTriangles();
        _gameContainer.SaveRoot!.SpaceHighlighted = -1;
        _gameContainer.SaveRoot.NumberUsed = 0;
        RepositionPieces();
    }
    private void Repaint()
    {
        _gameContainer.RepaintBoard();
    }
    private bool CanEndGame() => _gameContainer.SingleInfo!.CurrentTurnData!.PiecesAtHome == 15;
    public async Task UndoAllMovesAsync()
    {
        _gameContainer.SaveRoot!.NumberUsed = 0;
        _gameContainer.SaveRoot.SpaceHighlighted = -1;
        _gameContainer.MoveList.Clear();
        _gameContainer.SaveRoot.MovesMade = 0;
        _gameContainer.SaveRoot.MadeAtLeastOneMove = false;
        _model.LastStatus = "You decided to undo all moves";
        _gameContainer.PlayerList!.ForEach(player =>
        {
            player.CurrentTurnData = player.StartTurnData!.Clone();
        });
        _gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
        PopulateMoves();
        RepositionPieces();
        Repaint();
        if (_gameContainer.DiceVisibleProcesses == null)
        {
            throw new CustomBasicException("Nobody is handling dice visible.  Rethink");
        }
        _gameContainer.DiceVisibleProcesses.Invoke();
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    public void ReloadSavedGame()
    {
        if (_gameContainer is null)
        {
            throw new CustomBasicException("Must have game container at least though");
        }
        //throw new CustomBasicException("No autoresume for now because of issues with repainting.  If i decided to do it eventually, then will have to put into interfaces which will do the work required when the first paint works.");
        LoadTriangles();
        PopulateDiceValues();
        PopulateMoves();
        _gameContainer.DiceVisibleProcesses?.Invoke();
        //_graphicsBoard.Saved = true; //could be iffy
        //_thisE.RepaintBoard();
        //await Task.Delay(1000); //problem is this time, needs the actual coordinates for positioning.
        RepositionPieces();
        //_graphicsBoard.Saved = false;
        //_alreadyPainted = false;
    }
    //private bool _alreadyPainted;
    //public void StartPaint()
    //{
    //    if (_alreadyPainted)
    //        return;
    //    _thisE.RepaintBoard();
    //    _alreadyPainted = true;
    //}
    public async Task StartNewTurnAsync()
    {
        _gameContainer.SaveRoot!.NumberUsed = 0;
        _gameContainer.MoveList.Clear();
        _gameContainer.SaveRoot.MovesMade = 0;
        _gameContainer.SaveRoot.MadeAtLeastOneMove = false;
        foreach (var player in _gameContainer.PlayerList!)
        {
            player.StartTurnData = player.CurrentTurnData!.Clone();
        }
        PopulateDiceValues();
        PopulateTriangles();
        PopulateMoves();
        RepositionPieces();
        Repaint();
        if (_gameContainer.MoveList.Count == 0)
        {
            _model.LastStatus = "No Moves Available";
            if (_gameContainer.Test!.NoAnimations == false)
            {
                await _gameContainer.Delay!.DelaySeconds(.5);
            }
            await _gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        _gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
        _gameContainer.SaveRoot.SpaceHighlighted = -1;
    }
    public bool IsValidMove(int space)
    {
        if (_gameContainer.SaveRoot!.SpaceHighlighted == space)
        {
            return true; //can always highlight/unhighlight
        }
        if (_gameContainer.SaveRoot.SpaceHighlighted == -1)
        {
            return _gameContainer.MoveList.Any(items => items.SpaceFrom == space);
        }
        return _gameContainer.MoveList.Any(items => items.SpaceFrom == _gameContainer.SaveRoot.SpaceHighlighted && items.SpaceTo == space);
    }
    public async Task MakeMoveAsync(int space)
    {
        if (_gameContainer.SaveRoot!.SpaceHighlighted == space)
        {
            _gameContainer.SaveRoot.SpaceHighlighted = -1;
            Repaint();
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (_gameContainer.SaveRoot.SpaceHighlighted == -1)
        {
            _gameContainer.SaveRoot.SpaceHighlighted = space;
            Repaint();
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        _gameContainer.MoveInProgress = true;
        var thisMove = _gameContainer.MoveList.First(items => items.SpaceFrom == _gameContainer.SaveRoot.SpaceHighlighted && items.SpaceTo == space);
        var thisTriangle = _gameContainer.TriangleList[_gameContainer.SaveRoot.SpaceHighlighted];
        thisTriangle.NumberOfTiles--;
        var thisLocation = thisTriangle.Locations.Last();
        if (thisTriangle.NumberOfTiles == 0)
        {
            thisTriangle.PlayerOwns = 0;
        }
        _gameContainer.Animates!.LocationFrom = thisLocation;
        _gameContainer.Animates.LocationTo = CalculateNewLocation(space);
        RepositionPieces();
        _gameContainer.RefreshPieces = true;
        Repaint();
        await Task.Delay(10);
        await _gameContainer.Animates.DoAnimateAsync();
        var newTriangle = _gameContainer.TriangleList[space];
        if (newTriangle.NumberOfTiles == 1)
        {
            if (newTriangle.PlayerOwns != _gameContainer.WhoTurn)
            {
                var newPlayer = newTriangle.PlayerOwns;
                newTriangle.PlayerOwns = _gameContainer.WhoTurn;
                _gameContainer.Animates.LocationFrom = newTriangle.Locations.Single();
                if (NeedsReversed(newPlayer))
                {
                    await BackToStartAsync(newPlayer, 27, GetReversedID(space));
                }
                else
                {
                    await BackToStartAsync(newPlayer, 0, space);
                }
            }
            else
            {
                newTriangle.NumberOfTiles++;
            }
        }
        else
        {
            newTriangle.PlayerOwns = _gameContainer.WhoTurn;
            newTriangle.NumberOfTiles++;
        }
        bool privateReverse = NeedsReversed(_gameContainer.WhoTurn);
        int newSpace;
        if (privateReverse)
        {
            newSpace = GetReversedID(space);
        }
        else
        {
            newSpace = space;
        }
        if (_gameContainer.SaveRoot.SpaceHighlighted == 0 || _gameContainer.SaveRoot.SpaceHighlighted == 27)
        {
            _gameContainer.SingleInfo!.CurrentTurnData!.PiecesAtStart--;
        }
        else
        {
            if (privateReverse)
            {
                _gameContainer.SaveRoot.SpaceHighlighted = GetReversedID(_gameContainer.SaveRoot.SpaceHighlighted);
            }
            _gameContainer.SingleInfo!.CurrentTurnData!.PiecesOnBoard!.RemoveSpecificItem(_gameContainer.SaveRoot.SpaceHighlighted);
        }
        if (space == 25 || space == 26)
        {
            _gameContainer.SingleInfo.CurrentTurnData.PiecesAtHome++;
        }
        else
        {
            _gameContainer.SingleInfo.CurrentTurnData.PiecesOnBoard!.Add(newSpace);
        }
        _gameContainer.MoveInProgress = false;
        RepositionPieces();
        _gameContainer.SaveRoot.SpaceHighlighted = -1;
        Repaint();
        if (CanEndGame() || _gameContainer.Test.ImmediatelyEndGame)
        {
            await _gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        _gameContainer.SaveRoot.MovesMade++;
        _gameContainer.SaveRoot.MadeAtLeastOneMove = true;
        if (_gameContainer.HadDoubles() == false)
        {
            if (_gameContainer.SaveRoot.NumberUsed > 0)
            {
                if (_gameContainer.DiceVisibleProcesses == null)
                {
                    throw new CustomBasicException("Nobody is handling dice visible.  Rethink");
                }
                _gameContainer.DiceVisibleProcesses.Invoke();
                _gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            _gameContainer.SaveRoot.NumberUsed = thisMove.DiceNumber;
            if (_gameContainer.DiceVisibleProcesses == null)
            {
                throw new CustomBasicException("Nobody is handling dice visible.  Rethink");
            }
            _gameContainer.DiceVisibleProcesses.Invoke();
            PopulateMoves();
            if (_gameContainer.MoveList.Count == 0)
            {
                _gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
                await _gameContainer.ContinueTurnAsync!.Invoke();
                return;
            }
            _gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return; //i think old version could had this bug.
        }
        if (_gameContainer.DiceVisibleProcesses == null)
        {
            throw new CustomBasicException("Nobody is handling dice visible.  Rethink");
        }
        _gameContainer.DiceVisibleProcesses.Invoke();
        if (_gameContainer.SaveRoot.MovesMade == 4)
        {
            _gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
            await _gameContainer.ContinueTurnAsync!.Invoke(); //becase you made all 4 moves.
            return;
        }
        PopulateMoves();
        Repaint();
        if (_gameContainer.MoveList.Count == 0)
        {
            _gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        _gameContainer.SaveRoot.GameStatus = EnumGameStatus.MakingMoves;
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
}
