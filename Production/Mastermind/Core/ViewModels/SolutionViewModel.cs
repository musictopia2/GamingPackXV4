namespace Mastermind.Core.ViewModels;
[InstanceGame]
public partial class SolutionViewModel(GlobalClass global, IEventAggregator aggregator) : IPostGameVM, IHandleAsync<OpenPostGameEventModel>, IHandleAsync<ClosePostGameEventModel>
{
    public BasicList<Bead> SolutionList = [];
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IPostGameVM.Initialize()
    {
        Subscribe();
        if (global.Solution == null)
        {
            throw new CustomBasicException("There is no solution found.  Rethink");
        }
        SolutionList = global.Solution;
    }
    async Task IHandleAsync<OpenPostGameEventModel>.HandleAsync(OpenPostGameEventModel message)
    {
        //nothing is needed here.
    }
    async Task IHandleAsync<ClosePostGameEventModel>.HandleAsync(ClosePostGameEventModel message)
    {
        Unsubscribe();
    }
}