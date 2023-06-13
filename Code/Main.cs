using System;
using HarmonyLib;
using NCMS;
using UnityEngine;
using ReflectionUtility;

namespace BetterClan{
    [ModEntry]
    class Main : MonoBehaviour{
        void Awake()
        {
            new BetterClanPlotsLibrary();
            BetterClanLocalizeManager.Init();
            
            Harmony.CreateAndPatchAll(typeof(BetterClanEditor));
            Harmony.CreateAndPatchAll(typeof(BetterClanPlotsLibrary));
            Harmony.CreateAndPatchAll(typeof(BetterClanWorldLogMessageExtensions));
            Harmony.CreateAndPatchAll(typeof(BetterClanLocalizeManager));
            Harmony.CreateAndPatchAll(typeof(BetterClanManager));
        }

        //DebugTest
        
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.L))
        //     {
        //         WorldLogMessage message = new WorldLogMessage("king_usurp", "测试王国", "旧国王", "新国王")
        //         {
        //             color_special1 = Color.red,
        //             color_special2 = Color.red,
        //             color_special3 = Color.red
        //         };
        //         WorldLogMessageExtensions.add(ref message);
        //     }
        // }
    }
}