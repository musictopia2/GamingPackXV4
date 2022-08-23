namespace GamePackageSerializeGenerator;
internal static class SymbolExtensions
{
    public static bool IsDictionary(this INamedTypeSymbol symbol) => symbol.Name == "Dictionary";
    public static KeyValuePair<ITypeSymbol, ITypeSymbol> GetDictionarySymbols(this INamedTypeSymbol symbol)
    {
        BasicList<ITypeSymbol> symbols = symbol.TypeArguments.ToBasicList();
        if (symbols.Count != 2)
        {
            throw new Exception("Must have 2 generic types for dictionaries");
        }
        KeyValuePair<ITypeSymbol, ITypeSymbol> output = new(symbols.First(), symbols.Last());
        return output;
    }
    public static string GetSubName(this IPropertySymbol pp)
    {
        var aa = (INamedTypeSymbol)pp.Type;
        if (aa.IsDictionary())
        {
            //this means subname is different.
            var pairs = aa.GetDictionarySymbols();
            return $"{pp.Type.Name}{pairs.Key.Name}{pairs.Value.Name}"; //well see if this is correct.  if not, rethinking will be required.
        }
        EnumTypeCategory cat = pp.GetListCategory();
        //we may need simple type eventually (?)
        //EnumTypeCategory type = pp.Type.GetSimpleCategory();
        string name = pp.Type.Name;
        ITypeSymbol? temps;
        if (cat == EnumTypeCategory.None)
        {
            temps = pp.Type.GetSingleGenericTypeUsed();
            if (temps is null)
            {
                return name;
            }
            return $"{name}{temps!.Name}";
        }
        temps = pp.Type.GetSingleGenericTypeUsed();
        if (cat == EnumTypeCategory.SingleList)
        {
            var fins = temps!.GetSingleGenericTypeUsed();
            if (fins is null)
            {
                return $"{name}{temps!.Name}";
            }
            return $"{name}{temps!.Name}{fins.Name}";
        }
        if (cat == EnumTypeCategory.DoubleList)
        {
            temps = temps!.GetSingleGenericTypeUsed();
            var fins = temps!.GetSingleGenericTypeUsed();
            if (fins is null)
            {
                return $"{name}{name}{temps!.Name}";
            }
            return $"{name}{name}{temps!.Name}{fins.Name}";
        }
        throw new Exception($"Nothing found for GetSubName.  The type name was {name} and the property name was {pp.Name}");
    }
    public static EnumTypeCategory GetListCategory(this IParameterSymbol pp)
    {
        if (pp.Type.IsCollection() == false)
        {
            return EnumTypeCategory.None;
        }
        var others = pp.Type.GetSingleGenericTypeUsed();
        return others!.IsCollection() ? EnumTypeCategory.DoubleList : EnumTypeCategory.SingleList;
    }
    public static EnumTypeCategory GetListCategory(this ITypeSymbol symbol)
    {
        if (symbol.IsCollection() == false)
        {
            if (symbol.Implements("IPlayerCollection") || symbol.Implements("ISimpleList"))
            {
                return EnumTypeCategory.SingleList; //for now, always single.  if double is needed, requires rethinking.
            }
            return EnumTypeCategory.None;
        }
        var others = symbol.GetSingleGenericTypeUsed()!;
        return others.IsCollection() ? EnumTypeCategory.DoubleList : EnumTypeCategory.SingleList;
    }
    public static EnumTypeCategory GetListCategory(this IPropertySymbol pp)
    {
        return pp.Type.GetListCategory();
        
    }
    public static EnumTypeCategory GetSimpleCategory(this ITypeSymbol symbol)
    {
        if (symbol.TypeKind == TypeKind.Enum)
        {
            return EnumTypeCategory.StandardEnum;
        }
        if (symbol.Name.StartsWith("Enum"))
        {
            return EnumTypeCategory.CustomEnum;
        }
        if (symbol.Name == "Nullable")
        {
            INamedTypeSymbol others = (INamedTypeSymbol) symbol;
            if(others.GetSingleGenericTypeUsed()!.Name == "Int32")
            {
                return EnumTypeCategory.NullableInt;
            }
            throw new Exception($"There was nullable but no support.  The name of the generic type was {others.Name}");
        }
        if (symbol.Name == "Int32")
        {
            return EnumTypeCategory.Int;
        }
        if (symbol.Name == "Boolean")
        {
            return EnumTypeCategory.Bool;
        }
        if (symbol.Name == "String")
        {
            return EnumTypeCategory.String;
        }
        if (symbol.Name == "Decimal")
        {
            return EnumTypeCategory.Decimal;
        }
        if (symbol.Name == "PointF")
        {
            return EnumTypeCategory.PointF;
        }
        if (symbol.Name == "SizeF")
        {
            return EnumTypeCategory.SizeF;
        }
        if (symbol.Name == "Vector")
        {
            return EnumTypeCategory.Vector;
        }
        if (symbol.Name == "Char")
        {
            return EnumTypeCategory.Char;
        }
        return EnumTypeCategory.Complex;
    }
    public static bool PropertyIgnored(this IPropertySymbol p, BasicList<IPropertySymbol> completeIgnores)
    {
        foreach (var aa in completeIgnores)
        {
            if (aa.Name == p.Name && aa.OriginalDefinition.ToDisplayString() == p.OriginalDefinition.ToDisplayString())
            {
                return true;
            }
        }
        return false;
    }
}