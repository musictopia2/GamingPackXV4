namespace GamePackageSerializeGenerator;
[Generator]
public partial class MySourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //#if DEBUG
        //        if (Debugger.IsAttached == false)
        //        {
        //            Debugger.Launch();
        //        }
        //#endif

        //not sure if i need these 2 anymore (?)

        //context.RegisterPostInitializationOutput(c =>
        //{
        //    c.CreateCustomSource().BuildSourceCode();
        //    c.CreateCustomSource().AddAttributesToSourceOnly();
        //});
        IncrementalValuesProvider<FirstInformation> declares = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsSyntaxTarget(s),
            (t, _) => GetTarget(t))
            .Where(m => m != null)!;
        IncrementalValueProvider<(Compilation, ImmutableArray<FirstInformation>)> compilation
            = context.CompilationProvider.Combine(declares.Collect());
        context.RegisterSourceOutput(compilation, (spc, source) =>
        {
            Execute(source.Item1, source.Item2, spc);
        });
    }
    private bool IsSyntaxTarget(SyntaxNode syntax)
    {
        return syntax is ClassDeclarationSyntax; //looks like has to allow internal now.  especially because of the fluent.
    }
    private FirstInformation? GetTarget(GeneratorSyntaxContext context)
    {
        var ourClass = context.GetClassNode();
        var symbol = context.GetClassSymbol(ourClass);
        FirstInformation info = new();
        info.Node = ourClass;
        info.Symbol = symbol;
        if (symbol.Implements("ISaveInfo"))
        {
            if (symbol.IsAbstract)
            {
                return null; //something else has to consider now.
            }
            info.Category = EnumSerializeCategory.Save;
            return info;
        }
        if (symbol.Implements("ISerializable"))
        {
            info.Category = EnumSerializeCategory.Serializable;
            return info;
        }
        if (ourClass.BaseList is not null && ourClass.ToString().Contains("SerializeContext"))
        {
            info.Category = EnumSerializeCategory.Fluent;
            return info;
        }
        return null;
    }
    private void Execute(Compilation compilation, ImmutableArray<FirstInformation> list, SourceProductionContext context)
    {
        try
        {
            var others = list.Distinct();
            if (others.Count() == 0)
            {
                return;
            }
            ParserClass parses = new(compilation);
            var item = parses.GetResults(others);
            EmitClass emits = new(context, item, compilation);
            emits.Emit();
        }
        catch (Exception ex)
        {
            SourceCodeStringBuilder builder = new();
            builder.WriteCommentBlock(w =>
            {
                w.WriteLine(ex.Message);
                w.WriteLine(ex.StackTrace);
            });
            context.AddSource("errors.g", builder.ToString());
            //context.AddSource("errors.g", $"//{ex.Message}");
        }
    }
}