using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BetterClan
{
    public class BetterClanManager
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ClanManager), "checkClanMembers")]
        public static void checkNormalClanMembers(ref ClanManager __instance)
        {
            if (!World.world.worldLaws.world_law_diplomacy.boolVal)
                return;
            Traverse _timestamp_last_plot = Traverse.Create(__instance).Field("_timestamp_last_plot");

            for (int index = 0; index < __instance.list.Count; ++index)
            {
                foreach (Actor pActor in __instance.list[index].units.Values)
                {
                    if (pActor.isAlive() && pActor.getAge() > 18)
                    {
                        if ( World.world.getWorldTimeElapsedSince((double)_timestamp_last_plot.GetValue()) < 10.0)
                            return;
                        List<Plot> plotsFor = World.world.plots.getPlotsFor(pActor);
                        
                        if ((plotsFor != null ? ((plotsFor.Count) > 0 ? 1 : 0) : 0) == 0 && !pActor.isKing() && !pActor.isCityLeader())
                        {
                            if (pActor.isFighting())
                                continue;
                            bool flag = false;
                            if (!flag)
                                flag = tryPlotUsurp(pActor, BetterClanPlotsLibrary.usurp);
                            if (!flag)
                                continue;
                            _timestamp_last_plot.SetValue(World.world.getCurWorldTime());
                        }
                    }
                }
            }
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ClanManager), "checkActionLeader")]
        public static void checkNormalClanMembers(Actor pActor,ref ClanManager __instance)
        {
            if (pActor.isFighting())
                return;
            bool flag = false;
            if (!flag)
                flag = tryPlotUsurp(pActor, BetterClanPlotsLibrary.usurp);
            if (!flag)
                return;
            Traverse.Create(__instance).Field("_timestamp_last_plot").SetValue(World.world.getCurWorldTime());
        }

        public static bool tryPlotUsurp(Actor pActor, PlotAsset pPlotAsset)
        {
            if (pActor == null || pPlotAsset == null) return false;
            if (!World.world.worldLaws.world_law_rebellions.boolVal || !(pActor.getInfluence() >= pPlotAsset.cost && pPlotAsset.checkInitiatorPossible(pActor) && pPlotAsset.check_launch(pActor, pActor.kingdom)))
                return false;
            Plot plot = World.world.plots.newPlot(pActor, pPlotAsset);
            plot.rememberInitiators(pActor);
            
            if (plot.checkInitiatorAndTargets())
                return true;
            Debug.Log((object) "tryPlotUsurp  is missing start requirements");
            return true;
        }
    }
}