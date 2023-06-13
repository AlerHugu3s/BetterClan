using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace BetterClan
{
    public class BetterClanSpriteTextureLoader
    {
        private static Dictionary<string, Sprite> cached_sprites = new Dictionary<string, Sprite>();
        private static Dictionary<string, Sprite[]> cached_sprite_list = new Dictionary<string, Sprite[]>();
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SpriteTextureLoader), "getSprite")]
        public static void getSprite(string pPath, ref Sprite __result)
        {
            string[] strs = pPath.Split('_');
            if (strs[0] == "BetterClan")
            {
                Sprite sprite = (Sprite) null;
                cached_sprites.TryGetValue(strs[1], out sprite);
                if ((Object) sprite == (Object) null)
                {
                    sprite = NCMS.Utils.Sprites.LoadSprite(strs[2]);

                    cached_sprites[strs[1]] = sprite;
                    
                }
            }
        }
    }
}