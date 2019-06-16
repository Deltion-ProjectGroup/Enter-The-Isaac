using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ToggleController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    Toggle toggle;
    bool isOver = false;
    [SerializeField] string input = "uiSelect";
    void Start () {
        toggle = GetComponent<Toggle> ();
    }

    void Update () {
        if (isOver == true && Input.GetButtonDown (input) == true && Input.GetButtonDown ("Fire1") == false) {
            toggle.isOn = !toggle.isOn;
        }
    }

    public void OnPointerEnter (PointerEventData pointerEventData) {
        isOver = true;
    }

    public void OnPointerExit (PointerEventData pointerEventData) {
        isOver = false;
    }

    void OnDisable () {
        isOver = false;
    }
}