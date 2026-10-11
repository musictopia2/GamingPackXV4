namespace Chess.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses(GameBoardGraphicsCP graphicsBoard, ChessGameContainer gameContainer)
{
    private readonly BasicList<IChessMan> _chessList = [];

    public static SpaceCP GetSpace(int row, int column) => GameBoardGraphicsCP.GetSpace(row, column)!;
    private static int GetIndex(int row, int column) => GameBoardGraphicsCP.GetIndexByPoint(row, column);
    public void ClearBoard()
    {
        graphicsBoard.ClearBoard(); //i think.
        gameContainer.PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentPieceList.Clear();
            int x;
            int thisIndex;
            thisPlayer.CurrentPieceList = [];
            PlayerSpace thisPiece;
            for (x = 1; x <= 8; x++)
            {
                thisIndex = GetIndex(7, x);
                thisPiece = new PlayerSpace();
                {
                    var withBlock = thisPiece;
                    withBlock.Index = thisIndex;
                    withBlock.Piece = EnumPieceType.PAWN;
                }
                thisPlayer.CurrentPieceList.Add(thisPiece);
            }
            thisIndex = GetIndex(8, 1);
            thisPiece = new ();
            thisPiece.Index = thisIndex;
            thisPiece.Piece = EnumPieceType.ROOK;
            thisPlayer.CurrentPieceList.Add(thisPiece);
            thisIndex = GetIndex(8, 8);
            thisPiece = new ();
            thisPiece.Piece = EnumPieceType.ROOK;
            thisPiece.Index = thisIndex;
            thisPlayer.CurrentPieceList.Add(thisPiece);
            thisIndex = GetIndex(8, 2);
            thisPiece = new ();
            thisPiece.Index = thisIndex;
            thisPiece.Piece = EnumPieceType.KNIGHT;
            thisPlayer.CurrentPieceList.Add(thisPiece);
            thisIndex = GetIndex(8, 7);
            thisPiece = new ();
            thisPiece.Index = thisIndex;
            thisPiece.Piece = EnumPieceType.KNIGHT;
            thisPlayer.CurrentPieceList.Add(thisPiece);
            thisIndex = GetIndex(8, 3);
            thisPiece = new ();
            thisPiece.Index = thisIndex;
            thisPiece.Piece = EnumPieceType.BISHOP;
            thisPlayer.CurrentPieceList.Add(thisPiece);
            thisIndex = GetIndex(8, 6);
            thisPiece = new ();
            thisPiece.Piece = EnumPieceType.BISHOP;
            thisPiece.Index = thisIndex;
            thisPlayer.CurrentPieceList.Add(thisPiece);
            thisIndex = GetIndex(8, 4);
            thisPiece = new ();
            thisPiece.Index = thisIndex;
            thisPiece.Piece = EnumPieceType.QUEEN;
            thisPlayer.CurrentPieceList.Add(thisPiece);
            thisIndex = GetIndex(8, 5);
            thisPiece = new ();
            thisPiece.Index = thisIndex;
            thisPiece.Piece = EnumPieceType.KING;
            thisPlayer.CurrentPieceList.Add(thisPiece);
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
        int realIndex;
        graphicsBoard.ClearBoard(); // always needs to clear first before repopulating again
        bool wasReversed;
        foreach (var thisPlayer in gameContainer.PlayerList!)
        {
            foreach (var ThisTemp in thisPlayer.CurrentPieceList)
            {
                if (NeedsReversed(thisPlayer.Id) == true)
                {
                    realIndex = GameBoardGraphicsCP.GetRealIndex(ThisTemp.Index, true);
                    wasReversed = true;
                }
                else
                {
                    realIndex = GameBoardGraphicsCP.GetRealIndex(ThisTemp.Index, false);
                    wasReversed = false;
                }
                var thisSpace = gameContainer.SpaceList!.Single(items => items.MainIndex == realIndex);
                thisSpace.PlayerOwns = thisPlayer.Id;
                thisSpace.WasReversed = wasReversed;
                thisSpace.PlayerPiece = ThisTemp.Piece;
                thisSpace.PlayerColor = thisPlayer.Color;
            }
        }
        _chessList.Clear();
        var thisList = gameContainer.SpaceList!.Where(items => items.PlayerOwns != 0).ToBasicList();
        foreach (var thisSpace in thisList)
        {
            IChessMan man;
            if (thisSpace.PlayerPiece == EnumPieceType.BISHOP)
            {
                man = new BishopClass();
            }
            else if (thisSpace.PlayerPiece == EnumPieceType.KING)
            {
                man = new KingClass();
            }
            else if (thisSpace.PlayerPiece == EnumPieceType.KNIGHT)
            {
                man = new KnightClass();
            }
            else if (thisSpace.PlayerPiece == EnumPieceType.PAWN)
            {
                man = new PawnClass();
            }
            else if (thisSpace.PlayerPiece == EnumPieceType.QUEEN)
            {
                man = new QueenClass();
            }
            else if (thisSpace.PlayerPiece == EnumPieceType.ROOK)
            {
                man = new RookClass();
            }
            else
            {
                throw new CustomBasicException("Nothing found");
            }
            man.PieceCategory = thisSpace.PlayerPiece;
            man.Player = thisSpace.PlayerOwns;
            man.IsReversed = thisSpace.WasReversed;
            man.Row = thisSpace.Row;
            man.Column = thisSpace.Column;
            _chessList.Add(man);
        }
    }
    public async Task UndoAllMovesAsync()
    {
        gameContainer.SaveRoot!.SpaceHighlighted = 0;
        gameContainer.SaveRoot.GameStatus = EnumGameStatus.None;
        //await _gameContainer.PlayerList!.ForEachAsync(async thisPlayer => thisPlayer.CurrentPieceList = await GetNewTurnDataAsync(thisPlayer.StartPieceList));
        foreach (var thisPlayer in gameContainer.PlayerList!)
        {
            thisPlayer.CurrentPieceList = thisPlayer.StartPieceList.Clone();
        }
        PopulateSpaces();
        PopulateMoves();
        gameContainer.SaveRoot.PossibleMove = new PreviousMove();
        gameContainer.Aggregator.RepaintBoard();
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private void PopulateMoves()
    {
        gameContainer.CurrentMoveList = [];
        gameContainer.CompleteMoveList = [];
        GetActualMoves();
    }

    public async Task StartNewTurnAsync()
    {
        foreach (var thisPlayer in gameContainer.PlayerList!)
        {
            thisPlayer.StartPieceList = thisPlayer.CurrentPieceList.Clone();
            // has to do all players in case one player gets knocked back to start
            //thisPlayer.StartPieceList = await GetNewTurnDataAsync(thisPlayer.CurrentPieceList);
        }
        gameContainer.SaveRoot!.PossibleMove = new PreviousMove();
        gameContainer.SaveRoot.SpaceHighlighted = 0;
        gameContainer.SaveRoot.GameStatus = EnumGameStatus.None; // start with none.
        PopulateSpaces();
        PopulateMoves();
        gameContainer.Aggregator.RepaintBoard();
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private bool _currentReversed;
    private void GetActualMoves() // version 1 will just show all possible moves here.
    {
        SpaceCP oldSpace;
        _currentReversed = NeedsReversed(gameContainer.WhoTurn);
        var tempList = _chessList.Where(items => items.Player == gameContainer.WhoTurn).ToBasicList();
        foreach (IChessMan thisMan in tempList)
        {
            oldSpace = GetSpace(thisMan.Row, thisMan.Column);
            var thisList = thisMan.GetValidMoves();
            foreach (var thisV in thisList)
            {
                var thisSpace = GetSpace(thisV.Row, thisV.Column);
                MoveInfo thisMove = new();
                thisMove.SpaceFrom = oldSpace.MainIndex;
                thisMove.SpaceTo = thisSpace.MainIndex;
                thisMove.Piece = thisMove.Piece;
                if (thisSpace.PlayerOwns == 0)
                {
                    thisMove.Results = EnumStatusType.CompletelyOpen;
                }
                else
                {
                    thisMove.Results = EnumStatusType.PlayerOwns;// 
                }
                gameContainer.CompleteMoveList.Add(thisMove);
            }
        }
    }
    public bool IsValidMove(int space)
    {
        var hIndex = GameBoardGraphicsCP.GetRealIndex(gameContainer.SaveRoot!.SpaceHighlighted, _currentReversed);
        if (hIndex == space)
        {
            return true;
        }
        int index = GameBoardGraphicsCP.GetRealIndex(space, _currentReversed);
        if (gameContainer.CompleteMoveList.Any(items => items.SpaceFrom == index))
        {
            return true;
        }
        if (gameContainer.SaveRoot.SpaceHighlighted == 0)
        {
            return false;
        }
        return gameContainer.CompleteMoveList.Any(items => items.SpaceFrom == hIndex && items.SpaceTo == index);
    }
    private void PopulateCurrent()
    {
        if (gameContainer.SaveRoot!.SpaceHighlighted == 0)
        {
            throw new CustomBasicException("No current move to populate with");
        }
        gameContainer.CurrentMoveList = gameContainer.CompleteMoveList.Where(items => items.SpaceFrom == gameContainer.SaveRoot.SpaceHighlighted).ToBasicList();
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
        await gameContainer.Animates.DoAnimateAsync();
        gameContainer.WhoTurn = tempTurn;
        var thisPlayer = gameContainer.PlayerList![player];
        var thisSpace = thisPlayer.CurrentPieceList.Single(items => items.Index == oldPosition);
        thisPlayer.CurrentPieceList.RemoveSpecificItem(thisSpace);
    }
    public async Task MakeMoveAsync(int space)
    {
        var hIndex = GameBoardGraphicsCP.GetRealIndex(gameContainer.SaveRoot!.SpaceHighlighted, _currentReversed);
        if (gameContainer.SaveRoot.SpaceHighlighted == space)
        {
            gameContainer.SaveRoot.SpaceHighlighted = 0;
            gameContainer.CurrentMoveList.Clear();
            gameContainer.Aggregator.RepaintBoard();
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (gameContainer.SaveRoot.SpaceHighlighted == 0)
        {
            gameContainer.SaveRoot.SpaceHighlighted = space;
            PopulateCurrent();
            gameContainer.Aggregator.RepaintBoard();
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (gameContainer!.CompleteMoveList.Any(items => items.SpaceFrom == space))
        {
            gameContainer.SaveRoot.SpaceHighlighted = space;
            PopulateCurrent();
            gameContainer.Aggregator.RepaintBoard();
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        var thisMove = gameContainer.CurrentMoveList.Single(items => items.SpaceTo == space);
        var thisSpace = GameBoardGraphicsCP.GetSpace(gameContainer.SaveRoot.SpaceHighlighted, false);
        var newSpace = GameBoardGraphicsCP.GetSpace(space, false);
        gameContainer.Animates!.LocationFrom = thisSpace.GetLocation();
        gameContainer.Animates.LocationTo = newSpace.GetLocation();
        var myCurrent = gameContainer.SingleInfo!.CurrentPieceList.Single(items => items.Index == hIndex);
        gameContainer.CurrentPiece = thisSpace.PlayerPiece;
        await gameContainer.Animates.DoAnimateAsync();
        if (gameContainer.BasicData.MultiPlayer == false || gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            myCurrent.Index = space;
        }
        else
        {
            myCurrent.Index = GameBoardGraphicsCP.GetIndexByPoint(9 - newSpace.Row, 9 - newSpace.Column);
        }
        var newPlayer = newSpace.PlayerOwns;
        bool wasPawn = thisSpace.PlayerPiece == EnumPieceType.PAWN;
        thisSpace.ClearSpace();
        gameContainer.CurrentPiece = newSpace.PlayerPiece;
        newSpace.PlayerPiece = myCurrent.Piece; //try this.
        newSpace.PlayerColor = gameContainer.SingleInfo.Color;
        newSpace.PlayerOwns = gameContainer.WhoTurn;
        bool wasKing = false;
        if (thisMove.Results == EnumStatusType.PlayerOwns)
        {
            if (gameContainer.CurrentPiece == EnumPieceType.KING)
            {
                wasKing = true;
            }
            var tempReverse = NeedsReversed(newPlayer);
            var oldPosition = GameBoardGraphicsCP.GetRealIndex(space, tempReverse);
            gameContainer.Animates.LocationFrom = newSpace.GetLocation();
            await RemovePieceAsync(newPlayer, oldPosition);
        }
        gameContainer.SaveRoot.PossibleMove = new ();
        gameContainer.SaveRoot.PossibleMove.PlayerColor = gameContainer.SingleInfo.Color.Color;
        gameContainer.SaveRoot.PossibleMove.SpaceFrom = gameContainer.SaveRoot.SpaceHighlighted;
        gameContainer.SaveRoot.PossibleMove.SpaceTo = space;
        gameContainer.SaveRoot.SpaceHighlighted = 0;
        gameContainer.CurrentMoveList.Clear();
        gameContainer.Aggregator.RepaintBoard();
        if (wasKing || gameContainer.Test.ImmediatelyEndGame)
        {
            await gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        if (wasPawn)
        {
            if (newSpace.Row == 1 || newSpace.Row == 8)
            {
                myCurrent.Piece = EnumPieceType.QUEEN;
                newSpace.PlayerPiece = EnumPieceType.QUEEN;
                gameContainer.Aggregator.RepaintBoard();
            }
        }
        gameContainer.SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public Task ReloadSavedGameAsync()
    {
        gameContainer.SaveRoot!.SpaceHighlighted = 0; //i think if there was a space highlighted, it will return to 0.  so you have to choose the piece again if autoresume.
        PopulateSpaces();
        PopulateMoves();
        if (gameContainer.BasicData!.MultiPlayer && gameContainer.BasicData.Client && gameContainer.SaveRoot.PreviousMove.SpaceFrom > 0)
        {
            gameContainer.SaveRoot.PreviousMove.SpaceFrom = GameBoardGraphicsCP.GetRealIndex(gameContainer.SaveRoot.PreviousMove.SpaceFrom, true);
            gameContainer.SaveRoot.PreviousMove.SpaceTo = GameBoardGraphicsCP.GetRealIndex(gameContainer.SaveRoot.PreviousMove.SpaceTo, true);
        }
        gameContainer.Aggregator.RepaintBoard();
        return Task.CompletedTask; //i think.
    }
}