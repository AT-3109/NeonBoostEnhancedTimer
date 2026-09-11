using System.Collections;
using HarmonyLib;
using MelonLoader;

namespace EnhancedTimer
{
    public class RTATimerMod : MelonMod
    {
        private static int _retries;

        public override void OnInitializeMelon()
        {
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
        public static void Postfix()
        {
            RTAState.ShowRTA = true;
            RTAState.FinalSeconds = RTATimer.CurrentRTA;
        }
    }

    [HarmonyPatch(typeof(GameOverManager), "Update")]
    public class GameOverManagerUpdatePatch
    {
        public static void Postfix(GameOverManager __instance)
        {
            if (!RTAState.ShowRTA || RTAState.FinalSeconds <= 0.0)
                return;

            var ts = System.TimeSpan.FromSeconds(RTAState.FinalSeconds);
            string formatted;
            if (ts.TotalHours >= 1.0)
                formatted = string.Format("RTA: {0}:{1:00}:{2:00}.{3:000}",
                    (int)ts.TotalHours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            else
                formatted = string.Format("RTA: {0}:{1:00}.{2:000}",
                    ts.Minutes, ts.Seconds, ts.Milliseconds);

            __instance.FinalScore.text = formatted;
        }
    }
}