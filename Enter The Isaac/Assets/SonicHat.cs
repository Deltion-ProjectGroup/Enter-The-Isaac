using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicHat : HATBase
{
    [SerializeField] float multiplyWalkSpeed = 1.5f;
    [SerializeField] float multiplySpeed = 1.5f;
    [SerializeField] float mulitplyRoll = 3;
    AudioClip oldMusic;
    AudioClip oldSound;
    [SerializeField] AudioClip sonicMusic;
    [SerializeField] AudioClip shootSound;
    void Start()
    {
        player.gun.gunDel += AddSpeed;
        player.walkSpeed *= multiplyWalkSpeed;
        player.rollSpeed *= mulitplyRoll;
        player.rollDeceleration *= mulitplyRoll;

        if(GameObject.Find("Music") != null && sonicMusic != null){
            oldMusic = GameObject.Find("Music").GetComponent<AudioSource>().clip;
            GameObject.Find("Music").GetComponent<AudioSource>().clip = sonicMusic;
            GameObject.Find("Music").GetComponent<AudioSource>().Play();
        }
    }

    void AddSpeed()
    {
        player.gun.gunClone.bulletSpeed *= multiplySpeed;
        player.gun.gunClone.bulletDecelerationSpeed *= multiplySpeed / 2;
        oldSound = player.gun.gunClone.sound;
        player.gun.gunClone.sound = shootSound;
    }

    public override void StopHat()
    {
        player.gun.gunDel -= AddSpeed;
        player.walkSpeed /= mulitplyRoll;
        player.gun.gunClone.bulletSpeed /= multiplySpeed;
        player.gun.gunClone.bulletDecelerationSpeed /= multiplySpeed / 2;
        player.rollDeceleration /= mulitplyRoll;

        if(GameObject.Find("Music") != null){
            GameObject.Find("Music").GetComponent<AudioSource>().clip = oldMusic;
            GameObject.Find("Music").GetComponent<AudioSource>().Play();
        }
        player.gun.gunClone.sound = oldSound;
    }
}
