namespace SweetTreats
{
    internal class SweetButtons
    {
        private static GameObject sweetenButton;
        internal static GearItem? currentFoodItem;

        private static string buttonTextKey = "GAMEPLAY_SweetButton";

        private static int sugarUnitsDrink = 2;
        private static int jamPortionsDrink = 1;
        private static int sugarUnitsFood = 5;
        private static int jamPortionsFood = 2;
        private static int sugarCaloriesDrink = sugarUnitsDrink * 20;
        private static int sugarCaloriesFood = sugarUnitsFood * 20;

        // Temp state — set before the bar launches, consumed on success
        private static string tempOriginalName = "";
        private static float tempCondition;
        private static int tempStackCount;
        private static float tempOriginalCalories;
        private static bool tempUseJam;
        private static int tempIngredientAmount;

        private static readonly string[] AllowedFoodItems =
        {
            "GEAR_CookedPorridge",
            "GEAR_CookedBannock",
            "GEAR_CookedBannockAcorn",
            "GEAR_CookedBarkBannock",
            "GEAR_CookedBirchBarkBannock",
        };
        private static readonly string[] AllowedDrinkItems =
        {
            "GEAR_GreenTeaCup",
            "GEAR_BirchbarkTea",
            "GEAR_BurdockTea",
            "GEAR_ReishiTea",
            "GEAR_RoseHipTea",
            "GEAR_PineNeedleTea",
        };
        private static readonly string[] SugarOnlyItems =
        {
            "GEAR_CoffeeCup",
            "GEAR_AcornCoffeeCup",
        };
        private static readonly string[] JamOnlyItems =
        {
            // FoodPack by TKG
            // "GEAR_Bread",
            // "GEAR_Cookies",
        };

        private static bool IsInList(string gearName, string[] list)
        {
            foreach (string item in list)
                if (gearName.Contains(item)) return true;
            return false;
        }

        private static bool IsFoodItem(string gearName) => IsInList(gearName, AllowedFoodItems);
        private static bool IsSugarOnly(string gearName) => IsInList(gearName, SugarOnlyItems);
        private static bool IsJamOnly(string gearName) => IsInList(gearName, JamOnlyItems);

        internal static bool AllowButton(string gearName)
        {
            if (gearName.Contains("Jam") || gearName.Contains("Sugar") || gearName.Contains("Fruit")) return false;

            return IsInList(gearName, AllowedFoodItems)
                || IsInList(gearName, AllowedDrinkItems)
                || IsInList(gearName, SugarOnlyItems)
                || IsInList(gearName, JamOnlyItems);
        }

        internal static void InitializeItemDescriptionPage(ItemDescriptionPage itemDescriptionPage)
        {
            GameObject equipButton = itemDescriptionPage.m_MouseButtonEquip;
            sweetenButton = UnityEngine.Object.Instantiate<GameObject>(equipButton, equipButton.transform.parent, true);
            sweetenButton.transform.Translate(0f, -0.1f, 0);
            AddAction(sweetenButton, new System.Action(OnSweeten));
            SetSweetenActive(false);
        }

        private static void AddAction(GameObject button, System.Action action)
        {
            Il2CppSystem.Collections.Generic.List<EventDelegate> list = new Il2CppSystem.Collections.Generic.List<EventDelegate>();
            list.Add(new EventDelegate(action));
            Utils.GetComponentInChildren<UIButton>(button).onClick = list;
        }

        internal static void SetSweetenActive(bool active)
        {
            if (sweetenButton != null)
            {
                UILabel label = Utils.GetComponentInChildren<UILabel>(sweetenButton);
                if (label != null) label.text = Localization.Get(buttonTextKey);
            }
            NGUITools.SetActive(sweetenButton, active);
        }

        private static void Fail(string locKey)
        {
            HUDMessage.AddMessage(Localization.Get(locKey));
            GameAudioManager.PlayGUIError();
        }

        private static void OnSweeten()
        {
            GearItem? foodItem = currentFoodItem;
            if (foodItem == null) return;

            if (foodItem.GetNormalizedCondition() <= 0.2f)
            {
                Fail("GAMEPLAY_CannotSweetenRuinedItem");
                return;
            }

            FoodItem? food = foodItem.GetComponent<FoodItem>();
            if (food != null && food.m_CaloriesRemaining / food.m_CaloriesTotal < 0.5f)
            {
                Fail("GAMEPLAY_NotEnoughCaloriesOnItem");
                return;
            }

            bool isFood = IsFoodItem(foodItem.name);
            int sugarNeeded = isFood ? sugarUnitsFood : sugarUnitsDrink;
            int jamNeeded = isFood ? jamPortionsFood : jamPortionsDrink;

            bool hasSugar = GameManager.GetInventoryComponent().GearInInventory("GEAR_SugarA", sugarNeeded);
            bool hasJam = GameManager.GetInventoryComponent().GearInInventory("GEAR_RosehipJam", jamNeeded);

            bool useJam;
            int ingredientAmount;

            if (IsSugarOnly(foodItem.name))
            {
                if (!hasSugar) { Fail("GAMEPLAY_NoSugar"); return; }
                useJam = false;
                ingredientAmount = sugarNeeded;
            }
            else if (IsJamOnly(foodItem.name))
            {
                if (!hasJam) { Fail("GAMEPLAY_NoJam"); return; }
                useJam = true;
                ingredientAmount = jamNeeded;
            }
            else if (hasJam)
            {
                useJam = true;
                ingredientAmount = jamNeeded;
            }
            else if (hasSugar)
            {
                useJam = false;
                ingredientAmount = sugarNeeded;
            }
            else
            {
                Fail("GAMEPLAY_NotEnoughSugarOrJam");
                return;
            }

            // Store everything needed — nothing consumed yet
            tempOriginalName = foodItem.name;
            tempCondition = foodItem.CurrentHP;
            tempStackCount = foodItem.m_StackableItem != null ? foodItem.m_StackableItem.m_Units : 1;
            tempOriginalCalories = food != null ? food.m_CaloriesTotal : 0f;
            tempUseJam = useJam;
            tempIngredientAmount = ingredientAmount;

            GameAudioManager.PlayGuiConfirm();
            InterfaceManager.GetPanel<Panel_GenericProgressBar>().Launch(
                Localization.Get("GAMEPLAY_ProgressBarSweeten"), 2f, 0f, 0f,
                "", null, false, true,
                new System.Action<bool, bool, float>(OnSweetenFinished));
        }

        private static void OnSweetenFinished(bool success, bool playerCancel, float progress)
        {
            if (!success || playerCancel) return;

            if (tempUseJam)
                GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_RosehipJam", tempIngredientAmount);
            else
                GameManager.GetInventoryComponent().RemoveGearFromInventory("GEAR_SugarA", tempIngredientAmount);

            GearItem.Destroy(currentFoodItem);

            string sweetenedName = tempOriginalName + (tempUseJam ? "Jam" : "Sugar");
            GearItem? sweetened = GearItem.InstantiateGearItem(sweetenedName);
            if (sweetened == null) return;

            sweetened.m_CurrentHP = tempCondition;
            GameManager.GetPlayerManagerComponent().InstantiateItemInPlayerInventory(sweetened, tempStackCount);

            GearItem? inventoryItem = GameManager.GetInventoryComponent().GetBestGearItemWithName(sweetenedName);
            FoodItem? inventoryFood = inventoryItem?.GetComponent<FoodItem>();
            if (inventoryFood != null)
            {
                int added = tempUseJam
                    ? Settings.instance.JamCalories
                    : (IsFoodItem(tempOriginalName) ? sugarCaloriesFood : sugarCaloriesDrink);

                inventoryFood.m_CaloriesTotal = tempOriginalCalories + added;
                inventoryFood.m_CaloriesRemaining = tempOriginalCalories + added;
            }
        }
    }
    [HarmonyPatch(typeof(Panel_Inventory), nameof(Panel_Inventory.Initialize))]
    internal class SweetenInitialization
    {
        private static void Postfix(Panel_Inventory __instance)
        {
            SweetButtons.InitializeItemDescriptionPage(__instance.m_ItemDescriptionPage);
        }
    }
    [HarmonyPatch(typeof(ItemDescriptionPage), nameof(ItemDescriptionPage.UpdateGearItemDescription))]
    internal class UpdateSweetenButton
    {
        private static void Postfix(ItemDescriptionPage __instance, GearItem gi)
        {
            if (__instance != InterfaceManager.GetPanel<Panel_Inventory>()?.m_ItemDescriptionPage) return;

            SweetButtons.currentFoodItem = gi?.GetComponent<GearItem>();
            SweetButtons.SetSweetenActive(gi != null && SweetButtons.AllowButton(gi.name));
        }
    }
}