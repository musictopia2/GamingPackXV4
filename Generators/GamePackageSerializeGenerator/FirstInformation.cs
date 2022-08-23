namespace GamePackageSerializeGenerator;
internal class FirstInformation
{
    public EnumSerializeCategory Category { get; set; }
    public ClassDeclarationSyntax? Node { get; set; }
    public INamedTypeSymbol? Symbol { get; set; }
}