using Unity.VisualScripting;
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

    public CharacterController2D player1 { get; private set; }
    public CharacterController2D player2 { get; private set; }

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
        // Find existing players in scene, sorted by name for consistent ordering
        var existingPlayers = GameObject.FindGameObjectsWithTag("Player");
        System.Array.Sort(existingPlayers, (a, b) => a.name.CompareTo(b.name));

        if (existingPlayers.Length > 0) player1 = existingPlayers[0].GetComponent<CharacterController2D>();
        if (existingPlayers.Length > 1) player2 = existingPlayers[1].GetComponent<CharacterController2D>();

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
        // Find the keyboard player
        GameObject keyboardPlayer = null;
        GameObject otherPlayer = null;

        var allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var p in allPlayers) {
            var input = p.GetComponent<PlayerInput>();
            if (input != null && input.currentControlScheme == "Keyboard&Mouse") {
                keyboardPlayer = p;
            } else {
                otherPlayer = p;
            }
        }

        // Fallback to player1 if no keyboard player found
        if (keyboardPlayer == null) keyboardPlayer = player1.gameObject;

        // Setup keyboard player with both circles
        if (keyboardPlayer != null) {
            var controller = keyboardPlayer.GetComponent<CharacterController2D>();
            if (controller != null) {
                controller.myCircle = circle1;
                controller.secondCircle = circle2;
                controller.singlePlayerMode = true;
                controller.otherBoy = otherPlayer.GetComponent<CharacterController2D>();
            }
        }

        // Disable the other player
        if (otherPlayer != null) {
            otherPlayer.SetActive(false);
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
