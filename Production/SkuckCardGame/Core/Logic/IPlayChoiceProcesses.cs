namespace SkuckCardGame.Core.Logic;
public interface IPlayChoiceProcesses
{
    Task ChooseToPlayAsync();
    Task ChooseToPassAsync();
}