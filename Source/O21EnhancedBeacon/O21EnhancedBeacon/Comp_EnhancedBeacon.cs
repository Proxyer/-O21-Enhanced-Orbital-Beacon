using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace O21EnhancedBeacon
{
    public class Comp_EnhancedBeacon : ThingComp
    {
        public CompProperties_EnhancedBeacon Props => (CompProperties_EnhancedBeacon)this.props;

        public CompPowerTrader compPowerTrader;

        public Sustainer ambientSustainer = null;

        public int pingTick = 3000;

        public EnhancedBeaconStatus beaconStatus = EnhancedBeaconStatus.Inactive;

        public enum EnhancedBeaconStatus
        {
            Active,
            Inactive
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.UpdateTradeRegion();

            compPowerTrader = this.parent.TryGetComp<CompPowerTrader>();
        }
        
        public void UpdateTradeRegion()
        {
            if (this.parent.Spawned)
            {
                if (compPowerTrader != null)
                {
                    int homeSize = this.parent.Map.areaManager.Home.TrueCount;
                    if (homeSize < 193)
                    {
                        return;
                    }
                    compPowerTrader.PowerOutput = -(compPowerTrader.Props.basePowerConsumption + (EnhancedBeaconMod.mod.settings.powerCostOverride * (homeSize - 193)));
                }
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            DealWithAudio();
        }

        public void DealWithAudio()
        {
            if(Props.pingSound != null && beaconStatus == EnhancedBeaconStatus.Active && EnhancedBeaconMod.mod.settings.pingSound)
            {
                if(pingTick <= 0)
                {
                    SoundStarter.PlayOneShot(Props.pingSound, SoundInfo.InMap(this.parent));
                    pingTick = 3000;
                }
                else
                {
                    pingTick--;
                }
            }

            if (beaconStatus == EnhancedBeaconStatus.Active && !compPowerTrader.PowerOn)
            {
                if(Props.shutdownSound != null)
                {
                    SoundStarter.PlayOneShot(Props.shutdownSound, SoundInfo.InMap(this.parent));
                }
                if(ambientSustainer != null)
                {
                    ambientSustainer.End();
                }
                beaconStatus = EnhancedBeaconStatus.Inactive;
            }
            else if (beaconStatus == EnhancedBeaconStatus.Inactive && compPowerTrader.PowerOn)
            {
                if(Props.startupSound != null)
                {
                    SoundStarter.PlayOneShot(Props.startupSound, SoundInfo.InMap(this.parent));
                }
                if(Props.ambientSound != null)
                {
                    ambientSustainer = Props.ambientSound.TrySpawnSustainer(SoundInfo.InMap(this.parent));
                }
                beaconStatus = EnhancedBeaconStatus.Active;
            }
        }
    }
}
