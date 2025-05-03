using UnityEngine;

public class DoorChainController : MonoBehaviour
{
    [Header("Chain Settings")]
    public Transform chain;
    public Transform door;
    public float chainExtensionSpeed = 5f;
    public float maxChainLength = 5.7f;

    private float originalChainLength;
    private bool isExtending = false;

    void Start()
    {
        originalChainLength = chain.localScale.y;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            isExtending = true;
        if (Input.GetKeyDown(KeyCode.F))
            isExtending = false;

        ExtendChainAndMoveDoor();
    }

    public void TriggerChainExtension(bool extend)
    {
        isExtending = extend;
    }

    void ExtendChainAndMoveDoor()
    {
        float delta = chainExtensionSpeed * Time.deltaTime;
        float scaleDelta = delta * 0.5f;

        float currentScaleY = chain.localScale.y;
        float targetScaleY = isExtending
            ? Mathf.Min(currentScaleY + scaleDelta, maxChainLength)
            : Mathf.Max(currentScaleY - scaleDelta, originalChainLength);

        float appliedScaleDelta = targetScaleY - currentScaleY;

        chain.localScale = new Vector3(chain.localScale.x, targetScaleY, chain.localScale.z);
        chain.position += Vector3.down * appliedScaleDelta;
        door.position += Vector3.down * (appliedScaleDelta * 2f);
    }
}
