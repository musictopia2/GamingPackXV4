namespace LifeBoardGame.Core.Logic;
public interface IGenderProcesses
{
    Action? RefreshPlayer { get; set; }
    Action<string>? SetTurn { get; set; }
    Action<string>? SetInstructions { get; set; }
    Task ChoseGenderAsync(EnumGender gender);
}