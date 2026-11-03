namespace Fluxx.Core.Logic;
public interface IEverybodyOneProcesses
{
    Task EverybodyGetsOneAsync(BasicList<int> thisList, int selectedIndex);
}