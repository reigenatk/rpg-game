
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    // this script sits on the player, and will collide with stuff in the scene
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if player collided with an item (from calling GetComponent on the gameobject)
        Item item = collision.GetComponent<Item>();
        if (item != null) { 

            // here the player collided with the 2d colliders on the item prefabs (with trigger set on)
            // upon collision, we will add an item to the player's inventory manager
            ItemDetails details = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            // check if the item is pick-up able
            if (details.canBePickedUp)
            {
                // this also removes the item from the scene thanks to this overloaded version of AddItem
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);
            }
            
        }
    }
}
