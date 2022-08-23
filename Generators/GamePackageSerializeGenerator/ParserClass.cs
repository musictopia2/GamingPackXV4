namespace GamePackageSerializeGenerator;
internal class ParserClass
{
    private readonly Compilation _compilation;
    private HashSet<string> _properties = new();
    private HashSet<TypeModel> _types = new();
    private HashSet<string> _lookedAt = new();
    private bool _wasDeck;
    private readonly static BasicList<string> _serializeStrings = new();
    public ParserClass(Compilation compilation)
    {
        _compilation = compilation;
    }
    private void PopulateJsonInfo()
    {
        if (_serializeStrings.Count > 0)
        {
            return;
        }
        INamedTypeSymbol? container = _compilation.GetTypeByMetadataName("CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.SystemTextJsonStrings");
        var firstTemp = container!.GetAllPublicMethods();
        foreach (var item in firstTemp)
        {
            if (item.Name.StartsWith("Deser"))
            {
                _serializeStrings.Add(item.ToString());
            }
        }
    }
    private void Clear()
    {
        _properties = new();
        _types = new();
        _lookedAt = new();
        _wasDeck = false;
    }

    public CompleteInformation GetResults(IEnumerable<FirstInformation> firsts)
    {
        CompleteInformation output = new();
        foreach (var item in firsts)
        {
            if (item.Category == EnumSerializeCategory.Serializable)
            {
                PopulateSerialize(item.Node!, output);
            }
            else if (item.Category == EnumSerializeCategory.Save)
            {
                PopulateSaveInfo(item.Symbol!, output);
            }
            else if (item.Category == EnumSerializeCategory.Fluent)
            {
                PopulateFluent(item.Node!, output);
            }
        }
        return output;
    }
    private void PopulateSaveInfo(INamedTypeSymbol symbol, CompleteInformation info)
    {
        //easiest.
        Clear();
        ResultsModel model = new();
        model.Symbol = symbol;
        PopulateNames(symbol, model, info);
        model.PropertyNames = _properties;
        TypeModel fins = GetSimpleType(symbol); //has to be simple (?)
        fins.SpecialCategory = EnumSpecialCategory.Save;
        _types.Add(fins);
        model.Types = _types;
        model.Process(); //i think should be okay.  its going to be easy this time.
        AddResults(model, info);
    }
    private void AddResults(ResultsModel result, CompleteInformation info)
    {
        if (info.Results.ContainsKey(result.ContextName))
        {
            return;
        }
        info.Results.Add(result.ContextName, result); //try this way (since its a dictionary).
    }
    private BasicList<INamedTypeSymbol> GetFluentList(ClassDeclarationSyntax node)
    {
        BasicList<INamedTypeSymbol> output = new();
        ParseContext firstParse = new(_compilation, node);
        var members = node.DescendantNodes().OfType<MethodDeclarationSyntax>();
        foreach (var member in members)
        {
            var symbol = firstParse.SemanticModel.GetDeclaredSymbol(member) as IMethodSymbol;
            var makeCalls = ParseUtils.FindCallsOfMethodWithName(firstParse, node, "Make"); //can't do nameof anymore
            foreach (var make in makeCalls)
            {
                INamedTypeSymbol makeType = (INamedTypeSymbol)make.MethodSymbol.TypeArguments[0]!;
                output.Add(makeType);
            }
        }
        return output;
    }
    private void PopulateFluent(ClassDeclarationSyntax node, CompleteInformation info)
    {
        var list = GetFluentList(node);
        foreach (var item in list)
        {
            ProcessMisc(item, info);
        }
    }
    private TypeModel GetMainType(INamedTypeSymbol symbol)
    {
        if (symbol.IsDictionary())
        {
            return GetDictionary(symbol);
        }
        if (symbol.IsCollection())
        {
            //this means it implements collection.
            var others = symbol.GetSingleGenericTypeUsed();
            if (others is not null && others.IsCollection())
            {
                return GetDoubleList(others); //try this way (?)
            }
            return GetList(others!, symbol);
        }
        if (symbol.Implements("IPlayerCollection") || symbol.Implements("ISimpleList"))
        {
            return GetCollection(symbol);
        }
        if (symbol.Implements("IBoardCollection"))
        {
            return GetBoard(symbol);
        }
        return GetSimpleType(symbol);
    }
    private void ProcessMisc(INamedTypeSymbol symbol, CompleteInformation info)
    {
        Clear();
        ResultsModel model = new();
        model.Symbol = symbol;
        _wasDeck = true;
        PopulateNames(symbol, model, info);
        _wasDeck = false; //try this way now (?)
        model.PropertyNames = _properties;
        TypeModel fins = GetMainType(symbol);
        fins.SpecialCategory = EnumSpecialCategory.Main; //i think.
        _types.Add(fins);
        model.Types = _types;
        model.Process();
        Clear(); //its okay to clear out (i think).
        foreach (var item in model.Generics)
        {
            var firsts = item.GetListCategory();
            if (firsts == EnumTypeCategory.SingleList)
            {
                fins = GetList(fins.SymbolUsed!, item); //hopefully this simple.
                AddType(fins);
                continue; //for now, try to continue (not sure what will happen).
                //throw new Exception("Have not implemented list of a list yet");
            }
            //has to later figure out a list of a list.
            if (item.GetSimpleCategory() == EnumTypeCategory.Complex)
            {
                _wasDeck = true;
                PopulateNames(item, model, info); //hopefully this simple.
                _wasDeck = false; //hopefully this simple (?)
                fins = GetSimpleType(item);
                AddType(fins);
                continue;
            }
            fins = GetSimpleType(item);
            AddType(fins);
        }
        foreach (var item in _types)
        {
            if (model.Types.Any(x => x.FileName == item.FileName) == false)
            {
                model.Types.Add(item);
            }
        }
        AddResults(model, info); //hopefully this is it (?)
    }
    private void AddType(TypeModel model)
    {
        if (_types.Any(x => x.FileName == model.FileName) == false)
        {
            _types.Add(model);
        }
        else
        {

        }
    }
    private BasicList<INamedTypeSymbol> GetSerializeList(ClassDeclarationSyntax node)
    {
        BasicList<INamedTypeSymbol> output = new();
        if (_serializeStrings.Count == 0)
        {
            PopulateJsonInfo();
        }
        var model = _compilation.GetSemanticModel(node.SyntaxTree);
        foreach (var method in node.Members.OfType<MethodDeclarationSyntax>())
        {
            var bList = method.DescendantNodes().OfType<BlockSyntax>().ToBasicList();
            foreach (var localPossible in bList)
            {
                var needs1 = localPossible.DescendantNodes().OfType<GenericNameSyntax>().ToBasicList();
                foreach (var need in needs1)
                {
                    if (model.GetSymbolInfo(need).Symbol is IMethodSymbol t)
                    {
                        string results = t.ConstructedFrom.ToString();
                        bool isProper = false;
                        foreach (var d in _serializeStrings)
                        {
                            if (results == d)
                            {
                                isProper = true;
                            }
                        }
                        if (isProper)
                        {
                            var needs2 = need.DescendantNodes().ToBasicList();
                            var ss = model.GetSymbolInfo(needs2[1]);
                            output.Add((INamedTypeSymbol)ss.Symbol!);
                        }
                    }
                }
            }
        }
        return output;
    }
    private void PopulateSerialize(ClassDeclarationSyntax node, CompleteInformation info)
    {
        var list = GetSerializeList(node);
        foreach (var item in list)
        {
            ProcessMisc(item, info);
        }
    }
    private TypeModel GetDoubleList(ITypeSymbol collection)
    {
        TypeModel output = new();
        output.TypeCategory = EnumTypeCategory.DoubleList;
        output.CollectionNameSpace = $"{collection.ContainingSymbol.ToDisplayString()}.{collection.Name}";
        output.CollectionStringName = collection.Name;
        var mm = collection.GetSingleGenericTypeUsed()!;
        output.FileName = $"{collection.Name}{collection.Name}{mm!.Name}";
        output.SubName = $"{collection.Name}{mm.Name}";
        output.SymbolUsed = mm;
        output.LoopCategory = EnumLoopCategory.Standard; //i think.
        return output;
    }
    private IPropertySymbol? _current;
    private void PopulateNames(INamedTypeSymbol symbol, ResultsModel results, CompleteInformation complete)
    {
        if (_lookedAt.Contains(symbol.Name) == true)
        {
            return;
        }
        if (symbol.IsCollection())
        {
            return; //i don't think collections should be considered for populating names.
        }
        //if (symbol.GetSimpleCategory() == EnumTypeCategory.Complex)
        //{
        //    _wasDeck = true;
        //}
        //else
        //{
        //    _wasDeck = false; //try this way now (?)
        //}    
        _lookedAt.Add(symbol.Name);
        var properties = symbol.GetAllPublicProperties();
        properties.RemoveAllOnly(xx =>
        {
            return xx.IsReadOnly ||
            xx.CanBeReferencedByName == false ||
            xx.SetMethod is null;
        });
        foreach (var pp in properties)
        {
            _current = pp;
            if (pp.HasAttribute("JsonIgnore"))
            {
                complete.PropertiesToIgnore.Add(pp);
                _lookedAt.Add(pp.Name);
                continue; //try this way now.  so if being ignored, can't consider further.
            }
            if (_wasDeck)
            {
                if (pp.Name == "DefaultSize")
                {
                    complete.PropertiesToIgnore.Add(pp); //hopefully okay (?)
                    _lookedAt.Add(pp.Name); //i think needs to try this too.
                    continue;
                }
            }
            if (pp.Type.Name == "Action" || pp.Type.Name == "IGamePackageResolver" || pp.Type.Name == "IGamePackageGeneratorDI")
            {
                complete.PropertiesToIgnore.Add(pp); //you have to ignore this one somehow or another.
                TypeModel firstIgnore = new();
                firstIgnore.SpecialCategory = EnumSpecialCategory.Ignore;
                firstIgnore.SymbolUsed = pp.Type;
                results.Types.Add(firstIgnore);
                continue;
            }
            _properties.Add(pp.Name);
            ITypeSymbol others;
            INamedTypeSymbol temps = (INamedTypeSymbol)pp.Type;
            if (temps is not null && temps.IsDictionary())
            {
                //complete.PropertiesToIgnore.Add(pp);
                //TypeModel firstIgnore = new();
                //firstIgnore.SpecialCategory = EnumSpecialCategory.Ignore;
                //firstIgnore.SymbolUsed = pp.Type;
                //results.Types.Add(firstIgnore); //for now, ignore until i see what the problem is.
                AddDictionary(temps, results, complete);
                continue;
            }
            //if (pp.Type.is)
            if (pp.IsCollection())
            {
                //this is the lists.
                others = pp.GetSingleGenericTypeUsed()!;
                if (others is not null && others.IsCollection())
                {
                    TypeModel fins = GetDoubleList(others);
                    AddType(fins);
                    AddListNames(fins.SymbolUsed!, others, results, complete);
                    continue;
                }
                AddListNames(others!, pp.Type, results, complete);
                continue;
            }
            if (pp.Type.Implements("IPlayerCollection") || pp.Type.Implements("ISimpleList"))
            {
                AddCustomCollection(pp.Type, results, complete);
                continue;
            }
            if (pp.Type.Implements("IBoardCollection"))
            {
                AddBoardCollection(pp.Type, results, complete);
                continue;
            }
            bool nullable = pp.NullableAnnotation == NullableAnnotation.Annotated;
            AddSimpleName(pp, results, complete, nullable);
        }
    }
    private TypeModel GetCollection(ITypeSymbol symbol)
    {
        TypeModel output = new();
        output.CollectionNameSpace = $"{symbol.ContainingSymbol.ToDisplayString()}.{symbol.Name}";
        output.CollectionStringName = symbol.Name;
        output.TypeCategory = EnumTypeCategory.SingleList;
        output.LoopCategory = EnumLoopCategory.Custom;
        var otherSymbol = (INamedTypeSymbol)symbol.GetSingleGenericTypeUsed()!;
        output.SymbolUsed = otherSymbol;
        output.SubName = otherSymbol!.Name;
        var firsts = otherSymbol.GetSingleGenericTypeUsed();
        if (firsts is not null)
        {
            INamedTypeSymbol fins = (INamedTypeSymbol)firsts;
            output.SubSymbol = fins;
            output.SubName = $"{otherSymbol.Name}{fins.Name}";
        }
        else
        {
            output.SubName = otherSymbol.Name;
        }
        output.FileName = $"{symbol.Name}{output.SubName}";
        return output;
    }
    private void AddCustomCollection(ITypeSymbol symbol, ResultsModel results, CompleteInformation complete)
    {
        TypeModel fins = GetCollection(symbol);
        AddType(fins);
        AddSimpleName(fins.SymbolUsed!, results, complete);
    }
    private TypeModel GetBoard(ITypeSymbol symbol)
    {
        TypeModel output = new();
        output.FileName = symbol.Name;
        output.SymbolUsed = symbol;
        output.LoopCategory = EnumLoopCategory.Custom;
        var otherSymbol = (INamedTypeSymbol)symbol.AllInterfaces.First().GetSingleGenericTypeUsed()!;
        output.SubName = otherSymbol!.Name;
        output.SubSymbol = otherSymbol;
        return output;
    }
    private void AddBoardCollection(ITypeSymbol symbol, ResultsModel results, CompleteInformation complete)
    {
        TypeModel fins = GetBoard(symbol);
        AddType(fins);
        AddSimpleName(fins.SubSymbol!, results, complete);
    }
    private TypeModel GetDictionary(INamedTypeSymbol symbol)
    {
        TypeModel output = new();
        var items = symbol.GetDictionarySymbols();
        output.TypeCategory = EnumTypeCategory.Dictionary;
        output.SymbolUsed = symbol;
        output.FileName = $"{symbol.Name}{items.Key.Name}{items.Value.Name}";
        return output;
    }
    private TypeModel GetList(ITypeSymbol symbol, ISymbol collection)
    {
        TypeModel output = new();
        output.TypeCategory = EnumTypeCategory.SingleList;
        string name = collection.Name;
        output.CollectionNameSpace = $"{collection.ContainingSymbol.ToDisplayString()}.{name}";
        output.SymbolUsed = symbol;
        var others = output.SymbolUsed.GetSingleGenericTypeUsed();
        if (others is not null)
        {
            output.SubName = $"{symbol.Name}{others.Name}";
            output.SubSymbol = (INamedTypeSymbol)others;
        }
        else
        {
            output.SubName = symbol.Name;
        }
        output.LoopCategory = EnumLoopCategory.Standard;
        output.FileName = $"{name}{output.SubName}";
        return output;
    }
    private void AddDictionary(INamedTypeSymbol symbol, ResultsModel results, CompleteInformation complete)
    {
        TypeModel fins = GetDictionary(symbol);
        AddType(fins);
        var pairs = symbol.GetDictionarySymbols();
        AddSimpleName(pairs.Key, results, complete);
        AddSimpleName(pairs.Value, results, complete);
    }
    private void AddListNames(ITypeSymbol symbol, ISymbol collection, ResultsModel results, CompleteInformation complete)
    {
        TypeModel fins = GetList(symbol, collection);
        AddType(fins);
        AddSimpleName(symbol, results, complete);
    }
    private void AddSimpleName(IPropertySymbol symbol, ResultsModel results, CompleteInformation complete, bool nullable)
    {
        AddSimpleName(symbol.Type, results, complete, nullable);
    }
    private TypeModel GetSimpleType(ITypeSymbol symbol)
    {
        TypeModel output = new();
        output.SymbolUsed = symbol;
        output.FileName = symbol.Name;
        output.TypeCategory = output.SymbolUsed.GetSimpleCategory();
        output.LoopCategory = EnumLoopCategory.None;
        var others = symbol.GetSingleGenericTypeUsed();
        if (others is not null)
        {
            output.FileName = $"{symbol.Name}{others.Name}";
            output.SubSymbol = (INamedTypeSymbol)others;
        }
        if (output.TypeCategory == EnumTypeCategory.StandardEnum)
        {
            var list = output.SymbolUsed!.GetMembers();
            foreach (var a in list)
            {
                if (a.Name != ".ctor")
                {
                    output.EnumNames.Add(a.Name);
                }
            }
        }
        output.SubName = symbol.Name; //well see.
        return output;
    }
    private void AddSimpleName(ITypeSymbol symbol, ResultsModel results, CompleteInformation complete, bool nullable = false)
    {
        TypeModel fins = GetSimpleType(symbol);
        if (fins.TypeCategory == EnumTypeCategory.SizeF)
        {
            fins.SpecialCategory = EnumSpecialCategory.Ignore; //try to ignore.  if i run into problems. rethink.
            //because was unable to make it not show the sizef for some strange reason.
        }
        if (fins.TypeCategory == EnumTypeCategory.Complex)
        {
            AddType(fins);
            fins.NullablePossible = nullable;
            _wasDeck = true;
            PopulateNames((INamedTypeSymbol)symbol, results, complete);
            _wasDeck = false;
            return;
        }
        AddType(fins);
    }
}