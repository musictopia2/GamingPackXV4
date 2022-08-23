namespace GamePackageDIGenerator;
internal class ParserAutoClearClass
{
    private readonly Compilation _compilation;
    public ParserAutoClearClass(Compilation compilation)
    {
        _compilation = compilation;
    }
    public BasicList<INamedTypeSymbol> GetResults(IEnumerable<ClassDeclarationSyntax> list)
    {
        BasicList<INamedTypeSymbol> output = new();
        foreach (var item in list)
        {

            //FirstInformation firsts = new();

            SemanticModel model = _compilation.GetSemanticModel(item.SyntaxTree);
            INamedTypeSymbol symbol = (INamedTypeSymbol)model.GetDeclaredSymbol(item)!;
            output.Add(symbol);
        }
        return output;
    }
}
