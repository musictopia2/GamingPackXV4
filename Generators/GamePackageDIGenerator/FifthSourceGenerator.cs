using System.Diagnostics;
namespace GamePackageDIGenerator;
[Generator]
internal class FifthSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
//#if DEBUG
//        if (Debugger.IsAttached == false)
//        {
//            Debugger.Launch();
//        }
//#endif
        IncrementalValuesProvider<RegularCardInformation> declares = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsSyntaxTarget(s),
            (t, _) => GetTarget(t))
            .Where(m => m != null)!;
        IncrementalValueProvider<(Compilation, ImmutableArray<RegularCardInformation>)> compilation
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
    private bool? WasUsed(PropertyDeclarationSyntax syntax)
    {
        var firsts = syntax.DescendantNodes().OfType<LiteralExpressionSyntax>().FirstOrDefault();
        if (firsts is null)
        {
            return null;
        }
        bool rets = bool.TryParse(firsts.Token.ValueText, out bool output);
        if (rets == false)
        {
            return false;
        }
        return output;
    }
    private RegularCardInformation? GetTarget(GeneratorSyntaxContext context)
    {
        var ourClass = context.GetClassNode();
        var symbol = context.GetClassSymbol(ourClass);
        if (symbol.IsAbstract)
        {
            return null; //for sure don't consider if abstract.
        }
        foreach (var temp in symbol.Interfaces)
        {
            if (temp.Name == "IBeginningRegularCards")
            {
                RegularCardInformation output = new();
                output.Symbol = temp;
                var list = ourClass.Members.OfType<PropertyDeclarationSyntax>().Where(x => x.Identifier.ValueText == "AceLow" || x.Identifier.ValueText == "CustomDeck").ToBasicList();
                if (list.Count != 2)
                {
                    return null;
                }
                foreach (var member in list)
                {
                    bool? rets;
                    if (member.Identifier.ValueText == "AceLow")
                    {
                        //ace low stuff
                        rets = WasUsed(member);
                        if (rets.HasValue == false)
                        {
                            return null; //for now because its not valid.
                        }
                        output.AceLow = rets.Value;
                    }
                    else if (member.Identifier.ValueText == "CustomDeck")
                    {
                        rets = WasUsed(member);
                        if (rets.HasValue == false)
                        {
                            return null;
                        }
                        //custom deck stuff.
                        output.CustomDeck = rets.Value;
                    }
                }
                return output;
            }
        }
        return null;
    }
    private void Execute(Compilation compilation, ImmutableArray<RegularCardInformation> list, SourceProductionContext context)
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
    private void Emit(SourceProductionContext context, RegularCardInformation card, Compilation compilation)
    {
        SourceCodeStringBuilder builder = new();
        builder.StartGlobalProcesses(compilation, "DIFinishProcesses", "SpecializedRegularCardHelpers", w =>
        {
            w.WriteLine("public static void RegisterRegularDeckOfCardClasses(global::BasicGameFrameworkLibrary.DIContainers.IGamePackageDIContainer container)")
            .WriteCodeBlock(w =>
            {
                w.PopulateRegularCardsMethod(card, compilation);
                
            });
        });
        context.AddSource("RegularDeckOfCards.g", builder.ToString());
    }
}
