namespace GamePackageObservableGenerator;
internal record PropertyModel : ICustomProperty
{
    public EnumSimpleTypeCategory VariableCustomCategory { get; set; }
    public bool Nullable { get; set; }
    public string PropertyName { get; set; } = "";
    public string ContainingNameSpace { get; set; } = "";
    public string UnderlyingSymbolName { get; set; } = "";
    public Location? Location { get; set; }
    public bool PrivateAccesor { get; set; }
}