using Il2CppTLD.Gear;

namespace SweetTreats
{
    [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake))] //Patches for sweetened drinks to behave like vanilla counterparts but with extra features
    internal class SweetenedDrinks
    {
        internal static void Postfix(GearItem __instance)
        {
            if (__instance == null) return;

            string name = __instance.name;
            string lower = name.ToLowerInvariant();

            if (name.Contains("GEAR_RosehipJam") || name.Contains("GEAR_SugarA") || name.Contains("GEAR_UncookedRosehipJam"))
            {
                return;
            }
            if (!lower.Contains("jam") && !lower.Contains("sugar"))
            {
                return;
            }
            if (lower.Contains("bannock") || lower.Contains("porridge"))
            {
                return;
            }

            FoodItem food = __instance.gameObject.GetComponent<FoodItem>();
            if (food == null) return;

            food.m_HeatedWhenCooked = true;
            food.m_PercentHeatLossPerMinuteIndoors = 0.75f;
            food.m_PercentHeatLossPerMinuteOutdoors = 1.5f;

            FreezingBuff FB = __instance.gameObject.GetComponent<FreezingBuff>() ?? __instance.gameObject.AddComponent<FreezingBuff>();
            if (FB == null) return;
            
            FB.m_DurationHours = 1;
            FB.m_InitialPercentDecrease = 25;
            FB.m_RateOfIncreaseScale = 0.45f;

            if (lower.Contains("sugar"))
            {
                FoodMaxStatBuff foodMaxStat = __instance.gameObject.GetComponent<FoodMaxStatBuff>() ?? __instance.gameObject.AddComponent<FoodMaxStatBuff>();
                if (foodMaxStat == null) return;
                
                foodMaxStat.m_Buff = 10;
                foodMaxStat.m_CanStack = false;
                foodMaxStat.m_TimeActiveHours = 0.5f;
                foodMaxStat.m_Stat = FoodMaxStatBuff.StatType.Stamina;

                FatigueBuff fatigue = __instance.gameObject.GetComponent<FatigueBuff>() ?? __instance.gameObject.AddComponent<FatigueBuff>();
                if (fatigue == null) return;
                
                    fatigue.m_InitialPercentDecrease = 5f;
                    fatigue.m_RateOfIncreaseScale = 0.75f;
                    fatigue.m_DurationHours = 0.75f;
            }

            //Burdock Tea
            if (name.Contains("GEAR_BurdockTeaJam"))
            {
                food.m_Nutrients = Utilities.VitC(Settings.instance.VitaminCJam + 7);
            }
            //Birchbark Tea
            if (name.Contains("GEAR_BirchbarkTeaJam"))
            {
                food.m_Nutrients = Utilities.VitC(Settings.instance.VitaminCJam + 6);
            }
            //Green Tea
            if (name.Contains("GEAR_GreenTeaCupJam"))
            {
                food.m_Nutrients = Utilities.VitC(Settings.instance.VitaminCJam + 11);
            }
            //Reishi Tea
            if (name.Contains("ReishiTeaJam"))
            {
                food.m_Nutrients = Utilities.VitC(Settings.instance.VitaminCJam + 5);
            }
            //RoseHip Tea
            if (name.Contains("RoseHipTeaJam"))
            {
                food.m_Nutrients = Utilities.VitC(Settings.instance.VitaminCJam + 13);
            }
            //Pine Needle Tea
            if (name.Contains("PineNeedleTea"))
            {
                food.m_Nutrients = Utilities.VitC(Settings.instance.VitaminCJam + 30);
            }
            //Coffees
            if (lower.Contains("coffeecup"))
            {
                FatigueBuff fatigueBuff = __instance.gameObject.GetComponent<FatigueBuff>() ?? __instance.gameObject.AddComponent<FatigueBuff>();
                if (fatigueBuff == null) return;
                
                if (name.Contains("GEAR_AcornCoffeeCupSugar"))
                {
                    fatigueBuff.m_DurationHours = 0.75f;
                    fatigueBuff.m_InitialPercentDecrease = 7.5f;
                    fatigueBuff.m_RateOfIncreaseScale = 0.2f;
                }
                if (name.Contains("GEAR_CoffeeCupSugar"))
                {
                    fatigueBuff.m_DurationHours = 1.25f;
                    fatigueBuff.m_InitialPercentDecrease = 12.5f;
                    fatigueBuff.m_RateOfIncreaseScale = 0.4f;
                }
            }
            // Birchbark Tea
            if (lower.Contains("birchbarktea") || lower.Contains("birchbarknoodles"))
            {
                ConditionOverTimeBuff conditionOverTime = __instance.gameObject.GetComponent<ConditionOverTimeBuff>() ?? __instance.gameObject.AddComponent<ConditionOverTimeBuff>();
                if (conditionOverTime == null) return;
                
                    if (name.Contains("jam"))
                    {
                        conditionOverTime.m_ConditionIncreasePerHour = 3;
                        conditionOverTime.m_NumHours = 2.5f;
                    }
                    else if (name.Contains("sugar"))
                    {
                        conditionOverTime.m_ConditionIncreasePerHour = 2.5f;
                        conditionOverTime.m_NumHours = 2;
                    }
            }
            // FirstAid Teas
            if (name.Contains("GEAR_BurdockTea") || name.Contains("GEAR_ReishiTea") || name.Contains("GEAR_RoseHipTea"))
            {
                FirstAidItem fia = __instance.gameObject.GetComponent<FirstAidItem>() ?? __instance.gameObject.AddComponent<FirstAidItem>();
                if (fia == null) return;

                //For all
                {
                    fia.m_LocalizedInspectModeUseText = new LocalizedString { m_LocalizationID = "GAMEPLAY_DRINK" };
                    fia.m_LocalizedProgressBarMessage = new LocalizedString { m_LocalizationID = "GAMEPLAY_DrinkingProgress" };
                    fia.m_LocalizedRemedyText = new LocalizedString { m_LocalizationID = "GAMEPLAY_Drink" };
                    fia.m_UseAudio = "Play_Slurping1";
                    fia.m_TimeToUseSeconds = 2;
                    fia.m_UnitsPerUse = 1;

                    if (name.Contains("GEAR_BurdockTea") || name.Contains("GEAR_ReishiTea"))
                    {
                        fia.m_ProvidesAntibiotics = true;
                    }
                    if (name.Contains("GEAR_RoseHipTea"))
                    {
                        fia.m_KillsPain = true;
                    }
                }
            }
            //CookingPot Materials

            Cookable cookable = __instance.gameObject.GetComponent<Cookable>();
            if (cookable == null) return;
            
                if (name.Contains("GEAR_AcornCoffeeCup"))
                {
                    cookable.m_CookingPotMaterialsList = new Material[1] { Utilities.AcornCoffeeLiquidMaterial() };
                }
                if (name.Contains("GEAR_CoffeeCup"))
                {
                    cookable.m_CookingPotMaterialsList = new Material[1] { Utilities.CoffeeLiquidMaterial() };
                }
                if (name.Contains("GEAR_GreenTeaCup"))
                {
                    cookable.m_CookingPotMaterialsList = new Material[1] { Utilities.HerbalTeaLiquidMaterial() };
                }
                if (name.Contains("GEAR_BirchbarkTea") || name.Contains("GEAR_ReishiTea"))
                {
                    cookable.m_CookingPotMaterialsList = new Material[1] { Utilities.ReishiTeaLiquidMaterial() };
                }
                if (name.Contains("GEAR_BurdockTea") || name.Contains("GEAR_RoseHipTea"))
                {
                    cookable.m_CookingPotMaterialsList = new Material[1] { Utilities.RoseHipTeaLiquidMaterial() };
                }
                if (name.Contains("GEAR_RosehipJam"))
                {
                    cookable.m_CookingPotMaterialsList = new Material[1] { Utilities.JamLiquidMaterial() };
                }
        }
    }
}
