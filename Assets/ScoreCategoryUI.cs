using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreCategoryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] TextMeshProUGUI TextDelta;
    // Start is called before the first frame update
    private void Start()
    {
        TextDelta.text = "";
    }

    public void UpdateScoreUI(PlayerScore scoreToUpdate, float percentage, float delta)
    {
        
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
    }

    IEnumerator displayDeltaText()
    {

        yield return new WaitForSeconds(5.0f);
        TextDelta.text = "";

    }
}
