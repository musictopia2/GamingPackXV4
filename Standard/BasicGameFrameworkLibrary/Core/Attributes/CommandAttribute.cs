
namespace BasicGameFrameworkLibrary.Core.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public class CommandAttribute : Attribute
{
    public EnumCommandCategory Category { get; set; }
    public string Name { get; set; } = ""; //this means that you are able to attach a command name so it can link up properly.  only needed if the category is control.
                                           //if you attach name and its not control, will raise exception at compile time.
    public string Can { get; set; } = ""; //will be optional.
                                          //i propose that if you specify and its control, then just ignore (that would be fine).
    public CommandAttribute(EnumCommandCategory category)
    {
        Category = category;
    }
}