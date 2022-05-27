using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [ItemCodeDescription]
    [SerializeField]
    private int _itemCode;

    private SpriteRenderer spriteRenderer;

    public int ItemCode
    {
        get
        {
            return _itemCode;
        }
        set
        {
            _itemCode = value;
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    public void Init(int itemCode)
    {
        ItemCode = itemCode;
        if (itemCode != 0)
        {
            // set the sprite, based on the item code, from the inventory manager
            ItemDetails itemdetails = InventoryManager.Instance.GetItemDetails(ItemCode);
            spriteRenderer.sprite = itemdetails.itemSprite;
            // add the nudge script
            gameObject.AddComponent<ItemNudge>();
        }
        
    }
}
