namespace MonopolyDicedGame.Core.ViewModels;
[InstanceGame]
public partial class MonopolyDicedGameMainViewModel : BasicMultiplayerMainVM
{
    private readonly IToast _toast;
    public MonopolyDicedGameVMData VMData { get; set; }
    public MonopolyDicedGameMainViewModel(CommandContainer commandContainer,
        MonopolyDicedGameMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        MonopolyDicedGameVMData data,
        MonopolyDiceSet monopolyDice,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        MainGame = mainGame;
        VMData = data;
        MonopolyDice = monopolyDice;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    //anything else needed is here.
    public MonopolyDicedGameMainGameClass MainGame;
    public MonopolyDiceSet MonopolyDice;
    partial void CreateCommands(CommandContainer command);
    public override bool CanEndTurn() => MainGame.SaveRoot.RollNumber > 1;
    public bool CanRoll => MainGame.SaveRoot.NumberOfCops < 3;
    private BasicList<BasicDiceModel> GetSelectedDice => MainGame!.SaveRoot.DiceList.GetSelectedItems();

    [Command(EnumCommandCategory.Game)]
    public async Task RollAsync()
    {
        //will be a command now to roll the dice (getting closer to reals).
        if (MonopolyDice.HasSelectedDice())
        {
            _toast.ShowUserErrorToast("Need to either unselect the dice or use them.");
            return;
        }
        await MainGame.RollDiceAsync();
    }
    


    private bool HasChanceError(BasicList<BasicDiceModel> list, Func<OwnedModel, bool> selector)
    {
        int chancesSelected = list.Count(x => x.WhatDice == EnumBasicType.Chance);
        if (chancesSelected > 1)
        {
            _toast!.ShowUserErrorToast("You can only choose one chance to place in a group");
            return true;
        }
        int chancesOwned = MainGame.SaveRoot.Owns.Count(x => x.WasChance && selector.Invoke(x));
        if (chancesOwned > 0 && chancesSelected > 0)
        {
            _toast!.ShowUserErrorToast("Only one chance can be used per group");
            return true;
        }
        if (chancesOwned + chancesSelected > 1)
        {
            _toast!.ShowUserErrorToast("Only one chance can be used per group");
            return true;
        }
        return false;
    }
    public bool CanGroup(int group)
    {
        if (MainGame.SaveRoot.NumberOfCops > 2 || group < 1 || group > 8)
        {
            return false;
        }
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PropertyAsync(int group)
    {
        bool allProper;
        var list = GetSelectedDice;
        if (list.Count == 0)
        {
            _toast!.ShowUserErrorToast("Did not choose any dice");
            return;
        }
        allProper = list.All(x =>
        {
            if (x.WhatDice == EnumBasicType.Chance)
            {
                return true;
            }
            if (x.Group == group)
            {
                return true;
            }
            return false;
        });
        if (allProper == false)
        {
            _toast!.ShowUserErrorToast("You have dice selected that does not belong to this group");
            return;
        }
        if (HasChanceError(list, x => x.Group == group))
        {
            return;
        }
        int maxAllowed;
        if (group == 1 || group == 8)
        {
            //only 2 are allowed
            maxAllowed = 2;
        }
        else
        {
            maxAllowed = 3;
        }
        int total = list.Count + MainGame.SaveRoot.Owns.Count(x => x.Group == group);
        if (total > maxAllowed)
        {
            _toast!.ShowUserErrorToast("This will show too many dice used for this group");
            return;
        }
        if (MainGame.BasicData.MultiPlayer)
        {
            await MainGame.Network!.SendAllAsync("propertychosen", group);
        }
        await MainGame.ChosePropertyAsync(group);
    }
    public bool CanTrain => MainGame.SaveRoot.NumberOfCops < 3;
    [Command(EnumCommandCategory.Game)]
    public async Task TrainAsync()
    {
        var list = GetSelectedDice;
        if (list.Count == 0)
        {
            _toast!.ShowUserErrorToast("Did not choose any dice");
            return;
        }
        bool allTrains;
        allTrains = list.All(x =>
        {
            if (x.WhatDice == EnumBasicType.Railroad)
            {
                return true;
            }
            if (x.WhatDice == EnumBasicType.Chance)
            {
                return true;
            }
            return false;
        });
        if (allTrains == false)
        {
            _toast!.ShowUserErrorToast("You have some dice that is not train or chance selected");
            return;
        }
        int owns = MainGame!.SaveRoot.Owns.Count(x => x.UsedOn == EnumBasicType.Railroad);
        if (owns + list.Count > 4)
        {
            _toast!.ShowUserErrorToast("This results in too many trains.  Only 4 are allowed at the most");
            return;
        }
        if (HasChanceError(list, x => x.UsedOn == EnumBasicType.Railroad))
        {
            return;
        }
        if (MainGame.BasicData.MultiPlayer)
        {
            await MainGame.Network!.SendAllAsync("trainchosen");
        }
        await MainGame.ChoseTrainAsync();
    }
    public bool CanUtility(EnumUtilityType utility)
    {
        if (MainGame.SaveRoot.NumberOfCops > 2)
        {
            return false;
        }
        if (utility == EnumUtilityType.None)
        {
            return false; //i think.
        }
        return true;
        //return true;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task UtilityAsync(EnumUtilityType utility)
    {
        var list = GetSelectedDice;
        if (list.Count == 0)
        {
            _toast!.ShowUserErrorToast("Did not choose any dice");
            return;
        }
        if (list.Count > 1)
        {
            _toast!.ShowUserErrorToast("Can only choose one item when its utility");
            return;
        }
        int count = MainGame!.SaveRoot.Owns.Count(x => x.Utility != EnumUtilityType.None);
        if (count == 2)
        {
            _toast!.ShowUserErrorToast("Too many utilities would have been chosen");
            return;
        }
        var dice = list.Single();
        bool hasUsed = MainGame.SaveRoot.Owns.Any(x => x.Utility == utility);
        if (hasUsed)
        {
            _toast!.ShowUserErrorToast("You already own this utility");
            return;
        }
        if (HasChanceError(list, x => x.UsedOn == EnumBasicType.Utility))
        {
            return;
        }
        if (dice.WhatDice == EnumBasicType.Chance || list.Single().Index == (int)utility)
        {
            //this means can process for utility
            if (MainGame.BasicData.MultiPlayer)
            {
                await MainGame.Network!.SendAllAsync("utilitychosen");
            }
            await MainGame.ChoseUtilityAsync(utility);
            return;
        }
        _toast!.ShowUserErrorToast("This is not the proper utility");
    }
}