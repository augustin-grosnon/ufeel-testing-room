using UnityEngine;
using TMPro;

public class updateBPMText : MonoBehaviour
{
    public TMP_Text _bpmText;
        
    void Start()
    {
        int a = 0;
    }

    void Update()
    {
        await HeartbeatSingleton.Instance.FetchBPM();
        _bpmText.text = HeartbeatSingleton.Instance.Bpm.ToString();
    }
}
