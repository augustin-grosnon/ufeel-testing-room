using UFeel;
using UnityEngine;

public class DoorEyeController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DoorSelectionManager doorSelectionManager;

    [Header("Settings")]
    [SerializeField] private float doubleBlinkWindow = 0.5f;

    private bool previousBlink;

    private enum GazeDirection
    {
        Center,
        Left,
        Right
    }

    private GazeDirection previousGazeDirection = GazeDirection.Center;

    private float lastBlinkTime = -10f;

    private bool commandsEnabled = false;

    private void Awake()
    {
        if (doorSelectionManager == null)
        {
            Debug.LogError("DoorSelectionManager reference missing.");
            enabled = false;
            return;
        }

        UFeelAPI.StartEyeTrackingDetection();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ToggleCommands();
        }

        if (!commandsEnabled)
        {
            return;
        }

        EyeTrackingData? currentDirections = UFeelAPI.CurrentDirections;
        bool? blinkStatus = UFeelAPI.BlinkStatus;

        if (currentDirections is not EyeTrackingData directions)
            return;

        ProcessInput(directions, blinkStatus == true);
    }

    private void ToggleCommands()
    {
        commandsEnabled = !commandsEnabled;
    }

    private void ProcessInput(EyeTrackingData directions, bool blink)
    {
        bool blinkPressed = blink && !previousBlink;
        previousBlink = blink;
        bool doubleBlink = DetectDoubleBlink(blinkPressed);

        (bool leftGesture, bool rightGesture) = ProcessGaze(directions);

        if (leftGesture)
        {
            doorSelectionManager.SelectPreviousDoor();
        }

        if (rightGesture)
        {
            doorSelectionManager.SelectNextDoor();
        }

        if (doubleBlink)
        {
            doorSelectionManager.ConfirmCurrentSelection();
        }
    }

    private static GazeDirection GetGazeDirection(EyeTrackingData directions)
    {
        if (directions.left && !directions.right)
            return GazeDirection.Left;

        if (directions.right && !directions.left)
            return GazeDirection.Right;

        return GazeDirection.Center;
    }

    private (bool Left, bool Right) ProcessGaze(EyeTrackingData directions)
    {
        GazeDirection currentDirection = GetGazeDirection(directions);

        bool leftGesture =
            previousGazeDirection == GazeDirection.Left &&
            currentDirection == GazeDirection.Center;

        bool rightGesture =
            previousGazeDirection == GazeDirection.Right &&
            currentDirection == GazeDirection.Center;

        previousGazeDirection = currentDirection;

        return (leftGesture, rightGesture);
    }

    private bool DetectDoubleBlink(bool blinkPressed)
    {
        if (!blinkPressed)
            return false;

        if (Time.time - lastBlinkTime <= doubleBlinkWindow)
        {
            lastBlinkTime = -10f;
            return true;
        }

        lastBlinkTime = Time.time;
        return false;
    }
}
