namespace GamePackageDIGenerator;
internal class ParserAttributesClass(Compilation compilation, SourceProductionContext context)
{
    private static readonly DiagnosticDescriptor _missingPlayOrderClass = new(
    id: "GPDIG001",
    title: "Required PlayOrderClass was not found",
    messageFormat: "The dependency injection generator requires '{0}', but the type could not be found",
    category: "GamePackageDI",
    defaultSeverity: DiagnosticSeverity.Error,
    isEnabledByDefault: true);
    public BasicList<FirstInformation> GetResults(IEnumerable<ClassDeclarationSyntax> list)
    {
        BasicList<FirstInformation> output = new();
        foreach (var item in list)
        {
            FirstInformation firsts = new();

            SemanticModel model = compilation.GetSemanticModel(item.SyntaxTree);
            firsts.MainClass = (INamedTypeSymbol)model.GetDeclaredSymbol(item)!;
            bool instance = firsts.MainClass.HasAttribute(aa.InstanceGameAttribute);
            bool singleton = firsts.MainClass.HasAttribute(aa.SingletonGameAttribute);
            if (firsts.MainClass.InheritsFrom("BasicSubmitViewModel"))
            {
                instance = true; //do this way now.
            }
            if (instance && singleton)
            {
                firsts.Category = EnumCategory.Error; //can't do both.
            }
            else if (instance)
            {
                firsts.Category = EnumCategory.Instance;
            }
            else if (singleton)
            {
                firsts.Category = EnumCategory.Singleton;
            }
            else
            {
                firsts.Category = EnumCategory.None; //if none, just ignore.
            }
            if (firsts.Category != EnumCategory.None)
            {
                output.Add(firsts);
            }
        }
        //i think this may be still needed.
        foreach (var item in output)
        {
            var temps = item.MainClass!.AllInterfaces.ToBasicList();
            foreach (var temp in temps)
            {
                if (temp.Name != "IHandle" && temp.Name != "IHandleAsync")
                {
                    item.Assignments.Add(temp);
                } //cannot do anything with ihandle or ihandleasync since event aggravation handles that anyways.
            }

            var fins = item.MainClass.BaseType;
            if (fins is not null && fins.Name != "Object")
            {
                item.Assignments.Add(fins);
                //has to check for at least one more base type because sorry card game inherits from card but needs game one as well.
                var exps = fins.BaseType;
                if (exps is not null && exps.Name != "Object")
                {
                    item.Assignments.Add(exps);
                }
            }


            //item.Assignments = item.MainClass!.AllInterfaces.ToBasicList();
            var tests = item.MainClass!.Constructors.OrderByDescending(x => x.Parameters.Count()).FirstOrDefault();
            var nexts = item.MainClass!.Constructors.OrderByDescending(x => x.Parameters.Count()).FirstOrDefault().Parameters.ToBasicList();
            foreach (var a in nexts)
            {
                var symbol = a.Type;
                item.Constructors.Add((INamedTypeSymbol)symbol);
            }
        }
        //i think i need to do a third part though.
        //BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses.PlayOrderClass
        //INamedTypeSymbol? container = _compilation.GetTypeByMetadataName("BasicGameFrameworkLibrary.DIContainers.IGamePackageRegister");
        string playOrderMetadataName =
            "BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses.PlayOrderClass";
        INamedTypeSymbol? playSymbol =
            compilation.GetTypeByMetadataName(playOrderMetadataName); if (playSymbol is null)
        {
            Diagnostic diagnostic = Diagnostic.Create(
                _missingPlayOrderClass,
                Location.None,
                playOrderMetadataName);

            context.ReportDiagnostic(diagnostic);
        }
        else
        {
            FirstInformation lasts = new();
            lasts.MainClass = playSymbol;
            lasts.Assignments = playSymbol.Interfaces.ToBasicList();
            lasts.Category = EnumCategory.None;

            output.Add(lasts);
        }

        return output;
    }
}