using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class DoorSelectionManager : MonoBehaviour
{
    public CarouselRotator carouselRotator;
    public Transform doorHoldersParent;
    public TMP_InputField doorIDInput;
    public TMP_Dropdown doorNameDropdown;

    private string currentDropdownSelection = "";
    private DoorChainController savedChainControllers = null;

    void Awake()
    {
        SetupDoorDropdownOptions();
    }

    public void SetupDoorDropdownOptions()
    {
        List<string> options = new() { "" };

        foreach (Transform child in doorHoldersParent)
        {
            if (child.TryGetComponent<DoorIdentifier>(out var doorIDComponent) && doorIDComponent.doorName != "")
            {
                string doorName = doorIDComponent.doorName;
                options.Add(doorName);
            }
        }

        doorNameDropdown.ClearOptions();
        doorNameDropdown.AddOptions(options);

        doorNameDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    private void OnDropdownValueChanged(int index)
    {
        currentDropdownSelection = doorNameDropdown.options[index].text;
    }

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

    public void SelectDoor()
    {
        if (currentDropdownSelection != "")
        {
            SelectDoorFromDropdown();
        }
        else
        {
            SelectDoorFromInput();
        }
    }

    public void SelectDoorFromDropdown()
    {
        // TODO: check if we can setup enum instead of strings, or anything cleaner and more specific at least
        SelectDoorByName(currentDropdownSelection); ;
    }

    public void SelectDoorByID(int id)
    {
        foreach (Transform child in doorHoldersParent)
        {
            if (child.TryGetComponent<DoorIdentifier>(out var doorIDComponent) && doorIDComponent.doorID == id)
            {
                carouselRotator.RotateToDoor(id);
                StartCoroutine(WaitAndDropDoor(child));
                return;
            }
        }

        Debug.LogWarning("Door ID not found: " + id);
    }

    public void SelectDoorByName(string name)
    {
        foreach (Transform child in doorHoldersParent)
        {
            if (child.TryGetComponent<DoorIdentifier>(out var doorIDComponent) && doorIDComponent.doorName == name)
            {
                carouselRotator.RotateToDoor(doorIDComponent.doorID);
                StartCoroutine(WaitAndDropDoor(child));
                return;
            }
        }
    }

    private System.Collections.IEnumerator WaitAndDropDoor(Transform doorHolder)
    {
        yield return new WaitUntil(() => !carouselRotator.rotateToTarget);
        if (savedChainControllers?.IsExtending() ?? false)
        {
            savedChainControllers.ToggleChainExtension();
        }
        DoorChainController chainController = doorHolder.GetComponentInChildren<DoorChainController>();
        if (chainController != null)
        {
            chainController.ToggleChainExtension();
            savedChainControllers = chainController;
        }
    }
}
