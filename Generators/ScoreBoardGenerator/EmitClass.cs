namespace ScoreBoardGenerator;
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
    private readonly string _interfaceString = "global::BasicGameFrameworkLibrary.Core.ScoreBoardClassesCP.IScoreBoard"; //can be iffy.
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
                .Write(item.Symbol!.Name);
                if (item.Symbol.TypeParameters.Count() == 1)
                {
                    w.Write("<")
                    .Write(item.Symbol.TypeParameters.Single().Name)
                    .Write(">"); //for now, allow.
                }
                w.Write(" : ")
                .Write(_interfaceString);
            })
            .WriteCodeBlock(w =>
            {
                WriteVisibleMethod(w, item);
                WriteTextDisplayMethod(w, item);
            });
            _context.AddSource($"{item.Symbol!.Name}.Scoreboard.g", builder.ToString());
        }
    }
    private BasicList<IPropertySymbol> GetVisibleProperties(CompleteInformation item)
    {
        BasicList<IPropertySymbol> output = new();
        foreach (var p in item.Properties)
        {
            if (p.Type.Name == "Boolean")
            {
                output.Add(p);
            }
        }
        return output;
    }
    private BasicList<IPropertySymbol> GetDecimalProperties(CompleteInformation item)
    {
        BasicList<IPropertySymbol> output = new();
        foreach (var p in item.Properties)
        {
            if (p.Type.Name == "Decimal")
            {
                output.Add(p);
            }
        }
        return output;
    }
    private void WriteVisibleMethod(ICodeBlock w, CompleteInformation item)
    {
        var list = GetVisibleProperties(item);
        w.WriteLine(w =>
        {
            w.Write("private bool IsVisible(")
            .PopulateScoreBoardColumn()
            .Write(")");
        })
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("if (column.VisiblePath == ")
                    .AppendDoubleQuote()
                    .Write(")");
                })
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("return true;");
                });
                foreach (var p in list)
                {
                    w.WriteLine(w =>
                    {
                        w.Write("if (column.VisiblePath == ")
                        .AppendDoubleQuote(p.Name)
                        .Write(")");
                    })
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine(w =>
                        {
                            w.Write("return ")
                            .Write(p.Name)
                            .Write(";");
                        });
                    });
                }
                w.WriteLine(w =>
                {
                    w.CustomExceptionLine(w =>
                    {
                        w.Write("Nothing found with property {column.VisiblePath}");
                    });
                });
            });
    }
    private void WriteTextDisplayMethod(ICodeBlock w, CompleteInformation item)
    {
        w.WriteLine(w =>
        {
            w.Write("string ")
            .Write(_interfaceString)
            .Write(".TextToDisplay(")
            .PopulateScoreBoardColumn().Write(", bool useAbbreviationForTrueFalse)");
        }).WriteCodeBlock(w =>
        {
            w.WriteLine("if (IsVisible(column) == false)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("return ")
                    .AppendDoubleQuote()
                    .Write(";");
                });
            });
            w.PopulateScoreCategory("TrueFalse", w =>
            {
                WriteTrueFalseProcesses(w, item);
            });
            w.PopulateScoreCategory("Currency", w =>
            {
                WriteCurrencyProcesses(w, item);
            });
            WriteMiscProcesses(w, item);
        });
    }
    private void WriteTrueFalseProcesses(ICodeBlock w, CompleteInformation item)
    {
        w.WriteLine("bool rets = false;")
            .WriteLine("bool used = false;");
        WriteTrueFalseLines(w, item);
        w.WriteLine("if (used == false)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.CustomExceptionLine(w =>
                    {
                        w.Write("Nothing found for true/false boolean field for property {column.MainPath}");
                    });
                });
            });
        w.WriteLine("if (rets == true)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine("if (useAbbreviationForTrueFalse)")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine(w =>
                    {
                        w.Write("return ")
                        .AppendDoubleQuote("Y")
                        .Write(";");
                    });
                })
                .WriteLine(w =>
                {
                    w.Write("return ")
                    .AppendDoubleQuote("Yes")
                    .Write(";");
                });
            })
            .WriteLine("if (useAbbreviationForTrueFalse)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("return ")
                    .AppendDoubleQuote("N")
                    .Write(";");
                });
            })
            .WriteLine(w =>
            {
                w.Write("return ")
                .AppendDoubleQuote("No")
                .Write(";");
            }); ;
    }
    private void WriteTrueFalseLines(ICodeBlock w, CompleteInformation item)
    {
        var list = GetVisibleProperties(item);
        foreach (var p in list)
        {
            w.WriteLine(w =>
            {
                w.Write("if (column.MainPath == ")
                .AppendDoubleQuote(p.Name)
                .Write(" && used == false)");
            })
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("rets = ")
                    .Write(p.Name)
                    .Write(";");
                })
                .WriteLine("used = true;");
            });
        }
    }
    private void WriteCurrencyProcesses(ICodeBlock w, CompleteInformation item)
    {
        var list = GetDecimalProperties(item);
        foreach (var p in list)
        {
            w.PopulateProperty(p, w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("return global::CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions.Decimals.ToCurrency(")
                    .Write(p.Name)
                    .Write(", 0);");
                });
            });
        }
        w.WriteLine(w =>
        {
            w.CustomExceptionLine(w =>
            {
                w.Write("No currency field property found for {column.MainPath}");
            });
        });
    }
    private void WriteMiscProcesses(ICodeBlock w, CompleteInformation item)
    {
        foreach (var p in item.Properties)
        {
            w.PopulateProperty(p, w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("return ")
                    .Write(p.Name);
                    if (p.Type.Name == "String")
                    {
                        w.Write(";");
                    }
                    else
                    {
                        w.Write(".ToString();");
                    }
                });

            });
        }
        w.WriteLine(w =>
        {
            w.CustomExceptionLine(w =>
            {
                w.Write("No Misc field property found for {column.MainPath}");
            });
        });
    }
}