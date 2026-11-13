using UnityEngine.AddressableAssets;

namespace SweetTreats
{
    internal static class Utilities
    {
        public static GameObject Liquid = Addressables.LoadAssetAsync<GameObject>("GEAR_CoffeeCup").WaitForCompletion();

        public static Texture2D Texture(string textureName)
        {
            return Main.SweetTreatsBundle.LoadAsset<Texture2D>(textureName);
        }
        public static Material LiquidMaterial(string textureName)
        {
            Material liquid = new Material(Liquid.gameObject.GetComponent<Cookable>().m_CookingPotMaterialsList[0]);
            liquid.SetTexture("_Main_texture2", Texture(textureName));
            return liquid;
        }

        public static Material JamLiquidMaterial() => LiquidMaterial("T_RosehipJam_Cooking");
        public static Material HerbalTeaLiquidMaterial() => LiquidMaterial("T_HerbalTea_Cooking");
        public static Material AcornCoffeeLiquidMaterial() => LiquidMaterial("T_AcornCoffee_Cooking");
        public static Material CoffeeLiquidMaterial() => LiquidMaterial("T_Coffee_Cooking");
        public static Material ReishiTeaLiquidMaterial() => LiquidMaterial("T_ReishiTea_Cooking");
        public static Material RoseHipTeaLiquidMaterial() => LiquidMaterial("T_RoseHipTea_Cooking");

        [HarmonyPatch(typeof(GearItem), nameof(GearItem.Awake))]
        internal class Patch_RosehipJam
        {
            internal static void Postfix(GearItem __instance)
            {
                if (__instance == null) return;

                if (__instance.name.Contains("GEAR_RosehipJam"))
                {
                    FoodItem food = __instance.gameObject.GetComponent<FoodItem>() ?? __instance.gameObject.AddComponent<FoodItem>();
                    if (food == null) return;

                    food.m_CaloriesTotal = Settings.instance.JamCalories;
                    food.m_CaloriesRemaining = Settings.instance.JamCalories;
                    food.m_Nutrients = VitC(Settings.instance.VitaminCJam);
                }
            }
        }

        public static AssetBundle LoadFromStream(string name) // Asset Bundle Loader
        {
            using (Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
            {
                MemoryStream? memory = new((int)stream.Length);
                stream!.CopyTo(memory);

                Il2CppSystem.IO.MemoryStream memoryStream = new Il2CppSystem.IO.MemoryStream(memory.ToArray());

                AssetBundle loadFromMemoryInternal = AssetBundle.LoadFromStream(memoryStream);
                return loadFromMemoryInternal;
            }
        }
        //Vitamin C setup 
        public static Il2CppSystem.Collections.Generic.List<FoodItem.Nutrient> VitC(int amount = 10)
        {
            FoodItem.Nutrient vitc = new FoodItem.Nutrient();
            vitc.m_Amount = amount;
            vitc.m_Nutrient = new Il2CppTLD.Gameplay.AssetReferenceNutrientDefinition("13a8bda1e12982e428b7551cc01b01df");
            Il2CppSystem.Collections.Generic.List<FoodItem.Nutrient> list = new Il2CppSystem.Collections.Generic.List<FoodItem.Nutrient>();
            list.Add(vitc);
            return list;
        }
    }
}



