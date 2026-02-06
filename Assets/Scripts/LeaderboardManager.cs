using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeaderboardManager : MonoBehaviour {
    [Header("UI References")]
    public TextMeshProUGUI highScoresText;
    public TMP_InputField nameInputField;

    public TextMeshProUGUI currentScoreText;

    private float score;


    [Header("Settings")]
    public int maxEntries = 5;

    private const string LEADERBOARD_KEY = "Leaderboard";

    [Serializable]
    public class LeaderboardEntry {
        public string name;
        public float score;
    }

    [Serializable]
    public class LeaderboardData {
        public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
    }

    private LeaderboardData leaderboardData;

    private void Start() {
        LoadLeaderboard();
        DisplayLeaderboard();

        if (nameInputField != null) {
            nameInputField.onSubmit.AddListener(OnNameSubmit);
        }
    }

    private void OnDestroy() {
        if (nameInputField != null) {
            nameInputField.onSubmit.RemoveListener(OnNameSubmit);
        }
    }

    private void Update() {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.f1Key.wasPressedThisFrame) {
            Debug.Log("[Leaderboard] Cleared leaderboard (debug)");
            ClearLeaderboard();
        }
#endif
    }

    private void OnNameSubmit(string playerName) {
        if (string.IsNullOrWhiteSpace(playerName)) return;

        if (score > 0) {
            AddScore(playerName, score);
            nameInputField.text = "";
            score = 0;
        }

        // Clear pending score after submission
        PlayerPrefs.DeleteKey("PendingScore");
    }

    public void AddScore(string playerName, float score) {
        LeaderboardEntry newEntry = new LeaderboardEntry {
            name = playerName,
            score = score
        };

        leaderboardData.entries.Add(newEntry);
        leaderboardData.entries.Sort((a, b) => b.score.CompareTo(a.score));

        if (leaderboardData.entries.Count > maxEntries) {
            leaderboardData.entries.RemoveAt(leaderboardData.entries.Count - 1);
        }

        SaveLeaderboard();
        DisplayLeaderboard();
    }

    private void LoadLeaderboard() {
        string json = PlayerPrefs.GetString(LEADERBOARD_KEY, "");

        score = PlayerPrefs.GetFloat("PendingScore", 0f);
        currentScoreText.text = score.ToString("F1") + "m";

        if (string.IsNullOrEmpty(json)) {
            leaderboardData = new LeaderboardData();
        }
        else {
            leaderboardData = JsonUtility.FromJson<LeaderboardData>(json);
            if (leaderboardData == null) {
                leaderboardData = new LeaderboardData();
            }
        }
    }

    private void SaveLeaderboard() {
        string json = JsonUtility.ToJson(leaderboardData);
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
    }

    private void DisplayLeaderboard() {
        if (highScoresText == null) return;

        if (leaderboardData.entries.Count == 0) {
            highScoresText.text = "No scores yet!";
            return;
        }

        string display = "";
        for (int i = 0; i < leaderboardData.entries.Count; i++) {
            var entry = leaderboardData.entries[i];
            display += $"{i + 1}. {entry.name} - {entry.score:F1}m\n";
        }

        highScoresText.text = display.TrimEnd('\n');
    }

    // Call this to clear leaderboard (for testing)
    public void ClearLeaderboard() {
        PlayerPrefs.DeleteKey(LEADERBOARD_KEY);
        leaderboardData = new LeaderboardData();
        DisplayLeaderboard();
    }
}
