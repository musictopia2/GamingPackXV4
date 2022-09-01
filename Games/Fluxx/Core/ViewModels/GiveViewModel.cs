namespace Fluxx.Core.ViewModels;
public class GiveViewModel : BasicSubmitViewModel
{
    private readonly FluxxVMData _model;
    private readonly FluxxGameContainer _gameContainer;
    private readonly IGiveTaxationProcesses _processes;
    private readonly IToast _toast;
    public override string Text => "Give";
    public GiveViewModel(
        CommandContainer commandContainer,
        FluxxVMData model,
        FluxxGameContainer gameContainer,
        IGiveTaxationProcesses processes,
        IEventAggregator aggregator,
        IToast toast
        ) : base(commandContainer, aggregator)
    {
        _model = model;
        _gameContainer = gameContainer;
        _processes = processes;
        _toast = toast;
    }
    public override bool CanSubmit => true;
    public override async Task SubmitAsync()
    {
        if (_model.Keeper1!.HowManySelectedObjects > 0)
        {
            _toast.ShowUserErrorToast("Cannot select any keeper cards because you have to give the cards from your hand");
            _model.UnselectAllCards();
            return;
        }
        if (_model.Goal1!.ObjectSelected() > 0)
        {
            _toast.ShowUserErrorToast("Cannot select any goal cards because you have to give the cards from your hand");
            _model.UnselectAllCards();
            return;
        }
        int howMany = _gameContainer.IncreaseAmount() + 1;
        if (_model.PlayerHand1!.HowManySelectedObjects == howMany || _model.PlayerHand1.HowManySelectedObjects == _model.PlayerHand1.HandList.Count)
        {
            var thisList = _model.PlayerHand1.ListSelectedObjects(true);
            await _processes.GiveCardsForTaxationAsync(thisList);
            return;
        }
        if (howMany > _model.PlayerHand1.HandList.Count)
        {
            howMany = _model.PlayerHand1.HandList.Count;
        }
        _toast.ShowUserErrorToast($"Must give {howMany} not {_model.PlayerHand1.HowManySelectedObjects} cards");
        _model.UnselectAllCards();
    }
}