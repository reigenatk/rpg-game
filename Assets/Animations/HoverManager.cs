using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverManager : MonoBehaviour
{
    public TextMeshProUGUI tipText;
    public RectTransform tipWindow;

    public static Action<string, Vector2> OnMouseHover;
    public static Action OnMouseLoseFocus;

    private void OnEnable()
    {
        OnMouseHover += ShowMessage;
        OnMouseLoseFocus += HideMessage;
    }

    private void OnDisable()
    {
        OnMouseHover -= ShowMessage;
        OnMouseLoseFocus -= HideMessage;
    }

    // Update is called once per frame
    void Start()
    {
        HideMessage();
    }

    private void ShowMessage(string tip, Vector2 mousePos)
    {
        tipText.text = tip;

        // make the max width 200, otherwise it will keep going and fall off screen
        tipWindow.sizeDelta = new Vector2(tipText.preferredWidth > 200 ? 200 : tipText.preferredWidth, tipText.preferredHeight);
        tipWindow.gameObject.SetActive(true);
        tipWindow.transform.position = new Vector2(mousePos.x + 50.0f, mousePos.y);
    }

    private void HideMessage()
    {
        tipText.text = default;
        tipWindow.gameObject.SetActive(false);
    }
}
