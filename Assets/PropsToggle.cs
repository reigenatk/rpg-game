using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Yarn.Unity;

public class PropsToggle : MonoBehaviour
{
    // a light to turn on and off (if necessary)
    [SerializeField] private Light2D lightToToggle;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
    }

    [YarnCommand("toggleLight")]
    public void toggleLight(bool state)
    {
        if (state)
        {
            // turning on
            // on sprite should be first (index 0)
            spriteRenderer.sprite = sprites[0];
            lightToToggle.intensity = 0.7f;
            soundManager.playSoundString("LampToggle");
        }
        else
        {
            // turning off
            spriteRenderer.sprite = sprites[1];
            lightToToggle.intensity = 0.0f;
            soundManager.playSoundString("LampToggle");
        }
    }
}
