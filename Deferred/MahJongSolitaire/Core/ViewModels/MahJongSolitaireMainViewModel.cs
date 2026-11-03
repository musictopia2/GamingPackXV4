namespace MahJongSolitaire.Core.ViewModels;
[InstanceGame]
public partial class MahJongSolitaireMainViewModel(CommandContainer commandContainer,
    BasicData basicData, IEventAggregator aggregator, IGamePackageResolver resolver,
    IRandomGenerator rs
        ) : IBasicEnableProcess,
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    private partial void Subscribe();
    private partial void Unsubscribe();
    private MahJongSolitaireSaveInfo _saveRoot = null!;
    private MahJongSolitaireModGlobal _customGlobal = null!;
    private BaseMahjongGlobals _mainGlobal = null!;
    public MahJongSolitaireMainGameClass MainGame = null!;
    void IMainGameVM.Initialize()
    {
        Subscribe();
        _saveRoot = resolver.ReplaceObject<MahJongSolitaireSaveInfo>();
        _mainGlobal = resolver.ReplaceObject<BaseMahjongGlobals>();
        _customGlobal = resolver.ReplaceObject<MahJongSolitaireModGlobal>();
        _ = resolver.ReplaceObject<MahJongSolitaireGameBoardCP>(); //somehow i have to replace the gameboard as well.
        MainGame = resolver.ReplaceObject<MahJongSolitaireMainGameClass>();
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    public static int GameDrawing { get; set; } = 1; //i think needs to be static this time.
    public int TilesGone { get; set; }
    public bool CanUndoMove => _saveRoot!.PreviousList.Count > 0;
    [Command(EnumCommandCategory.Plain)]
    public Task UndoMoveAsync()
    {
        _saveRoot.FirstSelected = 0;
        _customGlobal.SecondSelected = 0;
        TilesGone -= 2;
        MainGame.GameBoard1!.PopulateBoardFromUndo();
        _saveRoot.PreviousList.Clear();
        commandContainer.UpdateAll();
        return Task.CompletedTask;
    }
    public bool CanGetHint => _saveRoot.FirstSelected == 0;
    [Command(EnumCommandCategory.Plain)]
    public void GetHint()
    {
        if (MainGame!.GameBoard1!.ValidList.Count == 0)
        {
            MainGame.GameBoard1.CheckForValidTiles();
        }
        _mainGlobal.CanShowTiles = false; //because animations.
        MahjongSolitaireTileInfo? tileSelected = null;
        bool hadHint = false;
        foreach (var tile1 in MainGame.GameBoard1.ValidList)
        {
            foreach (var tile2 in MainGame.GameBoard1.ValidList)
            {
                if (!(tile1 == tile2))
                {
                    _saveRoot.FirstSelected = tile1.Deck;
                    _customGlobal.SecondSelected = tile2.Deck;
                    if (MainGame.IsValidMove() == true)
                    {

                        int TempFirst = _saveRoot.FirstSelected;
                        int TempSecond = _customGlobal.SecondSelected;
                        int ask1 = rs.GetRandomNumber(2);
                        if (ask1 == 1)
                        {
                            _saveRoot.FirstSelected = tile1.Deck;
                            _customGlobal.SecondSelected = 0;
                            tileSelected = tile1;
                        }
                        else
                        {
                            _saveRoot.FirstSelected = tile2.Deck;
                            _customGlobal.SecondSelected = 0;
                            tileSelected = tile2;
                        }
                        hadHint = true;
                        break;
                    }
                }
            }
            if (hadHint == true)
            {
                break;
            }
        }
        _mainGlobal.CanShowTiles = true;
        if (hadHint == false)
        {
            _saveRoot.FirstSelected = 0;
            _customGlobal.SecondSelected = 0;
            return;
        }
        tileSelected!.IsSelected = true;
        MainGame.ShowSelectedItem(tileSelected.Deck);
    }
    public static bool CanSelectTile(MahjongSolitaireTileInfo tile) //iffy
    {
        return tile.IsEnabled;
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task SelectTileAsync(MahjongSolitaireTileInfo tile)
    {
        var tempList = _customGlobal.TileList.GetSelectedItems();
        if (tile.IsSelected == true)
        {
            _customGlobal.TileList.UnselectAllObjects();
            _saveRoot.FirstSelected = 0;
            _customGlobal.SecondSelected = 0; //try that as well.
            MainGame.ShowSelectedItem(0);
            return;
        }
        if (_saveRoot.FirstSelected == 0)
        {
            _saveRoot.FirstSelected = tile.Deck;
        }
        else if (_customGlobal.SecondSelected == 0)
        {
            _customGlobal.SecondSelected = tile.Deck;
        }
        if (_saveRoot.FirstSelected == 0 || _customGlobal.SecondSelected == 0)
        {
            tile.IsSelected = true;
            var nextTile = _customGlobal.TileList.GetSpecificItem(tile.Deck);
            if (nextTile.IsSelected == false)
            {
                throw new CustomBasicException("Did not commit properly");
            }
            MainGame.ShowSelectedItem(tile.Deck);
            return;
        }
        await MainGame.PairSelectedManuallyAsync(() => TilesGone += 2);
    }
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        GameDrawing++;
        basicData.GameDataLoading = false;
        TilesGone = 0;
        await MainGame.NewGameAsync(x => TilesGone = x);
    }

    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
    }
}