﻿namespace LifeBoardGame.Core.ViewModels;
public class ChooseCareerViewModel : BasicSubmitViewModel
{
    private readonly LifeBoardGameGameContainer _gameContainer;
    private readonly LifeBoardGameVMData _model;
    private readonly ICareerProcesses _processes;
    public ChooseCareerViewModel(CommandContainer commandContainer,
        LifeBoardGameGameContainer gameContainer,
        LifeBoardGameVMData model,
        ICareerProcesses processes,
        IEventAggregator aggregator
        ) : base(commandContainer, aggregator)
    {
        _gameContainer = gameContainer;
        _model = model;
        _processes = processes;
    }
    public override bool CanSubmit
    {
        get
        {
            if (_model.HandList.AutoSelect == EnumHandAutoType.SelectOneOnly)
            {
                return _model.HandList.ObjectSelected() > 0;
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
            {
                if (_gameContainer.SingleInfo!.OptionChosen != EnumStart.College)
                {
                    throw new CustomBasicException("Should have been one option alone.");
                }
                return _model.HandList.HowManySelectedObjects == 3;
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumWhatStatus.NeedNewCareer)
            {
                return _model.HandList.HowManySelectedObjects == 2;
            }
            throw new CustomBasicException("Cannot figure out whether it can submit or not.  Rethink");
        }
    }
    private void AdditionalCareerInstructions()
    {
        var tempList = _model.HandList!.ListSelectedObjects();
        tempList.MakeAllObjectsKnown();
        _model.HandList.HandList.ReplaceRange(tempList);
        _model.HandList.Text = "Choose One Career";
        if (_gameContainer.WasNight)
        {
            _model.Instructions = "Choose one career or end turn and keep your current career";
        }
        _model.HandList.AutoSelect = EnumHandAutoType.SelectOneOnly;
        _model.HandList.UnselectAllObjects();
    }
    public override Task SubmitAsync()
    {
        if (_model.HandList!.AutoSelect == EnumHandAutoType.SelectAsMany)
        {
            AdditionalCareerInstructions();
            return Task.CompletedTask;
        }
        if (_gameContainer.GameStatus == EnumWhatStatus.NeedNewCareer || _gameContainer.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
        {
            return _processes.ChoseCareerAsync(_model.HandList.ObjectSelected());
        }
        throw new CustomBasicException("Unable to submit based on the status of the game.  Rethink");
    }
}