using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public static bool IsCoop { get; private set; }

    [Header("Player Setup")]
    public Transform player1Spawn;
    public Transform player2Spawn;

    [Header("Circles (for single player toggle)")]
    public DebugCircle circle1;
    public DebugCircle circle2;

    private GameObject player1;
    private GameObject player2;

    private void Awake() {
        Instance = this;
        DetectCoopMode();
    }

    private void Start() {
        SetupPlayers();
    }

    private void DetectCoopMode() {
        // Check if any gamepad is connected
        IsCoop = Gamepad.current != null;
        Debug.Log(IsCoop ? "[GameManager] Co-op mode: Controller detected" : "[GameManager] Single player mode: No controller");
    }

    private void SetupPlayers() {
        // Find existing players in scene
        var existingPlayers = GameObject.FindGameObjectsWithTag("Player");
        if (existingPlayers.Length > 0) player1 = existingPlayers[0];
        if (existingPlayers.Length > 1) player2 = existingPlayers[1];

        if (IsCoop) {
            SetupCoopMode();
        } else {
            SetupSinglePlayerMode();
        }
    }

    private void SetupCoopMode() {
        // Each player gets their own circle
        if (player1 != null) {
            var controller1 = player1.GetComponent<CharacterController2D>();
            if (controller1 != null) controller1.myCircle = circle1;
        }
        if (player2 != null) {
            var controller2 = player2.GetComponent<CharacterController2D>();
            if (controller2 != null) controller2.myCircle = circle2;
        } else if (player2 == null && player1 != null) {
            // Disable P2 if no second player object
            Debug.Log("[GameManager] Co-op mode but only 1 player in scene");
        }
    }

    private void SetupSinglePlayerMode() {
        // Single player controls both circles with toggle
        if (player1 != null) {
            var controller = player1.GetComponent<CharacterController2D>();
            if (controller != null) {
                controller.myCircle = circle1;
                controller.secondCircle = circle2;
                controller.singlePlayerMode = true;
            }
        }
        // Disable second player if exists
        if (player2 != null) {
            player2.SetActive(false);
        }
    }

    private void Update() {
        // Re-check for controller connection during gameplay
        bool controllerNow = Gamepad.current != null;
        if (controllerNow != IsCoop) {
            Debug.Log(controllerNow ? "[GameManager] Controller connected!" : "[GameManager] Controller disconnected!");
            // Could trigger mode switch here if desired
        }
    }
}
