namespace DealCardGame.Core.ViewModels;
[InstanceGame]
public partial class RentViewModel : IBasicEnableProcess
{
    private readonly DealCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly DealCardGameVMData _model;
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
        _model = model;
        _privateAutoResume = privateAutoResume;
        _mainGame = mainGame;
        RentChooser rent = new(gameContainer);
        RentPicker = new(gameContainer.Command, rent);

        if (_gameContainer.PersonalInformation.RentInfo.Player == -1)
        {
            //this means needs to populate the picker.
            _mainGame.LoadPlayerPicker();
        }

        //may already load the entire list anyways (?)
        if (RentPicker.ItemList.Count == 0)
        {
            _toast.ShowUserErrorToast("Has to have at least one item for the rent picker ui");
            return;
        }
        if (RentPicker.ItemList.Count == 1)
        {
            //VMData.RentPicker.SelectSpecificItem(EnumRentCategory.Alone); //this will always be the option if no other options are available
            _gameContainer.PersonalInformation.RentInfo.RentCategory = EnumRentCategory.Alone;
            RentOwed = _gameContainer.PersonalInformation.RentInfo.RentOwed(_mainGame.SingleInfo!);
            NeedsRentPicker = false;
        }
        else
        {
            RentPicker.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
            RentPicker.ItemClickedAsync = ItemSelectedAsync;
            RentOwed = -1; //because don't know yet.
            NeedsRentPicker = true;
        }
        RentPicker.IsEnabled = NeedsRentPicker;
    }
    public void AddAction( Action action )
    {
        _gameContainer.Command.CustomStateHasChanged += action;
    }
    private Task ItemSelectedAsync(EnumRentCategory category)
    {
        _gameContainer.PersonalInformation.RentInfo.RentCategory = category;
        RentOwed = _gameContainer.PersonalInformation.RentInfo.RentOwed(_mainGame.SingleInfo!);
        RentPicker.SelectSpecificItem(category); //i think.
        return Task.CompletedTask;
    }
    public bool NeedsRentPicker { get; private set; }
    public bool NeedsPlayerPicker => _gameContainer.PersonalInformation.RentInfo.Player == -1;
    public int RentOwed { get; private set; }
    public ListViewPicker GetPlayerPicker => _model.PlayerPicker;
    public EnumColor ColorChosen => _gameContainer.PersonalInformation.RentInfo.Color;
    public SimpleEnumPickerVM<EnumRentCategory> RentPicker { get; set; }
    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.Game)]
    public async Task CancelAsync()
    {
        _gameContainer.PersonalInformation.RentInfo.RentCategory = EnumRentCategory.NA;
        _gameContainer.PersonalInformation.RentInfo.Color = EnumColor.None; //none chosen now.
        _gameContainer.PersonalInformation.RentInfo.Deck = 0;
        _gameContainer.PersonalInformation.RentInfo.Player = -1;
        _gameContainer.Command.ResetCustomStates();
        _model.PlayerHand1.UnselectAllObjects(); //if you can cancelling, must manually select one again.
        await _privateAutoResume.SaveStateAsync(_gameContainer);
        _gameContainer.Command.UpdateAll(); //i think.
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ProcessRentRequestAsync()
    {
        if (_gameContainer.PersonalInformation.RentInfo.RentCategory == EnumRentCategory.NeedChoice)
        {
            _toast.ShowUserErrorToast("Must choose the rent category");
            return;
        }
        if (_model.PlayerPicker.SelectedIndex > 0)
        {
            string nickName = _model.PlayerPicker.GetText(_model.PlayerPicker.SelectedIndex);
            _gameContainer.PersonalInformation.RentInfo.Player = _gameContainer.PlayerList!.Single(x => x.NickName == nickName).Id;
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
        _gameContainer.Command.ResetCustomStates();
        _model.PlayerPicker.SelectedIndex = 0; //i think.
        await _mainGame.PossibleRentRequestAsync(_gameContainer.PersonalInformation.RentInfo);
    }
    public bool CanEnableBasics()
    {
        return true;
    }
}