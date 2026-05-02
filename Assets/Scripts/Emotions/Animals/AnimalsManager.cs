using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UFeel;
using TMPro;
using System.Threading.Tasks;

public enum AnimalType
{
    Cat,
    Tiger,
    Deer,
    Penguin,
    Spider
};


public class AnimalsManager : MonoBehaviour
{
    [Header("Scene References")]
    public Transform spawnPoint;
    public GameObject[] animalPrefabs;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;



    [Header("Game Settings")]
    private int currentLevel = 1;
    private float timer;
    private int score = 0;
    private bool isLevelActive = false;
    
    private List<AnimalType> animalsList = new List<AnimalType>();
    private GameObject currentAnimalGO;
    Dictionary<AnimalType, EmotionData.EmotionType> animalEmotions =
        new Dictionary<AnimalType, EmotionData.EmotionType>()
    {
        { AnimalType.Cat, EmotionData.EmotionType.Happiness },
        { AnimalType.Tiger, EmotionData.EmotionType.Anger },
        { AnimalType.Deer, EmotionData.EmotionType.Surprise },
        { AnimalType.Penguin, EmotionData.EmotionType.Sadness },
        { AnimalType.Spider, EmotionData.EmotionType.Fear }
    };

    async void Start()
    {
        await UFeelAPI.StartAPI();
        await Task.Delay(10000);
        
        UFeelAPI.StartEmotionDetection();
        UFeelAPI.Status();

        score = 0;
        // UFeelDebugHUD.Clear();
        // UFeelDebugHUD.Set("Current Emotion", () => {
        //     var data = UFeelAPI.GetDominantEmotion();
        //     return data.HasValue ? data.Value.ToString() : "Unknown"; 
        // });

    }

    

    void Update()
    {
        Debug.Log("Current Emotion: " + UFeelAPI.GetDominantEmotion());
        return;
    }
}


