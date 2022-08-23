using System;
using System.Collections.Generic;
using System.Text;

namespace GamePackageDIGenerator;
internal static class MiscExtensions
{
    public static BasicList<FirstInformation> GetFirstInformation(this BasicList<string> symbols, Dictionary<string, INamedTypeSymbol> matches, Compilation compilation, EnumCategory category = EnumCategory.None)
    {
        BasicList<FirstInformation> output = new();
        foreach (var firsts in symbols)
        {
            INamedTypeSymbol? symbol = compilation.GetTypeByMetadataName(firsts);
            if (symbol is null)
            {
                throw new Exception($"Unable to get symbol for type {firsts}");
            }
            FirstInformation item = new();
            //not sure if i need a delegate or not (?)
            item.MainClass = symbol;
            item.Category = category; //this means i can have a case where i choose object for the type (when dealing with cards).
            item.GenericSymbols = matches;
            var temps = item.MainClass!.AllInterfaces.ToBasicList();
            foreach (var temp in temps)
            {
                if (temp.Name != "IHandle" && temp.Name != "IHandleAsync" && temp.Name != "IEquatable" && temp.Name != "IComparable" && temp.Name != "IDisposable")
                {
                    item.Assignments.Add(temp);
                } //cannot do anything with ihandle or ihandleasync since event aggravation handles that anyways.
            }
            if (item.MainClass!.Constructors.Count() > 0)
            {
                var tests = item.MainClass!.Constructors.OrderByDescending(x => x.Parameters.Count()).FirstOrDefault();
                var nexts = item.MainClass!.Constructors.OrderByDescending(x => x.Parameters.Count()).FirstOrDefault().Parameters.ToBasicList();
                foreach (var a in nexts)
                {
                    var lasts = a.Type;
                    item.Constructors.Add((INamedTypeSymbol)lasts);
                }
            }

            output.Add(item);
        }
        return output;
    }
}
