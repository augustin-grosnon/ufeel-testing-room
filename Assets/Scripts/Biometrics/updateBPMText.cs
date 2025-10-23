using UnityEngine;
using TMPro;

public class updateBPMText : MonoBehaviour
{
    public TMP_Text _bpmText;
    
    private async void FetchBPMData()
    {
        await HeartbeatSingleton.Instance.FetchBPM();
        
        _bpmText.text = $"BPM: {HeartbeatSingleton.Instance.Bpm.ToString()}";
    }

    void Start()
    {
        InvokeRepeating(nameof(FetchBPMData), 0f, 1f);
    }

    void Update()
    {
        
    }
}
