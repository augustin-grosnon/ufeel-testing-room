using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private VehicleEntryTrigger vehicleEntryTrigger;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Vehicle")) return;

        var playerObject = vehicleEntryTrigger.ExitVehicle();
        if (!playerObject) return;

        SceneLoader.Instance.UnloadAdditiveScene("TestingRoom_EyeTracking");

        var testingRoomScene = SceneManager.GetSceneByName("TestingRoom");
        SceneManager.LoadScene(testingRoomScene.name, LoadSceneMode.Single);
    }
}
