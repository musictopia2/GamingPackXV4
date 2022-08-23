using System.Diagnostics;

namespace CommandsGenerator;
[Generator]
public class MySourceGenerator : IIncrementalGenerator
{
   
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
//#if DEBUG
//        if (Debugger.IsAttached == false)
//        {
//            Debugger.Launch();
//        }
//#endif
        //context.RegisterPostInitializationOutput(c =>
        //{
        //    c.CreateCustomSource().AddAttributesToSourceOnly();
        //    c.CreateCustomSource().BuildSourceCode();
        //});
        IncrementalValuesProvider<ClassDeclarationSyntax> declares = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsSyntaxTarget(s),
            (t, _) => GetTarget(t))
            .Where(m => m != null)!;
        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilation
            = context.CompilationProvider.Combine(declares.Collect());
        context.RegisterSourceOutput(compilation, (spc, source) =>
        {
            Execute(source.Item1, source.Item2, spc);
        });
    }
    private bool IsSyntaxTarget(SyntaxNode syntax)
    {
        if (syntax is not ClassDeclarationSyntax ctx)
        {
            return false;
        }
        if (ctx.IsPublic() == false)
        {
            return false;
        }
        foreach (var item in ctx.Members)
        {
            if (item.AttributeLists.Count > 0)
            {
                return true;
            }
        }
        return false;
    }
    private ClassDeclarationSyntax? GetTarget(GeneratorSyntaxContext context)
    {
        var ourClass = context.GetClassNode();
        var symbol = context.GetClassSymbol(ourClass);
        foreach (var item in symbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (item.HasAttribute(aa.CommandAttribute))
            {
                return ourClass;
            }
        }
        return null;
    }
    private void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> list, SourceProductionContext context)
    {
        try
        {
            var others = list.Distinct();
            ParserClass parses = new(compilation);
            var results = parses.GetResults(others);
            EmitClass emit = new(context, results);
            emit.Emit();
        }
        catch (Exception ex)
        {
            context.AddSource("errors.g", $"//{ex.Message}");
        }
    }
}