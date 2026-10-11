namespace Checkers.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses(GameBoardGraphicsCP graphicsBoard, CheckersGameContainer gameContainer)
{
    public static SpaceCP? GetSpace(int row, int column) => GameBoardGraphicsCP.GetSpace(row, column);
    public void ClearBoard()
    {
        gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentPieceList.Clear();
            var firstList = GameBoardGraphicsCP.GetBlackStartingSpaces();
            firstList.ForEach(thisIndex =>
            {
                PlayerSpace thisSpace = new ();
                thisSpace.Index = thisIndex;
                thisSpace.IsCrowned = false;
                thisPlayer.CurrentPieceList.Add(thisSpace);
            });
        });
    }
    private bool NeedsReversed(int playerConsidered)
    {
        if (gameContainer.BasicData!.MultiPlayer)
        {
            var tempPlayer = gameContainer.PlayerList![playerConsidered];
            return tempPlayer.PlayerCategory == EnumPlayerCategory.OtherHuman;
        }
        return gameContainer.WhoTurn != playerConsidered; //try this
    }
    private void PopulateSpaces()
    {
        graphicsBoard.ClearBoard();
        int realIndex;
        bool wasReversed;
        gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentPieceList.ForEach(thisTemp =>
            {
                wasReversed = NeedsReversed(thisPlayer.Id);
                realIndex = GameBoardGraphicsCP.GetRealIndex(thisTemp.Index, wasReversed);
                var thisSpace = gameContainer.SpaceList!.Single(items => items.MainIndex == realIndex);
                thisSpace.PlayerOwns = thisPlayer.Id;
                thisSpace.WasReversed = wasReversed;
                thisSpace.IsCrowned = thisTemp.IsCrowned;
                thisSpace.PlayerColor = thisPlayer.Color;
            });
        });
    }
    public async Task StartNewTurnAsync()
    {
        gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.StartPieceList = thisPlayer.CurrentPieceList.Clone();
        });
        gameContainer.SaveRoot!.ForcedToMove = false;
        gameContainer.SaveRoot.SpaceHighlighted = 0;
        gameContainer.SaveRoot.GameStatus = EnumGameStatus.None;
        PopulateSpaces();
        PopulateMoves();
        gameContainer.Aggregator.RepaintBoard();
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private void PopulateMoves()
    {
        gameContainer.CurrentMoveList = [];
        gameContainer.CompleteMoveList = [];
        GetActualMoves();
    }
    public Task LoadPreviousGameAsync()
    {
        gameContainer.SaveRoot!.SpaceHighlighted = 0; //i think if there was a space highlighted, it will return to 0.  so you have to choose the piece again if autoresume.
        PopulateSpaces();
        PopulateMoves();
        gameContainer.Aggregator.RepaintBoard();
        return Task.CompletedTask;
    }
    #region "Move Processes"
    private bool _currentReversed;
    private static MoveInfo PopulateMove(SpaceCP oldSpace, SpaceCP newSpace)
    {
        MoveInfo output = new();
        output.SpaceFrom = oldSpace.MainIndex;
        output.SpaceTo = newSpace.MainIndex;
        return output;
    }
    private BasicList<MoveInfo> GetMovesWithPiece(SpaceCP oldSpace)
    {
        int row, column, checkRow, checkColumn;
        row = oldSpace.Row;
        column = oldSpace.Column;
        void UpdateChecker()
        {
            checkRow = row;
            checkColumn = column;
        }
        UpdateChecker();
        BasicList<MoveInfo> thisList = [];
        SpaceCP? newSpace;
        int playerIndex;
        MoveInfo thisMove;
        void CheckMoves(int incRow, int incColumn)
        {
            checkRow += incRow;
            checkColumn += incColumn;
            newSpace = GetSpace(checkRow, checkColumn);
            if (newSpace != null)
            {
                if (newSpace.PlayerOwns == 0)
                {
                    thisMove = PopulateMove(oldSpace, newSpace);
                    thisList.Add(thisMove);
                }
                else if (newSpace.PlayerOwns != oldSpace.PlayerOwns)
                {
                    playerIndex = newSpace.MainIndex;
                    checkRow += incRow;
                    checkColumn += incColumn;
                    newSpace = GetSpace(checkRow, checkColumn);
                    if (newSpace != null)
                    {
                        if (newSpace.PlayerOwns == 0)
                        {
                            thisMove = PopulateMove(oldSpace, newSpace);
                            thisMove.PlayerCaptured = playerIndex;
                            thisList.Add(thisMove);
                        }
                    }
                }
            }
        }
        if (_currentReversed == false || oldSpace.IsCrowned)
        {
            CheckMoves(-1, 1);
            UpdateChecker();
            CheckMoves(-1, -1);
        }
        if (oldSpace.IsCrowned == false && _currentReversed == false)
        {
            return thisList;
        }
        UpdateChecker();
        CheckMoves(1, 1);
        UpdateChecker();
        CheckMoves(1, -1);
        return thisList;
    }
    private void GetActualMoves()
    {
        _currentReversed = NeedsReversed(gameContainer.WhoTurn);
        var tempList = gameContainer.SpaceList!.Where(items => items.PlayerOwns == gameContainer.WhoTurn).ToBasicList();
        if (gameContainer.SaveRoot!.SpaceHighlighted > 0)
        {
            tempList.KeepConditionalItems(items => items.MainIndex == gameContainer.SaveRoot.SpaceHighlighted);
        }
        tempList.ForEach(oldSpace =>
        {
            gameContainer.CompleteMoveList.AddRange(GetMovesWithPiece(oldSpace));
        });
        if (gameContainer.CompleteMoveList.Any(items => items.PlayerCaptured > 0))
        {
            gameContainer.SaveRoot.ForcedToMove = true;
        }
        if (gameContainer.SaveRoot.ForcedToMove)
        {
            gameContainer.CompleteMoveList.KeepConditionalItems(items => items.PlayerCaptured > 0);
        }
    }
    public bool IsValidMove(int space)
    {
        if (space == gameContainer.SaveRoot!.SpaceHighlighted)
        {
            return true;
        }
        if (gameContainer.CompleteMoveList.Any(items => items.SpaceFrom == space))
        {
            return true;
        }
        if (gameContainer.SaveRoot.SpaceHighlighted == 0)
        {
            return false;
        }
        return gameContainer.CompleteMoveList.Any(items => items.SpaceFrom == gameContainer.SaveRoot.SpaceHighlighted && items.SpaceTo == space);
    }
    private async Task RemovePieceAsync(int player, int oldPosition)
    {
        int tempTurn = gameContainer.WhoTurn;
        if (gameContainer.WhoTurn == 1)
        {
            gameContainer.WhoTurn = 2;
        }
        else
        {
            gameContainer.WhoTurn = 1;
        }
        gameContainer.Animates!.LocationTo = graphicsBoard.SuggestedOffLocation(_currentReversed);
        var thisPlayer = gameContainer.PlayerList![player];
        var thisSpace = thisPlayer.CurrentPieceList.Single(items => items.Index == oldPosition);
        thisPlayer.CurrentPieceList.RemoveSpecificItem(thisSpace);
        await gameContainer.Animates.DoAnimateAsync();
        gameContainer.WhoTurn = tempTurn;
    }
    private void PopulateCurrent()
    {
        if (gameContainer.SaveRoot!.SpaceHighlighted == 0)
        {
            throw new CustomBasicException("No current move to populate with");
        }
        gameContainer.CurrentMoveList = gameContainer.CompleteMoveList.Where(items => items.SpaceFrom == gameContainer.SaveRoot.SpaceHighlighted).ToBasicList();
    }
    public async Task MakeMoveAsync(int space)
    {
        var hIndex = GameBoardGraphicsCP.GetRealIndex(gameContainer.SaveRoot!.SpaceHighlighted, _currentReversed);
        if (gameContainer.SaveRoot.SpaceHighlighted == space)
        {
            gameContainer.SaveRoot.SpaceHighlighted = 0;
            gameContainer.CurrentMoveList.Clear();
            gameContainer.Aggregator.RepaintBoard();
            await gameContainer.ContinueTurnAsync!();
            return;
        }
        if (gameContainer.SaveRoot.SpaceHighlighted == 0)
        {
            gameContainer.SaveRoot.SpaceHighlighted = space;
            PopulateCurrent();
            gameContainer.Aggregator.RepaintBoard();
            await gameContainer.ContinueTurnAsync!();
            return;
        }
        if (gameContainer.CompleteMoveList.Any(items => items.SpaceFrom == space))
        {
            gameContainer.SaveRoot.SpaceHighlighted = space;
            PopulateCurrent();
            gameContainer.Aggregator.RepaintBoard();
            await gameContainer.ContinueTurnAsync!();
            return;
        }
        var thisMove = gameContainer.CurrentMoveList.Single(items => items.SpaceTo == space);
        var thisSpace = GameBoardGraphicsCP.GetSpace(gameContainer.SaveRoot.SpaceHighlighted, false);
        var newSpace = GameBoardGraphicsCP.GetSpace(space, false);
        gameContainer.Animates!.LocationFrom = thisSpace.GetLocation();
        gameContainer.Animates.LocationTo = newSpace.GetLocation();
        thisSpace.ClearSpace();
        var myCurrent = gameContainer.SingleInfo!.CurrentPieceList.Single(items => items.Index == hIndex);
        gameContainer.CurrentCrowned = thisSpace.IsCrowned;
        await gameContainer.Animates.DoAnimateAsync(); //only problem is after getting to location, does not do the full location on single player.
        if (gameContainer.BasicData!.MultiPlayer == false || gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            myCurrent.Index = space;
        }
        else
        {
            myCurrent.Index = GameBoardGraphicsCP.GetIndexByPoint(9 - newSpace.Row, 9 - newSpace.Column);
        }
        gameContainer.CurrentCrowned = newSpace.IsCrowned;
        newSpace.PlayerColor = gameContainer.SingleInfo.Color;
        newSpace.IsCrowned = myCurrent.IsCrowned;
        newSpace.PlayerOwns = gameContainer.WhoTurn;
        bool isGameOver = false;
        if (thisMove.PlayerCaptured > 0)
        {
            var tempSpace = gameContainer.SpaceList!.Single(items => items.MainIndex == thisMove.PlayerCaptured);
            var newPlayer = tempSpace.PlayerOwns;
            var tempReverse = NeedsReversed(newPlayer);
            var oldPosition = GameBoardGraphicsCP.GetRealIndex(tempSpace.MainIndex, tempReverse);
            gameContainer.Animates.LocationFrom = tempSpace.GetLocation();
            tempSpace.PlayerOwns = 0;
            await RemovePieceAsync(newPlayer, oldPosition);

            var tempPlayer = gameContainer.PlayerList![newPlayer];
            if (tempPlayer.CurrentPieceList.Count == 0)
            {
                isGameOver = true;
            }
        }
        gameContainer.CurrentMoveList.Clear();
        if (isGameOver || gameContainer.Test.ImmediatelyEndGame)
        {
            await gameContainer.ShowWinAsync!(); //in this case, no problem sending that class to do the code for showwinasync.
            return;
        }
        if (newSpace.IsCrowned == false)
        {
            if (newSpace.Row == 1 || newSpace.Row == 8)
            {
                newSpace.IsCrowned = true;
                myCurrent.IsCrowned = true;
                gameContainer.Aggregator.RepaintBoard();
            }
        }
        if (gameContainer.SaveRoot.ForcedToMove)
        {
            gameContainer.SaveRoot.SpaceHighlighted = space;
            gameContainer.CompleteMoveList.Clear();
            gameContainer.CurrentMoveList.Clear();
            GetActualMoves();
            gameContainer.CurrentMoveList = gameContainer.CompleteMoveList.ToBasicList(); //needs to copy over.
            if (gameContainer.CompleteMoveList.Count > 0)
            {
                await gameContainer.ContinueTurnAsync!();
                return;
            }
        }
        await gameContainer.EndTurnAsync!();
    }
    #endregion
}