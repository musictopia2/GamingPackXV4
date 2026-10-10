namespace BasicGameFrameworkLibrary.Core.ViewModels;
public abstract partial class BasicSubmitViewModel : ISubmitText
{
    public abstract bool CanSubmit { get; }

    [Command(EnumCommandCategory.Plain)]
    public abstract Task SubmitAsync();

    public virtual string Text => "Submit";

    protected BasicSubmitViewModel(CommandContainer commandContainer)
    {
        CommandContainer = commandContainer;
        CreateCommands(commandContainer);
    }

    partial void CreateCommands(CommandContainer command);

    public CommandContainer CommandContainer { get; }

    ICustomCommand ISubmitText.Command => SubmitCommand!;
}