namespace Fluxx.Core.Logic;
public interface IFinalRuleProcesses
{
    Task TrashNewRuleAsync(int index);
    Task SimplifyRulesAsync(BasicList<int> thisList);
}