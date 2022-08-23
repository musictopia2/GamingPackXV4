using System.Numerics;

namespace GamePackageDIGenerator;
internal static class SpecializedExtensions
{
    public static void PopulateReplaceBoardGameColorClasses(this ICodeBlock w)
    {
        if (_player is null || _saved is null || _color is null)
        {
            return;
        }
        w.WriteLine("private static BasicList<Type> ReplaceBoardGameColorClasses()")
        .WriteCodeBlock(w =>
        {
            w.WriteLine("BasicList<Type> output = new()")
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("typeof(")
                    .PopulateBeginningColorClass(_color, _player, _saved)
                    .Write("),");
                })
                .WriteLine(w =>
                {
                    w.Write("typeof(")
                    .PopulateBeginningColorModel(_color, _player)
                    .Write("),");
                })
                .WriteLine(w =>
                {
                    w.Write("typeof(")
                    .PopulateBeginningChooseColorViewModel(_color, _player)
                    .Write(")");
                });
            }, true)
            .WriteLine("return output;");
        });
    }

    //private static bool _canreplaceBoardProcess = false;
    private static INamedTypeSymbol? _player;
    private static INamedTypeSymbol? _saved;
    private static INamedTypeSymbol? _color;


    private static void Reset()
    {
        _player = null;
        _saved = null;
        _color = null;
    }
    public static void PopulateDiceAloneMethod(this ICodeBlock w, INamedTypeSymbol symbol, Compilation compilation)
    {
        INamedTypeSymbol dice = CaptureDiceSymbol(symbol);
        INamedTypeSymbol player = compilation.GetTypeByMetadataName("BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data.YahtzeePlayerItem`1")!;
        INamedTypeSymbol saved = compilation.GetTypeByMetadataName("BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data.YahtzeeSaveInfo`1")!;
        w.PopulateCommonMethod(player, saved, dice, compilation); //hopefully works (?)
        w.PopulateDiceMethod(dice, player, compilation);
        w.FinishDiceAloneMethod(dice, compilation);
    }
    

    private static IWriter StartRegistrations(this IWriter w)
    {
        w.Write("container.RegisterType<")
            .GlobalWrite();
        return w;
    }
    //string logicns = "BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Logic.";
    //string containerns = "BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers.";
    //string datans = "BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data.";
    //string viewmodelns = "BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels.";
    private static IWriter WithYahtzeeLogic(this IWriter w, string name)
    {
        w.Write("BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Logic.")
            .Write(name);
        return w;   
    }
    private static IWriter WithYahtzeeContainer(this IWriter w, string name)
    {
        w.Write("BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers.")
            .Write(name);
        return w;
    }
    private static IWriter WithYahtzeeData(this IWriter w, string name)
    {
        w.Write("BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data.")
            .Write(name);
        return w;
    }
    private static IWriter WithYahtzeeViewModel(this IWriter w, string name)
    {
        w.Write("BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels.")
            .Write(name);
        return w;
    }
    private static IWriter WithGenerics(this IWriter w, INamedTypeSymbol dice, string extras = "")
    {
        w.Write("<")
            .Write(dice.Name)
            .Write(">>")
            .Write("(")
            .Write(extras)
            .Write(");");
        return w;
    }
    private static IWriter WithEndingAlone(this IWriter w)
    {
        w.Write(">();");
        return w;
    }
    private static void FinishDiceAloneMethod(this ICodeBlock w, INamedTypeSymbol dice, Compilation compilation)
    {
        w.WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeViewModel("YahtzeeShellViewModel")
            .WithGenerics(dice);
        })
        .WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeViewModel("YahtzeeScoresheetViewModel")
            .WithGenerics(dice, "false");
        })
        .WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeViewModel("YahtzeeMainViewModel")
            .WithGenerics(dice, "false");
        }).WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeContainer("YahtzeeVMData")
            .WithGenerics(dice);
        }).WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeLogic("BasicYahtzeeGame")
            .WithGenerics(dice);
        }).WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeData("YahtzeeSaveInfo")
            .WithGenerics(dice);
        })
        .WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeLogic("ScoreLogic")
            .WithEndingAlone();
        }).WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeContainer("YahtzeeGameContainer")
            .WithGenerics(dice);
        }).WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeContainer("ScoreContainer")
            .WithEndingAlone();
        }).WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeLogic("YahtzeeMove")
            .WithGenerics(dice);
        }).WriteLine(w =>
        {
            w.StartRegistrations()
            .WithYahtzeeLogic("YahtzeeEndRoundLogic")
            .WithGenerics(dice);
        });
        BasicList<FirstInformation> list = GetDiceAloneList(dice, compilation);
        w.ProcessFinishDIRegistrations(list);
    }
    private static BasicList<FirstInformation> GetDiceAloneList(INamedTypeSymbol dice, Compilation compilation)
    {
        string logicns = "BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Logic.";
        string containerns = "BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Containers.";
        string datans = "BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.Data.";
        string viewmodelns = "BasicGameFrameworkLibrary.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels.";
        string lastg = "`1";
        BasicList<string> temps = new()
        {
            $"{viewmodelns}YahtzeeShellViewModel{lastg}",
            $"{viewmodelns}YahtzeeScoresheetViewModel{lastg}",
            $"{viewmodelns}YahtzeeMainViewModel{lastg}",
            $"{containerns}YahtzeeVMData{lastg}",
            $"{logicns}BasicYahtzeeGame{lastg}",
            $"{datans}YahtzeeSaveInfo{lastg}",
            $"{logicns}ScoreLogic",
            $"{containerns}YahtzeeGameContainer{lastg}",
            $"{containerns}ScoreContainer",
            $"{logicns}YahtzeeMove{lastg}",
            $"{logicns}YahtzeeEndRoundLogic{lastg}"
        };
        Dictionary<string, INamedTypeSymbol> matches = new();
        matches.Add("D", dice);
        BasicList<FirstInformation> output = temps.GetFirstInformation(matches, compilation);
        return output;
    }
    public static void PopulateRegisterSpecializedMethod(this ICodeBlock w, INamedTypeSymbol symbol, Compilation compilation)
    {
        Reset();
        FinishDIRegistrationsExtensions.StartMethod();
        //you may have or not have the colors.
        INamedTypeSymbol player = CapturePlayerSymbol(symbol);
        INamedTypeSymbol saved = CaptureSaveSymbol(symbol);
        w.PopulateCommonMethod(player, saved, compilation);
        if (symbol.Name == "IBeginningColors" || symbol.Name == "IBeginningComboCardsColors")
        {
            //do color processes.
            INamedTypeSymbol color = CaptureColorSymbol(symbol);
            w.PopulateColorsMethod(color, player, saved, compilation);
        }
        if (symbol.Name == "IBeginningDice")
        {
            INamedTypeSymbol dice = CaptureDiceSymbol(symbol);
            w.PopulateDiceMethod(dice, player, compilation);
        }
        if (symbol.Name == "IBeginningCards" || symbol.Name == "IBeginningComboCardsColors")
        {
            INamedTypeSymbol deck = CaptureDeckSymbol(symbol);
            w.PopulateDeckMethod(deck, compilation);
        }
    }
    private static IWriter PopulatePlayerOrSaved(this IWriter w, INamedTypeSymbol symbol, INamedTypeSymbol? dice)
    {
        w.Write(symbol.Name);
        if (dice is not null)
        {
            w.Write("<")
            .Write(dice.Name)
            .Write(">");
        }
        return w;
    }
    private static void PopulateCommonMethod(this ICodeBlock w, INamedTypeSymbol player, INamedTypeSymbol saved, INamedTypeSymbol? dice, Compilation compilation)
    {
        BasicList<FirstInformation> list = GetCommonList(player, saved, dice, compilation);
        w.WriteLine(w =>
        {
            w.Write("container.RegisterType<").GlobalWrite()
            .Write("BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses.BasicGameLoader<")
            .PopulatePlayerOrSaved(player, dice)
            .Write(", ")
            .PopulatePlayerOrSaved(saved, dice)
            .Write(">>();");
        })
        .WriteLine(w =>
        {
            w.Write("container.RegisterType<").GlobalWrite()
            .Write("BasicGameFrameworkLibrary.MiscProcesses.RetrieveSavedPlayers<")
            .PopulatePlayerOrSaved(player, dice)
            .Write(", ")
            .PopulatePlayerOrSaved(saved, dice)
            .Write(">>();");
        })
        .WriteLine(w =>
        {
            w.Write("container.RegisterType<").GlobalWrite()
            .Write("BasicGameFrameworkLibrary.ViewModels.MultiplayerOpeningViewModel<")
            .PopulatePlayerOrSaved(player, dice)
            .Write(">>();");
        });
        w.ProcessFinishDIRegistrations(list);
    }
    private static void PopulateCommonMethod(this ICodeBlock w, INamedTypeSymbol player, INamedTypeSymbol saved, Compilation compilation)
    {
        w.PopulateCommonMethod(player, saved, null, compilation);

    }
    private static IWriter PopulateBeginningColorClass(this IWriter w, INamedTypeSymbol color, INamedTypeSymbol player, INamedTypeSymbol saved)
    {
        w.GlobalWrite()
        .Write("BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses.BeginningColorProcessorClass<")
            .Write(color.Name)
            .Write(", ")
            .Write(player.Name)
            .Write(", ")
            .Write(saved.Name)
            .Write(">");
        return w;
    }
    private static IWriter PopulateBeginningChooseColorViewModel(this IWriter w, INamedTypeSymbol color, INamedTypeSymbol player)
    {
        w.GlobalWrite()
        .Write("BasicGameFrameworkLibrary.ViewModels.BeginningChooseColorViewModel<")
           .Write(color.Name)
           .Write(", ")
           .Write(player.Name)
           .Write(">");
        return w;
    }
    private static IWriter PopulateBeginningColorModel(this IWriter w, INamedTypeSymbol color, INamedTypeSymbol player)
    {
        w.GlobalWrite()
           .Write("BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers.BeginningColorModel<")
           .Write(color.Name)
           .Write(", ")
           .Write(player.Name)
           .Write(">");
        return w;
    }
    public static void PopulateColorsMethod(this ICodeBlock w, INamedTypeSymbol color, INamedTypeSymbol player, INamedTypeSymbol saved, Compilation compilation)
    {
        _color = color;
        _player = player;
        _saved = saved;
        //i think this needs to figure out how to later register them as well (?)
        BasicList<FirstInformation> list = GetColorList(color, player, saved, compilation);
        w.WriteLine(w =>
        {
            w.Write("container.RegisterType<")
            .PopulateBeginningColorClass(color, player, saved)
            .Write(">();");
        })
       .WriteLine(w =>
       {
           w.Write("container.RegisterType<")
           .PopulateBeginningChooseColorViewModel(color, player)
           .Write(">();");
       })
       .WriteLine(w =>
       {
           w.Write("container.RegisterType<")
           .PopulateBeginningColorModel(color, player)
           .Write(">();");
       });
        w.ProcessFinishDIRegistrations(list);
        w.WriteLine("global::BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers.MiscDelegates.GetAutoGeneratedObjectsToReplace = ReplaceBoardGameColorClasses;");
    }
    private static BasicList<FirstInformation> GetColorList(INamedTypeSymbol color, INamedTypeSymbol player, INamedTypeSymbol saved, Compilation compilation)
    {
        BasicList<string> temps = new()
        {
            "BasicGameFrameworkLibrary.MultiplayerClasses.BasicGameClasses.BeginningColorProcessorClass`3",
            "BasicGameFrameworkLibrary.ViewModels.BeginningChooseColorViewModel`2",
            "BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers.BeginningColorModel`2"
        };
        Dictionary<string, INamedTypeSymbol> matches = new();
        matches.Add("P", player);
        matches.Add("S", saved);
        matches.Add("E", color);
        BasicList<FirstInformation> output = temps.GetFirstInformation(matches, compilation);
        return output;
    }
    private static BasicList<FirstInformation> GetDiceList(Compilation compilation, INamedTypeSymbol dice, INamedTypeSymbol player)
    {
        BasicList<string> temps = new()
        {
            "BasicGameFrameworkLibrary.Dice.StandardRollProcesses`2",
            $"{dice.ContainingNamespace.ToDisplayString()}.{dice.Name}"
        };
        INamedTypeSymbol? intSymbol = compilation.GetTypeByMetadataName("System.Int32");
        if (intSymbol is null)
        {
            throw new Exception("Integer was not found");
        }
        Dictionary<string, INamedTypeSymbol> matches = new();
        matches.Add("D", dice);
        matches.Add("P", player);
        matches.Add("Con", intSymbol);
        BasicList<FirstInformation> output = temps.GetFirstInformation(matches, compilation);
        return output;
    }
    private static BasicList<FirstInformation> GetDiceList(Compilation compilation, INamedTypeSymbol player)
    {
        BasicList<string> temps = new()
        {
            "BasicGameFrameworkLibrary.Dice.StandardRollProcesses`2",
            "BasicGameFrameworkLibrary.Dice.SimpleDice"
        };
        //simpledice this time.
        INamedTypeSymbol? diceSymbol = compilation.GetTypeByMetadataName("BasicGameFrameworkLibrary.Dice.SimpleDice");
        if (diceSymbol is null)
        {
            throw new Exception("There was no simple dice found");
        }
        INamedTypeSymbol? intSymbol = compilation.GetTypeByMetadataName("System.Int32");
        if (intSymbol is null)
        {
            throw new Exception("Integer was not found");
        }
        Dictionary<string, INamedTypeSymbol> matches = new();
        matches.Add("D", diceSymbol);
        matches.Add("P", player);
        matches.Add("Con", intSymbol); //this is used to match up so the generic names would correspond to the real symbols instead.
        BasicList<FirstInformation> output = temps.GetFirstInformation(matches, compilation);
        return output;
    }
    private static BasicList<FirstInformation> GetDeckList(Compilation compilation, INamedTypeSymbol deck)
    {
        BasicList<string> temps = new()
        {
            "BasicGameFrameworkLibrary.DrawableListsObservable.DeckObservablePile`1",
            "BasicGameFrameworkLibrary.BasicDrawables.BasicClasses.GenericCardShuffler`1"
        };
        Dictionary<string, INamedTypeSymbol> matches = new();
        matches.Add("D", deck);
        BasicList<FirstInformation> output = temps.GetFirstInformation(matches, compilation);
        return output;
    }
    
    private static BasicList<FirstInformation> GetCommonList(INamedTypeSymbol player, INamedTypeSymbol saved, INamedTypeSymbol? dice, Compilation compilation)
    {
        BasicList<string> temps = new()
        {
            "BasicGameFrameworkLibrary.MultiplayerClasses.LoadingClasses.BasicGameLoader`2",
            "BasicGameFrameworkLibrary.ViewModels.MultiplayerOpeningViewModel`1",
            "BasicGameFrameworkLibrary.MiscProcesses.RetrieveSavedPlayers`2"
        };
        Dictionary<string, INamedTypeSymbol> matches = new();
        matches.Add("P", player);
        matches.Add("S", saved);
        if (dice is not null)
        {
            matches.Add("D", dice);
        }
        BasicList<FirstInformation> output = temps.GetFirstInformation(matches, compilation);
        return output;
    }

    private static void PopulateDeckMethod(this ICodeBlock w, INamedTypeSymbol deck, Compilation compilation)
    {
        var list = GetDeckList(compilation, deck);
        w.WriteLine(w =>
        {
            w.Write("container.RegisterType<").GlobalWrite()
            .Write("BasicGameFrameworkLibrary.DrawableListsObservable.DeckObservablePile<")
            .Write(deck.Name)
            .Write(">>();");
        })
        .WriteLine(w =>
        {
            w.Write("container.RegisterType<").GlobalWrite()
            .Write("BasicGameFrameworkLibrary.BasicDrawables.BasicClasses.GenericCardShuffler<")
            .Write(deck.Name)
            .Write(">>();");
        });
        w.ProcessFinishDIRegistrations(list);
    }
    private static void PopulateDiceMethod(this ICodeBlock w, INamedTypeSymbol dice, INamedTypeSymbol player, Compilation compilation)
    {
        var list = GetDiceList(compilation, dice, player);
        w.WriteLine(w =>
        {
            w.Write("container.RegisterType<").GlobalWrite()
            .Write("BasicGameFrameworkLibrary.Dice.StandardRollProcesses<")
            .Write(dice.Name)
            .Write(", ")
            .Write(player.Name);
            if (player.TypeArguments.Count() == 1)
            {
                w.Write("<")
                .Write(dice.Name)
                .Write(">");
            }
            else if (player.TypeArguments.Count() > 1)
            {
                throw new Exception("When populating dice, the player argument only supports one typed parameter");
            }
            w.Write(">>();");
        })
        .WriteLine(w =>
        {
            w.Write("container.RegisterSingleton<").GlobalWrite()
            .Write("BasicGameFrameworkLibrary.Dice.IGenerateDice<int>, ")
            .Write(dice.Name)
            .Write(">();");
        });
        w.ProcessFinishDIRegistrations(list);
    }
    public static void PopulateStandardDiceMethod(this ICodeBlock w, Compilation compilation, INamedTypeSymbol symbol)
    {
        FinishDIRegistrationsExtensions.StartMethod();
        var player = CapturePlayerSymbol(symbol);
        w.WriteLine(w =>
        {
            w.Write("container.RegisterType<global::BasicGameFrameworkLibrary.Dice.StandardRollProcesses<global::BasicGameFrameworkLibrary.Dice.SimpleDice, ")
            .Write(player.Name)
            .Write(">>();");
        })
        .WriteLine("container.RegisterSingleton<global::BasicGameFrameworkLibrary.Dice.IGenerateDice<int>, global::BasicGameFrameworkLibrary.Dice.SimpleDice>();");
        var list = GetDiceList(compilation, player);
        w.ProcessFinishDIRegistrations(list);
    }
    private static INamedTypeSymbol CapturePlayerSymbol(INamedTypeSymbol symbol)
    {
        foreach (var item in symbol.TypeArguments)
        {
            if (item.Implements("IPlayerItem"))
            {
                return (INamedTypeSymbol)item;
            }
        }
        throw new Exception("No player found");
    }
    private static INamedTypeSymbol CaptureSaveSymbol(INamedTypeSymbol symbol)
    {
        foreach (var item in symbol.TypeArguments)
        {
            var temps = (INamedTypeSymbol)item;
            if (temps.InheritsFrom("BasicSavedGameClass"))
            {
                return temps;
            }
        }
        throw new Exception("No basic saved game class inherited from found");
    }
    private static INamedTypeSymbol CaptureColorSymbol(INamedTypeSymbol symbol)
    {
        foreach (var item in symbol.TypeArguments)
        {
            //since one source generator cannot access information from another source generator, then means that I can first attempt to do iequatable.
            //if that works, great.  if that does not work, then rethinking will be required (could be required to make this implement another interface just to make this work).  not sure yet.
            if (item.Implements("IEquatable"))
            {
                return (INamedTypeSymbol)item;
            }
        }
        //return null;
        throw new Exception("No IEquatable Found Which Represents Color");
    }
    private static INamedTypeSymbol CaptureDeckSymbol(INamedTypeSymbol symbol)
    {
        foreach (var item in symbol.TypeArguments)
        {
            if (item.Implements("IDeckObject"))
            {
                return (INamedTypeSymbol)item;
            }
        }
        throw new Exception("No deck found");
    }
    private static INamedTypeSymbol CaptureDiceSymbol(INamedTypeSymbol symbol)
    {
        foreach (var item in symbol.TypeArguments)
        {
            if (item.Implements("IStandardDice"))
            {
                return (INamedTypeSymbol)item;
            }
        }
        throw new Exception("No dice found");
    }
}