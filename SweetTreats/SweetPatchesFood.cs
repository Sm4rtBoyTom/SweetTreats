using Il2CppTLD.Gear;
using Il2CppTLD.IntBackedUnit;

namespace SweetTreats
{
    [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake))] //Patches for sweetened food to behave like vanilla counterparts but with extra features (litle more powerful than drinks)
    internal class SweetenedFood
    {
        internal static void Postfix(GearItem __instance)
        {
            if (__instance == null) return;

            string name = __instance.name;
            string lower = name.ToLowerInvariant();

            if (!lower.Contains("jam") && !lower.Contains("sugar"))
            {
                return;
            }
            if (!lower.Contains("bannock") && !lower.Contains("porridge") && !lower.Contains("noodles"))
            {
                return;
            }
            FoodItem food = __instance.gameObject.GetComponent<FoodItem>();
            if (food == null) return;
            
            food.m_ChanceFoodPoisoningLowCondition = 5;
            food.m_ChanceFoodPoisoningRuined = 10;

            if (name.Contains("GEAR_CookedBannockJam")
             || name.Contains("GEAR_CookedBannockAcornJam") 
             || name.Contains("GEAR_CookedBarkBannockJam") 
             || name.Contains("GEAR_CookedBirchBarkBannockJam"))
            {
                food.m_Nutrients = Utilities.VitC(Settings.instance.VitaminCJam);
            }
            if (name.Contains("GEAR_CookedPorridgeJam"))
            {
                food.m_Nutrients = Utilities.VitC(Settings.instance.VitaminCJam * 2);
            }

            if (lower.Contains("sugar"))
            {
                FoodMaxStatBuff foodbuff = __instance.GetComponent<FoodMaxStatBuff>() ?? __instance.gameObject.AddComponent<FoodMaxStatBuff>();
                FatigueBuff fatigue = __instance.GetComponent<FatigueBuff>() ?? __instance.gameObject.AddComponent<FatigueBuff>();
                if (foodbuff == null || fatigue == null ) return;
                
                foodbuff.m_Stat = FoodMaxStatBuff.StatType.Stamina;
                foodbuff.m_CanStack = false;
                foodbuff.m_Buff = 20;
                foodbuff.m_TimeActiveHours = 0.75f;
                
                fatigue.m_InitialPercentDecrease = 7.5f;
                fatigue.m_RateOfIncreaseScale = 0.75f;
                fatigue.m_DurationHours = 0.75f;
            }
            // Leatherworks Compatibility
            if (name.Contains("GEAR_CookedBirchBarkBannock"))
            {
                ConditionOverTimeBuff condition = __instance.GetComponent<ConditionOverTimeBuff>() ?? __instance.gameObject.AddComponent<ConditionOverTimeBuff>();
                if (condition == null) return;

                if (lower.Contains("jam"))
                {
                    condition.m_ConditionIncreasePerHour = 3f;
                    condition.m_NumHours = 2;
                }
                if (lower.Contains("sugar"))
                {
                    condition.m_ConditionIncreasePerHour = 2.5f;
                    condition.m_NumHours = 1.75f;
                }
            }
            if (name.Contains("GEAR_CookedBarkBannock") && !name.Contains("Birch"))
            {
                IngestedCarryCapacityBuff ingestedCarryCapacityBuff = __instance.gameObject.GetComponent<IngestedCarryCapacityBuff>() ?? __instance.gameObject.AddComponent<IngestedCarryCapacityBuff>();
                if (ingestedCarryCapacityBuff == null) return;

                if (lower.Contains("jam"))
                {
                    ingestedCarryCapacityBuff.m_CarryCapacityBuffDurationInHours = 2f;
                    ingestedCarryCapacityBuff.m_CarryCapacityChange = ItemWeight.FromKilograms(3);
                }
                if (lower.Contains("sugar"))
                {
                    ingestedCarryCapacityBuff.m_CarryCapacityBuffDurationInHours = 1.75f;
                    ingestedCarryCapacityBuff.m_CarryCapacityChange = ItemWeight.FromKilograms(2.5f);
                }
            }
        }
    }
}

