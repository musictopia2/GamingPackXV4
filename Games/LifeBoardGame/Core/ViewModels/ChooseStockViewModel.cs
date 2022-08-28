namespace LifeBoardGame.Core.ViewModels;
public class ChooseStockViewModel : BasicSubmitViewModel
{
    private readonly LifeBoardGameGameContainer _gameContainer;
    private readonly LifeBoardGameVMData _model;
    private readonly IChooseStockProcesses _processes;
    public ChooseStockViewModel(CommandContainer commandContainer,
        LifeBoardGameGameContainer gameContainer,
        LifeBoardGameVMData model,
        IChooseStockProcesses processes,
        IEventAggregator aggregator
        ) : base(commandContainer, aggregator)
    {
        _gameContainer = gameContainer;
        _model = model;
        _processes = processes;
        if (_gameContainer.SaveRoot.GameStatus != EnumWhatStatus.NeedChooseStock)
        {
            throw new CustomBasicException("Does not even need to choose stock.  Rethink");
        }
    }
    public override bool CanSubmit => _model.HandList.ObjectSelected() > 0;
    public override Task SubmitAsync()
    {
        return _processes.ChoseStockAsync(_model.HandList.ObjectSelected());
    }
}