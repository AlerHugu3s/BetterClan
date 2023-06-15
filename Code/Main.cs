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

            Harmony.CreateAndPatchAll(typeof(BetterClanEditor));
            Harmony.CreateAndPatchAll(typeof(BetterClanPlotsLibrary));
            Harmony.CreateAndPatchAll(typeof(BetterClanWorldLogMessageExtensions));
            Harmony.CreateAndPatchAll(typeof(BetterClanManager));
        }
    }
}