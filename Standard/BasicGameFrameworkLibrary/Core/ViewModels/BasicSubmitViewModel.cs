namespace BasicGameFrameworkLibrary.Core.ViewModels;
public abstract partial class BasicSubmitViewModel : ScreenViewModel, IBlankGameVM, IMainScreen, ISubmitText
{
    public abstract bool CanSubmit { get; } //i think this is the best way to go.
    [Command(EnumCommandCategory.Plain)]
    public abstract Task SubmitAsync();
    public virtual string Text => "Submit"; //since this is default, will use this to start with.
    public BasicSubmitViewModel(CommandContainer commandContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    ICustomCommand ISubmitText.Command => SubmitCommand!;
}