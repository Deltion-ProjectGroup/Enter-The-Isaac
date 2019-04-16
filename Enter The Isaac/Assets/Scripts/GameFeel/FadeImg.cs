using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class FadeImg : MonoBehaviour
{
    [SerializeField] Color goalColor;
    [SerializeField] Color startColor;
    Image image;
    bool scaledTime = false;
    [SerializeField] float speed;
    [SerializeField] float waitTime = 0.1f;
    [SerializeField] UnityEvent endEvent;

    void Start()
    {
        image = GetComponent<Image>();
        image.color = startColor;
        Invoke("Wait", waitTime);
    }

    void Wait()
    {
        //invoke
    }

    void Update()
    {
        if (IsInvoking("Wait") == false)
        {
            if (scaledTime == true)
            {
                image.color = Color.Lerp(image.color, goalColor, Time.deltaTime * speed);
            }
            else
            {
                image.color = Color.Lerp(image.color, goalColor, Time.unscaledDeltaTime * speed);
            }
            if (image.color == goalColor)
            {
                endEvent.Invoke();
            }
        }
    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    public void ChangeGoalColor(Color newGoal)
    {
        goalColor = newGoal;
    }
    public void ChangeGoalAlpha(float value)
    {
        ChangeGoalColor(new Color(image.color.r, image.color.g, image.color.b, value));
    }
}
