using BoneLib;
using BoneLib.BoneMenu;
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
        public static Page page;
        public static MelonPreferences_Category category;
        public static MelonPreferences_Entry<bool> modEnabled;
        public override void OnInitializeMelon() {
            LoggerInstance.Msg("Initialized HexaBack.");
            CreatePrefs();
            CreateBoneMenu();
            Hooking.OnLevelLoaded += (_) => MakeHeptabodyHexabody(!modEnabled.Value);
        }

        private static void CreatePrefs() {
            category = MelonPreferences.CreateCategory("Hexaback");
            category.SetFilePath("UserData/freakycheesy.cfg", true);
            modEnabled = category.CreateEntry("Enabled", true);
        }

        private void CreateBoneMenu() {          
            page = Page.Root.CreatePage("Hexaback", Color.green);
            page.CreateBool("Toggle Mod (Works on Level Load)", Color.green, modEnabled.Value, (a) => {
                modEnabled.Value = a;
                MakeHeptabodyHexabody(!modEnabled.Value);
                Save();
            });
        }

        private static void Save() {
            category.SaveToFile();
            MelonPreferences.Save();
        }

        public override void OnApplicationQuit() {
            MelonPreferences.Save();
            base.OnApplicationQuit();
        }

        private static void MakeHeptabodyHexabody(bool revert) {
            var rig = Player.PhysicsRig;
            var lShoulder = rig.m_shoulderLf.GetComponent<Collider>();
            var rShoulder = rig.m_shoulderRt.GetComponent<Collider>();
            //var lElbow = rig.m_elbowLf.GetComponent<Collider>();
            //var rElbow = rig.m_elbowRt.GetComponent<Collider>();       
            Hand[] hands = [
                    rig.leftHand,
                    rig.rightHand,
                ];
            lShoulder.isTrigger = !revert;
            rShoulder.isTrigger = !revert;
            //lElbow.isTrigger = !revert;
            //rElbow.isTrigger = !revert;
            float jumpVelocity = rig.manager.remapHeptaRig.jumpVelocity;
            rig.manager.remapHeptaRig.jumpVelocity = revert ? jumpVelocity * jumpVelocityDivider : jumpVelocity / jumpVelocityDivider;
            foreach (var hand in hands) {
                float _gripMult = hand.physHand.gripMult;
                _gripMult = revert ? _gripMult / gripMultiplier : _gripMult * gripMultiplier;
                hand.physHand.gripMult = _gripMult;
            }
        }
        [HarmonyPatch(typeof(PhysicsRig))]
        public static class ClimbPatch {
            [HarmonyPatch(nameof(PhysicsRig.CheckClimb)), HarmonyPrefix]
            public static bool CheckDangle(PhysicsRig __instance, ref bool __result) {
                if (modEnabled.Value) {
                    __result = false;
                    return false;
                }
                else {
                    return __result;
                }
            }
        }
        private const float jumpVelocityDivider = 1.5f;
        private const float gripMultiplier = 0.1f;

        /*
        private static void ResetToHeptaRig(Collider lShoulder, Collider rShoulder, Collider lElbow, Collider rElbow, Hand[] hands) {
            lShoulder.isTrigger = DefaultPhysicsRigInfo.colliderTrigger;
            rShoulder.isTrigger = DefaultPhysicsRigInfo.colliderTrigger;
            lElbow.isTrigger = DefaultPhysicsRigInfo.colliderTrigger;
            rElbow.isTrigger = DefaultPhysicsRigInfo.colliderTrigger;
            foreach (var hand in hands) {
                hand.physHand.armInternalMult = DefaultPhysicsRigInfo.armInternalMult;
                //hand.physHand._forceMultiplier = DefaultPhysicsRigInfo.forceMultiplier;
            }
        }

        public static class DefaultPhysicsRigInfo {
            public static bool colliderTrigger = false;
            public static float jumpVelocity = default;
            public static float armInternalMult = default;
            public static float forceMultiplier = default;
            public static bool Setup {
                get; private set;
            } = false;
            public static void SetupRigInfo(PhysicsRig rig){
                if (Setup)
                    return;
                colliderTrigger = false;
                jumpVelocity = rig.manager.remapHeptaRig.jumpVelocity;
                Hand[] hands = [
                    rig.leftHand,
                    rig.rightHand,
                ];
                foreach (var hand in hands) {
                    armInternalMult = hand.physHand.armInternalMult;
                    forceMultiplier = hand.physHand._forceMultiplier;
                }
                Setup = true;
            }
        }
                */
    }
}