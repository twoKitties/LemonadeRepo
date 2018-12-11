using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blinker
{
    private Color defaultColor;
    private Color blinkColor;
    private Text textToBlink;

    public Blinker(Color defaultColor, Color blinkColor, Text textToBlink)
    {
        this.defaultColor = defaultColor;
        this.blinkColor = blinkColor;
        this.textToBlink = textToBlink;
    }
    public IEnumerator Blink()
    {
        float maxTime = 1f;
        float time = Time.deltaTime;

        textToBlink.color = blinkColor;

        while (maxTime <= 1)
        {
            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
            float perc = time / maxTime;

            textToBlink.color = Color.Lerp(blinkColor, defaultColor, perc);
        }
    }
}
