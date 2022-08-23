﻿namespace LabelGridGenerator;
internal class ParserClass
{
    private readonly Compilation _compilation;
    public ParserClass(Compilation compilation)
    {
        _compilation = compilation;
    }
    public BasicList<CompleteInformation> GetResults(IEnumerable<ClassDeclarationSyntax> list)
    {
        BasicList<CompleteInformation> output = new();
        foreach (var item in list)
        {
            SemanticModel compilationSemanticModel = item.GetSemanticModel(_compilation);
            INamedTypeSymbol symbol = (INamedTypeSymbol)compilationSemanticModel.GetDeclaredSymbol(item)!;
            CompleteInformation info = new();
            info.Symbol = symbol;
            info.GenericInfo = symbol.GetGenericString();
            if (item.IsPartial() == false)
            {
                info.NeededPartial = true;
            }
            else
            {
                info.Properties = symbol.GetAllPublicProperties(aa.LabelColumnAttribute);
            }
            output.Add(info);
        }
        return output;
    }
}