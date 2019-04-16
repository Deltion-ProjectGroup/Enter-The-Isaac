using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{

    [SerializeField] Color newColor = Color.white;
    Color originalColor;
    [SerializeField] float lerpSpeed = 5;
    float waitTime = 0;


    public void SetCustomWaitTime(float t)
    {
        waitTime = t;
    }
    public void FlashUIImg(float speed)
    {
        lerpSpeed = speed;
        Image rend = GetComponent<Image>();
        if (IsInvoking("GoBackImg") == false)
        {
            originalColor = rend.color;
        }
        rend.color = newColor;
        InvokeRepeating("GoBackImg", waitTime, Time.unscaledDeltaTime);
    }

    public void FlashUITxt(float speed)
    {
        lerpSpeed = speed;
        Text rend = GetComponent<Text>();
        if (IsInvoking("GoBackTxt") == false)
        {
            originalColor = rend.color;
        }
        rend.color = newColor;
        InvokeRepeating("GoBackTxt", waitTime, Time.unscaledDeltaTime);
    }

    public void FlashMat(float speed)
    {
        lerpSpeed = speed;
        Material rend = GetComponent<Renderer>().material;
        if (IsInvoking("GoBackMat") == false)
        {
            originalColor = rend.color;
        }
        rend.color = newColor;
        InvokeRepeating("GoBackMat", waitTime, Time.unscaledDeltaTime);
    }


    void GoBackImg()
    {
        Image rend = GetComponent<Image>();
        if (rend.color != originalColor)
        {
            rend.color = Color.Lerp(rend.color, originalColor, lerpSpeed);
        }
        else
        {
            waitTime = 0;
            CancelInvoke("GoBackImg");
        }
    }

    void GoBackTxt()
    {
        Text rend = GetComponent<Text>();
        if (rend.color != originalColor)
        {
            rend.color = Color.Lerp(rend.color, originalColor, lerpSpeed);
        }
        else
        {
            waitTime = 0;
            CancelInvoke("GoBackTxt");
        }
    }

    void GoBackMat()
    {
        Material rend = GetComponent<Renderer>().material;
        if (rend.color != originalColor)
        {
            rend.color = Color.Lerp(rend.color, originalColor, lerpSpeed);
        }
        else
        {
            waitTime = 0;
            CancelInvoke("GoBackMat");
        }
    }

}
