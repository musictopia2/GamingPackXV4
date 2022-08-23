namespace CommandsGenerator;
internal static class StaticHelpers
{
    //okay to be manually done now.
    public static string CommandAttribute => "Command";
    public static AttributeProperty GetCategoryInfo => new("Category", 0);
    public static string Category => "Category";
    public static AttributeProperty GetNameInfo => new("Name", -1);
    public static string Name => "Name";
    public static AttributeProperty GetCanInfo => new("Can", -1);
    public static string Can => "Can";
}