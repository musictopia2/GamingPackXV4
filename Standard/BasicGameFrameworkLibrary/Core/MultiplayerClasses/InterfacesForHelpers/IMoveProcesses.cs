namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfacesForHelpers;
public interface IMoveProcesses<M>
{
    Task MakeMoveAsync(M space);
}