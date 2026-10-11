
namespace MahJongSolitaire.Core.ViewModels;
[InstanceGame]
public partial class MahJongSolitaireMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
{
    private readonly MahJongSolitaireSaveInfo _saveRoot;
    private readonly MahJongSolitaireModGlobal _customGlobal;
    private readonly BaseMahjongGlobals _mainGlobal;
    private readonly IRandomGenerator _rs;
    private readonly BasicData _basicData;
    public readonly MahJongSolitaireMainGameClass MainGame;
    public MahJongSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IRandomGenerator rs,
        BasicData basicData,
        IGamePackageResolver resolver) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _rs = rs;
        _basicData = basicData;
        //there are lots of things that has to be replaced here because of new game.
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
        CommandContainer.UpdateAll();
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
                        int ask1 = _rs!.GetRandomNumber(2);
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
    public CommandContainer CommandContainer { get; set; }
    IEventAggregator IAggregatorContainer.Aggregator => Aggregator;
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    protected override async Task ActivateAsync()
    {
        GameDrawing++;
        _basicData.GameDataLoading = false;
        TilesGone = 0;
        await base.ActivateAsync();
        await MainGame.NewGameAsync(x => TilesGone = x);
    }
}