using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    List<Image> containers = new List<Image>();
    [SerializeField] int curHealth;
    [SerializeField] int parts = 2;
    int lastHP = 0;
    [SerializeField] Hitbox hpBox;


    void Start()
    {
        containers.Clear();
        containers.AddRange(transform.GetComponentsInChildren<Image>());
        for (int i = 0; i < containers.Count; i++)
        {
            if (containers[i].transform.parent != transform)
            {
                containers.RemoveAt(i);
                i = 0;
            }
        }
        lastHP = 666;//dont question it
    }

    void Update()
    {
        curHealth = (int)hpBox.curHealth;
        if (curHealth != lastHP)//so it doesn't do all of setui every frame
        {
            SetUI();
        }
    }

    void SetUI()
    {
        float fill = curHealth % parts;
        fill /= parts;
        int curHeart = (curHealth + 1) / parts;
        for (int i = 0; i < containers.Count; i++)
        {
            if (i == (curHealth - 1) / parts)
            {
                if (fill > 0)
                {
                    containers[containers.Count - 1 - i].fillAmount = fill;
                }
                else if (curHealth != 0)
                {
                    containers[containers.Count - 1 - i].fillAmount = 1;
                }
                else
                {
                    containers[containers.Count - 1 - i].fillAmount = 0;
                }
            }
            if (i > (curHealth - 1) / parts)
            {
                containers[containers.Count - 1 - i].fillAmount = 0;
            }
            if (i < (curHealth - 1) / parts)
            {
                containers[containers.Count - 1 - i].fillAmount = 1;
            }
        }
        lastHP = curHealth;
    }
}
