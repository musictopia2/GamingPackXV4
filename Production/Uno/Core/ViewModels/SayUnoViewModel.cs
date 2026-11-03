namespace Uno.Core.ViewModels;
[InstanceGame]
public partial class SayUnoViewModel
{
    private readonly ISayUnoProcesses _processes;
    private readonly UnoVMData _model;
    public SayUnoViewModel(CommandContainer commandContainer, ISayUnoProcesses processes, UnoVMData model)
    {
        CommandContainer = commandContainer;
        _processes = processes;
        _model = model;
        _model.Stops.TimeUp += Stops_TimeUp;
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer.IsExecuting)
        {
            return;
        }
        _model.Stops.StartTimer();
    }

    private async void Stops_TimeUp()
    {
        CommandContainer!.ManuelFinish = true;
        CommandContainer.IsExecuting = true;
        await _processes.ProcessUnoAsync(false);
    }
    public void Close()
    {
        _model.Stops.TimeUp -= Stops_TimeUp;
    }
    public CommandContainer CommandContainer { get; set; }
    [Command(EnumCommandCategory.Plain)]
    public async Task SayUnoAsync()
    {
        _model.Stops!.PauseTimer();
        await _processes!.ProcessUnoAsync(true);
    }
}
