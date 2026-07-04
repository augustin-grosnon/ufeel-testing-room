using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UFeel;
using UnityEngine;
using UnityEngine.UI;

public enum AnimalType
{
    Cat,
    Tiger,
    Deer,
    Penguin,
    Spider
}

public class AnimalsManager : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds1_5 = new(1.5f);
    private static readonly WaitForSeconds _waitForSeconds2 = new(2f);
    private static readonly WaitForSeconds _waitForSeconds5 = new(5f);

    [Header("Scene References")]
    public Transform spawnPoint;
    public GameObject[] animalPrefabs;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    [Header("Game Settings")]
    private int currentLevel = 1;
    private float timer;
    private int score;
    private bool isLevelActive;

    private List<AnimalType> animalsList = new();
    private GameObject currentAnimalGO;
    private readonly Dictionary<AnimalType, EmotionData.EmotionType> animalEmotions =
        new()
    {
        { AnimalType.Cat, EmotionData.EmotionType.Happiness },
        { AnimalType.Tiger, EmotionData.EmotionType.Anger },
        { AnimalType.Deer, EmotionData.EmotionType.Surprise },
        { AnimalType.Penguin, EmotionData.EmotionType.Sadness },
        { AnimalType.Spider, EmotionData.EmotionType.Fear }
    };

    public Slider emotionProgressBar;
    private Image barFillImage;

    [Header("Progress Settings")]
    private float currentProgress = 0f;
    private const float maxProgressDuration = 2.5f;
    private bool isTrackingEmotion = false;
    private AnimalType currentTargetAnimal;

    private async void Start()
    {
        // UFeelDebugHUD.UseDefaultDebugHUD = false; // decoment when merge with main
        // UFeelDebugHUD.Clear();
        // UFeelDebugHUD.Set("Current Emotion", () => {
        //     var data = UFeelAPI.GetDominantEmotion();
        //     return data.HasValue ? data.Value.ToString() : "Unknown";
        // });

        await UFeelAPI.StartAPI();
        await Task.Delay(10000);

        UFeelAPI.StartEmotionDetection();
        UFeelAPI.Status();
        if (emotionProgressBar != null)
        {
            barFillImage = emotionProgressBar.fillRect.GetComponent<Image>();
        }

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (currentLevel <= 5)
        {
            yield return StartCoroutine(InitLevel(currentLevel));
            yield return StartCoroutine(PlayLevel(currentLevel));

            if (timer <= 0 && animalsList.Count > 0)
            {
                instructionText.text = "GAME OVER...";
                yield return _waitForSeconds5;
                PauseMenu.GoToLobby();
                yield break;
            }

            currentLevel++;
        }

        instructionText.text = "CONGRATULATIONS! YOU ARE A MASTER OF EMOTIONS!";

        yield return _waitForSeconds5;
        PauseMenu.GoToLobby();
    }

    private IEnumerator InitLevel(int level)
    {
        isLevelActive = false;
        string newAnimalInfo = string.Empty;

        switch (level)
        {
            case 1: newAnimalInfo = "Cat -> Happiness"; break;
            case 2: newAnimalInfo = "Tiger -> Anger"; break;
            case 3: newAnimalInfo = "Deer -> Surprise"; break;
            case 4: newAnimalInfo = "Penguin -> Sadness"; break;
            case 5: newAnimalInfo = "Spider -> Fear"; break;
        }

        instructionText.text = $"LEVEL {level}\n\n{newAnimalInfo}";
        yield return _waitForSeconds5;

        timer = GetLevelTime(level);
        animalsList = GenerateAnimals(level);
        isLevelActive = true;
    }

    private IEnumerator PlayLevel(int level)
    {
        instructionText.text = string.Empty;
        SpawnAnimal();

        while (isLevelActive && timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = $"Time: 0:{Mathf.CeilToInt(timer)}";
            if (animalsList.Count == 0 && currentAnimalGO == null)
            {
                isLevelActive = false;
                instructionText.text = "LEVEL COMPLETED !";
                yield return _waitForSeconds2;
            }
            yield return null;
        }
    }

    private static List<AnimalType> GenerateAnimals(int level)
    {
        List<AnimalType> pool = new() { AnimalType.Cat };
        if (level >= 2) pool.Add(AnimalType.Tiger);
        if (level >= 3) pool.Add(AnimalType.Deer);
        if (level >= 4) pool.Add(AnimalType.Penguin);
        if (level >= 5) pool.Add(AnimalType.Spider);

        List<AnimalType> result = new();
        int totalTarget = GetAnimalNumber(level);
        const int minPerAnimal = 2;

        // adding at least 2 of each type in the pool
        foreach (AnimalType type in pool)
        {
            for (int i = 0; i < minPerAnimal; i++)
            {
                if (result.Count < totalTarget)
                {
                    result.Add(type);
                }
            }
        }

        while (result.Count < totalTarget)
        {
            result.Add(pool[Random.Range(0, pool.Count)]);
        }

        Shuffle(result);
        return result;
    }

    // Fisher-Yates shuffle algorithm to randomize the order of animals in the list
    private static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void SpawnAnimal()
    {
        if (animalsList.Count == 0)
        {
            Debug.Log("LEVEL COMPLETE");
            isLevelActive = true;
            return;
        }

        AnimalType animal = animalsList[0];

        Quaternion rotation180 = Quaternion.Euler(0f, 180f, 0f);

        currentAnimalGO = Instantiate(animalPrefabs[(int)animal], spawnPoint.position, rotation180);
        Debug.Log($"Emotion: {animalEmotions[animal]} for {animal}");

        CheckEmotion(animal);
    }

    private void UpdateScore()
    {
        score++;
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    // void CheckEmotion(AnimalType animal)
    // {

    //     UFeelAPI.TriggerActionOnEmotionOnce(animalEmotions[animal], () =>
    //     {
    //         if (!isLevelActive) return;

    //         Debug.Log("CORRECT EMOTION!");
    //         UpdateScore();

    //         Destroy(currentAnimalGO);
    //         animalsList.RemoveAt(0);

    //         StartCoroutine(WaitAndSpawn());
    //     });
    // }

    private void CheckEmotion(AnimalType animal)
    {
        currentTargetAnimal = animal;
        isTrackingEmotion = true;
        currentProgress = 0f;
        if (emotionProgressBar != null) emotionProgressBar.value = 0f;
    }

    private IEnumerator WaitAndSpawn()
    {
        yield return _waitForSeconds1_5;
        SpawnAnimal();
    }

    private static float GetLevelTime(int level)
    {
        if (level == 1) return 30f;
        if (level == 2) return 40f;
        if (level == 3) return 50f;
        if (level == 4) return 60f;
        return 75f;
    }

    private static int GetAnimalNumber(int level)
    {
        return level switch
        {
            1 => 3,
            2 => 5,
            3 => 7,
            4 => 9,
            5 => 12,
            _ => 3,
        };
    }

    private void Update()
    {
        if (!isLevelActive || !isTrackingEmotion || currentAnimalGO == null) return;

        EmotionData.EmotionType? dominantEmotion = UFeelAPI.GetDominantEmotion();
        EmotionData.EmotionType targetEmotion = animalEmotions[currentTargetAnimal];

        if (dominantEmotion.HasValue && dominantEmotion.Value == targetEmotion)
        {
            currentProgress += Time.deltaTime / maxProgressDuration;
            if (barFillImage != null) barFillImage.color = Color.green;
        }
        else
        {
            currentProgress -= Time.deltaTime / maxProgressDuration / 3;
            if (barFillImage != null) barFillImage.color = Color.red;
        }

        currentProgress = Mathf.Clamp01(currentProgress);

        if (emotionProgressBar != null) emotionProgressBar.value = currentProgress;

        if (currentProgress >= 1f)
        {
            isTrackingEmotion = false;
            OnAnimalSuccess();
        }
    }

    private void OnAnimalSuccess()
    {
        Debug.Log("CORRECT EMOTION MAINTAINED FOR 5S!");
        UpdateScore();

        Destroy(currentAnimalGO);
        animalsList.RemoveAt(0);

        StartCoroutine(WaitAndSpawn());
    }

    // void OnDestroy()    // decomment when merge with main
    // {
    //     UFeelDebugHUD.UseDefaultDebugHUD = true;
    // }
}

/*
 TODO:
 - Add some music for the whole game play or animal sounds for each animal ?
 - Modifie debug HUB -> Just display the dominant emotion
 - Modfie the algo of spawning animals so 2 same animals don't spawn one after the other (ex: cat, cat, tiger, deer, tiger, deer, penguin, spider, spider)
 - Add ranking table at the end
*/
