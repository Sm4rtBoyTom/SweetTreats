using ExpandedTradingFramework.Framework;

namespace SweetTreats
{
	internal class Main : MelonMod
	{
		private static AssetBundle? assetBundle;
		internal static AssetBundle SweetTreatsBundle
		{
			get => assetBundle ?? throw new System.NullReferenceException(nameof(assetBundle));
		}
        public override void OnInitializeMelon()
        {
			assetBundle = Utilities.LoadFromStream("SweetTreats.Resources.Assets.sweettreatsbundle");
            Settings.instance.AddToModSettings("Sweet Treats");
			CustomTradeDefinition[] enabledTrades = CustomTrades.GetEnabledTrades();
			CustomTradeRegistry.RegisterExternalTrades(enabledTrades);
            ConsoleCommands.Register();
        }
    }
	internal static class ConsoleCommands
	{
		internal static void Register()
		{
            uConsole.RegisterCommand("Aurora", new Action(() =>
            {
                var mgr = GameManager.GetAuroraManager();
                if (mgr == null) return;

                mgr.BoostAurora(true);
            }));
            uConsole.RegisterCommand("Aurora_Stop", new Action(() =>
            {
                var mgr = GameManager.GetAuroraManager();
                if (mgr == null) return;

                mgr.BoostAurora(false);
            }));
        }
	}

}
