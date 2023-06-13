using System;
using HarmonyLib;
using Debug = UnityEngine.Debug;

namespace BetterClan
{
    public class BetterClanPlotsLibrary : PlotsLibrary
    {
        public static PlotAsset usurp = null;
        private static bool BetterClanAllowUsurp = true;
        private static double last_usurpTimer = 0.0f;
        private static float interval_king_usurp = 2000.0f;
        private static float interval_city_usurp = 20.0f;

        public BetterClanPlotsLibrary()
        {
            init();
        }

        public override void init()
        {
            last_usurpTimer = World.world.getCreationTime();

            //篡位Asset初始化
            #region usurpPlotAsset
            PlotAsset plotAsset = new PlotAsset();
            plotAsset.id = "usurp";
            plotAsset.path_icon = "UI/Icons/plot_usurp";
            plotAsset.translation_key = "plot_usurp";
            plotAsset.description = "plot_description_usurp";
            plotAsset.check_supporters = (pActor,pPlot) => !pActor.isAlive() || pActor.kingdom.asset.mad || pActor.kingdom != pPlot.initiator_kingdom;
            plotAsset.check_launch = (PlotCheckerLaunch) ((pActor, pKingdom) =>
            {
                if (World.world.getWorldTimeElapsedSince(last_usurpTimer) <= interval_king_usurp)
                    return false;

                if (pKingdom.hasEnemies() || pActor == pKingdom.king)
                    return false;
                
                Clan pClan = pActor.getClan();
                if (pActor.getClan() == null)
                    return false;

                int usurpInfluence = pActor.getInfluence();

                int clanInfluence = 0;
                foreach (Actor member in pClan.units.Values)
                { 
                    clanInfluence += member.getInfluence();
                }

                float ageAdvantage = 1.0f;
                float age = pActor.getAge();
                BaseStats actorStats = Traverse.Create(pActor).Field("stats").GetValue() as BaseStats;
                float maxAge = actorStats[S.max_age];
                float middleAge = maxAge / 2.0f;
                ageAdvantage = 1.5f * Math.Abs(middleAge - age / middleAge);
                

                if (pClan.units.Count <= 5 || clanInfluence / 2.0 > usurpInfluence * ageAdvantage) return false;
                if (pActor.city.leader != pActor)
                    return false;
                
                if ((pActor.city.leader == pActor && (pKingdom.king == null || !pKingdom.king.isAlive() || pActor.getClan() != pKingdom.king.getClan() || pKingdom.data.timer_new_king > 0.0f)))
                    // ||(pActor.city.leader != pActor && (pActor.city.leader == null || !pActor.city.leader.isAlive() || pActor.getClan() != pActor.city.leader.getClan()))) 暂时禁用城主篡位模式
                    return false;
                
                last_usurpTimer = World.world.getCreationTime();

                return true;
            });

            plotAsset.check_initiator_city = true;
            plotAsset.check_initiator_kingdom = true;
            plotAsset.check_initiator_actor = true;
            plotAsset.action = (PlotAction) (pPlot =>
            {
                Actor pActor = pPlot.initiator_actor;
                Kingdom pKingdom = pPlot.initiator_kingdom;
                Actor oldTarget;
                if (pActor.city.leader == pActor)
                {
                    oldTarget = pKingdom.king;
                    pKingdom.clearKingData();
                    pActor.city.removeLeader();
                    if (pKingdom.capital != null && pActor.city != pKingdom.capital)
                    {
                        if (pActor.city != null)
                            pActor.city.removeCitizen(pActor);
                        pKingdom.capital.addNewUnit(pActor);
                    }
                    pKingdom.setKing(pActor);

                    WorldLogMessage message = new WorldLogMessage("king_usurp", pKingdom.name, oldTarget.getName(), pKingdom.king.getName())
                    {
                        kingdom = pKingdom,
                        color_special1 = pKingdom.getColor().getColorText(),
                        color_special2 = pKingdom.getColor().getColorText(),
                        color_special3 = pKingdom.getColor().getColorText()
                    };
                    WorldLogMessageExtensions.add(ref message);
                }
                else
                {
                    oldTarget = pActor.city.leader;
                    pActor.city.removeLeader();
                    City.makeLeader(pActor, pActor.city);
                }
                
                return true;
            });
            plotAsset.check_should_continue = (PlotCheckerDelegate)(pPlot =>
                BetterClanAllowUsurp && 
                pPlot.initiator_actor.isAlive() && 
                !pPlot.initiator_actor.kingdom.asset.mad && 
                !pPlot.initiator_kingdom.hasEnemies() && 
                pPlot.initiator_actor.getClan().units.Count > 5 &&
                (Object)pPlot.initiator_kingdom.king != (Object)pPlot.initiator_actor &&
                ((pPlot.initiator_city.leader == pPlot.initiator_actor && (pPlot.initiator_kingdom.king != null ||
                                                                             pPlot.initiator_kingdom.king.isAlive() ||
                                                                             pPlot.initiator_actor.getClan() ==
                                                                             pPlot.initiator_kingdom.king.getClan() ||
                                                                             pPlot.initiator_kingdom.data
                                                                                 .timer_new_king > 0.0f))
                || (pPlot.initiator_city.leader != pPlot.initiator_actor && (pPlot.initiator_city.leader != null ||
                                                                             pPlot.initiator_city.leader.isAlive() ||
                                                                             pPlot.initiator_actor.getClan() ==
                                                                             pPlot.initiator_city.leader.getClan()))));
            plotAsset.plot_power = (PlotActorDelegate) (pActor =>
            {
                return 10000;
            });
            plotAsset.cost = 100;
            PlotAsset pAsset = plotAsset;
            t = plotAsset;
            usurp = AssetManager.plots_library.add(pAsset);
            #endregion
            
        }
    }
}