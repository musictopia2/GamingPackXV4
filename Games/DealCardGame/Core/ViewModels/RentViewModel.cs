﻿namespace DealCardGame.Core.ViewModels;
[InstanceGame]
public partial class RentViewModel : IBasicEnableProcess
{
    private readonly DealCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    public readonly DealCardGameVMData VMData;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    private readonly DealCardGameMainGameClass _mainGame;
    public RentViewModel(DealCardGameGameContainer gameContainer,
        IToast toast,
        DealCardGameVMData model,
        PrivateAutoResumeProcesses privateAutoResume,
        DealCardGameMainGameClass mainGame
        )
    {
        CreateCommands(gameContainer.Command);
        _gameContainer = gameContainer;
        _toast = toast;
        VMData = model;
        _privateAutoResume = privateAutoResume;
        _mainGame = mainGame;
        VMData.RentPicker.LoadEntireList(); //i think.
        VMData.RentPicker.AutoSelectCategory = EnumAutoSelectCategory.AutoSelect;
        if (VMData.RentPicker.ItemList.Count == 1)
        {
            VMData.RentPicker.SelectSpecificItem(EnumRentCategory.Alone); //this will always be the option if no other options are available
        }

        //well see about the player picker.

    }
    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.Game)]
    public async Task CancelAsync()
    {
        _gameContainer.PersonalInformation.RentInfo.RentCategory = EnumRentCategory.NA;
        _gameContainer.PersonalInformation.RentInfo.Color = EnumColor.None; //none chosen now.
        _gameContainer.PersonalInformation.RentInfo.Deck = 0;
        _gameContainer.PersonalInformation.RentInfo.Player = -1;
        await _privateAutoResume.SaveStateAsync(_gameContainer);
        _gameContainer.Command.UpdateAll(); //i think.
    }
    public async Task ProcessRentRequestAsync()
    {
        if (VMData.RentPicker.ItemChosen == default)
        {
            _toast.ShowUserErrorToast("Must choose the rent category");
            return;
        }
        if (_gameContainer.PersonalInformation.RentInfo.Player == -1)
        {
            _toast.ShowUserErrorToast("Needs to choose a player to charge rent to");
            return;
        }
        if (_gameContainer.PersonalInformation.RentInfo.Deck == 0)
        {
            _toast.ShowUserErrorToast("Needs to know which card was being used to charge rent");
            return;
        }
        if (_gameContainer.PersonalInformation.RentInfo.Color == EnumColor.None)
        {
            _toast.ShowUserErrorToast("Needs to know the color being used to charge rent for");
            return;
        }
        if (_gameContainer.BasicData.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("rentrequest", _gameContainer.PersonalInformation.RentInfo);
        }
        await _mainGame.RentRequestAsync(_gameContainer.PersonalInformation.RentInfo);
    }
    public bool CanEnableBasics()
    {
        return true;
    }
}