namespace GamePackageDIGenerator;
internal class ParserBasicClass
{
    private readonly Compilation _compilation;
    public ParserBasicClass(Compilation compilation)
    {
        _compilation = compilation;
    }
    public BasicList<FirstInformation> GetResults(IEnumerable<ClassDeclarationSyntax> list)
    {
        if (list.Count() == 0)
        {
            return new(); //i think.
        }
        INamedTypeSymbol? container = _compilation.GetTypeByMetadataName("BasicGameFrameworkLibrary.DIContainers.IGamePackageRegister");
        //INamedTypeSymbol? others = _compilation.GetTypeByMetadataName("BasicGameFrameworkLibrary.DIContainers.GamePackageDIContainer");
        BasicList<FirstInformation> output = new();
        //output.ContainerSymbol = container;
        var firstTemp = container!.GetAllPublicMethods();
        BasicList<string> generics = new();
        BasicList<string> regulars = new();
        string objectText = "BasicGameFrameworkLibrary.DIContainers.GamePackageDIContainer.RegisterSingleton<TIn>(TIn, string)";
        //string regular = "";
        string instance = "";
        foreach (var item in firstTemp)
        {
            if (item.Name == "RegisterSingleton")
            {
                if (item.TypeArguments.Count() == 2)
                {
                    generics.Add(item.ToString());
                }
                else
                {
                    regulars.Add(item.ToString());
                    //regular = item.ToString();
                }
            }
            else if (item.Name == "RegisterInstanceType")
            {
                instance = item.ToString();
            }
            else if (item.Name == "RegisterType")
            {
                generics.Add(item.ToString()); //try another possibility for generics.
            }
        }
        foreach (var item in list)
        {
            var model = _compilation.GetSemanticModel(item.SyntaxTree);
            foreach (var method in item.Members.OfType<MethodDeclarationSyntax>())
            {
                var expressList = method.DescendantNodes().OfType<ExpressionStatementSyntax>();
                foreach (var expressPossible in expressList)
                {
                    var needs = expressPossible.DescendantNodes().First();
                    if (model.GetSymbolInfo(needs).Symbol is IMethodSymbol t)
                    {
                        string results = t.ConstructedFrom.ToString();
                        bool isGeneric = false;
                        bool isRegular = false;
                        bool isObject = false;
                        isObject = results == objectText;
                        if (isObject == false)
                        {
                            foreach (var r in regulars)
                            {
                                if (results == r)
                                {
                                    isRegular = true;
                                }
                            }
                            if (isRegular == false && results == instance)
                            {
                                isRegular = true;
                            }
                            foreach (var g in generics)
                            {
                                if (results == g)
                                {
                                    isGeneric = true;
                                    break;
                                }
                            }
                        }
                        

                        if (isGeneric == false && isRegular == false  && isObject == false)
                        {
                            continue;
                        }
                        FirstInformation fins = new();
                        var possibleTag = expressPossible.DescendantNodes().OfType<LiteralExpressionSyntax>().SingleOrDefault();
                        if (possibleTag is not null)
                        {
                            fins.Tag = possibleTag.Token.ValueText;
                        }
                        if (fins.Tag.ToLower() == "true" || fins.Tag.ToLower() == "false")
                        {
                            fins.Tag = ""; //i don't think can be true/false.  if i am wrong, rethink.
                        }
                        if (isGeneric)
                        {
                            var i = expressPossible.DescendantNodes().OfType<GenericNameSyntax>().ToBasicList();
                            //var i = expressPossible.DescendantNodes().OfType<IdentifierNameSyntax>().ToBasicList();
                            if (model.GetSymbolInfo(i.Last()).Symbol is IMethodSymbol)
                            {
                                var j = expressPossible.DescendantNodes().OfType<IdentifierNameSyntax>().ToBasicList();
                                INamedTypeSymbol lasts = (INamedTypeSymbol)model.GetSymbolInfo(j.Last()).Symbol!;
                                fins.MainClass = lasts;
                            }
                            else
                            {
                                var j = expressPossible.DescendantNodes().OfType<IdentifierNameSyntax>().ToBasicList();
                                INamedTypeSymbol lasts1 = (INamedTypeSymbol)model.GetSymbolInfo(i.Last()).Symbol!;
                                INamedTypeSymbol lasts2 = (INamedTypeSymbol)model.GetSymbolInfo(j.Last()).Symbol!;
                                if (lasts1.IsAbstract)
                                {
                                    fins.MainClass = lasts2;
                                }
                                else
                                {
                                    fins.MainClass = lasts1;
                                }
                            }
                            output.Add(fins);
                        }
                        else if (isRegular)
                        {

                            var l = expressPossible.DescendantNodes().OfType<IdentifierNameSyntax>().Last();
                            INamedTypeSymbol symbol = (INamedTypeSymbol)model.GetSymbolInfo(l).Symbol!;
                            fins.MainClass = symbol;
                            output.Add(fins);
                        }
                        else if (isObject)
                        {
                            var l = expressPossible.DescendantNodes().OfType<IdentifierNameSyntax>().Last();
                            var aa = model.GetSymbolInfo(l).Symbol;
                            if (aa is ILocalSymbol ll)
                            {
                                fins.MainClass = (INamedTypeSymbol)ll.Type;
                            }
                            else if (aa is IFieldSymbol ff)
                            {
                                fins.MainClass = (INamedTypeSymbol)ff.Type;
                            }
                            else if (aa is IPropertySymbol pp)
                            {
                                fins.MainClass = (INamedTypeSymbol)pp.Type;
                            }
                            else
                            {
                                throw new Exception("Cannot find type");
                            }
                            fins.Category = EnumCategory.Object;
                            output.Add(fins);
                        }
                    }
                }
            }
        }
        foreach (var item in output)
        {
            var temps = item.MainClass!.AllInterfaces.ToBasicList();
            foreach (var temp in temps)
            {
                if (temp.Name != "IHandle" && temp.Name != "IHandleAsync" && temp.Name != "IEquatable" && temp.Name != "IComparable")
                {
                    item.Assignments.Add(temp);
                } //cannot do anything with ihandle or ihandleasync since event aggravation handles that anyways.
            }
            //there is exception because of trick taking games (not worth doing another process since there are 3 or more possibilities for ones to register).
            var fins = item.MainClass.BaseType;
            if (fins is not null && fins.Name == "BasicTrickAreaObservable")
            {
                item.Assignments.Add(fins);
                //try to make this accomodate the trick taking games.  since they are registered this way.
            }

            //item.Assignments = item.MainClass!.AllInterfaces.ToBasicList(); //if the inherited version implements, has to show it too obviously.
            if (item.Category != EnumCategory.Object)
            {
                var tests = item.MainClass!.Constructors.OrderByDescending(x => x.Parameters.Count()).FirstOrDefault();
                if (tests is not null)
                {
                    var nexts = item.MainClass!.Constructors.OrderByDescending(x => x.Parameters.Count()).FirstOrDefault().Parameters.ToBasicList();
                    foreach (var a in nexts)
                    {
                        var symbol = a.Type;
                        item.Constructors.Add((INamedTypeSymbol)symbol);
                    }
                }
            }
        }
        return output;
    }
}