using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

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
        DoubleCheckInputs();
        //Debug.Log("Player 1 scheme: " + player1.playerInput.currentControlScheme);
        //Debug.Log("Player 2 scheme: " + player2.playerInput.currentControlScheme);
    }

    string kbm = "Keyboard&Mouse";
    string gp = "Gamepad";
    //Gamepad lastGamePad = null;
    // gotta do some bullshit to get this working on webGL reliably sheeeesh
    private void DoubleCheckInputs() {
        if (player1.playerInput.currentControlScheme != kbm && Keyboard.current != null && Mouse.current != null) {
            player1.playerInput.SwitchCurrentControlScheme(kbm, Keyboard.current, Mouse.current);
        }
        if (player2.playerInput.currentControlScheme != gp && Gamepad.current != null) {
            player2.playerInput.SwitchCurrentControlScheme(gp, Gamepad.current);
        }
        //if (lastGamePad == null) {
        //    foreach(Gamepad pad in Gamepad.all) {
        //        if(pad != null) {
        //            if (pad.buttonSouth.wasPressedThisFrame || pad.buttonEast.wasPressedThisFrame || pad.startButton.wasPressedThisFrame){
        //                Debug.Log("switching scheme to " + pad);
        //                player2.playerInput.SwitchCurrentControlScheme(gp, pad);
        //                lastGamePad = pad;
        //                break;
        //            }
        //        }
        //    }
        //}
    }

    private void DetectCoopMode() {
        bool controllerActive = Gamepad.current != null;
        if (controllerActive && !IsCoop) {
            SetupCoopMode();
            IsCoop = true;
        } else if (!controllerActive && IsCoop) {
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
