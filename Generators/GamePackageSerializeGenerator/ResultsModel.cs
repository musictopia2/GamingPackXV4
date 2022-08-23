namespace GamePackageSerializeGenerator;
internal class ResultsModel
{
    public HashSet<string> PropertyNames { get; set; } = new();
    public HashSet<TypeModel> Types = new();
    public string GlobalName { get; private set; } = "";
    public string ClassName { get; private set; } = "";
    public string ContextName => $"{ClassName}Context";
    //public string ContextName => $"{Symbol!.Name}Context";
    public INamedTypeSymbol? Symbol { get; set; }
    public bool HasChildren { get; private set; } //can act differently if no children.
    public BasicList<INamedTypeSymbol> Generics { get; private set; } = new(); //can have more than one.
    public void ForceChildren()
    {
        HasChildren = true;
    }
    private bool CalculateChildren()
    {
        if (Types.Count > 1)
        {
            return true;
        }
        if (Types.Count == 0)
        {
            return false;
        }
        var symbol = Types.Single();
        return symbol.TypeCategory != EnumTypeCategory.Complex && symbol.SpecialCategory != EnumSpecialCategory.None;
    }
    public void Process()
    {
        //hopefully this does not care about generic stuff.
        //this means that everything was set properly.
        
        if (Symbol!.IsDictionary())
        {
            var items = Symbol!.GetDictionarySymbols();
            HasChildren = true;
            Generics.Add((INamedTypeSymbol)items.Key);
            Generics.Add((INamedTypeSymbol)items.Value);
            ClassName = $"{Symbol!.Name}{items.Key.Name}{items.Value.Name}";
            GlobalName = $"global::{Symbol.ContainingNamespace.ToDisplayString()}.{Symbol.Name}<{items.Key.ContainingNamespace.ToDisplayString()}.{items.Key.Name}, {items.Value.ContainingNamespace.ToDisplayString()}.{items.Value.Name}>";
            return;
        }
        var first = Symbol!.GetSingleGenericTypeUsed();
        if (first is null)
        {
            //populate the stuff needed.
            ClassName = Symbol!.Name;
            GlobalName = $"global::{Symbol.ContainingNamespace.ToDisplayString()}.{Symbol.Name}";
            HasChildren = CalculateChildren();
            return;
        }
        if (first is not null)
        {
            HasChildren = true; //i think will propose we have children.
            Generics.Add((INamedTypeSymbol)first);
            var second = first.GetSingleGenericTypeUsed();
            if (second is null)
            {
                ClassName = $"{Symbol!.Name}{first.Name}";
                GlobalName = $"global::{Symbol.ContainingNamespace.ToDisplayString()}.{Symbol.Name}<{first.ContainingNamespace.ToDisplayString()}.{first.Name}>";
                return;
            }
            Generics.Add((INamedTypeSymbol)second);
            ClassName = $"{Symbol!.Name}{first.Name}{second.Name}";
            GlobalName = $"global::{Symbol.ContainingNamespace.ToDisplayString()}.{Symbol.Name}<{first.ContainingNamespace.ToDisplayString()}.{first.Name}<{second.ContainingNamespace.ToDisplayString()}.{second.Name}>>"; //iffy.
        }
    }
}
