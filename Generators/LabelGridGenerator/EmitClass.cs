namespace LabelGridGenerator;
internal class EmitClass
{
    private readonly SourceProductionContext _context;
    private readonly BasicList<CompleteInformation> _list;
    public EmitClass(SourceProductionContext context, BasicList<CompleteInformation> list)
    {
        _context = context;
        _list = list;
    }
    private void ProcessErrors()
    {
        foreach (var item in _list)
        {
            if (item.NeededPartial)
            {
                _context.RaiseNoPartialClassException(item.Symbol!.Name);
            }
        }
    }
    public void Emit()
    {
        ProcessErrors();
        foreach (var item in _list)
        {
            if (item.NeededPartial == true)
            {
                continue;
            }
            SourceCodeStringBuilder builder = new();
            string interfaceString = "global::BasicGameFrameworkLibrary.Core.BasicGameDataClasses.ILabelGrid";
            builder.WriteLine("#nullable enable")
                .WriteLine(w =>
                {
                    w.Write("namespace ")
                    .Write(item.Symbol!.ContainingNamespace)
                    .Write(";");
                })
            .WriteLine(w =>
            {
                w.Write("public partial class ")
                .Write(item.Symbol!.Name)
                .Write(item.GenericInfo)
                .Write(" : ")
                .Write(interfaceString);
            })
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("string ")
                    .Write(interfaceString)
                    .Write(".GetValue(string propertyName, int decimalPlaces)");
                })
                .WriteCodeBlock(w =>
                {
                    foreach (var p in item.Properties)
                    {
                        w.WriteLine(w =>
                        {
                            w.Write("if (propertyName == ")
                            .AppendDoubleQuote(p.Name)
                            .Write(")");
                        })
                        .WriteCodeBlock(w =>
                        {
                            if (p.Type.Name == "Decimal")
                            {
                                w.WriteLine(w =>
                                {
                                    w.Write("return global::CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions.Decimals.ToCurrency(")
                                    .Write(p.Name)
                                    .Write(", decimalPlaces);");
                                });
                                return; //try this way.
                            }
                            WritePossibleRoll(w, p);
                            w.WriteLine(w =>
                            {
                                w.Write("return ")
                                .Write(p.Name)
                                .Write(".ToString();");
                            });
                        });
                    }
                    w.WriteLine(w =>
                    {
                        w.CustomExceptionLine(w =>
                        {
                            w.Write("Nothing found with property name {propertyName}");
                        });
                    });
                });
            });
            _context.AddSource($"{item.Symbol!.Name}.LabelGrid.g", builder.ToString());
        }
    }
    private void WritePossibleRoll(ICodeBlock w, IPropertySymbol p)
    {
        if (p.Type.Name == "Int32" && p.Name.ToLower().StartsWith("roll"))
        {
            w.WriteLine(w =>
            {
                w.Write("if (propertyName.ToLower().StartsWith(")
                .AppendDoubleQuote("roll")
                .Write("))");
            })
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("int incrs = ")
                    .Write(p.Name)
                    .Write(" - 1;");
                })
                .WriteLine("if (incrs == -1)")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("incrs = 0;");
                })
                .WriteLine("return incrs.ToString();");
            });
        }
    }
}