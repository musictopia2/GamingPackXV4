using System.Diagnostics;

namespace ScoreBoardGenerator;
[Generator] //this is important so it knows this class is a generator which will generate code for a class using it.
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
        //context.RegisterPostInitializationOutput(c => c.CreateCustomSource().AddAttributesToSourceOnly());
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
        return syntax is ClassDeclarationSyntax ctx &&
            ctx.AttributeLists.Count > 0;
    }
    private ClassDeclarationSyntax? GetTarget(GeneratorSyntaxContext context)
    {
        var ourClass = context.GetClassNode(); //can use the sematic model at this stage
        var symbol = context.GetClassSymbol(ourClass);
        bool rets = symbol.HasAttribute(aa.UseScoreboardAttribute); //change to what attribute i use.
        if (rets == false)
        {
            return null;
        }
        return ourClass;
    }
    private void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> list, SourceProductionContext context)
    {
        //at this point, we have a list of classes.  its already been filtered.
        var others = list.Distinct();
        ParserClass parses = new(compilation);
        var results = parses.GetResults(others);
        EmitClass emits = new(context, results);
        emits.Emit();
    }
}