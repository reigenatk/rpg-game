using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryBar : MonoBehaviour
{
    private RectTransform rectTransform;
    private bool _isInventoryBarPositionBottom = true;
    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    // the sprite for the 
    [SerializeField] private Sprite blankSprite = null;
    [SerializeField] private UIInventorySlot[] inventorySlots = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    private void Update()
    {
        SwitchInventoryBarPosition();
    }
    // subscribe to the item pickup event
    private void OnEnable()
    {
        // this is basically saying, call SetAnimationParameters whenever the delegate is called (which will happen from the input class)
        // also called SUBSCRIBING to the event
        EventHandler.InventoryUpdateEvent += UpdateInventoryBar;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdateEvent -= UpdateInventoryBar;
    }

    // so this is the function that does the work of updating the UI. Called via delegate. We do a few steps.
    // 0. Check that it is player's inventory that is being changed (if we ever get chests then those inventory updates also would trigger this delegate)
    // 1. clear all the slots' UI data (second line of code)
    // 2. Go through the number of slots in our inventory and get the code of the item
    // 3. Get item details such as sprite, and number of this item and change the textmeshpro and sprite to display this
    private void UpdateInventoryBar(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            if (inventorySlots.Length > 0 && inventoryList.Count > 0)
            {
                // loop through inventory slots and update with corresponding inventory list item
                for (int i = 0; i < inventorySlots.Length; i++)
                {
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        // ItemDetails itemDetails = InventoryManager.Instance.itemList.itemDetails.Find(x => x.itemCode == itemCode);
                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        if (itemDetails != null)
                        {
                            // add images and details to inventory item slot
                            inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                            inventorySlots[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                            inventorySlots[i].itemDetails = itemDetails;
                            inventorySlots[i].itemQuantity = inventoryList[i].itemQuantity;
                            // SetHighlightedInventorySlots(i);

                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// This function just goes thru each inventory slot and removes the sprite, the text, the details, and the quantity
    /// </summary>
    private void ClearInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            // loop through inventory slots and update with blank sprite
            for (int i = 0; i < inventorySlots.Length; i++)

            {
                inventorySlots[i].inventorySlotImage.sprite = blankSprite;
                inventorySlots[i].textMeshProUGUI.text = "";
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].itemQuantity = 0;
                // SetHighlightedInventorySlots(i);
            }
        }
    }

    /// <summary>
    /// Bring the inventory bar up if the player walks too close to the bottom of the screen
    /// </summary>
    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        // move it to bottom if we are greater than a 1/3 of the way up the screen and its not already there
        if (playerViewportPosition.y > 0.3f && IsInventoryBarPositionBottom == false)
        {
            // transform.position = new Vector3(transform.position.x, 7.5f, 0f); // this was changed to control the recttransform see below
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            IsInventoryBarPositionBottom = true;
        }
        else if (playerViewportPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            // move it to top if we are in the bottom 1/3 of the screen and its at the top

            //transform.position = new Vector3(transform.position.x, mainCamera.pixelHeight - 120f, 0f);// this was changed to control the recttransform see below
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
    }


}
