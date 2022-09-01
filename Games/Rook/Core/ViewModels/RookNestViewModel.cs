namespace Rook.Core.ViewModels;
[InstanceGame]
public partial class RookNestViewModel : ScreenViewModel, IBlankGameVM, ISubmitText
{
    private readonly RookVMData _model;
    private readonly INestProcesses _processes;
    private readonly IToast _toast;
    public string Text => "Choose Nest Cards";
    public RookNestViewModel(CommandContainer commandContainer,
        RookVMData model,
        INestProcesses processes,
        IEventAggregator aggregator,
        IToast toast
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _model = model;
        _processes = processes;
        _toast = toast;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    ICustomCommand ISubmitText.Command => NestCommand!;

    [Command(EnumCommandCategory.Plain)]
    public async Task NestAsync()
    {
        var thisList = _model.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count != 5)
        {
            _toast.ShowUserErrorToast("Sorry, you must choose 5 cards to throw away");
            return;
        }
        await _processes!.ProcessNestAsync(thisList);
    }
}