namespace GamePackageObservableGenerator;

internal class ParserClass(IEnumerable<ClassDeclarationSyntax> list, Compilation compilation,
    string propertyAttributeUsed, string classAttributeUsed)
{
    public BasicList<ResultsModel> GetResults()
    {
        BasicList<ResultsModel> output = [];
        foreach (var item in list)
        {
            ResultsModel results = GetResult(item);
            //class names cannot be repeating.
            //that ended up being a good idea.
            if (output.Any(x =>
                x.ClassName == results.ClassName &&
                x.Namespace == results.Namespace) == false)
            {
                output.Add(results);
            }
        }
        return output;
    }
    //eventually have as public extension (not yet).
    private static bool HasAttributeOnParent(
        INamedTypeSymbol symbol,
        string attributeName)
    {
        INamedTypeSymbol? parent = symbol.BaseType;

        while (parent is not null)
        {
            if (parent.GetAttributes()
                .Any(x => x.AttributeClass?.Name == $"{attributeName}Attribute"))
            {
                return true;
            }

            parent = parent.BaseType;
        }

        return false;
    }
    private static bool ParentHasStateChanged(INamedTypeSymbol symbol)
    {
        INamedTypeSymbol? parent = symbol.BaseType;

        while (parent is not null)
        {
            foreach (IPropertySymbol property in parent.GetMembers("StateChanged")
                         .OfType<IPropertySymbol>())
            {
                //if (property.DeclaredAccessibility != Accessibility.Public)
                //{
                //    continue;
                //}

                if (property.GetMethod is null || property.SetMethod is null)
                {
                    continue;
                }

                if (property.Type is INamedTypeSymbol type &&
                    type.Name == "Action" &&
                    type.ContainingNamespace.ToDisplayString() == "System" &&
                    type.Arity == 0)
                {
                    return true;
                }
            }

            parent = parent.BaseType;
        }

        return false;
    }
    //private static string GetGenericString(INamedTypeSymbol symbol)
    //{
    //    return "<TEST>";
    //}
    private static string GetGenericString(INamedTypeSymbol symbol)
    {
        if (symbol.TypeParameters.Length == 0)
        {
            return "";
        }

        return $"<{string.Join(", ", symbol.TypeParameters.Select(x => x.Name))}>";
    }
    private ResultsModel GetResult(ClassDeclarationSyntax classDeclaration)
    {
        ResultsModel output;
        //SemanticModel compilationSemanticModel = classDeclaration.GetSemanticModel(compilation);
        INamedTypeSymbol symbol = compilation.GetClassSymbol(classDeclaration)!;
        output = symbol.GetStartingResults<ResultsModel>();
        output.InheritedEvent = HasAttributeOnParent(symbol, classAttributeUsed);
        output.GenericString = GetGenericString(symbol);
        output.AlreadyHadAction = ParentHasStateChanged(symbol);
        //anything else needed to get the results needed.
        //if i have several parts, rethink.  obviously can change to anything you want.
        var list = symbol.GetAllPublicProperties(propertyAttributeUsed);
        foreach (var p in list)
        {
            if (SymbolEqualityComparer.Default.Equals(p.ContainingType, symbol) == false)
            {
                continue;
            }
            if (p.SetMethod is null)
            {
                continue; //if there is no set then values won't change anyways so no need for it to be added in this case.
            }
            var fins = p.GetStartingPropertyInformation<PropertyModel>();
            if (p.IsPartialDefinition == false)
            {
                var syntax = p.DeclaringSyntaxReferences
               .FirstOrDefault()?
               .GetSyntax();

                if (syntax is PropertyDeclarationSyntax property)
                {
                    fins.Location = property.Identifier.GetLocation();
                }
            }
            fins.PrivateAccesor = p.SetMethod?.DeclaredAccessibility == Accessibility.Private;
            output.Properties.Add(fins);
        }
        return output;
    }
}