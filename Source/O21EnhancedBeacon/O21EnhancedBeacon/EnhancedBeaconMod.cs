using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21EnhancedBeacon
{
    public class EnhancedBeaconMod : Mod
    {
        public EnhancedBeaconSettings settings;
        public static EnhancedBeaconMod mod;
        
        public EnhancedBeaconMod(ModContentPack content) : base(content)
        {
            Log.Message(":: O21 Enhanced Orbital Beacon - Version 1.0 ::");
            settings = GetSettings<EnhancedBeaconSettings>();
            mod = this;
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);
            listing.Label("Energy Cost Per Home Tile:");
            listing.Label("Min: 0, Max: 10, Current: " + settings.powerCostOverride.ToString());
            settings.powerCostOverride = (int)listing.Slider(settings.powerCostOverride, 0, 10);
            listing.CheckboxLabeled("Beacon Ping Sound", ref settings.pingSound, "Enable/Disable ping sound effect, plays occasionally, but could undestandably be annoying for some people.");
            listing.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Enhanced Orbital Beacon";
        }
    }

    public class EnhancedBeaconSettings : ModSettings
    {
        public int powerCostOverride = 2;

        public bool pingSound = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref powerCostOverride, "powerCostOverride", 2);
            Scribe_Values.Look<bool>(ref pingSound, "pingSound", false);
        }
    }
}
