using System;
using HarmonyLib;

namespace BetterClan
{
    public class BetterClanListSorters
    {
        public static int sortUnitByAttributes(Actor pActor1, Actor pActor2)
        {
            BaseStats actorStats1 = pActor1.stats;
            BaseStats actorStats2 = pActor2.stats;
            float totalAttributes1 = actorStats1 != null? actorStats1[S.intelligence] + actorStats1[S.diplomacy] + actorStats1[S.stewardship] : 0 ;
            float totalAttributes2 = actorStats2 != null? actorStats2[S.intelligence] + actorStats2[S.diplomacy] + actorStats2[S.stewardship] : 0 ;
            return totalAttributes2.CompareTo(totalAttributes1);
        }
    }
}