using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetailsDict;
    [SerializeField] private ScriptableObjectItemList itemList = null;

    // we have one list of InventoryItems for each holder (player, chests, etc.)
    // index into it using the InventoryLocation enum
    public List<InventoryItem>[] inventoryLists;

    // easy way to access capacity of an inventorylist, again index using InventoryLocation
    public int[] inventoryListCapacityArray; 

    // must call this in the Awake method since Items will use InventoryManager in their start
    // methods, and we might get a nullptrexception if we put it in a start method for this class
    protected override void Awake()
    {
        base.Awake();

        CreateInventoryLists();

        CreateItemDetailsDictionary();
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];
        for (int i = 0; i < (int) InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        inventoryListCapacityArray = new int[(int)InventoryLocation.count];
        inventoryListCapacityArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    /// <summary>
    /// Find if an itemCode is already in the inventory. Returns the item position
    /// in the inventory list, or -1 if the item is not in the inventory
    /// </summary>
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        for (int i = 0; i < inventoryList.Count; i++)
        {
            // just check if the item code exists in the list
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Add item to the end of the inventory
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

        //DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Add item to position in the inventory
    /// </summary>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity + 1;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryList[position] = inventoryItem;


        //DebugPrintInventoryList(inventoryList);
    }

    /// <summary>
    /// Add an item to the inventory list for the inventoryLocation
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        // Check if inventory already contains the item
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        
        if (itemPosition != -1)
        {
            // if already in the inventory, just add to its count
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            // else we have to add a whole new entry
            AddItemAtPosition(inventoryList, itemCode);
        }

        //  Send event that inventory has been updated
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    /// <summary>
    /// Add an item to the inventory list for the inventoryLocation and then destroy the gameObjectToDelete
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {

        AddItem(inventoryLocation, item);

        // basically the same except we have to delete a gameobject too
        // this is useful because when you pickup an item, the object should dissapear from the scene
        Destroy(gameObjectToDelete);
    }

    /// <summary>
    /// populates the dictionary from the ItemList
    /// </summary>
    private void CreateItemDetailsDictionary()
    {
        itemDetailsDict = new Dictionary<int, ItemDetails>();
        foreach (ItemDetails a in itemList.itemDetails)
        {
            itemDetailsDict.Add(a.itemCode, a);
        }
    }

    /// <summary>
    /// Get the itemdetails for a specific item given the itemCode of an object
    /// </summary>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails ret;
        if (itemDetailsDict.TryGetValue(itemCode, out ret))
        {
            return ret;
        }
        else
        {
            return null;
        }
    }

    // for debugging
    private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    {
        foreach (InventoryItem inventoryItem in inventoryList)
        {
            Debug.Log("Item Description:" + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + "    Item Quantity: " + inventoryItem.itemQuantity);
        }
        Debug.Log("******************************************************************************");
    }

}
