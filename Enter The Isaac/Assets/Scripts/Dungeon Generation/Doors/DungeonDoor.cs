using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
    public Animator doorAnimator;
    public bool open;

    public void Start()
    {

    }
    public virtual void ToggleDoor()
    {
        open = !open;
        doorAnimator.SetBool("Open", open);
    }
}
