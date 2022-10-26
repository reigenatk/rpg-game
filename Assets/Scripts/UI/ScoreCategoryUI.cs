using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreCategoryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] TextMeshProUGUI TextDelta;
    [SerializeField] float smallestFontSize; // largest and smallest font sizes we will lerp over
    [SerializeField] float largestFontSize;
    // Start is called before the first frame update
    private void Start()
    {
        TextDelta.text = "";
    }

    public void UpdateScoreUI(PlayerScore scoreToUpdate, float percentage, float delta)
    {
        // enable the UI to show score update! For instance, we hid UI cuz cutscene, but then if in cutscene we award like +10 or something, then we wanna show it.
        // Else the coroutine wont work properly. Also another important thing, we dont have to worry about -1 Energy or something calling this cuz during cutscenes, time is paused
        if (GameUI.Instance.UIDisabled == true) GameUI.Instance.enableUI();


        // divide by 100 cuz a change of 100 should be the largest on the UI
        float fontSize = Mathf.Lerp(smallestFontSize, largestFontSize, Math.Abs(delta) / 30.0f);
        // Debug.Log("Font size is " + fontSize + " for change of " + delta);
        if (delta > 0.0f)
        {
            // green
            TextDelta.color = new Color(0, 255, 8, 2);
            TextDelta.text = String.Format("+{0:0.#\\%}", delta);
        }
        else
        {
            // red 
            TextDelta.color = new Color(255, 57, 0, 255);
            TextDelta.text = String.Format("{0:0.#\\%}", delta); 
        }
        /*if (delta != -1.0f && delta != 0.0f)
        {
            // we will not do a UI update each time a score tick happens or nothing happens.
            // Otherwise, do an FLASHY animation of the +x% or -x% text
            

        }*/

        StartCoroutine(displayDeltaText());
  
        string sformatted = ((int)Math.Round(percentage)).ToString() + "%";
        Text.text = sformatted;
        TextDelta.fontSize = fontSize;
    }

    IEnumerator displayDeltaText()
    {
        // blink twice then wait 4sec
        yield return new WaitForSeconds(0.2f);
        TextDelta.enabled = false;
        yield return new WaitForSeconds(0.2f);
        TextDelta.enabled = true;
        yield return new WaitForSeconds(0.2f);
        TextDelta.enabled = false;
        yield return new WaitForSeconds(0.2f);
        TextDelta.enabled = true;
        yield return new WaitForSeconds(4.0f);
        TextDelta.text = "";

    }
}
