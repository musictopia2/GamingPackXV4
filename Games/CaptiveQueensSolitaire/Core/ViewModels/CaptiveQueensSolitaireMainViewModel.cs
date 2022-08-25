namespace CaptiveQueensSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class CaptiveQueensSolitaireMainViewModel : SolitaireMainViewModel<CaptiveQueensSolitaireSaveInfo>
{
    public CaptiveQueensSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver
        )
        : base(aggregator, command, resolver)
    {
    }
    protected override SolitaireGameClass<CaptiveQueensSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        return resolver.ReplaceObject<CaptiveQueensSolitaireMainGameClass>();
    }
    [LabelColumn]
    public static int FirstNumber
    {
        get
        {
            return 5;
        }
    }
    [LabelColumn]
    public static int SecondNumber
    {
        get
        {
            return 6;
        }
    }
}