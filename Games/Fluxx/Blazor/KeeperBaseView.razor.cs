namespace Fluxx.Blazor;
public abstract partial class KeeperBaseView<K>
    where K : class
{
    [CascadingParameter]
    public CompleteContainerClass? CompleteContainer { get; set; }
    [CascadingParameter]
    public K? DataContext { get; set; }
    private string GetRowsText
    {
        get
        {
            return bb.RepeatAuto(BottomRow + 1);

        }
    }
    private int BottomRow
    {
        get
        {
            if (CompleteContainer!.GameContainer.PlayerList!.Count <= 3)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
    }
    private string GetColumnsText
    {
        get
        {
            if (CompleteContainer!.GameContainer.PlayerList!.Count == 2 || CompleteContainer.GameContainer.PlayerList!.Count == 4)
            {
                return "50vw 50vw";
            }
            else
            {
                return "33vw 33vw 33vw";
            }
        }
    }
    private BasicList<FluxxPlayerItem> _players = new();
    protected override void OnInitialized()
    {
        _players = CompleteContainer!.GameContainer.PlayerList!.GetAllPlayersStartingWithSelf();
        base.OnInitialized();
    }

    //this means overrided versions can do command
    protected virtual ICustomCommand? Command { get; private set; }

    //private ICustomCommand CloseCommand => DataContext.
    //private static string CloseMethod => nameof(KeeperShowViewModel.CloseKeeperAsync);
    //protected virtual string CommandText => "";
    protected abstract EnumKeeperCategory KeeperCategory { get; }
}