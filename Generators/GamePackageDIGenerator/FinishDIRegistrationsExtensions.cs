namespace GamePackageDIGenerator;
internal static class FinishDIRegistrationsExtensions
{
    private static bool _initCompleted = false;
    public static void StartMethod()
    {
        _initCompleted = false;
    }
    public static void ProcessFinishDIRegistrations(this ICodeBlock w, BasicList<FirstInformation> list)
    {
        if (list.Count == 0)
        {
            return; //try this way.
        }
        if (_initCompleted == false)
        {
            if (list.Any(x => x.Category != EnumCategory.Object))
            {
                w.WriteLine("Func<object> action;");
            }
            w.WriteLine(w =>
            {
                w.BasicListWrite()
                .Write("<Type> types;");
            });
            _initCompleted = true;
        }
        foreach (var item in list)
        {
            if (item.Category != EnumCategory.Object)
            {
                w.WriteLine("action = () =>")
                .WriteCodeBlock(w =>
                {
                    if (item.Constructors.Count == 0)
                    {
                        w.WriteLine(w =>
                        {
                            w.Write("return new ")
                           .SymbolFullNameWrite(item.MainClass!, item.GenericSymbols)
                           .Write("();");
                        });
                    }
                    else
                    {
                        int index = 0;
                        BasicList<string> variables = new();
                        w.WriteLine("object output;");
                        foreach (var c in item.Constructors)
                        {
                            w.WriteLine(w =>
                            {
                                w.Write("output = container.LaterGetObject(typeof(")
                                .SymbolFullNameWrite(c, item.GenericSymbols)
                                .Write("));");
                            });
                            string value = $"item{index}";
                            variables.Add(value);
                            w.WriteLine(w =>
                            {
                                w.SymbolFullNameWrite(c, item.GenericSymbols)
                                .Write(" ")
                                .Write(value)
                                .Write(" = (")
                                .SymbolFullNameWrite(c, item.GenericSymbols)
                                .Write(")output;");
                            });
                            index++;
                        }
                        w.WriteLine(w =>
                        {
                            w.Write("return new ")
                            .SymbolFullNameWrite(item.MainClass!, item.GenericSymbols)
                            .Write("(");
                            w.InitializeFromCustomList(variables, (w, fins) =>
                            {
                                w.Write(fins);
                            });
                            w.Write(");");
                        });
                    }
                }, endSemi: true);
            }

            w.WriteLine("types = new()")
            .WriteCodeBlock(w =>
            {
                BasicList<INamedTypeSymbol> assignments = item.Assignments.ToBasicList();
                assignments.Add(item.MainClass!); //you can always assign that as well.
                w.InitializeFromCustomList(assignments, (w, assignment) =>
                {
                    w.PopulateTypeOf(assignment, item.GenericSymbols);
                });
            }, endSemi: true);
            w.WriteLine(w =>
            {
                w.Write("container.LaterRegister(typeof(")
                .SymbolFullNameWrite(item.MainClass!, item.GenericSymbols)
                .Write("), types");
                if (item.Category != EnumCategory.Object)
                {
                    w.Write(", action");
                }
                if (item.Tag == "")
                {
                    w.Write(");");
                }
                else
                {
                    w.Write(", ")
                    .AppendDoubleQuote(item.Tag)
                    .Write(");");
                }
            });
        }
    }
}
