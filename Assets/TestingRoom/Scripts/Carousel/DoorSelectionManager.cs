using UnityEngine;
using TMPro;

public class DoorSelectionManager : MonoBehaviour
{
    public CarouselRotator carouselRotator;
    public Transform doorHoldersParent;
    public TMP_InputField doorIDInput;

    public void SelectDoorFromInput()
    {
        if (int.TryParse(doorIDInput.text, out int id))
        {
            SelectDoorByID(id);
        }
        else
        {
            Debug.LogWarning("Invalid Door ID input.");
        }
    }

    public void SelectDoorByID(int id)
    {
        int count = 0;
        foreach (Transform child in doorHoldersParent)
        {
            if (child.TryGetComponent<DoorIdentifier>(out var doorIDComponent) && doorIDComponent.doorID == id)
            {
                carouselRotator.RotateToDoor(id);
                StartCoroutine(WaitAndDropDoor(child));
                return;
            }
            count++;
        }

        Debug.LogWarning("Door ID not found: " + id);
    }

    private System.Collections.IEnumerator WaitAndDropDoor(Transform doorHolder)
    {
        yield return new WaitUntil(() => !carouselRotator.rotateToTarget);
        DoorChainController chainController = doorHolder.GetComponentInChildren<DoorChainController>();
        if (chainController != null)
        {
            chainController.TriggerChainExtension(true);
        }
    }
}

// TODO: check if a door was down to make it go up before bringing the new one down
