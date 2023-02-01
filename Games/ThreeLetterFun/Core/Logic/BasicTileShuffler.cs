
namespace ThreeLetterFun.Core.Logic;
[SingletonGame]
[AutoReset]
public class BasicTileShuffler : IShuffleTiles
{
    private readonly GlobalHelpers _global;
    private readonly IListShuffler<ThreeLetterFunCardData> _deck;
    private readonly GameBoard _board;
    private readonly ThreeLetterFunVMData _model;
    public BasicTileShuffler(GlobalHelpers global,
        IListShuffler<ThreeLetterFunCardData> deck,
        GameBoard board,
        ThreeLetterFunVMData model
        )
    {
        _global = global;
        _deck = deck;
        _board = board;
        _model = model;
    }
    public async Task StartShufflingAsync(ThreeLetterFunMainGameClass mainGame, int cardsToPassOut = 0)
    {
        mainGame.SaveRoot.CanStart = true;
        _deck.ClearObjects();
        _deck.ShuffleObjects();
        mainGame.SaveRoot.TileList = _global.GetTiles();
        mainGame.SaveRoot.TileList.ShuffleList();
        mainGame.SaveRoot.UpTo = 1;
        DeckRegularDict<ThreeLetterFunCardData> cardList = _deck.Take(36).ToRegularDeckDict();
        if (mainGame.SaveRoot.TileList.Count != 100)
        {
            throw new CustomBasicException("Must have 100 tiles");
        }
        if (cardList.Count != 36)
        {
            throw new CustomBasicException("Must have 36 cards");
        }
        if (mainGame.BasicData.MultiPlayer == true)
        {
            mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.ClearTurn();
                thisPlayer.CardsWon = 0;
                thisPlayer.CardUsed = 0;
                thisPlayer.MostRecent = 0;
                thisPlayer.TookTurn = false;
            });
        }
        DeckRegularDict<ThreeLetterFunCardData> firstList = _deck.ToRegularDeckDict();
        if (cardsToPassOut > 0)
        {
            DeckRegularDict<ThreeLetterFunCardData> temps = new();
            ps1.CardProcedures.PassOutCards(mainGame.PlayerList!, firstList, cardsToPassOut, 0, false, ref temps);
        }
        if (mainGame.SaveRoot.Level != EnumLevel.Easy)
        {
            _board.ClearBoard(cardList);
        }
        else
        {
            mainGame.SingleInfo = mainGame.PlayerList.GetSelf(); //try this.
            _board.ClearBoard(mainGame.SingleInfo!.MainHandList);
        }
        _board.Visible = true;
        if (mainGame.BasicData.MultiPlayer == false)
        {
            _model.TileBoard1!.UpdateBoard();
            return;
        }
        await mainGame.PopulateSaveRootAsync();
        await mainGame.Network!.SendAllAsync("restoregame", mainGame.SaveRoot); //they need to know what to do now.
        _model.TileBoard1!.UpdateBoard(); //try to have no update.  since we have continue turn anyways.
        await mainGame.ContinueTurnAsync();
    }
}