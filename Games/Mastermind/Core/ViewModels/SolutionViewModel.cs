namespace Mastermind.Core.ViewModels;
[InstanceGame]
public class SolutionViewModel : ScreenViewModel, IMainScreen, IBlankGameVM
{
    public BasicList<Bead> SolutionList = new();
    public SolutionViewModel(GlobalClass global, CommandContainer command, IEventAggregator aggregator) : base(aggregator)
    {
        if (global.Solution == null)
        {
            throw new CustomBasicException("There is no solution found.  Rethink");
        }
        CommandContainer = command;
        SolutionList = global.Solution;
    }
    public CommandContainer CommandContainer { get; set; }
}