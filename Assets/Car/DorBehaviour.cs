using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DorBehaviour : Interactable
{
    [SerializeField] float openYRot = 90;
    [SerializeField] float closedYRot = 0;
    bool closed = true;
    float t = 0f;
    public override string GetText()
    {
        return closed ? "Open car door" : "Close car door";
    }
    public override bool Interact()
    {
        StopAllCoroutines();
        if (closed)
        {
            closed = false;
            StartCoroutine(OpenClose(closedYRot, openYRot));
        }
        else
        {
            closed= true;
            StartCoroutine(OpenClose(openYRot, closedYRot));
        }

        return true;
    }
    IEnumerator OpenClose(float startYrot, float endYrot)
    {
        float duration = 0.5f; // Czas trwania interpolacji (w sekundach)
        if (t != 0)
            t = 1 - t;
        float y = 0;
        while (t < 1f)
        {
            

            // Zwiêkszenie t w zale¿noœci od up³ywu czasu
            t += Time.deltaTime / duration;
            y = (float)Math.Pow(t, 2.3f);
            // Interpolacja obrotu wokó³ osi Y
            float newYrot = Mathf.Lerp(startYrot, endYrot, y);

            // Ustawienie nowego obrotu obiektu
            transform.localRotation = Quaternion.Euler(0, newYrot, 0);
            yield return new WaitForEndOfFrame(); // Czekanie na koniec klatki
        }
        t = 0;
        // Ustawienie koñcowego obrotu dok³adnie na endYrot
        transform.localRotation = Quaternion.Euler(0, endYrot, 0);
    }
}
