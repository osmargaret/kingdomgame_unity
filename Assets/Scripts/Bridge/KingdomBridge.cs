using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KingdomQuest.Bridge
{
    [Serializable]
    public class RNToUnityPayload
    {
        public string campaignSlug;
        public string chapterId;
        public string missionId;
        public string presentationMode; // "CHILD" | "ADULT"
        public string locale;           // "en" | "fr" | "de"
    }

    [Serializable]
    public class RNToUnityMessage
    {
        public string type; // "LOAD_MISSION" | "SET_LOCALE" | "RESUME_CHECKPOINT"
        public RNToUnityPayload payload;
    }

    [Serializable]
    public class UnityToRNPayload
    {
        public string campaignSlug;
        public string missionId;
        public string collectibleId;
        public string scriptureRef;
        public int xpEarned;
    }

    [Serializable]
    public class UnityToRNMessage
    {
        public string type; // "MISSION_COMPLETED" | "CLUE_FOUND" | "TELEMETRY_EVENT" | "EXIT_TO_HUB"
        public UnityToRNPayload payload;
    }

    /// <summary>
    /// Attach to a persistent GameObject named "KingdomBridge" in your bootstrap scene.
    /// Matches the typed contract in kingdomgame_front/src/unity/UnityBridge.ts.
    /// </summary>
    public class KingdomBridge : MonoBehaviour
    {
        public static KingdomBridge Instance { get; private set; }
        public string CurrentCampaignSlug { get; private set; } = "the-exodus";
        public string CurrentPresentationMode { get; private set; } = "ADULT";
        public string CurrentLocale { get; private set; } = "en";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Called from React Native via UnityView.postMessage("KingdomBridge", "OnReceiveMessage", json)
        /// </summary>
        public void OnReceiveMessage(string json)
        {
            if (string.IsNullOrEmpty(json)) return;
            var msg = JsonUtility.FromJson<RNToUnityMessage>(json);
            if (msg == null || msg.payload == null) return;

            if (msg.type == "LOAD_MISSION")
            {
                CurrentCampaignSlug = msg.payload.campaignSlug;
                CurrentPresentationMode = string.IsNullOrEmpty(msg.payload.presentationMode)
                    ? "ADULT"
                    : msg.payload.presentationMode;
                CurrentLocale = string.IsNullOrEmpty(msg.payload.locale) ? "en" : msg.payload.locale;

                LoadCampaignScene(CurrentCampaignSlug);
            }
            else if (msg.type == "SET_LOCALE" && !string.IsNullOrEmpty(msg.payload.locale))
            {
                CurrentLocale = msg.payload.locale;
            }
        }

        private void LoadCampaignScene(string slug)
        {
            string sceneName = slug switch
            {
                "the-exodus" => "Scene_TheExodus",
                "the-great-ark" => "Scene_TheGreatArk",
                "the-dream-interpreter" => "Scene_JosephEgypt",
                "the-giants-challenge" => "Scene_ValleyOfElah",
                "the-queens-mission" => "Scene_QueensMission",
                _ => "Scene_TheExodus"
            };

            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.Log($"[KingdomBridge] Scene '{sceneName}' not yet added to Build Settings.");
            }
        }

        public void SendMissionCompleted(string missionId, string collectibleId, string scriptureRef, int xpEarned)
        {
            var outgoing = new UnityToRNMessage
            {
                type = "MISSION_COMPLETED",
                payload = new UnityToRNPayload
                {
                    campaignSlug = CurrentCampaignSlug,
                    missionId = missionId,
                    collectibleId = collectibleId,
                    scriptureRef = scriptureRef,
                    xpEarned = xpEarned
                }
            };
            SendToReactNative(JsonUtility.ToJson(outgoing));
        }

        public void ExitToHub()
        {
            var outgoing = new UnityToRNMessage
            {
                type = "EXIT_TO_HUB",
                payload = new UnityToRNPayload { campaignSlug = CurrentCampaignSlug }
            };
            SendToReactNative(JsonUtility.ToJson(outgoing));
        }

        private void SendToReactNative(string json)
        {
            Debug.Log($"[KingdomBridge -> RN] {json}");
            // When @azesmway/react-native-unity UnityMessageManager is imported:
            // UnityMessageManager.Instance.SendMessageToRN(json);
        }
    }
}

