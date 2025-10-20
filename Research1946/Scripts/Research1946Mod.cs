using HarmonyLib;
using UnityEngine;

namespace Research1946Mod
{
    class Research1946Mod : GameModification
    {
        Harmony _harmony;

        public Research1946Mod(Mod p_mod) : base(p_mod)
        {
        }

        public override void OnModInitialization(Mod p_mod)
        {
            PatchGame();
        }

        public override void OnModUnloaded()
        {
            _harmony?.UnpatchAll("com.research1946");
        }

        void PatchGame()
        {
            _harmony = new Harmony("com.research1946");
            _harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(ResearchMenu))]
    [HarmonyPatch("GetResearchSetup")]
    class ResearchMenuPatch
    {
        static void Postfix(ref int o_minYear, ref int o_maxYear, ref int o_timeStep)
        {
            if (GameData.Instance.map.Date.Year >= 1936 && GameData.Instance.map.Date.Year <= 1946)
            {
                o_minYear = 1936;
                o_maxYear = 1947;
            }
        }
    }
}