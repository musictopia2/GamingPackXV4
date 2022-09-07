namespace BasicGameFrameworkLibrary.Core.ViewModels;
public abstract partial class BasicSubmitViewModel : ScreenViewModel, IBlankGameVM, IMainScreen, ISubmitText
{
    public abstract bool CanSubmit { get; } 
    [Command(EnumCommandCategory.Plain)]
    public abstract Task SubmitAsync();
    public virtual string Text => "Submit";
    public BasicSubmitViewModel(CommandContainer commandContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    ICustomCommand ISubmitText.Command => SubmitCommand!;
}