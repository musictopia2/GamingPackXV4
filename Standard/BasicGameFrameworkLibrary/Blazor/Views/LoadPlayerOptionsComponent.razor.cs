namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class LoadPlayerOptionsComponent<P>
    where P : class, IPlayerItem, new()
{
    [Parameter]
    public EnumPlayOptions PlayOption { get; set; }
    //good news is with computerextra, can use that as a hint to see what should be displayed.
    [CascadingParameter]
    public IGameInfo? GameData { get; set; }
    [CascadingParameter]
    public BasicData? BasicInfo { get; set; }
    [CascadingParameter]
    public MultiplayerOpeningViewModel<P>? DataContext { get; set; }
    private (ICustomCommand? Method, int Parameter, string Display) CommandData(int amount)
    {
        int increment;
        if (PlayOption == EnumPlayOptions.HumanLocal)
        {
            increment = amount + 1;
        }
        else
        {
            increment = amount;
        }
        ICustomCommand command;
        string header;
        switch (PlayOption)
        {
            case EnumPlayOptions.ComputerExtra:
                command = DataContext!.StartCommand!;
                header = $"{increment} Extra Computer Players";
                break;
            case EnumPlayOptions.ComputerLocal:
                command = DataContext!.StartComputerSinglePlayerGameCommand!;
                header = $"{increment} Local Computer Players";
                break;
            case EnumPlayOptions.HumanLocal:
                command = DataContext!.StartPassAndPlayGameCommand!;
                header = $"{increment} Pass And Play Human Players";
                break;
            default:
                return (null, 0, "");
        }
        return (command, increment, header);
    }
    private BasicList<int> _completeList = new();
    private (ICustomCommand Method, string Display) SolitaireData()
    {
        return (DataContext!.SolitaireCommand!, "New Single Player Game");
    }
    protected override void OnParametersSet()
    {
        if (GameData == null)
        {
            return;
        }
        _completeList = OpenPlayersHelper.GetPossiblePlayers(GameData);
        if (PlayOption == EnumPlayOptions.ComputerExtra)
        {
            for (int i = 0; i < _completeList.Count; i++)
            {
                _completeList[i] = _completeList[i] - DataContext!.ClientsConnected;
            }
            if (_completeList.First() == 0)
            {
                _completeList.RemoveFirstItem(); //because there is already an option to have no extra computer players.
            }
        }
        base.OnParametersSet();
    }
}