﻿namespace Xactika.Core.ViewModels;
[InstanceGame]
public partial class XactikaModeViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly XactikaVMData Model;
    private readonly XactikaGameContainer _gameContainer;
    private readonly IModeProcesses _processes;
    public bool WasResumed { get; private set; }
    public XactikaModeViewModel(CommandContainer commandContainer,
        XactikaVMData model,
        XactikaGameContainer gameContainer,
        IModeProcesses processes,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        Model = model;
        WasResumed = Model.ModeChosen != EnumGameMode.None;
        _gameContainer = gameContainer;
        _processes = processes;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    public bool CanMode => Model.ModeChosen != EnumGameMode.None;
    [Command(EnumCommandCategory.Plain)]
    public async Task ModeAsync()
    {
        if (_gameContainer.BasicData.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("gameoptionchosen", Model.ModeChosen);
        }
        await _processes.ProcessGameOptionChosenAsync(Model.ModeChosen, true);
    }
}
