namespace ThreeLetterFun.Core.ViewModels;
[InstanceGame]
public partial class CardsPlayerViewModel : ScreenViewModel, IBlankGameVM, IHandleAsync<CardsChosenEventModel>
{
    public CardsPlayerViewModel(CommandContainer commandContainer,
        IGamePackageResolver resolver, BasicData basicData,
        ICardsChosenProcesses processes,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        _processes = processes;
        CardList1 = new (CommandContainer, resolver);
        CardList1.IndexMethod = EnumIndexMethod.ZeroBased;
        CardList1.ItemSelectedAsync += CardList1_ItemSelectedAsync;
        BasicList<string> thisList = new() { "4 Cards", "6 Cards", "8 Cards", "10 Cards" };
        CardList1.LoadTextList(thisList);
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public ListViewPicker CardList1;
    public CommandContainer CommandContainer { get; set; }
    public bool CanSubmit()
    {
        if (_basicData.MultiPlayer == false)
        {
            return false; //because single player game if it shows it is only for testing.
        }
        if (_basicData.Client == true)
        {
            return false;
        }
        return HowManyCards > 0;
    }
    [Command(EnumCommandCategory.Plain)] //had to be plain.  otherwise, had to implement another interface.
    public Task SubmitAsync()
    {
        return _processes.CardsChosenAsync(HowManyCards);
    }

    private readonly BasicData _basicData;
    private readonly ICardsChosenProcesses _processes;
    public int HowManyCards { get; set; }
    public void SelectGivenValue()
    {
        int index;
        if (HowManyCards == 4)
        {
            index = 0;
        }
        else if (HowManyCards == 6)
        {
            index = 1;
        }
        else if (HowManyCards == 8)
        {
            index = 2;
        }
        else if (HowManyCards == 10)
        {
            index = 3;
        }
        else
        {
            throw new CustomBasicException("Nothing found.");
        }
        CardList1.SelectSpecificItem(index);
    }
    private Task CardList1_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        var temps = selectedText.Replace(" Cards", "");
        HowManyCards = int.Parse(temps); // otherwise, error.
        return Task.CompletedTask;
    }
    async Task IHandleAsync<CardsChosenEventModel>.HandleAsync(CardsChosenEventModel message)
    {
        HowManyCards = message.HowManyCards;
        SelectGivenValue();
        await SubmitAsync();
    }
    //protected override Task TryCloseAsync()
    //{
    //    Aggregator.Unsubscribe(this);
    //    return base.TryCloseAsync();
    //}
}
