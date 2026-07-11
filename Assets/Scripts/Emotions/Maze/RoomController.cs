using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Room Data")]
    public RoomData Data;

    [Header("Fog References")]
    public GameObject NorthFog;
    public GameObject SouthFog;
    public GameObject EastFog;
    public GameObject WestFog;

    public void Setup(int x, int y)
    {
        Data = new RoomData
        {
            X = x,
            Z = y
        };

        DisableBorders();
    }

    public void DisableBorders()
    {
        if (Data.X == 0 && WestFog != null)
        {
            WestFog.SetActive(false);
        }

        if (Data.X == 4 && EastFog != null)
        {
            EastFog.SetActive(false);
        }

        if (Data.Z == 0 && SouthFog != null)
        {
            SouthFog.SetActive(false);
        }

        if (Data.Z == 4 && NorthFog != null)
        {
            NorthFog.SetActive(false);
        }
    }
}