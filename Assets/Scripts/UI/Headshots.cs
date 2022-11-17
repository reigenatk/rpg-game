using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using static SO_Headshots;

public class Headshots : MonoBehaviour
{
    Image headshotimg;
    [SerializeField] DialogueCharacterNameView dcnv;
    [SerializeField] Sprite nothing;
    [SerializeField] List<SO_Headshots> headshotObjects;

    // Start is called before the first frame update
    void Start()
    {
        headshotimg = GetComponent<Image>();
    }

    public void nameUpdated()
    {
        string charName = dcnv.characterName;
        if (charName == "")
        {
            // Debug.Log("No char name");
            goto remove_headshot;
        }

        // Debug.Log("name was changed to " + dcnv.characterName);
        // Debug.Log("Metadata array of size: " + dcnv.metadata.Length);

        // dunno why this doesn't work. Anyways, format is "e:slight-smile" so lets chop off first two chars
        string expression = "normal";
        if (dcnv.metadata != null)
        {
            foreach (string s in dcnv.metadata)
            {
                if (s[0] == 'e')
                {
                    expression = s.Substring(2);
                }
            }
        }

        // Debug.Log("expression is: " + expression);
        foreach (SO_Headshots obj in headshotObjects)
        {
            if (obj.charactersThatUseTheseHeadshots.Contains(charName))
            {
                // found the headshot obj
                foreach (HeadshotItem hi in obj.headshots)
                {
                    if (hi.headshotName == expression)
                    {
                        // Debug.Log("Setting headshot expression!");
                        headshotimg.sprite = hi.sprite;
                        headshotimg.color = new Color(1, 1, 1, 1);
                        headshotimg.rectTransform.localScale = new Vector3(obj.scaleToUse, obj.scaleToUse, obj.scaleToUse);
                        goto done_setting_headshot;
                    }
                }
            }
        }

        // if we get here it means no headshot was set...
        // means one of two things, either we dont have the character or the headshot with that given name
        // either way let's just remove whatever image we had before (aka blank headshot)
        remove_headshot:;
        Debug.Log("Removing image");
        headshotimg.sprite = nothing;
        headshotimg.color = new Color(1, 1, 1, 0);

        done_setting_headshot:;

    }


}
