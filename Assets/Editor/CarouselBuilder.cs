using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CarouselBuilder : EditorWindow
{
    GameObject doorHolderPrefab;
    Transform carouselTop;
    int doorCount = 44;
    float radius = 19.7f;

    [System.Serializable]
    public class SpecialDoorConfig
    {
        public int doorIndex;
        public string doorName;
        public string targetSceneName;
        public Color doorColor = Color.red;
    }

    List<SpecialDoorConfig> specialDoors = new();

    [MenuItem("Tools/Build Carousel")]
    static void Init()
    {
        CarouselBuilder window = (CarouselBuilder)GetWindow(typeof(CarouselBuilder));
        window.titleContent = new GUIContent("Carousel Builder");
        window.Show();
    }

    Vector2 scrollPos;

    void OnGUI()
    {
        GUILayout.Label("Carousel Door Placer", EditorStyles.boldLabel);

        doorHolderPrefab = (GameObject)EditorGUILayout.ObjectField("Door Holder Prefab", doorHolderPrefab, typeof(GameObject), false);
        carouselTop = (Transform)EditorGUILayout.ObjectField("Door Holder Container (Parent)", carouselTop, typeof(Transform), true);
        doorCount = EditorGUILayout.IntField("Number of Doors", doorCount);
        radius = EditorGUILayout.FloatField("Radius", radius);

        EditorGUILayout.Space();
        GUILayout.Label("Special Doors", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200)); // TODO: setup window to take the appropriate size -» currently very small window by default, need to scroll to see anything
        for (int i = 0; i < specialDoors.Count; i++)
        {
            var config = specialDoors[i];
            EditorGUILayout.BeginVertical("box");
            config.doorIndex = EditorGUILayout.IntField("Door Index", config.doorIndex);
            config.doorName = EditorGUILayout.TextField("Door Name", config.doorName);
            config.targetSceneName = EditorGUILayout.TextField("Target Scene", config.targetSceneName);
            config.doorColor = EditorGUILayout.ColorField("Door Color", config.doorColor);

            if (GUILayout.Button("Remove"))
                specialDoors.RemoveAt(i);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Special Door"))
            specialDoors.Add(new SpecialDoorConfig());

        EditorGUILayout.Space();
        if (GUILayout.Button("Build Carousel"))
        {
            if (doorHolderPrefab == null || carouselTop == null)
            {
                Debug.LogError("Missing prefab or parent.");
                return;
            }

            BuildCarousel();
        }
    }

    void BuildCarousel()
    {
        for (int i = carouselTop.childCount - 1; i >= 0; i--)
        {
            if (Application.isEditor)
                DestroyImmediate(carouselTop.GetChild(i).gameObject);
        }

        float angleStep = 360f / doorCount;

        for (int i = 0; i < doorCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Sin(angle) * radius;
            float z = Mathf.Cos(angle) * radius;

            Vector3 pos = new(x, 0f, z);

            GameObject holder = (GameObject)PrefabUtility.InstantiatePrefab(doorHolderPrefab);
            holder.name = $"DoorHolder_{i}";
            holder.transform.SetParent(carouselTop, false);
            holder.transform.localPosition = pos;

            int doorID = i;
            DoorIdentifier idComp = holder.GetComponent<DoorIdentifier>() ?? holder.AddComponent<DoorIdentifier>();
            idComp.doorID = doorID;

            var special = specialDoors.Find(d => d.doorIndex == i);

            if (special != null)
            {
                idComp.doorName = special.doorName;

                if (holder.TryGetComponent<DoorReferences>(out var references))
                {
                    idComp.targetSceneName = special.targetSceneName; // TODO: integrate enum (DoorIdentifierName), avoid duplicata
                    references.trigger.targetSceneName = special.targetSceneName; // TODO: remove
                    references.controller.SetDoorColor(special.doorColor); // TODO: check for potential null value
                }
            }
        }

        Debug.Log($"Built {doorCount} doors (including {specialDoors.Count} special doors).");
    }
}

// TODO: update door color parametrization to handle specific texture and details

// TODO: auto close the menu on build complete
