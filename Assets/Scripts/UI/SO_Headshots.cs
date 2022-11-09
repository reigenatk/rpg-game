using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_Headshots", menuName = "Scriptable Objects/Headshots")]
public class SO_Headshots : ScriptableObject
{
    [System.Serializable]
    public class HeadshotItem
    {
        public string headshotName; // set this to "normal" to indicate default expression
        public Sprite sprite;
    }
    public List<HeadshotItem> headshots;
    public List<string> charactersThatUseTheseHeadshots;
    public float scaleToUse; // too lazy to make all the faces exactly 50px by 50px, so I'll just scale the image instead for each face

}