using System.Collections.Generic;
using UnityEngine;

// allow asset menu to see this
[CreateAssetMenu(fileName="so_ItemList", menuName = "Scriptable Objects/Item/Item List")]
public class ScriptableObjectItemList : ScriptableObject
{
    [SerializeField]
    public List<ItemDetails> itemDetails;

}
