using System.Diagnostics;
namespace GamePackageDIGenerator;
[Generator]
public class FourthSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
//#if DEBUG
//        if (Debugger.IsAttached == false)
//        {
//            Debugger.Launch();
//        }
//#endif
        IncrementalValuesProvider<INamedTypeSymbol> declares = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsSyntaxTarget(s),
            (t, _) => GetTarget(t))
            .Where(m => m != null)!;
        IncrementalValueProvider<(Compilation, ImmutableArray<INamedTypeSymbol>)> compilation
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
    private INamedTypeSymbol? GetTarget(GeneratorSyntaxContext context)
    {
        var ourClass = context.GetClassNode();
        var symbol = context.GetClassSymbol(ourClass);
        if (symbol.IsAbstract)
        {
            return null; //for sure don't consider if abstract.
        }
        //var list = symbol.AllInterfaces.Reverse().ToBasicList();
        //can't be inheritance this time.
        foreach (var temp in symbol.AllInterfaces) //try this way (?)
        {
            if (temp.Name == "ICommonMultiplayer"
                || temp.Name == "IBeginningColors"
                || temp.Name == "IBeginningDice"
                || temp.Name == "IDiceAlone"
                || temp.Name == "IBeginningCards"
                || temp.Name == "IBeginningComboCardsColors")
            {
                return temp;
            }
        }
        return null;
        //if (symbol.Implements("ICommonMultiplayer"))
        //{
        //    return symbol;
        //}
        //return null;
    }
    private void Execute(Compilation compilation, ImmutableArray<INamedTypeSymbol> list, SourceProductionContext context)
    {
        //good news is i was able to capture the proper information for implementing interfaces (i think choosing a class that has all 3 is the best option).
        //maingame is a good option.
        if (list.Any() == false)
        {
            return;
        }
        if (list.Count() > 1)
        {
            //try to raise error.
            context.RaiseDuplicateInstances();
            return;
        }
        try
        {
            //code.
            Emit(context, list.Single(), compilation);
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
        }
    }
    private void Emit(SourceProductionContext context, INamedTypeSymbol symbol, Compilation compilation)
    {
        SourceCodeStringBuilder builder = new();
        builder.StartGlobalProcesses(compilation, "DIFinishProcesses", "SpecializedRegistrationHelpers", w =>
        {
            if (symbol.Name == "IDiceAlone")
            {
                w.WriteLine("public static void RegisterBasicYahtzeeStyleClasses(global::BasicGameFrameworkLibrary.DIContainers.IGamePackageDIContainer container)")
                .WriteCodeBlock(w =>
                {
                    w.PopulateDiceAloneMethod(symbol, compilation);
                });
                return;
            }
            w.WriteLine("public static void RegisterCommonMultplayerClasses(global::BasicGameFrameworkLibrary.DIContainers.IGamePackageDIContainer container)")
            .WriteCodeBlock(w =>
            {
                w.PopulateRegisterSpecializedMethod(symbol, compilation);
            });
            if (symbol.Name != "IBeginningDice")
            {
                w.WriteLine("public static void RegisterStandardDice(global::BasicGameFrameworkLibrary.DIContainers.IGamePackageDIContainer container)")
                .WriteCodeBlock(w =>
                {
                    w.PopulateStandardDiceMethod(compilation, symbol);
                });
            }
            w.PopulateReplaceBoardGameColorClasses();
            //if i have a third one, will do.
        });
        context.AddSource("Specialized.g", builder.ToString());
    }
}