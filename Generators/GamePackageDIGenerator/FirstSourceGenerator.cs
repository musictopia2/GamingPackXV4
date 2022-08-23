using System.Diagnostics;
namespace GamePackageDIGenerator;
[Generator] //this is important so it knows this class is a generator which will generate code for a class using it.
public class FirstSourceGenerator : IIncrementalGenerator
{
    //this will not have the attributes.  the second one will have attributes.
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
//#if DEBUG
//        if (Debugger.IsAttached == false)
//        {
//            Debugger.Launch();
//        }
//#endif
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
        return syntax is ClassDeclarationSyntax; //just classes no matter what here.
    }
    private ClassDeclarationSyntax? GetTarget(GeneratorSyntaxContext context)
    {
        var node = context.Node;
        //for now, should be okay.
        //good news is it can look for some key text to decide if it will consider.
        //unfortunately, could pick up comments even though not what i wanted.  its the best i can do unfortunately.
        //could eventually have other tags (emulating methods).
        //to make it simple, if the key words are there, consider it.
        //obviously if i comment lots of stuff, would include more than needed.
        //otherwise, this process would be too slow.
        if (node.ToString().Contains(".RegisterSingleton") || node.ToString().Contains("RegisterInstanceType") || node.ToString().Contains("RegisterType"))
        {
            var ourClass = context.GetClassNode(); //can use the sematic model at this stage
            if (ourClass.Identifier.ValueText == "GamePackageDIContainer")
            {
                return null; //try this way.
            }    
            return ourClass;
        }
        return null;
        //GamePackageDIContainer
    }
    private void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> list, SourceProductionContext context)
    {
        //at this point, we have a list of classes.  its already been filtered.
        var others = list.Distinct();
        ParserBasicClass parses = new(compilation);
        var results = parses.GetResults(others);
        EmitClass emits = new(context, compilation, results);
        emits.EmitBasic();
    }
}