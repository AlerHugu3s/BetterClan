using System;
using System.Collections.Generic;
using ai.behaviours;
using HarmonyLib;


namespace BetterClan{
    class BetterClanEditor{
        [HarmonyPostfix]
        [HarmonyPatch(typeof(KingdomBehCheckKing),"getKingFromRoyalClan")]
        public static void getKingFromRoyalClanByAttributes(Kingdom pKingdom,ref Actor __result)
        {
            if (string.IsNullOrEmpty(pKingdom.data.royal_clan_id))
                return;
            Clan clan = MapBox.instance.clans.get(pKingdom.data.royal_clan_id);
            if (clan == null)
                return;
            List<Actor> _actorList = new List<Actor>();
            _actorList.Clear();
            foreach (Actor pActor in clan.units.Values)
            {
                if (pActor !=  null && clan.fitToRule(pActor, pKingdom))
                    _actorList.Add(pActor);
            }
            if (_actorList.Count == 0)
                return;
            _actorList.Sort((BetterClanListSorters.sortUnitByAttributes));
            __result = _actorList[0];
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CityBehCheckLeader),"tryGetClanLeader")]
        public static void getCityLeaderByAttributes(City pCity,ref Actor __result)
        {
            Kingdom kingdom = Traverse.Create(pCity).Property("kingdom").GetValue() as Kingdom;
            Clan clan = (Clan) null;
            if (kingdom != null &&　kingdom.data.royal_clan_id != string.Empty)
                clan = MapBox.instance.clans.get(kingdom.data.royal_clan_id);
            List<Actor> actorList1 = new List<Actor>();
            List<Actor> actorList2 = new List<Actor>();
            
            if (kingdom == null)
                foreach (Actor unit in (ObjectContainer<Actor>) pCity.units)
                {
                    ActorData unitData = Traverse.Create(unit).Property("data").GetValue() as ActorData;
                    if (unitData == null) continue;
                    if (unit.isAlive() && unitData.profession != UnitProfession.Leader && unitData.profession != UnitProfession.King && unit.getClan() != null)
                    {
                        actorList2.Add(unit);
                    }
                }
            else
            {
                foreach (City city in kingdom.cities)
                {
                    foreach (Actor unit in (ObjectContainer<Actor>) city.units)
                    {
                        ActorData unitData = Traverse.Create(unit).Property("data").GetValue() as ActorData;
                        if (unit.isAlive() && unitData.profession != UnitProfession.Leader && unitData.profession != UnitProfession.King && unit.getClan() != null)
                        {
                            if (clan != null && unit.getClan() == clan)
                                actorList1.Add(unit);
                            else
                                actorList2.Add(unit);
                        }
                    }
                }
                
            }
            
            if (actorList1.Count > 0)
            {
                actorList1.Sort(new Comparison<Actor>(BetterClanListSorters.sortUnitByAttributes));
                __result = actorList1[0];
                return;
            }
            if (actorList2.Count <= 0)
                return;
            actorList2.Sort(new Comparison<Actor>(BetterClanListSorters.sortUnitByAttributes));
            __result = actorList2[0];
        }
    }
}