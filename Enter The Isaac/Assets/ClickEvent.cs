using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ClickEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] UnityEvent mouseEnterEvent;
    [SerializeField] UnityEvent mouseExitEvent;
    [SerializeField] UnityEvent clickEvent;
    bool isOver = false;
    [SerializeField] string inputName = "Fire1";
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isOver = true;
        mouseEnterEvent.Invoke();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isOver = false;
        mouseExitEvent.Invoke();
    }

    void Update()
    {
        if (isOver == true && Input.GetButtonDown(inputName) == true)
        {
            clickEvent.Invoke();
        }
    }

    void OnDisable()
    {
        isOver = false;
    }
}
