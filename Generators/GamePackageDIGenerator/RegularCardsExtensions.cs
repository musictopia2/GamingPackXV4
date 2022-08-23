namespace GamePackageDIGenerator;
internal static class RegularCardsExtensions
{
    public static void PopulateRegularCardsMethod(this ICodeBlock w, RegularCardInformation card, Compilation compilation)
    {
        FinishDIRegistrationsExtensions.StartMethod();
        INamedTypeSymbol symbol = CaptureRegularCard(card.Symbol!);
        w.WriteLine("bool rets = container.RegistrationExist<IRegularCardsSortCategory>();")
        .WriteLine("if (rets == true)")
        .WriteCodeBlock(w =>
        {
            w.WriteLine("IRegularCardsSortCategory thisCat = container.Resolve<IRegularCardsSortCategory>();")
            .WriteLine(w =>
            {
                w.Write("SortSimpleCards<")
                .Write(symbol.Name)
                .Write("> thisSort = new ();");
            })
            .WriteLine("thisSort.SuitForSorting = thisCat.SortCategory;")
            .WriteLine("container.RegisterSingleton(thisSort);");
        });
        w.WriteLine(w =>
        {
            w.Write("container.RegisterSingleton<IRegularAceCalculator, ");
            if (card.AceLow)
            {
                w.Write("RegularLowAceCalculator");
            }
            else
            {
                w.Write("RegularAceHighCalculator");
            }
            w.Write(">();");
        });
        if (card.CustomDeck == false)
        {
            w.WriteLine(w =>
            {
                w.Write("container.RegisterSingleton<IDeckCount, ");
                if (card.AceLow)
                {
                    w.Write("RegularAceLowSimpleDeck");
                }
                else
                {
                    w.Write("RegularAceHighSimpleDeck");
                }
                w.Write(">();");
            });
        }
        var list = GetCardList(compilation, card);
        w.ProcessFinishDIRegistrations(list);
        list = GetSortList(compilation, symbol);
        w.ProcessFinishDIRegistrations(list);
    }
    private static INamedTypeSymbol CaptureRegularCard(INamedTypeSymbol symbol)
    {
        foreach (var item in symbol.TypeArguments)
        {
            if (item.Implements("IRegularCard"))
            {
                return (INamedTypeSymbol)item;
            }
        }
        throw new Exception("No regular deck found");
    }
    private static BasicList<FirstInformation> GetSortList(Compilation compilation, INamedTypeSymbol symbol)
    {
        BasicList<string> temps = new()
        {
            "BasicGameFrameworkLibrary.RegularDeckOfCards.SortSimpleCards`1"
        };
        Dictionary<string, INamedTypeSymbol> matches = new();
        matches.Add("R", symbol!);
        BasicList<FirstInformation> output = temps.GetFirstInformation(matches, compilation, EnumCategory.Object);
        return output;
    }
    private static BasicList<FirstInformation> GetCardList(Compilation compilation, RegularCardInformation card)
    {
        BasicList<string> temps = new();
        if (card.AceLow)
        {
            temps.Add("BasicGameFrameworkLibrary.RegularDeckOfCards.RegularLowAceCalculator");
        }
        else
        {
            temps.Add("BasicGameFrameworkLibrary.RegularDeckOfCards.RegularAceHighCalculator");
        }
        if (card.CustomDeck == false && card.AceLow)
        {
            temps.Add("BasicGameFrameworkLibrary.RegularDeckOfCards.RegularAceLowSimpleDeck");
        }
        else if (card.CustomDeck == false && card.AceLow == false)
        {
            temps.Add("BasicGameFrameworkLibrary.RegularDeckOfCards.RegularAceHighSimpleDeck");
        }
        Dictionary<string, INamedTypeSymbol> matches = new(); //this has no generics.
        BasicList<FirstInformation> output = temps.GetFirstInformation(matches, compilation);
        return output;
    }
}