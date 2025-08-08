using BoneLib;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(HexaBack.Core), "HexaBack", "1.0.0", "freakycheesy", null)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace HexaBack
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized HexaBack.");
            Hooking.OnLevelLoaded += Hooking_OnMarrowGameStarted;
        }

        private void Hooking_OnMarrowGameStarted(LevelInfo info) {
            MakeHeptabodyHexabody(Player.PhysicsRig);
        }
        private static void MakeHeptabodyHexabody(PhysicsRig rig) {
            var lShoulder = rig.m_shoulderLf.GetComponent<Collider>();
            var rShoulder = rig.m_shoulderRt.GetComponent<Collider>();
            var lElbow = rig.m_elbowLf.GetComponent<Collider>();
            var rElbow = rig.m_elbowRt.GetComponent<Collider>();
            lShoulder.isTrigger = true;
            rShoulder.isTrigger = true;
            lElbow.isTrigger = true;
            rElbow.isTrigger = true;
            rig.manager.remapHeptaRig.jumpVelocity /= 1.3f;
            Hand[] hands = new Hand[] {
                    rig.leftHand,
                    rig.rightHand,
                };
            foreach (var hand in hands) {
                float multiplier = 2f;
                hand.physHand.armInternalMult *= multiplier;
                hand.physHand._forceMultiplier *= multiplier - 0.5f;
            }
        }
    }
}