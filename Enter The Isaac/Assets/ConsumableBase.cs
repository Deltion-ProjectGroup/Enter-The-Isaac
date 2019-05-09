using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class ConsumableBase : MonoBehaviour
{

    [HideInInspector] public PlayerController player;
    public int value = 1;
    public AudioClip soundEffect;

    void Start()
    {
        Initailize();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Use();
        }
    }

    public virtual void Initailize()
    {
        player = FindObjectOfType<PlayerController>();
        GetComponent<Collider>().isTrigger = true;
    }

    public virtual void Use()
    {
        FindObjectOfType<SoundSpawn>().SpawnEffect(soundEffect);
        player.hitbox.GetComponent<Hitbox>().AddHealth(value);
        Destroy(gameObject);
    }
}
