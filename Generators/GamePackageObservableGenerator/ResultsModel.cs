namespace GamePackageObservableGenerator;
internal record ResultsModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public BasicList<PropertyModel> Properties { get; set; } = [];
    public bool InheritedEvent { get; set; }
    public string GenericString { get; set; } = "";
    public bool AlreadyHadAction { get; set; }
}