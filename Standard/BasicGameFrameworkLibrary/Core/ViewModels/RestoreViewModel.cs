﻿namespace BasicGameFrameworkLibrary.Core.ViewModels;
public partial class RestoreViewModel : ScreenViewModel, IRestoreVM, IBlankGameVM
{
    public CommandContainer CommandContainer { get; set; }
    public RestoreViewModel(CommandContainer command, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = command;
        CreateCommands();
    }
    partial void CreateCommands();
    [Command(EnumCommandCategory.Old)]
    public Task RestoreAsync()
    {
        return Aggregator.PublishAsync(new RestoreEventModel());
    }
}