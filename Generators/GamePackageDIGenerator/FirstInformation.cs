namespace GamePackageDIGenerator;
internal class FirstInformation
{
    public INamedTypeSymbol? MainClass { get; set; }
    public BasicList<INamedTypeSymbol> Constructors { get; set; } = new();
    public BasicList<INamedTypeSymbol> Assignments { get; set; } = new();
    public EnumCategory Category { get; set; }
    public string Tag { get; set; } = "";
    //public BasicList<INamedTypeSymbol> GenericSymbols { get; set; } = new(); //if there are any generic symbols, then has to do something else for parts of the processes.
    public Dictionary<string, INamedTypeSymbol> GenericSymbols { get; set; } = new();
}