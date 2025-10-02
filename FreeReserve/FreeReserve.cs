using HarmonyLib;
using System;

namespace RemoveReserveUpkeepMod
{
    public class RemoveReserveUpkeepMod : GameModification
    {
        Harmony _harmony;

        public RemoveReserveUpkeepMod(Mod p_mod) : base(p_mod) { }

        public override void OnModInitialization(Mod p_mod)
        {
            mod = p_mod;
            PatchGame();
        }

        public override void OnModUnloaded()
        {
            _harmony?.UnpatchAll(_harmony.Id);
        }

        void PatchGame()
        {
            _harmony = new Harmony("com.hexofsteel.removereserveupkeep");
            _harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Unit))]
    [HarmonyPatch(nameof(Unit.GetUpkeepCost))]
    public static class RemoveReserveUpkeepPatch
    {
        [HarmonyPrefix]
        static bool Prefix(Unit __instance, bool p_getReserveCost, ref int __result)
        {
            try
            {
                if (p_getReserveCost && __instance.IsReserve)
                {
                    __result = 0;
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
    }
}