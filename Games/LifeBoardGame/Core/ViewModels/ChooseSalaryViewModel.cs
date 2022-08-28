namespace LifeBoardGame.Core.ViewModels;
public class ChooseSalaryViewModel : BasicSubmitViewModel
{
    private readonly LifeBoardGameGameContainer _gameContainer;
    private readonly LifeBoardGameVMData _model;
    private readonly IBasicSalaryProcesses _processes;
    public ChooseSalaryViewModel(CommandContainer commandContainer,
        LifeBoardGameGameContainer gameContainer,
        LifeBoardGameVMData model,
        IBasicSalaryProcesses processes,
        IEventAggregator aggregator
        ) : base(commandContainer, aggregator)
    {
        _gameContainer = gameContainer;
        _model = model;
        _processes = processes;
        if (_gameContainer.SaveRoot.GameStatus != EnumWhatStatus.NeedChooseSalary)
        {
            throw new CustomBasicException("Does not even need to choose salary.  Rethink");
        }
    }
    public override bool CanSubmit => _model.HandList.ObjectSelected() > 0;
    public override Task SubmitAsync()
    {
        return _processes.ChoseSalaryAsync(_model.HandList.ObjectSelected());
    }
}