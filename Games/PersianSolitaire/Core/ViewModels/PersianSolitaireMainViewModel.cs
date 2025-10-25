namespace PersianSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class PersianSolitaireMainViewModel : SolitaireMainViewModel<PersianSolitaireSaveInfo>
{
    [LabelColumn]
    public int DealNumber { get; set; }
    private readonly WastePiles _tempWaste;
    private readonly ISolitaireData _thisData;
    public PersianSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        ISolitaireData thisData
        )
        : base(aggregator, command, resolver)
    {
        _tempWaste = (WastePiles)WastePiles1;
        _thisData = thisData;
        CreateCommands(command);
    }
    partial void CreateCommands(CommandContainer container);
    public bool CanNewDeal => _thisData.Deals != DealNumber;
    [Command(EnumCommandCategory.Plain)]
    public void NewDeal()
    {
        _tempWaste.Redeal();
    }
    protected override SolitaireGameClass<PersianSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        //this may be iffy now because you are going to new game.  well see.
        //if i need to clear something else, has to rethink.
        return resolver.ReplaceObject<PersianSolitaireMainGameClass>();
    }
}