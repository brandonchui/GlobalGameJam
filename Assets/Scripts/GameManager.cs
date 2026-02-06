using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public static bool IsCoop { get; private set; }

    [Header("Circles (for single player toggle)")]
    public DebugCircle circle1;
    public DebugCircle circle2;

    public CharacterController2D player1;
    public CharacterController2D player2;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        SetupSinglePlayerMode();
        DetectCoopMode();
    }
    private void Update() {
        DetectCoopMode();
    }

    private void DetectCoopMode() {
        bool controllerActive = Gamepad.current != null;
        if (controllerActive && !IsCoop) {
            SetupCoopMode();
            IsCoop = true;
        }
        else if (!controllerActive && IsCoop) {
            SetupSinglePlayerMode();
            IsCoop = false;
        }
    }

    private void SetupCoopMode() {
        player1.gameObject.SetActive(true);
        player2.gameObject.SetActive(true);

        player1.myCircle = circle1;
        player2.myCircle = circle2;

        player1.Reset();
        player2.Reset();
    }

    private void SetupSinglePlayerMode() {
        player1.myCircle = circle1;
        player1.secondCircle = circle2;
        player1.otherBoy = player2;

        player2.gameObject.SetActive(false);
    }

}
