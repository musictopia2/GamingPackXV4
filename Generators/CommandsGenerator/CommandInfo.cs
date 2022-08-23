namespace CommandsGenerator;
internal class CommandInfo
{
    public ISymbol? CanSymbol { get; set; }
    public EnumCommandCategory Category { get; set; }
    public IMethodSymbol? MethodSymbol { get; set; }
    public bool IsAsync { get; set; }
    public ISymbol? ParameterUsed { get; set; } //cannot use inamedtype because sometimes causes casting errors.
    public int CanParameters { get; set; }
    public bool IsProperty { get; set; }
    public bool NotImplemented { get; set; }
    public bool InvalidCast { get; set; }
    public bool WrongReturnType { get; set; }
    //public bool CannotUseNames { get; set; }
    public bool HasTooManyParameters { get; set; }
    //public EnumMiscCategory MiscError { get; set; } = EnumMiscCategory.None;
    public bool ReportedError { get; set; }
    public EnumCreateCategory CreateCategory { get; set; }
    public string MethodName { get; set; } = "";
    public string CommandName { get; set; } = ""; //try without the symbol.  especially since i only need the name and i captured that anyways.
    //public IPropertySymbol? CommandProperty { get; set; } //sometimes you will have this as well.
    public bool RequiresPlain { get; set; }
    //you need the symbol for can now.

}