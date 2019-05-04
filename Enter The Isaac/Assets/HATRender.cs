using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HATRender : MonoBehaviour
{

    [SerializeField] GameObject hatRenderPrefab;
    GameObject lastHat;
    [SerializeField] HumanBodyBones part = HumanBodyBones.Head;
    GameObject currentHat;

    void Start()
    {
        UpdateHat();
        lastHat = hatRenderPrefab;
    }

    void Update()
    {
        if (hatRenderPrefab != lastHat)
        {
            lastHat = hatRenderPrefab;
            UpdateHat();
        }
    }

    void UpdateHat()
    {
        if (currentHat != null)
        {
            if (currentHat.GetComponent<HATBase>() != null)
            {
                currentHat.GetComponent<HATBase>().StopHat();
            }
        }
        Destroy(currentHat);
        Transform hatParent = GetComponent<Animator>().GetBoneTransform(part);
        currentHat = Instantiate(hatRenderPrefab, hatParent.position, hatParent.rotation, hatParent);
        if (currentHat != null)
        {
            if (currentHat.GetComponent<HATBase>() != null)
            {
                currentHat.GetComponent<HATBase>().GetPlayer(transform.parent.GetComponent<PlayerController>());
            }
        }
    }
}
