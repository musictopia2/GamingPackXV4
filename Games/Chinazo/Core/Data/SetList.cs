namespace Chinazo.Core.Data;
public class SetList
{
    public string Description { get; set; } = "";
    public BasicList<SetInfo> PhaseSets { get; set; } = new();
}