using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CircleManager : MonoBehaviour {
    public List<DebugCircle> circles;
    public TextMeshProUGUI heightText;

    CharacterController2D[] players;

    public static CircleManager instance;

    public void Awake() {
        instance = this;
    }

    public float maxHeightBaby { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Application.targetFrameRate = 240;
        players = FindObjectsByType<CharacterController2D>(FindObjectsSortMode.None);
        maxHeightBaby = Mathf.NegativeInfinity;
    }

    // Update is called once per frame
    void Update() {
        foreach (var p in players) {
            maxHeightBaby = Mathf.Max(maxHeightBaby, p.transform.position.y);
        }
        heightText.SetText(maxHeightBaby.ToString("F1") + "m");

        var centers = new List<Vector4>();
        var radii = new List<float>();
        for (int i = 0; i < circles.Count; i++) {
            centers.Add(circles[i].transform.position);
            radii.Add(circles[i].radius);
        }

        Shader.SetGlobalInt("_CircleCount", circles.Count);
        if (centers.Count > 0) {
            Shader.SetGlobalVectorArray("_CircleCenters", centers);
            Shader.SetGlobalFloatArray("_CircleRadii", radii);
        }

    }
}
