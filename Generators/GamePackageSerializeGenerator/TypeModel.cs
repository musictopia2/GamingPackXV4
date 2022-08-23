using System;
using System.Collections.Generic;
using System.Text;

namespace GamePackageSerializeGenerator;

internal class TypeModel
{
    public string GetGlobalNameSpace => $"global::{SymbolUsed!.ContainingNamespace.ToDisplayString()}";
    public string CollectionNameSpace { get; set; } = "";
    public string CollectionStringName { get; set; } = "";
    public bool NullablePossible { get; set; } = false; //try to require to be true (?)
    public string FileName { get; set; } = ""; //try to search by filename now.
    public string SubName { get; set; } = ""; //if its generic, then needs to get the name of the underlying one.
    public INamedTypeSymbol? SubSymbol { get; set; }
    public ITypeSymbol? SymbolUsed { get; set; }
    public string TypeName => SymbolUsed!.Name;
    public BasicList<string> EnumNames { get; set; } = new(); //for enums, can capture it here for efficiency (instead of running several times)
    public EnumTypeCategory TypeCategory { get; set; }
    public EnumSpecialCategory SpecialCategory { get; set; }
    public EnumLoopCategory LoopCategory { get; set; }
}
