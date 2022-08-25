namespace EightOffSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class EightOffSolitaireMainViewModel : SolitaireMainViewModel<EightOffSolitaireSaveInfo>
{
    [Command(EnumCommandCategory.Plain)]
    public void AddToReserve()
    {
        _mainGame!.AddToReserve();
    }
    public ReservePiles ReservePiles1;
    private EightOffSolitaireMainGameClass? _mainGame;
    public EightOffSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
        GlobalClass.MainModel = this;
        ReservePiles1 = new(command);
        ReservePiles1.Maximum = 8;
        ReservePiles1.AutoSelect = EnumHandAutoType.SelectOneOnly;
        ReservePiles1.Text = "Reserve Pile";
        CreateCommands(command);
    }
    partial void CreateCommands(CommandContainer command);
    protected override SolitaireGameClass<EightOffSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        _mainGame = resolver.ReplaceObject<EightOffSolitaireMainGameClass>();
        return _mainGame;
    }
}