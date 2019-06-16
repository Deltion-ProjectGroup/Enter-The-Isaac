using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class FadeImg : MonoBehaviour {
    [SerializeField] Color goalColor;
    [SerializeField] Color startColor;
    Image image;
    SpriteRenderer sprite;
    bool scaledTime = false;
    [SerializeField] float speed;
    [SerializeField] float waitTime = 0.1f;
    [SerializeField] UnityEvent endEvent;

    void Start () {
        image = GetComponent<Image> ();
        sprite = GetComponent<SpriteRenderer> ();
        if (image != null) {
            image.color = startColor;
        } else {
            sprite.color = startColor;
        }
        Invoke ("Wait", waitTime);
    }

    void Wait () {
        //invoke
    }

    void Update () {
        if (IsInvoking ("Wait") == false) {
            if (scaledTime == true) {
                if (image != null) {
                    image.color = Color.Lerp (image.color, goalColor, Time.deltaTime * speed);
                } else {
                    sprite.color = Color.Lerp (sprite.color, goalColor, Time.deltaTime * speed);
                }
            } else {
                if (image != null) {
                    image.color = Color.Lerp (image.color, goalColor, Time.unscaledDeltaTime * speed);
                } else {
                    sprite.color = Color.Lerp (sprite.color, goalColor, Time.unscaledDeltaTime * speed);
                }
            }
            if (image != null) {
                if (image.color == goalColor) {
                    image.color = goalColor;
                    endEvent.Invoke ();
                }
            } else {
                if (sprite.color == goalColor) {
                    sprite.color = goalColor;
                    endEvent.Invoke ();
                }
            }
        }
    }
    public void SetSpeed (float newSpeed) {
        speed = newSpeed;
    }
    public void ChangeGoalColor (Color newGoal) {
       // goalColor = newGoal;
    }
    public void ChangeGoalAlpha (float value) {
        if (image != null) {
            ChangeGoalColor (new Color (image.color.r, image.color.g, image.color.b, value));
        } else {
            ChangeGoalColor (new Color (sprite.color.r, sprite.color.g, sprite.color.b, value));
        }
    }
}