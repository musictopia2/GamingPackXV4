namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfacesForHelpers;
public interface IAdditionalRollProcess
{
    Task<bool> CanRollAsync();
    Task BeforeRollingAsync();
}