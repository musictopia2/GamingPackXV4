namespace Xactika.Core.Logic;
public interface IModeProcesses
{
    Task EnableOptionsAsync();
    Task ProcessGameOptionChosenAsync(EnumGameMode optionChosen, bool doShow);
}