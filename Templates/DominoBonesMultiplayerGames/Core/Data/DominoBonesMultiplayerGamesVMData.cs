namespace DominoBonesMultiplayerGames.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class DominoBonesMultiplayerGamesVMData : IDominoGamesData<SimpleDominoInfo>
{
    public DominoBonesMultiplayerGamesVMData(CommandContainer command,
            IGamePackageResolver resolver,
            DominosBasicShuffler<SimpleDominoInfo> shuffle
            )
    {
        PlayerHand1 = new HandObservable<SimpleDominoInfo>(command);
        BoneYard = new DominosBoneYardClass<SimpleDominoInfo>(this, command, resolver, shuffle);
        PlayerHand1.ObjectClickedAsync = PlayerHand1_ObjectClickedAsync;
        PlayerHand1.BoardClickedAsync = PlayerHand1_BoardClickedAsync;
    }
    private Task PlayerHand1_BoardClickedAsync()
    {
        if (PlayerBoardClickedAsync == null)
        {
            throw new CustomBasicException("Board clicked was never created.  Rethink");
        }
        return PlayerBoardClickedAsync.Invoke();
    }
    private Task PlayerHand1_ObjectClickedAsync(SimpleDominoInfo domino, int index)
    {
        if (HandClickedAsync == null)
        {
            throw new CustomBasicException("The hand clicked was never done.  Rethink");
        }
        return HandClickedAsync.Invoke(domino, index);
    }
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public Func<SimpleDominoInfo, Task>? DrewDominoAsync { get; set; }
    public HandObservable<SimpleDominoInfo> PlayerHand1 { get; set; }
    public DominosBoneYardClass<SimpleDominoInfo> BoneYard { get; set; }
    public Func<Task>? PlayerBoardClickedAsync { get; set; }
    public Func<SimpleDominoInfo, int, Task>? HandClickedAsync { get; set; }
    public bool CanEnableBasics()
    {
        return true;
    }
    public bool CanEnableAlways()
    {
        return true;
    }

    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}