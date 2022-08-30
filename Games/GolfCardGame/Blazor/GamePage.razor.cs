namespace GolfCardGame.Blazor;
public partial class GamePage
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    [CascadingParameter]
    public IGameInfo? GameData { get; set; }
    [CascadingParameter]
    public BasicData? BasicData { get; set; }
    [CascadingParameter]
    public MultiplayerBasicParentShell? Shell { get; set; }
    private int TargetHeight => 15;
    private static GolfCardGameVMData? GetData => aa.Resolver!.Resolve<GolfCardGameVMData>();
}