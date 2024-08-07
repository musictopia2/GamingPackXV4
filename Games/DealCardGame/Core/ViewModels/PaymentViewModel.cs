﻿namespace DealCardGame.Core.ViewModels;
[InstanceGame]
public partial class PaymentViewModel : IBasicEnableProcess
{
    private readonly DealCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    public readonly DealCardGameVMData VMData;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    private readonly DealCardGameMainGameClass _mainGame;
    private readonly DealCardGamePlayerItem _player;
    public PaymentViewModel(DealCardGameGameContainer gameContainer,
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
        _player = _gameContainer.PlayerList!.GetSelf();
        VMData.OtherTurn = _player.NickName;
        VMData.Owed = _player.Debt;
        VMData.Payments.HandList = _gameContainer.PersonalInformation.Payments;
        VMData.PaidSoFar = _gameContainer.PersonalInformation.Payments.Sum(x => x.ClaimedValue);
        VMData.Bank.HandList = _gameContainer.PersonalInformation.BankedCards;
        VMData.Bank.HandList.Sort();
        VMData.Properties.HandList = _gameContainer.PersonalInformation.SetData.GetAllCardsFromPlayersSet();
    }
    partial void CreateCommands(CommandContainer command);
    
    public void AddCommand(Action action)
    {
        _gameContainer.Command.CustomStateHasChanged += action;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task StartOverAsync()
    {
        //this is for self.
        //NotifyStateChange?.Invoke();
        _gameContainer.Command.UpdateAll();
        var player = _gameContainer.PlayerList!.GetSelf();
        _gameContainer.PersonalInformation.BankedCards = player.BankedCards.ToRegularDeckDict();
        player.ClonePlayerProperties(_gameContainer.PersonalInformation);
        VMData.Payments.HandList = _gameContainer.PersonalInformation.Payments;
        VMData.Bank.HandList = _gameContainer.PersonalInformation.BankedCards; //may need to hook up again.
        VMData.Bank.HandList.Sort();
        _gameContainer.PersonalInformation.Payments.Clear(); //you are clearing the payments.
        VMData.PaidSoFar = 0;
        VMData.Properties.HandList = _gameContainer.PersonalInformation.SetData.GetAllCardsFromPlayersSet();
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task AddPaymentsAsync()
    {
        var bankedCards = _gameContainer.PersonalInformation.BankedCards.GetSelectedItems();
        var properties = _gameContainer.PersonalInformation.SetData.GetSelectedProperties();
        if (properties.Count > 1)
        {
            _toast.ShowUserErrorToast("Should not have been allowed to choose more than one property");
            return;
        }
        //you should be able to do both though.
        if (properties.Count == 1)
        {
            SetPropertiesModel property = properties.Single();
            if (property.Cards.Count > 1)
            {
                _toast.ShowUserErrorToast("Should not have been allowed to choose more than one card for a property");
                return;
            }
            if (property.Cards.Count == 0)
            {
                _toast.ShowUserErrorToast("No cards was even selected for the property.  Should have returned no properties");
                return;
            }
            var card = property.Cards.Single();
            if (card.ActionCategory != EnumActionCategory.House && card.ActionCategory != EnumActionCategory.Hotel)
            {
                var list = _gameContainer.PersonalInformation.SetData.GetCards(property.Color);

                //if you have any properties there that is not house or hotel must remove those first.
                //var list = _player.SetData.GetCards(property.Color);
                if (list.Any(x => x.ActionCategory == EnumActionCategory.House || x.ActionCategory == EnumActionCategory.Hotel))
                {
                    _toast.ShowUserErrorToast("You must remove the house or hotel before removing the property for payment");
                    return;
                }
            }
            if (card.ActionCategory == EnumActionCategory.House)
            {
                var list = _gameContainer.PersonalInformation.SetData.GetCards(property.Color);
                if (list.Any(x => x.ActionCategory == EnumActionCategory.Hotel))
                {
                    _toast.ShowUserErrorToast("You must remove the hotel before removing the house");
                    return;
                }
            }
            if (card.ClaimedValue == 0)
            {
                _toast.ShowUserErrorToast("You do not have to use this card to pay because it has no claimed value");
                return;
            }
            card.IsSelected = false;
            VMData.PaidSoFar += card.ClaimedValue;
            _gameContainer.PersonalInformation.Payments.Add(card);
            _gameContainer.PersonalInformation.SetData.RemoveCardFromPlayerSet(card.Deck, property.Color);
            VMData.Properties.HandList = _gameContainer.PersonalInformation.SetData.GetAllCardsFromPlayersSet();
            await _privateAutoResume.SaveStateAsync(_gameContainer);
            return;
        }
        if (bankedCards.Count == 0)
        {
            _toast.ShowUserErrorToast("You did not choose any cards for payment");
            return;
        }
        VMData.PaidSoFar += bankedCards.Sum(x => x.ClaimedValue);
        _gameContainer.PersonalInformation.BankedCards.RemoveSelectedItems();
        bankedCards.UnselectAllObjects();
        _gameContainer.PersonalInformation.Payments.AddRange(bankedCards);
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task FinishPaymentAsync()
    {
        if (VMData.PaidSoFar < VMData.Owed)
        {
            //if you never had enough, would have done automatically.
            _toast.ShowUserErrorToast("You did not pay enough to satisfy the payment");
            return;
        }
        //you need a list of cards used for payment.
        BasicList<int> cards = _gameContainer.PersonalInformation.Payments.GetDeckListFromObjectList();
        await _mainGame.ProcessPaymentsAsync(cards);   
    }
    public bool CanEnableBasics()
    {
        return true;
    }
}