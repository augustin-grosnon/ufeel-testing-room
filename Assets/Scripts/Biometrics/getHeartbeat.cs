using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Biometrics
{
    public string bpm;
}

public class getHeartbeat : MonoBehaviour
{
    private string apiEndpoint = "http://localhost:4321";
    
    void Start()
    {
        InvokeRepeating("GetHeartbeatWrapper", 0f, 1f);
    }

    void GetHeartbeatWrapper()
    {
        StartCoroutine(GetHeartbeat());
    }

    IEnumerator GetHeartbeat()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiEndpoint))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("GOT FETCH ERROR = " + request.error);
            }
            else
            {
                Biometrics bio = JsonUtility.FromJson<Biometrics>(request.downloadHandler.text);
            
                Debug.Log($"BPM = {bio.bpm}");
            }
        }
    }

    void Update()
    {
        
    }
}
