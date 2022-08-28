namespace LifeBoardGame.Core.ViewModels;
public class ReturnStockViewModel : BasicSubmitViewModel
{
    private readonly LifeBoardGameGameContainer _gameContainer;
    private readonly LifeBoardGameVMData _model;
    private readonly IReturnStockProcesses _processes;
    public ReturnStockViewModel(CommandContainer commandContainer,
        LifeBoardGameGameContainer gameContainer,
        LifeBoardGameVMData model,
        IReturnStockProcesses processes,
        IEventAggregator aggregator
        ) : base(commandContainer, aggregator)
    {
        _gameContainer = gameContainer;
        _model = model;
        _processes = processes;
        if (_gameContainer.SaveRoot.GameStatus != EnumWhatStatus.NeedReturnStock)
        {
            throw new CustomBasicException("Does not even need to return stock.  Rethink");
        }
    }
    public override bool CanSubmit => _model.HandList.ObjectSelected() > 0;
    public override Task SubmitAsync()
    {
        return _processes.StockReturnedAsync(_model.HandList.ObjectSelected());
    }
}