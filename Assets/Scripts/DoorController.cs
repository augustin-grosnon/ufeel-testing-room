using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        _animator.SetTrigger("Open");
    }

    public void CloseDoor()
    {
        _animator.SetTrigger("Close");
    }
}
