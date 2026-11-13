using ModSettings;

namespace SweetTreats
{
    internal class Settings : JsonModSettings
    {
        internal static Settings instance = new Settings();

        [Section("Rosehip Jam Tweaks")]
        
        [Name("Calories per Serving")]
        [Description("Adjust how many calories Rosehip Jam provides per serving. Default: 150. [Requires scene reload.]")]
        [Slider(50, 250, 9)]
        public int JamCalories = 150;

        [Name("Vitamin C per Serving")]
        [Description("Adjust how much Vitamin C Rosehip Jam provides per serving. Default: 24. [Requires scene reload.]")]
        [Slider(10, 50, 9)]
        public int VitaminCJam = 24;

        [Section("Reset Settings")]

        [Name("Reset to Default Settings")]
        [Description("Resets all settings to default. [Requires scene reload.]")]
        public bool ResetSettings = false;

        protected override void OnChange(FieldInfo field, object? oldValue, object? newValue) => RefreshFields();

        protected override void OnConfirm()
        {
            ApplyReset();
            base.OnConfirm();
            instance.ResetSettings = false;
            instance.RefreshGUI();
        }
        public static void ApplyReset()
        {
            if(instance.ResetSettings == true) 
            {
                instance.JamCalories = 150;
                instance.VitaminCJam = 24;
                instance.RefreshGUI();
            }
        }
        internal static void OnLoad()
        {
            instance.RefreshFields();
        }

        internal void RefreshFields()
        {

        }
    }
}