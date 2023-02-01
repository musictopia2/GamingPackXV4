namespace ThreeLetterFun.Core.ViewModels;
[InstanceGame]
public partial class AdvancedOptionsViewModel : ScreenViewModel, IBlankGameVM, IHandleAsync<AdvancedSettingsEventModel>, ISerializable
{
    public AdvancedOptionsViewModel(CommandContainer commandContainer,
        IGamePackageResolver resolver,
        BasicData basicData,
        IAdvancedProcesses processes,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _basicData = basicData;
        _processes = processes;
        Game1 = new ListViewPicker(commandContainer, resolver);
        Easy1 = new ListViewPicker(commandContainer, resolver);
        Game1.IndexMethod = EnumIndexMethod.OneBased;
        Easy1.IndexMethod = EnumIndexMethod.OneBased;
        var thisList = new BasicList<string>() { "Easy Words", "Any Words" };
        Easy1.LoadTextList(thisList);
        thisList = new() { "Short Game", "Regular Game" };
        Game1.LoadTextList(thisList);
        SelectSpecificOptions();
        Game1.ItemSelectedAsync = Game1_ItemSelectedAsync;
        Easy1.ItemSelectedAsync = Easy1_ItemSelectedAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    public ListViewPicker Game1;
    public ListViewPicker Easy1;
    public bool ShortGame { get; set; }
    public bool EasyWords { get; set; }
    private readonly BasicData _basicData;
    private readonly IAdvancedProcesses _processes;
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
            return true;
        }
    }
    [Command(EnumCommandCategory.Plain)]
    public Task SubmitAsync()
    {
        return _processes.ChoseAdvancedOptions(EasyWords, ShortGame);
    }
    private Task Easy1_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        if (selectedIndex == 1)
        {
            EasyWords = true;
        }
        else
        {
            EasyWords = false;
        }
        return Task.CompletedTask;
    }
    private Task Game1_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        if (selectedIndex == 1)
        {
            ShortGame = true;
        }
        else
        {
            ShortGame = false;
        }
        return Task.CompletedTask;
    }
    public void SelectSpecificOptions()
    {
        if (EasyWords == true)
        {
            Easy1.SelectSpecificItem(1);
        }
        else
        {
            Easy1.SelectSpecificItem(2);
        }
        if (ShortGame == true)
        {
            Game1.SelectSpecificItem(1);
        }
        else
        {
            Game1.SelectSpecificItem(2);
        }
    }
    async Task IHandleAsync<AdvancedSettingsEventModel>.HandleAsync(AdvancedSettingsEventModel message)
    {
        AdvancedSettingModel model = await js1.DeserializeObjectAsync<AdvancedSettingModel>(message.Message);
        EasyWords = model.IsEasy;
        ShortGame = model.ShortGame;
        SelectSpecificOptions();
        await SubmitAsync();
    }
}