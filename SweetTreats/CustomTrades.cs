using Il2CppTLD.Trader;
using ExpandedTradingFramework.Framework;
using Random = UnityEngine.Random;

namespace SweetTreats
{
    internal static class CustomTrades
    {
        private static readonly string[] HarvestablePlants =
        [
            "GEAR_MapleSapling",
            "GEAR_BirchSapling",
            "GEAR_BarkTinder",
            "GEAR_Burdock",
            "GEAR_OldMansBeardHarvested",
            "GEAR_ReishiMushroom",
            "GEAR_RoseHip",
            "GEAR_CattailStalk",
            "GEAR_Acorn"
        ];

        internal static readonly CustomTradeDefinition[] Trades =
            [
                TradeIncreaseTrust(),
                TradeSugar()
            ];

        internal static CustomTradeDefinition[] GetEnabledTrades()
        {
            {
                List<CustomTradeDefinition> enabledTrades = [];
                for (int i = 0; i < Trades.Length; i++)
                {
                    CustomTradeDefinition trade = Trades[i];
                    if (trade != null && trade.Enabled) enabledTrades.Add(trade);
                }
                return [.. enabledTrades];
            }
        }
        // Trades List Starting Here
        //______________________________________________________________________________________________________________
        private static CustomTradeDefinition TradeSugar()
        {
            return new CustomTradeDefinition
            {
                Id = "MOD_Sugar",
                Enabled = true,
                Description = "Stalker Aids And Supplements - Sugar.",
                IsSpecialRequest = false,
                SelectionMode = CustomTradeSelectionMode.DrawPool,
                DrawPool = CustomTradeDrawPool.Common,
                MinTrust = 75,
                MaxPerTrade = 10,
                Repeatable = true,
                CostItems =
                [
                    CategoryAmount("Harvestable Plants", HarvestablePlants, 12, CategoryIcon.AnyHarvestPlant),
                    PotableWater(1.5f)
                ],
                DisplayReward = CustomTradeDisplay.Gear("GEAR_SugarA", 1),
                Reward = CustomTradeReward.Gear("GEAR_SugarA", 1, CustomRewardStacking.Auto)
            };
        }
        private static CustomTradeDefinition TradeIncreaseTrust()
        {
            return new CustomTradeDefinition
            {
                Id = "MOD_TrustIncrease",
                Enabled = true,//Settings.instance.TraderExtras,
                IsSpecialRequest = true,
                SelectionMode = CustomTradeSelectionMode.DrawPool,
                DrawPool = CustomTradeDrawPool.AlwaysShown,
                MinTrust = 50,
                MaxPerTrade = 1,
                Repeatable = false,
                SpecialRequest = new CustomSpecialRequestDisplay
                {
                    Title = "Sweet tooth",
                    Description = "Sutherland seems quite grumpy.",
                    RequirementText = "Bring something sweet to cheer up Sutherland.",
                    HideRequirementIcon = true,
                    JournalTitle = "Sweet Tooth",
                    JournalDescription = "Bring something sweet to cheer up Sutherland.",
                    JournalTips = [""]
                },
                CostItems =
                [
                    GearAmount("GEAR_RosehipJam", 10)
                ],
                DisplayReward = CustomTradeDisplay.Gear("GEAR_RosehipJam"),
                Reward = CustomTradeReward.None(),
                OnCompleted = context =>
                {
                    TraderManager trader = GameManager.GetTraderManager();
                    if (trader == null) return;

                    int roll = Random.RandomRangeInt(50, 150);
                    trader.AddTrust(roll);
                }
            };
        }
        
        // Trade list ends here
        //________________________________________________________________________________
        private static CustomTradeItem CategoryAmount(string categoryName, string[] gearNames, int amount, CategoryIcon categoryIcon)
        {
            return new CustomTradeItem
            {
                ExchangeType = ExchangeItemType.CategoryAmount,
                CategoryName = categoryName,
                CategoryGearNames = gearNames,
                CategoryIcon = categoryIcon,
                Amount = amount
            };
        }
        private static CustomTradeItem GearAmount(string gearName, int amount, CustomTradeIcon displayIcon = null)
        {
            return new CustomTradeItem
            {
                ExchangeType = ExchangeItemType.GearAmount,
                GearName = gearName,
                Amount = amount,
                DisplayIcon = displayIcon ?? CustomTradeIcon.None()
            };
        }
        private static CustomTradeItem PotableWater(float liters)
        {
            return new CustomTradeItem
            {
                ExchangeType = ExchangeItemType.Liquid,
                LiquidKind = CustomLiquidKind.PotableWater,
                LiquidLiters = liters
            };
        }
    }
}
