using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [SerializeField] List<Image> containers;
    [SerializeField] int curHealth;//the 2 represents hp per heart, the 3 is the amount of containers

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
    }

    void Update()
    {
        SetUI();
    }

    void SetUI()
    {
        float fill = curHealth % 2;
        fill /= 2;
        int curHeart = (curHealth + 1) / 2;
        for (int i = 0; i < containers.Count - 1; i++)
        {
            containers[i].fillAmount = 1;
            if (i + 1 == curHeart)
            {
                containers[containers.Count - 2 - i].fillAmount -= fill;
            }
            if (i + 1 > curHeart)
            {
                containers[containers.Count - 2 - i].fillAmount = 0;
            }
        }
    }
}
