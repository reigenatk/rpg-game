
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if player collided with an item
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            ItemDetails details = InventoryManager.Instance.GetItemDetails(item.ItemCode);
            Debug.Log(details.itemDescription);
        }
    }
}
