namespace MahJongSolitaire.Core.Logic;
[SingletonGame]
public class MahJongSolitaireMainGameClass : IAggregatorContainer
{
    private readonly ISystemError _error;
    private readonly MahJongSolitaireModGlobal _customGlobal;
    private readonly MahJongSolitaireSaveInfo _saveRoot;
    private readonly CommandContainer _command;
    private readonly IToast _toast;
    public MahJongSolitaireGameBoardCP GameBoard1;
    private readonly BaseMahjongGlobals _baseGlobal;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="deck">0 to hide</param>
    public void ShowSelectedItem(int deck)
    {
        Aggregator.Publish(new TileChosenEventModel { Deck = deck });
    }
    public MahJongSolitaireMainGameClass(IEventAggregator aggregator,
        ISystemError error,
        MahJongSolitaireModGlobal global,
        MahJongSolitaireGameBoardCP board,
        BaseMahjongGlobals baseGlobal,
        MahJongSolitaireSaveInfo saveRoot,
        CommandContainer command,
        IToast toast
        )
    {
        Aggregator = aggregator;
        _customGlobal = global;
        GameBoard1 = board;
        _baseGlobal = baseGlobal;
        _saveRoot = saveRoot;
        _command = command;
        _toast = toast;
        _error = error;
    }
    public IEventAggregator Aggregator { get; }
    public Task NewGameAsync(Action<int> finalProcess)
    {
        _customGlobal.TileList.ShuffleObjects();
        _baseGlobal.CanShowTiles = true;
        LoadBoard();
        GameBoard1.PositionTiles();
        finalProcess.Invoke(0);
        ShowSelectedItem(0);
        _command.UpdateAll();
        return Task.CompletedTask;
    }
    private void LoadBoard()
    {
        int x = 0;
        _saveRoot!.BoardList.ForEach(thisBoard =>
        {
            int upTo;
            if (thisBoard.BoardCategory == BoardInfo.EnumBoardCategory.Regular)
            {
                upTo = thisBoard.HowManyColumns;
            }
            else if (thisBoard.BoardCategory == BoardInfo.EnumBoardCategory.FarRight)
            {
                upTo = 2;
            }
            else
            {
                upTo = 1;
            }
            upTo.Times(y =>
            {
                AddCardToBoard(thisBoard, x);
                x++;
            });
        });
    }
    public bool IsValidMove()
    {
        MahjongSolitaireTileInfo firstTile = _customGlobal.TileList.GetSpecificItem(_saveRoot!.FirstSelected);
        MahjongSolitaireTileInfo secondTile = _customGlobal.TileList.GetSpecificItem(_customGlobal.SecondSelected);
        if (firstTile.WhatNumber != BasicMahjongTile.EnumNumberType.IsNoNumber && secondTile.WhatNumber != BasicMahjongTile.EnumNumberType.IsNoNumber)
        {
            return firstTile.NumberUsed == secondTile.NumberUsed && firstTile.WhatNumber == secondTile.WhatNumber;
        }
        if (firstTile.WhatBonus != BasicMahjongTile.EnumBonusType.IsNoBonus && secondTile.WhatBonus != BasicMahjongTile.EnumBonusType.IsNoBonus)
        {
            return firstTile.WhatBonus == secondTile.WhatBonus;
        }
        if (firstTile.WhatColor != BasicMahjongTile.EnumColorType.IsNoColor && secondTile.WhatColor != BasicMahjongTile.EnumColorType.IsNoColor)
        {
            return firstTile.WhatColor == secondTile.WhatColor;
        }
        if (firstTile.WhatDirection != BasicMahjongTile.EnumDirectionType.IsNoDirection && secondTile.WhatDirection != BasicMahjongTile.EnumDirectionType.IsNoDirection)
        {
            return firstTile.WhatDirection == secondTile.WhatDirection;
        }
        return false;
    }
    public async Task PairSelectedManuallyAsync(Action updateTiles)
    {
        ShowSelectedItem(0);
        if (IsValidMove() == false)
        {
            GameBoard1!.UnselectTiles();
            return;
        }
        GameBoard1!.ProcessPair(false);
        updateTiles.Invoke();
        if (GameBoard1.IsGameOver() == true)
        {
            await ShowWinAsync();
            return;
        }
    }
    private void AddCardToBoard(BoardInfo thisBoard, int whichOne)
    {
        MahjongSolitaireTileInfo thisTile = _customGlobal.TileList.GetIndexedTile(whichOne);
        thisBoard.TileList.Add(thisTile);
        thisTile.IsEnabled = false;
    }
    public async Task ShowWinAsync()
    {
        _toast.ShowSuccessToast("You Win");
        await this.SendGameOverAsync(_error);
    }
}