namespace GamePackageSerializeGenerator;
internal static class WriterExtensions
{
    public static IWriter PopulateInterface(this IWriter w, ResultsModel model)
    {
        w.Write("global::CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.ICustomJsonContext<")
            .Write(model.GlobalName)
            .Write(">");
        return w;
    }
    private static void PopulateSubSymbol(this IWriter w, TypeModel model)
    {
        if (model.SubSymbol is null)
        {
            return;
        }
        if (model.LoopCategory == EnumLoopCategory.Custom && model.CollectionNameSpace == "")
        {
            return;
        }
        w.Write("<")
        .GlobalWrite()
        .Write(model.SubSymbol.ContainingNamespace.ToDisplayString())
        .Write(".")
        .Write(model.SubSymbol.Name)
        .Write(">");
    }
    public static IWriter PopulateFullClassName(this IWriter w, TypeModel model)
    {
        //get sample.
        //Dictionary<int, BingoItem>
        //if there is a list involved, has to show the list part as well.
        if (model.TypeCategory == EnumTypeCategory.NullableInt)
        {
            w.Write("int?");
            return w;
        }
        if (model.TypeCategory == EnumTypeCategory.Dictionary)
        {
            //has to do the dictionary part.
            var temps = (INamedTypeSymbol)model.SymbolUsed!;
            var pairs = temps.GetDictionarySymbols();
            w.Write(model.GetGlobalNameSpace)
                .Write(".")
                .Write(model.TypeName)
                .Write("<")
                .GlobalWrite()
                .Write(pairs.Key.ContainingNamespace)
                .Write(".")
                .Write(pairs.Key.Name)
                .Write(", ")
                .GlobalWrite()
                .Write(pairs.Value.ContainingNamespace)
                .Write(".")
                .Write(pairs.Value.Name)
                .Write(">");
            return w;
        }
        if (model.TypeCategory != EnumTypeCategory.SingleList && model.TypeCategory != EnumTypeCategory.DoubleList)
        {
            w.Write(model.GetGlobalNameSpace)
                .Write(".")
                .Write(model.TypeName);
            w.PopulateSubSymbol(model);
            return w;
        }

        if (model.TypeCategory == EnumTypeCategory.SingleList)
        {
            //this is single list
            w.GlobalWrite()
            .Write(model.CollectionNameSpace)
            .Write("<")
            .Write(model.GetGlobalNameSpace)
            .Write(".")
            .Write(model.TypeName);
            w.PopulateSubSymbol(model);
            w.Write(">");
            return w;
        }
        if (model.TypeCategory == EnumTypeCategory.DoubleList)
        {
            w.GlobalWrite()
                .Write(model.CollectionNameSpace)
                .Write("<")
                .GlobalWrite()
                .Write(model.CollectionNameSpace)
                .Write("<")
                .Write(model.GetGlobalNameSpace)
                .Write(".")
                .Write(model.TypeName)
                .Write(">>");
            return w;
        }
        throw new Exception("List Not Implemented");
    }
}
