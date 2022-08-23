namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfacesForHelpers;

public interface IBeginningColorProcesses<E>
    where E : IFastEnumColorSimple
{
    Task ChoseColorAsync(E colorChosen);
    Task InitAsync();
    Action<string>? SetTurn { get; set; }
    Action<string>? SetInstructions { get; set; }
}