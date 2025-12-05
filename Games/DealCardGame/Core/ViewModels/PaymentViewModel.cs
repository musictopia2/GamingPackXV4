namespace DealCardGame.Core.ViewModels;
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
    public async Task PutSelectedBackAsync()
    {
        _gameContainer.Command.UpdateAll();
        var player = _gameContainer.PlayerList!.GetSelf();
        var payments = _gameContainer.PersonalInformation.Payments.GetSelectedItems();
        if (payments.Count == 0)
        {
            _toast.ShowUserErrorToast("You did not choose any cards to put back");
            return;
        }
        PrivateModel temporaryPersonal = _gameContainer.PersonalInformation.CloneTemporaryModel();
        decimal soFar = VMData.PaidSoFar;
        DeckRegularDict<DealCardGameCardInformation> properties = [];
        foreach (var card in payments)
        {
            VMData.PaidSoFar -= card.ClaimedValue;
            _gameContainer.PersonalInformation.Payments.RemoveObjectByDeck(card.Deck);
            card.IsSelected = false;
            if (card.ActionCategory == EnumActionCategory.House || card.ActionCategory == EnumActionCategory.Hotel)
            {
                if (player.WasHouseOrHotelBanked(card))
                {
                    _gameContainer.PersonalInformation.BankedCards.Add(card);
                }
                else
                {
                    properties.Add(card);
                    //housesHotels.Add(card); //has to figure out if you can do or not.  i think instead of going to bank, give an error message instead.
                }
            }
            else if (card.CardType != EnumCardType.PropertyRegular && card.CardType != EnumCardType.PropertyWild)
            {
                //put in their bank now.
                _gameContainer.PersonalInformation.BankedCards.Add(card); //hopefully this simple (?)
            }
            else
            {
                //if it is a property, then you have to put it back in the set.
                properties.Add(card);
            }
        }
        player.CloneSelectedPropertiesForPayments(_gameContainer.PersonalInformation, properties);
        if (_gameContainer.PersonalInformation.IsValidState == false)
        {
            ProcessInvalidState("Cannot choose these to put back because houses/hotels would be corrupted", soFar, temporaryPersonal);
            return;
        }
        VMData.Properties.HandList = _gameContainer.PersonalInformation.SetData.GetAllCardsFromPlayersSet();
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    private void ProcessInvalidState(string message, decimal soFar, PrivateModel temporaryPersonal)
    {
        VMData.PaidSoFar = soFar;
        _gameContainer.PersonalInformation = temporaryPersonal; //back to this now.
        VMData.Bank.HandList = _gameContainer.PersonalInformation.BankedCards; //may need to hook up again.
        VMData.Bank.HandList.Sort();
        VMData.Payments.HandList = _gameContainer.PersonalInformation.Payments;
        VMData.Properties.HandList = _gameContainer.PersonalInformation.SetData.GetAllCardsFromPlayersSet();
        _toast.ShowUserErrorToast(message);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task AddAllAsync()
    {
        _gameContainer.Command.UpdateAll();
        var player = _gameContainer.PlayerList!.GetSelf();
        var bankedCards = _gameContainer.PersonalInformation.BankedCards;
        var properties = _gameContainer.PersonalInformation.SetData;
        foreach (var property in properties)
        {
            var list = property.Cards.ToRegularDeckDict();
            foreach (var card in property.Cards)
            {
                VMData.PaidSoFar += card.ClaimedValue;
                _gameContainer.PersonalInformation.Payments.Add(card);
            }
        }
        VMData.PaidSoFar += bankedCards.Sum(x => x.ClaimedValue);
        foreach (var item in bankedCards)
        {
            _gameContainer.PersonalInformation.Payments.Add(item);
            VMData.PaidSoFar += item.ClaimedValue;
        }
        bankedCards.Clear();
        VMData.Properties.ClearHand();
        _gameContainer.PersonalInformation.SetData.Clear();
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task StartOverAsync()
    {
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
        decimal soFar = VMData.PaidSoFar;
        PrivateModel temporaryPersonal = _gameContainer.PersonalInformation.CloneTemporaryModel();
        foreach (var property in properties)
        {
            foreach (var card in property.Cards)
            {
                card.IsSelected = false;
                VMData.PaidSoFar += card.ClaimedValue;
                _gameContainer.PersonalInformation.Payments.Add(card);
                _gameContainer.PersonalInformation.SetData.RemoveCardFromPlayerSet(card.Deck, property.Color);
                VMData.Properties.HandList = _gameContainer.PersonalInformation.SetData.GetAllCardsFromPlayersSet();
            }
        }
        VMData.PaidSoFar += bankedCards.Sum(x => x.ClaimedValue);
        _gameContainer.PersonalInformation.BankedCards.RemoveSelectedItems();
        bankedCards.UnselectAllObjects();
        _gameContainer.PersonalInformation.Payments.AddRange(bankedCards);
        if (_gameContainer.PersonalInformation.IsValidState == false)
        {
            ProcessInvalidState("Cannot add the cards for payments because did not follow the rules for houses/hotels", soFar, temporaryPersonal);
            return;
        }
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