using System.Collections;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace EnhancedTimer
{
    public class RTATimerMod : MelonMod
    {
        public static MelonPreferences_Category Settings;
        public static MelonPreferences_Entry<bool> RTAEndScreen;
        public static MelonPreferences_Entry<bool> RTALevelSelect;
        public static MelonPreferences_Entry<bool> ShowIGTLabels;

        private static int _retries;

        public override void OnInitializeMelon()
        {
            Settings = MelonPreferences.CreateCategory("EnhancedTimer", "Enhanced Timer");
            RTAEndScreen = Settings.CreateEntry<bool>("RTAEndScreen", true, "RTA on End Screen", null, false, false, null, null);
            RTALevelSelect = Settings.CreateEntry<bool>("RTALevelSelect", true, "RTA in Level Select", null, false, false, null, null);
            ShowIGTLabels = Settings.CreateEntry<bool>("ShowIGTLabels", true, "Show IGT Labels", null, false, false, null, null);

            RTABestTimes.Load();

            var harmony = new HarmonyLib.Harmony("EnhancedTimer");
            harmony.PatchAll(typeof(RTATimerMod).Assembly);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (buildIndex < 1)
                return;

            _retries = 0;
            TryCreate();
        }

        private static void TryCreate()
        {
            if (RTATimer.EnsureCreated())
                return;

            if (_retries++ < 30)
                MelonCoroutines.Start(TryAgainCoroutine());
        }

        private static IEnumerator TryAgainCoroutine()
        {
            yield return null;
            TryCreate();
        }
    }

    [HarmonyPatch(typeof(GameManager), "FixedUpdate")]
    public class GameManagerFixedUpdatePatch
    {
        public static void Postfix(GameManager __instance)
        {
            if (__instance.GameStart)
            {
                if (__instance.LastTime < 10f)
                    __instance.LastTimeText.text = string.Format("{0:0.000}", __instance.LastTime);
                else
                    __instance.LastTimeText.text = string.Format("{0:0.0}", __instance.LastTime);
            }
        }
    }

    public static class RTAState
    {
        public static bool ShowRTA;
        public static double FinalSeconds;
    }

    [HarmonyPatch(typeof(GameManager), "StartGameFinish")]
    public class GameManagerStartGameFinishPatch
    {
        public static void Postfix(GameManager __instance)
        {
            RTAState.ShowRTA = true;
            RTAState.FinalSeconds = RTATimer.CurrentRTA;

            if (RTAState.FinalSeconds > 0.0)
                RTABestTimes.SetIfBetter(__instance.currentLevel, (float)RTAState.FinalSeconds);
        }
    }

    [HarmonyPatch(typeof(GameOverManager), "Update")]
    public class GameOverManagerUpdatePatch
    {
        public static void Postfix(GameOverManager __instance)
        {
            bool useRTA = RTATimerMod.RTAEndScreen.Value && RTAState.ShowRTA && RTAState.FinalSeconds > 0.0;
            bool useIGT = RTATimerMod.ShowIGTLabels.Value;

            if (useRTA)
            {
                __instance.FinalScore.text = "RTA: " + RTABestTimes.Format((float)RTAState.FinalSeconds);

                float bestRTA;
                if (RTABestTimes.TryGet(__instance.gm.currentLevel, out bestRTA))
                    __instance.BestScore.text = "Best RTA: " + RTABestTimes.Format(bestRTA);
                else if (useIGT)
                    __instance.BestScore.text = "Best IGT: " + __instance.gm.BestTime;
            }
            else if (useIGT)
            {
                __instance.FinalScore.text = "IGT: " + string.Format("{0:0.0}", __instance.gm.LastTime);
                __instance.BestScore.text = "Best IGT: " + __instance.gm.BestTime;
            }
        }
    }

    [HarmonyPatch(typeof(MenuManager), "ShowLevelInfo")]
    public class MenuManagerShowLevelInfoPatch
    {
        public static void Postfix(MenuManager __instance)
        {
            var levelTimeField = typeof(MenuManager).GetField("leveltime");
            if (levelTimeField == null)
                return;

            var levelTimeText = levelTimeField.GetValue(__instance) as TMPro.TextMeshProUGUI;
            if (levelTimeText == null)
                return;

            var lvlSelectedField = typeof(MenuManager).GetField("lvlselected",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (lvlSelectedField == null)
                return;

            int levelIndex = (int)lvlSelectedField.GetValue(__instance);

            bool useRTA = RTATimerMod.RTALevelSelect.Value;
            bool useIGT = RTATimerMod.ShowIGTLabels.Value;

            float bestRTA;
            if (useRTA && RTABestTimes.TryGet(levelIndex, out bestRTA))
            {
                float igtBest = PlayerPrefs.GetFloat("score" + levelIndex);
                levelTimeText.text = string.Format("{0:0.0}  RTA: {1}", igtBest, RTABestTimes.Format(bestRTA));
            }
            else if (useIGT && !levelTimeText.text.StartsWith("IGT:"))
            {
                levelTimeText.text = "IGT: " + levelTimeText.text;
            }
        }
    }
}