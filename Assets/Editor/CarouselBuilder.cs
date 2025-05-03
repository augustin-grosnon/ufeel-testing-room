using UnityEngine;
using UnityEditor;

public class CarouselBuilder : EditorWindow
{
    GameObject doorHolderPrefab;
    Transform carouselTop;
    int doorCount = 64;
    float radius = 14.8f;

    [MenuItem("Tools/Build Carousel")]
    static void Init()
    {
        CarouselBuilder window = (CarouselBuilder)GetWindow(typeof(CarouselBuilder));
        window.titleContent = new GUIContent("Carousel Builder");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Carousel Door Placer", EditorStyles.boldLabel);

        doorHolderPrefab = (GameObject)EditorGUILayout.ObjectField("Door Holder Prefab", doorHolderPrefab, typeof(GameObject), false);
        carouselTop = (Transform)EditorGUILayout.ObjectField("Door Holder Container (Parent)", carouselTop, typeof(Transform), true);
        doorCount = EditorGUILayout.IntField("Number of Doors", doorCount);
        radius = EditorGUILayout.FloatField("Radius", radius);

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
            // Quaternion rot = Quaternion.Euler(0f, -i * angleStep, 0f);

            GameObject holder = (GameObject)PrefabUtility.InstantiatePrefab(doorHolderPrefab);
            holder.name = $"DoorHolder_{i + 1}";

            var doorID = i + 1;

            if (holder.TryGetComponent<DoorIdentifier>(out var doorIDComponent))
            {
                doorIDComponent.doorID = doorID;
            }
            else
            {
                doorIDComponent = holder.AddComponent<DoorIdentifier>();
                doorIDComponent.doorID = doorID;
            }

            holder.transform.SetParent(carouselTop, false);
            holder.transform.localPosition = pos;
        }

        Debug.Log($"Placed {doorCount} doors around carousel.");
    }
}
