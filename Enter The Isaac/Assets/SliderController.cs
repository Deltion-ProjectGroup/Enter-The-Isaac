 using System.Collections.Generic;
 using System.Collections;
 using UnityEngine.UI;
 using UnityEngine;
 using UnityEngine.EventSystems;

 public class SliderController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

     Slider slider;
     [SerializeField] float speed = 2;
     [SerializeField] bool isSelected = true;
     [SerializeField] string input = "uiSelect";
     bool isOver = false;
     void Start () {
         slider = GetComponent<Slider> ();
         isSelected = false;
     }

     void Update () {

         if (isSelected == true) {
             slider.value += Input.GetAxis ("Horizontal") * Time.unscaledDeltaTime * (slider.maxValue - slider.minValue) * speed;
             if (Input.GetButtonUp (input) == true) {
                 isSelected = false;
             }
         } else {
             if (Input.GetButtonDown (input) == true && isOver == true) {
                 isSelected = true;
             }
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