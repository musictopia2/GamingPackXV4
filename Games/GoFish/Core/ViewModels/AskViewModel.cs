﻿namespace GoFish.Core.ViewModels;
[InstanceGame]
public partial class AskViewModel : ScreenViewModel, IBlankGameVM, IBasicEnableProcess
{
    private readonly GoFishVMData _model;
    private readonly GoFishGameContainer _gameContainer;
    private readonly IAskProcesses _processes;
    public AskViewModel(CommandContainer commandContainer, GoFishVMData model, GoFishGameContainer gameContainer, IAskProcesses processes, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _model = model;
        _gameContainer = gameContainer;
        _processes = processes;
        _model.AskList.ItemClickedAsync = AskList_ItemClickedAsync;
        _model.AskList.SendEnableProcesses(this, () => _gameContainer.SaveRoot.RemovePairs == false && _gameContainer.SaveRoot.NumberAsked == false);
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private Task AskList_ItemClickedAsync(EnumRegularCardValueList piece)
    {
        _model.CardYouAsked = piece;
        _model.AskList!.SelectSpecificItem(piece);
        return Task.CompletedTask;
    }
    public CommandContainer CommandContainer { get; set; }
    bool IBasicEnableProcess.CanEnableBasics()
    {
        return true;
    }
    public bool CanAsk
    {
        get
        {
            if (_gameContainer.SaveRoot.RemovePairs == true || _gameContainer.SaveRoot.NumberAsked == true)
            {
                return false;
            }
            return _model.CardYouAsked != EnumRegularCardValueList.None;
        }
    }

    [Command(EnumCommandCategory.Game)]
    public async Task AskAsync()
    {
        if (_gameContainer.BasicData!.MultiPlayer == true)
        {
            await _gameContainer.Network!.SendAllAsync("numbertoask", _model.CardYouAsked);
        }
        await _processes!.NumberToAskAsync(_model.CardYouAsked);
    }
}