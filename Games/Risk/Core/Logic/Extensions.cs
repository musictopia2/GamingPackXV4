namespace Risk.Core.Logic;
public static class Extensions
{
    extension (RiskGameContainer container)
    {
        public AttackResultModel GetAttackResults()
        {
            int attackLoss = 0;
            int defenseLoss = 0;
            for (int i = 0; i < container.SaveRoot.ArmiesInBattle; i++)
            {
                if (i + 1 > container.SaveRoot.NumberDefenseArmies)
                {
                    break;
                }
                if (container.VMData.AttackCup!.DiceList[i].Value > container.VMData.DefenseCup!.DiceList[i].Value)
                {
                    defenseLoss++;
                }
                else
                {
                    attackLoss++;
                }
            }
            return new(attackLoss, defenseLoss);
        }
        public void PopulatePlaceArmies()
        {
            if (container.SaveRoot.PreviousTerritory == 0)
            {
                return;
            }
            if (container.VMData.ArmiesToPlace == 0)
            {
                throw new CustomBasicException("You cannot have 0 armies to place");
            }
            int placements;
            if (container.VMData.ArmiesToPlace > 20)
            {
                placements = 20;
            }
            else
            {
                placements = container.VMData.ArmiesToPlace;
            }
            container.VMData.NumberPicker.LoadNormalNumberRangeValues(1, placements);
            container.VMData.NumberPicker.UnselectAll();
        }
        public void PopulateMoveArmies()
        {
            if (container.SaveRoot.PreviousTerritory == 0 || container.SaveRoot.CurrentTerritory == 0)
            {
                return;
            }
            TerritoryModel territory = container.GetTerritory(container.SaveRoot.PreviousTerritory);
            if (territory.Armies == 1)
            {
                throw new CustomBasicException("Should not have been able to choose territory because only one army which has to be left behind");
            }
            if (container.SaveRoot.Stage == EnumStageList.Move)
            {
                container.VMData.NumberPicker.LoadNormalNumberRangeValues(1, territory.Armies - 1);
            }
            else
            {
                container.VMData.NumberPicker.LoadNormalNumberRangeValues(0, territory.Armies - 1);
                container.VMData.ArmiesChosen = -1; //to force a person to choose.
            }
            container.VMData.NumberPicker.UnselectAll(); //make sure to unselect all.  no automation.  you have to manually choose.
        }
        public void PopulateAttackArmies(int howMany)
        {
            if (howMany < 1 || howMany > 3)
            {
                throw new CustomBasicException("Must have between 1 and 3 armies");
            }
            container.SaveRoot!.ArmiesInBattle = howMany; //start with this.
            BasicList<string> list = new()
        {
            "1 Army"
        };
            if (howMany >= 2)
            {
                list.Add("2 Armies");
            }
            if (howMany == 3)
            {
                list.Add("3 Armies");
            }
            container.VMData.AttackPicker.LoadTextList(list);
            container.VMData.AttackPicker.UnselectAll();
            container.VMData.AttackPicker.SelectSpecificItem(howMany); //default to maximum.
        }
        public void PopulateInstructions()
        {
            var saveRoot = container.SaveRoot;
            var data = container.VMData;
            if (container.SaveRoot.Stage == EnumStageList.None)
            {
                throw new CustomBasicException("Stage was not even filled out");
            }
            if (saveRoot.Stage == EnumStageList.Begin)
            {
                if (data.PlayerHand1.HandList.Count >= 5)
                {
                    saveRoot.Instructions = "Return 3 risk cards";
                    return;
                }
                saveRoot.Instructions = "Return any risk cards or continue turn";
                return;
            }
            if (saveRoot.Stage == EnumStageList.Place)
            {
                saveRoot.Instructions = "Please place your new army pieces on your territory";
                return;
            }
            if (saveRoot.Stage == EnumStageList.StartAttack)
            {
                saveRoot.Instructions = "Choose territory to attack from and the territory to attack as well or continue turn";
                return;
            }
            if (saveRoot.Stage == EnumStageList.Roll)
            {
                saveRoot.Instructions = "Choose how many armies for attack and start attack to roll";
                return;
            }
            if (saveRoot.Stage == EnumStageList.TransferAfterBattle)
            {
                saveRoot.Instructions = "Please choose how many armies to move to the newly conquered territory";
                return;
            }
            if (saveRoot.Stage == EnumStageList.Move)
            {
                saveRoot.Instructions = "Please make one move or continue turn";
                return;
            }
            if (saveRoot.Stage == EnumStageList.EndTurn)
            {
                saveRoot.Instructions = "Please end your turn";
                return;
            }
        }
        public string NextReenforcementLabel
        {
            get
            {
                int firsts = container.SaveRoot.SetsReturned + 1;
                int nexts = firsts.NextSetExtraReenforcements;
                return $"The next risk cards sent in gains {nexts} reenforcements";
            }
            
        }
        public int RiskCardsReturnedExtraReenforcements
        {
            get
            {
                if (container.SaveRoot.SetsReturned == 0)
                {
                    throw new CustomBasicException("Must return at least one set");
                }
                return container.SaveRoot.SetsReturned.NextSetExtraReenforcements;
            }
           
        }
    }
    extension (int setsReturned)
    {
        internal int NextSetExtraReenforcements
        {
            get
            {
                if (setsReturned == 1)
                {
                    return 4;
                }
                if (setsReturned == 2)
                {
                    return 6;
                }
                if (setsReturned == 3)
                {
                    return 8;
                }
                if (setsReturned == 4)
                {
                    return 10;
                }
                if (setsReturned == 5)
                {
                    return 12;
                }
                if (setsReturned == 6)
                {
                    return 15;
                }
                int diffs = setsReturned - 6;
                int starts = 15;
                int extras = diffs * 5;
                return extras + starts;
            }
            
        }
    }
    extension (IBasicList<RiskCardInfo> cards)
    {
        public bool CanReturnRiskCards
        {
            get
            {
                if (cards.Count < 3)
                {
                    return false; //needs at least 3.
                }
                if (cards.Count(xx => xx.Army == EnumArmyType.Wild) == 2)
                {
                    return false;
                }
                int counts = cards.DistinctCount(xx => xx.Army);
                if (counts == 1)
                {
                    return true;
                }
                if (counts == 3)
                {
                    return true;
                }
                return cards.Any(xx => xx.Army == EnumArmyType.Wild);
            }
        }
    }
}