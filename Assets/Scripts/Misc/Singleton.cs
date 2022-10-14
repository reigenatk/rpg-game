
using UnityEngine;

// singletons have two requirements- must be globally accessible, and only have ONE instance in
// a scene.
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    // this is how we refer to the singleton. For example in the InventoryManager global object, we 
    // access using "InventoryManager.Instance" like so: "ItemDetails details = InventoryManager.Instance.GetItemDetails(item.ItemCode);"
    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        // will only instatiate an instance if there isn't already one yet!
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
