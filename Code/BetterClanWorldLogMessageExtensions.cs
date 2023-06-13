using HarmonyLib;
using UnityEngine;

namespace BetterClan
{
    public static class BetterClanWorldLogMessageExtensions
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WorldLogMessageExtensions),"getFormatedText")]
        public static void getExtendFormatedText(ref WorldLogMessage pMessage,UnityEngine.UI.Text pTextField, bool pColorField, bool pColorTags,ref string __result)
        {
            string formatedText = LocalizedTextManager.getText(pMessage.text);
            switch (pMessage.text)
            {
                case "king_usurp":
                    formatedText = NCMS.Utils.Localization.Get("king_usurp").Replace("$kingdom$", "<color=" + Toolbox.colorToHex((Color32) pMessage.color_special1) + ">" + pMessage.special1 + "</color>")
                        .Replace("$oldKing$", "<color=" + Toolbox.colorToHex((Color32) pMessage.color_special2) + ">" + pMessage.special2 + "</color>").Replace("$newKing$", "<color=" + Toolbox.colorToHex((Color32) pMessage.color_special3) + ">" + pMessage.special3 + "</color>");
                    pMessage.icon = "iconKings";
                    __result = formatedText;
                    break;
            }
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlotsLibrary),"getDescriptionGeneric")]
        public static bool getDescriptionGeneric(Plot pPlot,ref string __result)
        {
            string desKey = pPlot.getAsset().description;
            if (desKey == "plot_description_usurp")
            {
                string desVal = NCMS.Utils.Localization.Get("plot_description_usurp");
                __result = desVal.Replace("$initiator_actor$", pPlot.initiator_actor.getName())
                    .Replace("$initiator_kingdom$", pPlot.initiator_kingdom.name);
                return false;
            }
            return true;
        }
    }
}