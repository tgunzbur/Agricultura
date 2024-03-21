using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    private bool isPressing = false;

    public void Update() {
        if (isPressing) {
            GetComponent<Button>().onClick.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        isPressing = true;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isPressing = false;
    }

    public void OnDisable() {
        isPressing = false;
    }
}