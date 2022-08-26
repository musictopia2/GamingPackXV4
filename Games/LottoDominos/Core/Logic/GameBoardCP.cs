namespace LottoDominos.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardCP : GameBoardObservable<SimpleDominoInfo>
{
    private readonly IAsyncDelayer _delayer;
    private readonly LottoDominosVMData _model;
    private readonly CommandContainer _command;
    public GameBoardCP(CommandContainer container,
        IAsyncDelayer delayer,
        LottoDominosVMData model
        ) : base(container)
    {
        Columns = 7;
        Rows = 4;
        _delayer = delayer;
        _model = model;
        Text = "Dominos"; //just do here since its inherited anyways.
        Visible = true;
        _command = container;
    }
    internal Func<int, Task>? MakeMoveAsync { get; set; }
    protected override Task ClickProcessAsync(SimpleDominoInfo domino)
    {
        if (MakeMoveAsync == null)
        {
            throw new CustomBasicException("Make move was never populated to run.  Rethink");
        }
        return MakeMoveAsync.Invoke(domino.Deck);
    }
    public void ClearPieces()
    {
        _model.DominosList!.ForEach(items =>
        {
            items.IsUnknown = true;
        });
        ObjectList.ReplaceRange(_model.DominosList);
    }
    public DeckRegularDict<SimpleDominoInfo> GetVisibleList()
    {
        return ObjectList.Where(xx => xx.Visible).ToRegularDeckDict();
    }
    public async Task ShowDominoAsync(int deck)
    {
        SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
        thisDomino.IsUnknown = false;
        _command.UpdateAll(); //to show up on blazor.
        await _delayer.DelaySeconds(2);
        thisDomino.IsUnknown = true;
    }
    public void MakeInvisible(int deck)
    {
        SimpleDominoInfo thisDomino = ObjectList.GetSpecificItem(deck);
        thisDomino.Visible = false;
    }
}