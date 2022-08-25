namespace BlockElevenSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class BlockElevenSolitaireMainViewModel : SolitaireMainViewModel<BlockElevenSolitaireSaveInfo>
{
    public BlockElevenSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
        GlobalClass.MainMod = this;
    }
    protected override SolitaireGameClass<BlockElevenSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<BlockElevenSolitaireMainGameClass>();
    }
    [LabelColumn]
    public int CardsLeft { get; set; }
    protected override void CommandExecutingChanged()
    {
        CardsLeft = DeckPile!.CardsLeft();
    }
}