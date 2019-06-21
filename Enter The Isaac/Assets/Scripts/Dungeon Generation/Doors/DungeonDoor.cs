using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
    public Animator doorAnimator;
    public bool open;
    public AudioClip openSound;
    public AudioClip closeSound;

    public virtual void ToggleDoor()
    {
        open = !open;
        doorAnimator.SetBool("Open", open);
        if (open)
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<SoundSpawn>().SpawnEffect(openSound);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Manager").GetComponent<SoundSpawn>().SpawnEffect(closeSound);
        }
    }
}
