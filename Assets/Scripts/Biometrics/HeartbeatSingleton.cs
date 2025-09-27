using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public struct BPM
{
    public int bpm
}

public class HeartbeatSingleton
{
    private static HeartbeatSingleton _instance;
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string apiEndpoint = "http://localhost:4321";
    public int Bpm { get; private set; }

    public static HeartbeatSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new HeartbeatSingleton();
            }
            
            return _instance;
        }
    }

    private HeartbeatSingleton()
    {
        Bpm = 0;
    }

    public async Task FetchBPM()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(apiEndpoint);
            BPM bpmData = JsonUtility.FromJson<BPM>(response);
            Bpm = bpmData.bpm;
        }
        catch (Exception ex)
        {
            Debug.Log("Error fetching BPM data: " + ex.Message);
        }
    }
}
