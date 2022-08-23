namespace GamePackageDIGenerator;
internal class EmitClass
{
    private readonly SourceProductionContext _context;
    private readonly Compilation _compilation;
    private readonly BasicList<FirstInformation> _list;
    public EmitClass(SourceProductionContext context, Compilation compilation, BasicList<FirstInformation> list)
    {
        _context = context;
        _compilation = compilation;
        _list = list;
    }
    public EmitClass(SourceProductionContext context, Compilation compilation)
    {
        _context = context;
        _compilation = compilation;
        _list = new();
    }
    
    public void EmitResetAttributes(BasicList<INamedTypeSymbol> symbols)
    {
        //if (symbols.Count == 0)
        //{
        //    return; //don't even bother doing if there are none.  could rethink later though.
        //}
        SourceCodeStringBuilder builder = new();
        builder.StartGlobalProcesses(_compilation, "DIFinishProcesses", "AutoResetClass", w =>
        {

            w.WriteLine("public static void RegisterAutoResets()")
            .WriteCodeBlock(w =>
            {
                w.WriteLine("global::BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers.MiscDelegates.GetAutoResets = GetTypesToAutoReset;");
            });

            w.WriteLine("private static global::CommonBasicLibraries.CollectionClasses.BasicList<Type> GetTypesToAutoReset()")
            .WriteCodeBlock(w =>
            {
                w.WriteLine("global::CommonBasicLibraries.CollectionClasses.BasicList<Type> output = new();");
                foreach (var symbol in symbols)
                {
                    w.WriteLine(w =>
                    {
                        w.Write("output.Add(")
                        .PopulateTypeOf(symbol, new())
                        .Write(");");
                    });
                }
                w.WriteLine("return output;");
            });
        });
        _context.AddSource($"AutoReset.g", builder.ToString());
    }
    public void EmitBasic()
    {
        FinishDIRegistrationsExtensions.StartMethod();
        //finishDIRegistrations has to change because of parameters now.
        SourceCodeStringBuilder builder = new();
        builder.StartGlobalProcesses(_compilation, "DIFinishProcesses", "GlobalDIFinishClass", w =>
        {
            w.WriteLine("public static void FinishDIRegistrations(global::BasicGameFrameworkLibrary.DIContainers.IGamePackageGeneratorDI container)")
            .WriteCodeBlock(w =>
            {
                w.ProcessFinishDIRegistrations(_list);
            });
        });
        _context.AddSource($"BasicFinishDI.g", builder.ToString());
    }
    public void EmitLifetimeAttributes()
    {
        if (_list.Count == 0)
        {
            return; //there was none.  means can ignore.
        }
        FinishDIRegistrationsExtensions.StartMethod();
        SourceCodeStringBuilder builder = new();
        builder.StartGlobalProcesses(_compilation, "DIFinishProcesses", "GlobalDIAutoRegisterClass", w =>
        {
            //i think should simulate the old function as much as possible.
            w.WriteLine("public static void RegisterNonSavedClasses(global::BasicGameFrameworkLibrary.DIContainers.IGamePackageDIContainer container)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine("container.RegisterSingleton<global::BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses.IPlayOrder, global::BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses.PlayOrderClass>();"); //had to change namespaces in order to support this.
                RegisterBasics(w);
                w.ProcessFinishDIRegistrations(_list);
            });
        });
        _context.AddSource($"AttributesFinishDI.g", builder.ToString());
    }
    private void RegisterBasics(ICodeBlock w)
    {
        foreach (var item in _list)
        {
            if (item.Category == EnumCategory.Error)
            {
                _context.RaiseDuplicateException(item.MainClass!.Name);
                continue;
            }
            if (item.Category == EnumCategory.None)
            {
                continue;
            }
            w.WriteLine(w =>
            {
                w.Write("container.");
                if (item.Category == EnumCategory.Singleton)
                {
                    w.Write("RegisterSingleton(thisType: ");
                }
                else if (item.Category == EnumCategory.Instance)
                {
                    w.Write("RegisterInstanceType(");
                }
                w.PopulateTypeOf(item.MainClass!, new())
                .Write(");");
            });
        }
    }
}