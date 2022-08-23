namespace GamePackageSerializeGenerator;
internal class CompleteInformation
{
    //doing as dictionary to make sure we don't run into any issue.
    public Dictionary<string, ResultsModel> Results { get; set; } = new();
    //public BasicList<ResultsModel> Results { get; set; } = new();
    public BasicList<IPropertySymbol> PropertiesToIgnore { get; set; } = new();
}