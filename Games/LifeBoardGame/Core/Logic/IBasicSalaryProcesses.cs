namespace LifeBoardGame.Core.Logic;
public interface IBasicSalaryProcesses
{
    Task ChoseSalaryAsync(int salary);
    Task LoadSalaryListAsync();
}