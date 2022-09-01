namespace Fluxx.Core.ViewModels;
public class PlayViewModel : BasicSubmitViewModel
{
    private readonly IPlayProcesses _playProcesses;
    private readonly FluxxMainGameClass _mainGame;
    private readonly FluxxGameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly FluxxVMData _model;
    public override string Text => "Play";
    public PlayViewModel(CommandContainer commandContainer, 
        IPlayProcesses playProcesses, 
        FluxxMainGameClass mainGame, 
        FluxxGameContainer gameContainer,
        IToast toast,
        FluxxVMData model, 
        IEventAggregator aggregator) : base(commandContainer, aggregator)
    {
        _playProcesses = playProcesses;
        _mainGame = mainGame;
        _gameContainer = gameContainer;
        _toast = toast;
        _model = model;
    }
    private bool IsOtherTurn => _mainGame!.OtherTurn > 0;
    public override bool CanSubmit => !IsOtherTurn;
    public override async Task SubmitAsync()
    {
        if (_model.Goal1!.ObjectSelected() > 0)
        {
            _toast.ShowUserErrorToast("Cannot select any goal cards to play");
            _model.UnselectAllCards();
            return;
        }
        if (_model.Keeper1!.ObjectSelected() > 0)
        {
            _toast.ShowUserErrorToast("Cannot select any keeper cards to play");
            _model.UnselectAllCards();
            return;
        }
        if (_gameContainer!.NeedsToRemoveGoal())
        {
            _toast.ShowUserErrorToast("Cannot choose any cards to play until you discard a goal");
            _model.UnselectAllCards();
            return;
        }
        int howMany = _model.PlayerHand1!.HowManySelectedObjects;
        if (howMany != 1)
        {
            _toast.ShowUserErrorToast("Must choose only one card to play");
            _model.UnselectAllCards();
            return;
        }
        if (_mainGame.SaveRoot!.PlaysLeft <= 0)
        {
            _toast.ShowUserErrorToast("Sorry; you don't have any plays left");
            _model.UnselectAllCards();
            return;
        }
        int deck = _model.PlayerHand1.ObjectSelected();
        await _playProcesses.SendPlayAsync(deck);
        await _playProcesses.PlayCardAsync(deck);
    }
}