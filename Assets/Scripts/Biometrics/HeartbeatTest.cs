using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class HeartbeatTest : MonoBehaviour
{
    void Start()
    {
        FetchBPMData();
    }
    
    private async void FetchBPMData()
    {
        await HeartbeatSingleton.Instance.FetchBPM();
        Debug.Log("BPM = " + HeartbeatSingleton.Instance.Bpm);
    }

    void Update()
    {
    }
}
