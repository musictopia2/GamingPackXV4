namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfacesForHelpers;
public interface IHoldUnholdProcesses : IEndTurn
{
    Task HoldUnholdDiceAsync(int index);
}