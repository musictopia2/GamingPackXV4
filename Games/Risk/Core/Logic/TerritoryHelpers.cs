namespace Risk.Core.Logic;
public static class TerritoryHelpers
{
    private readonly static Dictionary<string, PointF> _territoryLabels = new()
    {
        { "Alaska", new PointF(217.9965f, 189.55507f) },
        { "Northwest Territory", new PointF(296.99652f, 192.55507f) },
        { "Greenland", new PointF(418.99652f, 161.55507f) },
        { "Alberta", new PointF(275.99652f, 229.55507f) },
        { "Ontario", new PointF(325.99652f, 241.55507f) },
        { "Quebec", new PointF(376.99652f, 238.55507f) },
        { "Western United States", new PointF(278.99652f, 280.55505f) },
        { "Eastern United States", new PointF(333.99652f, 296.55505f) },
        { "Central America", new PointF(275.99652f, 329.55505f) },
        { "Venezuela", new PointF(336.99652f, 388.55505f) },
        { "Peru", new PointF(348.99652f, 455.55505f) },
        { "Brazil", new PointF(391.99652f, 434.55505f) },
        { "Argentina", new PointF(351.99652f, 508.55505f) },
        { "Iceland", new PointF(478.99652f, 214.55507f) },
        { "Scandinavia", new PointF(531.9965f, 214.55507f) },
        { "Ukraine", new PointF(603.9965f, 244.55507f) },
        { "Great Britain", new PointF(476.99652f, 278.55505f) },
        { "Northern Europe", new PointF(532.9965f, 281.55505f) },
        { "Western Europe", new PointF(478.99652f, 341.55505f) },
        { "Southern Europe", new PointF(546.9965f, 323.55505f) },
        { "North Africa", new PointF(501.99652f, 419.55505f) },
        { "Egypt", new PointF(558.9965f, 395.55505f) },
        { "Congo", new PointF(565.9965f, 479.55505f) },
        { "East Africa", new PointF(592.9965f, 447.55505f) },
        { "South Africa", new PointF(567.9965f, 541.55505f) },
        { "Madagascar", new PointF(632.9965f, 544.55505f) },
        { "Ural", new PointF(672.9965f, 228.55507f) },
        { "Siberia", new PointF(707.9965f, 188.55507f) },
        { "Yakursk", new PointF(765.9965f, 177.55507f) },
        { "Kamchatka", new PointF(821.9965f, 180.55507f) },
        { "Irkutsk", new PointF(758.9965f, 239.55507f) },
        { "Afghanistan", new PointF(659.9965f, 293.55505f) },
        { "China", new PointF(745.9965f, 328.55505f) },
        { "Mongolia", new PointF(764.9965f, 286.55505f) },
        { "Japan", new PointF(840.9965f, 291.55505f) },
        { "Middle East", new PointF(616.9965f, 369.55505f) },
        { "India", new PointF(699.9965f, 369.55505f) },
        { "Siam", new PointF(758.9965f, 388.55505f) },
        { "Indonesia", new PointF(774.9965f, 469.55505f) },
        { "New Guinea", new PointF(837.9965f, 454.55505f) },
        { "Western Australia", new PointF(802.9965f, 540.55505f) },
        { "Eastern Australia", new PointF(861.9965f, 525.55505f) }
    };

    private readonly static Dictionary<string, int> _territoryIds = new()
    {
        { "Alaska", 1 },
        { "Northwest Territory", 2 },
        { "Greenland", 3 },
        { "Alberta", 4 },
        { "Ontario", 5 },
        { "Quebec", 6 },
        { "Western United States", 7 },
        { "Eastern United States", 8 },
        { "Central America", 9 },
        { "Venezuela", 10 },
        { "Peru", 11 },
        { "Brazil", 12 },
        { "Argentina", 13 },
        { "Iceland", 14 },
        { "Scandinavia", 15 },
        { "Ukraine", 16 },
        { "Great Britain", 17 },
        { "Northern Europe", 18 },
        { "Western Europe", 19 },
        { "Southern Europe", 20 },
        { "North Africa", 21 },
        { "Egypt", 22 },
        { "Congo", 23 },
        { "East Africa", 24 },
        { "South Africa", 25 },
        { "Madagascar", 26 },
        { "Ural", 27 },
        { "Siberia", 28 },
        { "Yakursk", 29 },
        { "Kamchatka", 30 },
        { "Irkutsk", 31 },
        { "Afghanistan", 32 },
        { "China", 33 },
        { "Mongolia", 34 },
        { "Japan", 35 },
        { "Middle East", 36 },
        { "India", 37 },
        { "Siam", 38 },
        { "Indonesia", 39 },
        { "New Guinea", 40 },
        { "Western Australia", 41 },
        { "Eastern Australia", 42 }
    };
    private static Dictionary<string, string> _territoryPaths = [];
    public static async Task PopulateTerritoriesAsync()
    {
        if (_territoryPaths.Count > 0)
        {
            return;
        }
        _territoryPaths = await Resources.TerritoriesData.GetResourceAsync<Dictionary<string, string>>();
    }
    extension (RiskGameContainer container)
    {
        public TerritoryModel GetTerritory(int id) => container.SaveRoot.TerritoryList.Single(xx => xx.Id == id);
    }
    extension (TerritoryModel territory)
    {
        public PointF LabelLocation => _territoryLabels[territory.Name];
        public EnumContinent Continent
        {
            get
            {
                int id = territory.Id;
                return id switch
                {
                    < 10 => EnumContinent.NorthAmerica,
                    < 14 => EnumContinent.SouthAmerica,
                    < 21 => EnumContinent.Europe,
                    < 27 => EnumContinent.Africa,
                    < 39 => EnumContinent.Asia,
                    _ => EnumContinent.Austrailia
                };
            }
        }
        public int BorderWidth(RiskGameContainer container)
        {
            if (container.SaveRoot.PreviousTerritory == territory.Id || container.SaveRoot.CurrentTerritory == territory.Id)
            {
                return 4;
            }
            return 2;
        }
    }
    
    extension (PlayerCollection<RiskPlayerItem> players)
    {
        internal IBasicList<EnumColorChoice> GetRandomColors()
        {
            int x = 0;
            int y = 0;
            BasicList<EnumColorChoice> output = new();
            do
            {
                y++;
                x++;
                if (y > 42)
                {
                    break;
                }
                if (x > players.Count)
                {
                    x = 1;
                }
                output.Add(players[x].Color);

            } while (true);
            return output.GetRandomList(); //hopefully this works.
        }
        internal int ArmiesToBeginWith
        {
            get
            {
                if (players.Any(xx => xx.InGame == false))
                {
                    return 40;
                }
                int reduces = players.Count - 3;
                int firstAmount = 35;
                int mults = reduces * 5;
                return firstAmount - mults;
            }
            
        }
        internal void SetUpBeginningArmies()
        {
            int armies = players.ArmiesToBeginWith;
            players.ForEach(player => player.ArmiesToBegin = armies);
        }
        public BasicList<TerritoryModel> GetTerritories()
        {
            players.SetUpBeginningArmies();
            BasicList<TerritoryModel> output = [];
            IBasicList<EnumColorChoice> colors = players.GetRandomColors();
            if (colors.Count is not 42)
            {
                throw new CustomBasicException("Must have 42 items chosen");
            }
            for (int i = 1; i <= 42; i++)
            {
                TerritoryModel territory = new();
                EnumColorChoice color = colors[i - 1];
                RiskPlayerItem player = players.Single(xx => xx.Color == color);
                territory.Owns = player.Id;
                player.ArmiesToBegin--; //because needs at least one in the claimed areas.
                territory.Armies++;
                territory.Color = color.WebColor;
                territory.Id = i;
                territory.Name = _territoryIds.Where(xx => xx.Value == i).Select(xx => xx.Key).Single();
                output.Add(territory);
                switch (i)
                {
                    case 1:
                        AddNeighbors(territory, "2,4,30");
                        break;
                    case 2:
                        AddNeighbors(territory, "1,3,4,5");
                        break;
                    case 3:
                        AddNeighbors(territory, "2,5,6,14");
                        break;
                    case 4:
                        AddNeighbors(territory, "1,2,5,7");
                        break;
                    case 5:
                        AddNeighbors(territory, "4,2,3,6,7,8");
                        break;
                    case 6:
                        AddNeighbors(territory, "3,5,8");
                        break;
                    case 7:
                        AddNeighbors(territory, "4,5,8,9");
                        break;
                    case 8:
                        AddNeighbors(territory, "5,6,7,9");
                        break;
                    case 9:
                        AddNeighbors(territory, "7,8,10");
                        break;
                    case 10:
                        AddNeighbors(territory, "9,11,12");
                        break;
                    case 11:
                        AddNeighbors(territory, "10,12,13");
                        break;
                    case 12:
                        AddNeighbors(territory, "10,11,13,21");
                        break;
                    case 13:
                        AddNeighbors(territory, "11,12");
                        break;
                    case 14:
                        AddNeighbors(territory, "3,17,15");
                        break;
                    case 15:
                        AddNeighbors(territory, "14,17,16,18");
                        break;
                    case 16:
                        AddNeighbors(territory, "15,18,20,36,32,27,28");
                        break;
                    case 17:
                        AddNeighbors(territory, "14,15,18,19");
                        break;
                    case 18:
                        AddNeighbors(territory, "17,15,19,20,16");
                        break;
                    case 19:
                        AddNeighbors(territory, "17,18,20,21");
                        break;
                    case 20:
                        AddNeighbors(territory, "19,18,16,36,21,22");
                        break;
                    case 21:
                        AddNeighbors(territory, "12,19,22,24,23,20");
                        break;
                    case 22:
                        AddNeighbors(territory, "21,20,24,36");
                        break;
                    case 23:
                        AddNeighbors(territory, "21,24,25");
                        break;
                    case 24:
                        AddNeighbors(territory, "21,22,36,23,25,26");
                        break;
                    case 25:
                        AddNeighbors(territory, "23,24,26");
                        break;
                    case 26:
                        AddNeighbors(territory, "24,25");
                        break;
                    case 27:
                        AddNeighbors(territory, "16,32,28,33");
                        break;
                    case 28:
                        AddNeighbors(territory, "27,33,29,31,34");
                        break;
                    case 29:
                        AddNeighbors(territory, "28,31,30");
                        break;
                    case 30:
                        AddNeighbors(territory, "1, 29,31,34,35");
                        break;
                    case 31:
                        AddNeighbors(territory, "28,34,30,29");
                        break;
                    case 32:
                        AddNeighbors(territory, "36,16,27,33,37");
                        break;
                    case 33:
                        AddNeighbors(territory, "38,37,32,27,28,34");
                        break;
                    case 34:
                        AddNeighbors(territory, "33,28,31,30,35");
                        break;
                    case 35:
                        AddNeighbors(territory, "34,30");
                        break;
                    case 36:
                        AddNeighbors(territory, "24,22,20,16,32,37");
                        break;
                    case 37:
                        AddNeighbors(territory, "36,32,33,38");
                        break;
                    case 38:
                        AddNeighbors(territory, "37,33,39");
                        break;
                    case 39:
                        AddNeighbors(territory, "38,40,41");
                        break;
                    case 40:
                        AddNeighbors(territory, "39,42,41");
                        break;
                    case 41:
                        AddNeighbors(territory, "39,40,42");
                        break;
                    case 42:
                        AddNeighbors(territory, "41,40");
                        break;
                    default:
                        break;
                }
            }
            return output;
        }
    }
    extension (BasicList<TerritoryModel> territories)
    {
        public BasicList<TerritoryModel> GetSelectedTerritories(RiskGameContainer container)
        {
            BasicList<TerritoryModel> output = [];
            var saveRoot = container.SaveRoot;
            if (saveRoot.PreviousTerritory > 0)
            {
                output.Add(territories.Single(xx => xx.Id == saveRoot.PreviousTerritory));
            }
            if (saveRoot.CurrentTerritory > 0)
            {
                output.Add(territories.Single(xx => xx.Id == saveRoot.CurrentTerritory));
            }
            return output;
        }
        public BasicList<TerritoryModel> GetUnselectedTerritories(RiskGameContainer container)
        {
            var output = territories.ToBasicList();
            var temps = territories.GetSelectedTerritories(container);
            output.RemoveGivenList(temps);
            return output;
        }
        public void PopulateTerritories(PlayerCollection<RiskPlayerItem> players)
        {
            players.ForEach(player =>
            {
                BasicList<int> ownList = territories.Where(xx => xx.Owns == player.Id).Select(xx => xx.Id).ToBasicList();
                int remaining = player.ArmiesToBegin;
                remaining.Times(xx =>
                {
                    int id = ownList.GetRandomItem();
                    var territory = territories.Single(xx => xx.Id == id);
                    territory.Armies++;
                });
            });
        }
    }
    extension (TerritoryModel territory)
    {
        public string GetBorderColor(RiskGameContainer container)
        {
            if (territory.Id == container.SaveRoot.PreviousTerritory)
            {
                return cc1.LimeGreen.ToWebColor;
            }
            if (territory.Id == container.SaveRoot.CurrentTerritory)
            {
                return cc1.Purple.ToWebColor;
            }
            EnumContinent continent = territory.Continent;
            return continent switch
            {
                EnumContinent.SouthAmerica => cc1.Tomato.ToWebColor,
                EnumContinent.NorthAmerica => cc1.Aqua.ToWebColor,
                EnumContinent.Africa => cc1.SaddleBrown.ToWebColor,
                EnumContinent.Europe => cc1.DodgerBlue.ToWebColor,
                EnumContinent.Asia => cc1.Black.ToWebColor,
                EnumContinent.Austrailia => cc1.Indigo.ToWebColor,
                _ => cc1.Indigo.ToWebColor
            };
        }
        internal void AddNeighbors(string list)
        {
            list = list.Replace(" ", "");
            BasicList<string> tempList = list.Split(",").ToBasicList();
            territory.Neighbors.Clear();
            tempList.ForEach(tt =>
            {
                territory.Neighbors.Add(int.Parse(tt));
            });
        }
    }
    extension (string territoryName)
    {
        public string Path => _territoryPaths[territoryName];
    }
}