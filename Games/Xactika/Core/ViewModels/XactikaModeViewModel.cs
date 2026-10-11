namespace Xactika.Core.ViewModels;
[InstanceGame]
public partial class XactikaModeViewModel(CommandContainer commandContainer,
        XactikaVMData model,
        XactikaGameContainer gameContainer,
        IModeProcesses processes) : IPromptVM
{
    public readonly XactikaVMData Model = model;
    public bool WasResumed { get; private set; }
    partial void CreateCommands(CommandContainer command);
    public bool CanMode => Model.ModeChosen != EnumGameMode.None;
    [Command(EnumCommandCategory.Plain)]
    public async Task ModeAsync()
    {
        if (gameContainer.BasicData.MultiPlayer)
        {
            await gameContainer.Network!.SendAllAsync("gameoptionchosen", Model.ModeChosen);
        }
        await processes.ProcessGameOptionChosenAsync(Model.ModeChosen, true);
    }
    void IPromptVM.Initialize()
    {
        WasResumed = Model.ModeChosen != EnumGameMode.None;
        CreateCommands(commandContainer);
    }
}
