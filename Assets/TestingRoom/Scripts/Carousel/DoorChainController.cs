using UnityEngine;

public class DoorChainController : MonoBehaviour
{
    [Header("Chain Settings")]
    public Transform chain;
    public Transform door;
    public float chainExtensionSpeed = 5f;
    public float maxChainLength = 16f;
    public float doorLowerSpeed = 3f;
    public float minChainLength = 0f;

    private bool isExtending = false;
    private float originalChainHeight;
    private Vector3 originalDoorPosition;

    void Start()
    {
        originalChainHeight = chain.localScale.y;
        originalDoorPosition = door.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TriggerChainExtension(true);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TriggerChainExtension(false);
        }

        ExtendChainAndMoveDoor();
    }

    public void TriggerChainExtension(bool extend)
    {
        isExtending = extend;
    }

    void ExtendChainAndMoveDoor()
    {
        if (isExtending)
        {
            if (chain.localScale.y < maxChainLength)
            {
                chain.localScale += new Vector3(0f, chainExtensionSpeed * Time.deltaTime, 0f);
            }

            if (chain.localScale.y < maxChainLength)
            {
                door.position = new Vector3(door.position.x, originalDoorPosition.y - (chain.localScale.y - originalChainHeight), door.position.z);
            }
        }
        else
        {
            if (chain.localScale.y > minChainLength)
            {
                chain.localScale -= new Vector3(0f, chainExtensionSpeed * Time.deltaTime, 0f);
            }

            if (chain.localScale.y > minChainLength)
            {
                door.position = new Vector3(door.position.x, originalDoorPosition.y - (chain.localScale.y - originalChainHeight), door.position.z);
            }
        }
    }
}
