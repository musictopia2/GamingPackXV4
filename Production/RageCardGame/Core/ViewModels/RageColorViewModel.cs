namespace RageCardGame.Core.ViewModels;
[InstanceGame]
public class RageColorViewModel(RageCardGameVMData model, RageCardGameGameContainer gameContainer)
{
    public readonly RageCardGameVMData Model = model;
    public RageCardGameGameContainer GameContainer { get; } = gameContainer;
}