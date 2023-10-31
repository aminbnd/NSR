using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace SilverTau.RecorderSystem
{
    [RequireComponent(typeof(Button))]
    public class ButtonTimer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool pressed;
        public float pressedTime;
    
        void Start()
        {
        
        }

        void Update()
        {
            if (pressed)
            {
                pressedTime += Time.deltaTime;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pressedTime = 0.0f;
            pressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressed = false;
            pressedTime = 0.0f;
        }
    }
}