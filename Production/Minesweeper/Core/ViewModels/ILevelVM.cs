namespace Minesweeper.Core.ViewModels;
public interface ILevelVM
{
    EnumLevel LevelChosen { get; set; }
    int HowManyMinesNeeded { get; set; }
}