using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using Harmony;

namespace O21EnhancedBeacon
{
    [StaticConstructorOnStartup]
    public static class Harmony
    {
        static Harmony()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("com.neronix17.enhancedorbitalbeacon");

            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(TradeShip), "ColonyThingsWillingToBuy")]
        public static class TradeShip_ColonyThingsWillingToBuy
        {
            static void Postfix(TradeShip __instance, ref IEnumerable<Thing> __result)
            {
                if(__instance.Map.listerBuildings.allBuildingsColonist.Any(b => b.def.HasComp(typeof(Comp_EnhancedBeacon))))
                {
                    List<Thing> things = new List<Thing>();
                    List<Zone> tradeZones = __instance.Map.zoneManager.AllZones.Where(z => z is Zone_Stockpile).ToList();
                    //IEnumerable<Thing> enumerable = from thing in __instance.Map.listerThings.AllThings
                    //                                where thing.def.category == ThingCategory.Item
                    //                                && TradeUtility.PlayerSellableNow(thing)
                    //                                && !thing.Position.Fogged(thing.Map)
                    //                                && (thing.Map.areaManager.Home[thing.Position] || thing.IsInAnyStorage())
                    //                                select thing;
                    //foreach (Thing thing in enumerable)
                    //{
                    //    things.Add(thing);
                    //}
                    foreach(Zone zone in tradeZones)
                    {
                        things.AddRange(zone.AllContainedThings);
                    }
                    __result = things.AsEnumerable<Thing>();
                }
            }
        }

        [HarmonyPatch(typeof(Area_Home), "Set")]
        public static class Area_Home_Set
        {
            static void Postfix(Area_Home __instance)
            {
                foreach (Building building in __instance.Map.listerBuildings.allBuildingsColonist)
                {
                    if (building.def.HasComp(typeof(Comp_EnhancedBeacon)))
                    {
                        building.GetComp<Comp_EnhancedBeacon>().UpdateTradeRegion();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CompPower), "ConnectToTransmitter")]
        public static class CompPower_ConnectToTransmitter
        {
            static void Postfix(CompPower __instance)
            {
                if(__instance.parent != null && __instance.parent.def.HasComp(typeof(Comp_EnhancedBeacon)))
                {
                    __instance.parent.GetComp<Comp_EnhancedBeacon>().UpdateTradeRegion();
                }
            }
        }

        [HarmonyPatch(typeof(PassingShip), "CommFloatMenuOption")]
        public static class PassingShip_CommFloatMenuOption
        {
            static bool Prefix(PassingShip __instance, Building_CommsConsole console, Pawn negotiator, ref FloatMenuOption __result)
            {
                string label = "CallOnRadio".Translate(__instance.GetCallLabel());
                if (!__instance.Map.listerBuildings.allBuildingsColonist.Any(b => b.def.HasComp(typeof(Comp_EnhancedBeacon)) && b.TryGetComp<CompPowerTrader>().PowerOn))
                {
                    return true;
                }
                else
                {
                    Action action = delegate ()
                    {
                        console.GiveUseCommsJob(negotiator, __instance);
                    };
                    __result = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.InitiateSocial, null, null, 0f, null, null), negotiator, console, "ReservedBy");
                    return false;
                }
            }
        }
    }
}
