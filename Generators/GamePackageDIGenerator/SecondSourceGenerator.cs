using System.Diagnostics;

namespace GamePackageDIGenerator;
[Generator]
public class SecondSourceGenerator : IIncrementalGenerator
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
        if (syntax is ClassDeclarationSyntax ctx)
        {
            if (ctx.BaseList is not null && ctx.ToString().Contains("BasicSubmitViewModel"))
            {
                //if i ever have 2 part inheritance, then rethinking will be necessary.
                return true;
            }
            
            return ctx.AttributeLists.Count > 0;
        }
        return false;
        //return syntax is ClassDeclarationSyntax ctx &&
        //    ctx.AttributeLists.Count > 0;
    }
    private ClassDeclarationSyntax? GetTarget(GeneratorSyntaxContext context)
    {
        var ourClass = context.GetClassNode(); //can use the sematic model at this stage
        var symbol = context.GetClassSymbol(ourClass);
        if (symbol.InheritsFrom("BasicSubmitViewModel") && symbol.Name != "BasicSubmitViewModel")
        {
            return ourClass; //i think this now.
        }
        bool rets1 = symbol.HasAttribute(aa.InstanceGameAttribute);
        bool rets2 = symbol.HasAttribute(aa.SingletonGameAttribute);
        if (rets1 == false && rets2 == false)
        {
            return null;
        }
        if (symbol.IsAbstract)
        {
            return null;//try to ignore this too (?)
        }
        return ourClass;
    }
    private void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> list, SourceProductionContext context)
    {
        var others = list.Distinct();
        ParserAttributesClass parses = new(compilation);
        var results = parses.GetResults(others);
        EmitClass emits = new(context, compilation, results);
        emits.EmitLifetimeAttributes();
    }

}
