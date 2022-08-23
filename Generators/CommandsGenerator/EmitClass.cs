namespace CommandsGenerator;
internal class EmitClass
{
    private readonly SourceProductionContext _context;
    private readonly BasicList<CompleteInfo> _list;
    public EmitClass(SourceProductionContext context, BasicList<CompleteInfo> list)
    {
        _context = context;
        _list = list;
    }
    private void ProcessErrors()
    {
        foreach (var item in _list)
        {
            if (item.HasPartialClass == false)
            {
                _context.RaiseNoPartialClassException(item.ClassSymbol!.Name);
                item.ReportedError = true;
            }
            if (item.HasPartialCreateCommandsOnly == false && item.NeedsCommandsOnly && item.AdvancedCategory != EnumAdvancedCategory.PlainCommand)
            {
                _context.RaiseNoCreateCommandsRegularException(item.ClassSymbol!.Name);
                item.ReportedError = true;
            }
            if (item.ContainerName == "" && item.NeedsCommandContainer)
            {
                _context.RaiseNoCreateCommandsContainerException(item.ClassSymbol!.Name);
                item.ReportedError = true;
            }
            if (item.AdvancedCategory == EnumAdvancedCategory.Error)
            {
                _context.RaiseNoMultipleImplementsCommand(item.ClassSymbol!.Name);
                item.ReportedError = true;
            }
            if (item.Commands.Count is not 1 && item.AdvancedCategory != EnumAdvancedCategory.None && item.Commands.Any(x => x.CommandName == ""))
            {
                _context.RaiseNeedsSingleMethod(item.ClassSymbol!.Name);
                item.ReportedError = true;
            }
            if (item.CommandProperty is null && item.AdvancedCategory != EnumAdvancedCategory.None && item.Commands.Any(x => x.CommandName == ""))
            {
                _context.RaiseNeedsSingleCommand(item.ClassSymbol!.Name);
                item.ReportedError = true;
            }
            foreach (var c in item.Commands)
            {
                if (item.AdvancedCategory == EnumAdvancedCategory.Error)
                {
                    c.ReportedError = true;
                    continue;
                }
                if (c.RequiresPlain)
                {
                    _context.RaiseNeedsPlain(item.ClassSymbol!.Name, c.MethodSymbol!.Name);
                    c.ReportedError = true;
                }
                if (c.HasTooManyParameters)
                {
                    _context.RaiseTooManyParameters(item.ClassSymbol!.Name, c.MethodSymbol!.Name);
                    c.ReportedError = true;
                }
                //if (c.MiscError == EnumMiscCategory.MisMatch)
                //{
                //    _context.RaiseMismatchParameters(item.ClassSymbol!.Name, c.MethodSymbol!.Name);
                //    c.ReportedError = true;
                //}
                //if (c.MiscError == EnumMiscCategory.TooMany)
                //{
                //    _context.RaiseTooManyParameters(item.ClassSymbol!.Name, c.MethodSymbol!.Name);
                //    c.ReportedError = true;
                //}
                if (c.WrongReturnType)
                {
                    _context.RaiseWrongReturnType(item.ClassSymbol!.Name, c.MethodSymbol!.Name);
                    c.ReportedError = true;
                }
                if (c.NotImplemented)
                {
                    _context.RaiseNotImplemented(item.ClassSymbol!.Name, c.MethodSymbol!.Name, c.Category.GetEnumText());
                    c.ReportedError = true;
                }
                if (c.InvalidCast)
                {
                    _context.RaiseInvalidCast(item.ClassSymbol!.Name, c.MethodSymbol!.Name);
                    c.ReportedError = true;
                }
                //if (c.CannotUseNames)
                //{
                //    _context.RaiseWrongNameType(item.ClassSymbol!.Name, c.MethodSymbol!.Name);
                //    c.ReportedError = true;
                //}
            }
        }
    }
    public void Emit()
    {
        ProcessErrors();
        foreach (var item in _list)
        {
            if (item.HasPartialClass == false || item.ReportedError)
            {
                continue;
            }
            SourceCodeStringBuilder builder = new();
            builder.WriteLine("#nullable enable")
                .WriteLine(w =>
                {
                    w.Write("namespace ")
                    .Write(item.ClassSymbol!.ContainingNamespace)
                    .Write(";");
                })
            .WriteLine(w =>
            {
                w.Write("public partial class ")
                .Write(item.ClassSymbol!.Name)
                .Write(item.GenericInfo);
            })
            .WriteCodeBlock(w =>
            {
                if (item.AdvancedCategory == EnumAdvancedCategory.None)
                {
                    WriteCompleteCommands(w, item);
                }
                if (item.NeedsCommandsOnly && item.HasPartialCreateCommandsOnly)
                {
                    w.WriteLine("partial void CreateCommands()")
                    .WriteCodeBlock(w =>
                    {
                        if (item.AdvancedCategory == EnumAdvancedCategory.None)
                        {
                            WriteCommandsAlone(w, item);
                        }
                        else
                        {
                            WriteCommandsWithContainer(w, item);
                        }
                    });
                }
                if (item.NeedsCommandContainer && item.ContainerName != "")
                {
                    w.WriteLine(w =>
                    {
                        w.Write("partial void CreateCommands(global::BasicGameFrameworkLibrary.CommandClasses.CommandContainer ");
                        w.Write(item.ContainerName)
                        .Write(")");
                    })
                    .WriteCodeBlock(w =>
                    {
                        WriteCommandsWithContainer(w, item);
                    });
                }
            });
            _context.AddSource($"{item.ClassSymbol!.Name}.Command.g", builder.ToString());
        }
    }
    private void WriteCompleteCommands(ICodeBlock w, CompleteInfo info)
    {
        foreach (var item in info.Commands)
        {
            if (item.ReportedError == false)
            {
                w.WriteLine(w =>
                {
                    w.Write("public ")
                    .AppendCommandType(item.Category)
                    .AppendCommandName(item)
                    .Write(";");
                });
            }
        }
    }
    private void WriteCommandsAlone(ICodeBlock w, CompleteInfo info)
    {
        foreach (var item in info.Commands)
        {
            WriteCommandsAlone(w, item);
        }
    }
    private void WriteCommandsAlone(ICodeBlock w, CommandInfo command)
    {
        if (command.ReportedError || command.CreateCategory == EnumCreateCategory.Container)
        {
            return;
        }
        w.WriteLine(w =>
        {
            w.AppendCommandName(command)
            .StartNewCommandMethod()
            .PopulateRestCommand(command, "")
            .EndNewCommandMethod();
        });
    }
    private void WriteCommandsWithContainer(ICodeBlock w, CompleteInfo info)
    {
        foreach (var item in info.Commands)
        {
            WriteCommandsWithContainer(w, info, item);
        }
    }
    private void WriteCommandsWithContainer(ICodeBlock w, CompleteInfo info, CommandInfo command)
    {
        if (command.ReportedError || command.CreateCategory == EnumCreateCategory.Regular)
        {
            return;
        }
        if (info.CommandProperty is null && info.AdvancedCategory != EnumAdvancedCategory.None)
        {
            if (info.CommandProperty is null && command.CommandName == "")
            {
                return;
            }
        }
        //if (info.CommandProperty is null && info.IsControl && info.Commands.Any(x => x.CommandName == ""))
        //{
        //    return;
        //}
        //will rethink once i figure out how to support other command types
        w.WriteLine(w =>
        {
            if (info.AdvancedCategory == EnumAdvancedCategory.None)
            {
                w.AppendCommandName(command);
            }
            else if (info.CommandProperty is not null)
            {
                w.Write(info.CommandProperty!.Name);
            }
            else
            {
                w.Write(command.CommandName); //try this way.
            }
            w.StartNewCommandMethod();
            if (info.AdvancedCategory != EnumAdvancedCategory.Control)
            {
                w.Write(info.ContainerName);
            }
            else
            {
                w.Write("CommandContainer");
            }
            string generic;
            //this would account for enum pickers since generics are needed.
            //if somehow its needed for other cases, requires rethinking.
            if (info.AdvancedCategory != EnumAdvancedCategory.None)
            {
                generic = info.GenericInfo;
            }
            else
            {
                generic = "";
            }
            w.Write(", ")
            .PopulateRestCommand(command, generic)
            .EndNewCommandMethod();
        });
    }
}