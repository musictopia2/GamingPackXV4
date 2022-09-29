namespace ThreeLetterFun.Core.ViewModels;
[InstanceGame]
public partial class FirstOptionViewModel : ScreenViewModel, IBlankGameVM, IHandleAsync<FirstOptionEventModel>, ISerializable
{
    public ListViewPicker Option1;
    public FirstOptionViewModel(CommandContainer commandContainer,
        IGamePackageResolver resolver,
        BasicData basicData,
        IFirstOptionProcesses first,
        IEventAggregator aggregator,
        IMessageBox message
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        _first = first;
        _message = message;
        Option1 = new ListViewPicker(commandContainer, resolver);
        Option1.ItemSelectedAsync = Option1_ItemSelectedAsync;
        BasicList<string> list = new() { "Beginner", "Advanced" };
        Option1.IndexMethod = EnumIndexMethod.OneBased;
        Option1.LoadTextList(list);
        CreateCommands();
        CreateCommands(commandContainer);
    }
    partial void CreateCommands();
    partial void CreateCommands(CommandContainer command);
    protected override Task ActivateAsync()
    {
        Option1.ReportCanExecuteChange();
        return base.ActivateAsync();
    }
    private readonly BasicData _basicData;
    private readonly IFirstOptionProcesses _first;
    private readonly IMessageBox _message;
    public EnumFirstOption OptionChosen { get; set; } = EnumFirstOption.None;
    [Command(EnumCommandCategory.Old)]
    public async Task DescriptionAsync()
    {
        if (Aggregator is null)
        {
            return;
        }
        await _message.ShowMessageAsync("The beginner option only allows easy words to be formed.  Plus the beginner also has a choice start out with 4, 6, 8, or 10 cards.  Whoeever gets rid of their cards first by spelling them from the tiles wins." + Constants.VBCrLf + "The advanced option has a choice between allowing easy words or any common 3 letter words.  Also; for the advanced option; all the cards are layed out.  There is a short option in which the first player who spells 5 words correctly wins.  For the regular; once all the cards or tiles are gone; then whoever wins the most tiles wins.  In event of a tie; whoever won it first wins.");
    }
    public bool CanSubmit
    {
        get
        {
            if (_basicData.MultiPlayer == false)
            {
                return false; //because single player game if it shows it is only for testing.
            }
            if (_basicData.Client == true)
            {
                return false;
            }
            return OptionChosen != EnumFirstOption.None;
        }
    }
    [Command(EnumCommandCategory.Plain)]
    public Task SubmitAsync()
    {
        return _first.BeginningOptionSelectedAsync(OptionChosen);
    }
    private Task Option1_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        OptionChosen = EnumFirstOption.FromValue(selectedIndex);
        return Task.CompletedTask;
    }

    async Task IHandleAsync<FirstOptionEventModel>.HandleAsync(FirstOptionEventModel message)
    {
        OptionChosen = await js.DeserializeObjectAsync<EnumFirstOption>(message.Message);
        Option1.SelectSpecificItem(OptionChosen.Value);
        await SubmitAsync(); //can just act like you are submitting.
    }
    public CommandContainer CommandContainer { get; set; }
}