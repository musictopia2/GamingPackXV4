namespace GamePackageDIGenerator;
internal static class SymbolExtensions
{
    //public static IWriter PopulateTypeOf(this IWriter w, INamedTypeSymbol symbol)
    //{
    //    w.Write("typeof(")
    //               .SymbolFullNameWrite(symbol)
    //               .Write(")");
    //    return w;
    //}
    public static IWriter PopulateTypeOf(this IWriter w, INamedTypeSymbol symbol, Dictionary<string, INamedTypeSymbol> matches)
    {
        w.Write("typeof(")
                   .SymbolFullNameWrite(symbol, matches)
                   .Write(")");
        return w;
    }
}