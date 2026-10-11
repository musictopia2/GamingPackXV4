namespace RageCardGame.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class RageCardGamePromptViewModel(RageDelgates delgates) : IPromptVM
{
    [StateProperty]
    public partial RageBiddingViewModel? BidScreen { get; private set; }
    [StateProperty]
    public partial RageColorViewModel? ColorScreen { get; private set; }
    void IPromptVM.Initialize()
    {
        if (delgates.GetCategory is null)
        {
            throw new CustomBasicException("Nobody is handling the screens");
        }
        EnumScreenCategory category = delgates.GetCategory.Invoke();
        if (category == EnumScreenCategory.Bid)
        {
            BidScreen = Resolver!.Resolve<RageBiddingViewModel>();
            return;
        }
        if (category == EnumScreenCategory.Color)
        {
            ColorScreen = Resolver!.Resolve<RageColorViewModel>();
            return;
        }
        //try to do nothing (?)
        throw new CustomBasicException("Not Supported");
    }
}