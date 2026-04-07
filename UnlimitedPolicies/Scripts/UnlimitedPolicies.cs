using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace UnlimitedPoliciesMod
{
    public class UnlimitedPoliciesMod : GameModification
    {
        Harmony _harmony;

        public UnlimitedPoliciesMod(Mod p_mod) : base(p_mod) { }

        public override void OnModInitialization(Mod p_mod)
        {
            Mod = p_mod;
            PatchGame();
        }

        public override void OnModUnloaded()
        {
            _harmony?.UnpatchAll(_harmony.Id);
        }

        void PatchGame()
        {
            _harmony = new Harmony("com.hexofsteel.unlimitedpoliciesmod");
            _harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(Player))]
    static class Patch_Player
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.AddPolicy))]
        static bool Patch_Pre_AddPolicy(Player __instance, Policy p_policy, bool p_sendRPC)
        {
            __instance.ListActivePolicies.Add(p_policy);

            if (p_policy.Type == Policies.Type.Military_scientists_1)
            {
                __instance.NumberOfAvailableResearchPoints++;
            }
            else if (p_policy.Type == Policies.Type.Military_scientists_2)
            {
                __instance.NumberOfAvailableResearchPoints += 2;
            }

            if (p_sendRPC)
                MultiplayerManager.Instance.RunRPC("RPC_SyncPlayer", "O", new object[1] { __instance });

            return false;
        }
    }

    [HarmonyPatch(typeof(PoliciesMenu))]
    [HarmonyPatch(nameof(PoliciesMenu.ClickOnPolicy))]
    static class Patch_PoliciesMenu_ClickOnPolicy
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldstr)
                {
                    string operand = codes[i].operand as string;
                    
                    if (operand == "confirmation.pickNewPolicy")
                    {
                        codes[i].operand = "confirmation.pickPolicy";
                        break;
                    }
                }
            }
            
            return codes;
        }
    }
}