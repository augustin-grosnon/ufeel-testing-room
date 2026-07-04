using UnityEngine;
// using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private VehicleEntryTrigger vehicleEntryTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Vehicle")) return;

        GameObject playerObject = vehicleEntryTrigger.ExitVehicle();
        if (!playerObject) return;

        PauseMenu.GoToLobby();

        // SceneLoader.Instance.UnloadAdditiveScene("TestingRoom_EyeTracking");

        // Scene testingRoomScene = SceneManager.GetSceneByName("TestingRoom");
        // SceneManager.LoadScene(testingRoomScene.name, LoadSceneMode.Single);
    }
}
