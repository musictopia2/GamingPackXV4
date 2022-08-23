namespace ScoreBoardGenerator;
internal class CompleteInformation
{
    public bool NeededPartial { get; set; } //if partial is not used, then will raise error because needs partial.
    public INamedTypeSymbol? Symbol { get; set; }
    public BasicList<IPropertySymbol> Properties { get; set; } = new(); //needs a list of properties.
}