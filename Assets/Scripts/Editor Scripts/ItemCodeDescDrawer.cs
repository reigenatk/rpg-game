using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// make it a drawer for our type "ItemCodeDescription"
[CustomPropertyDrawer(typeof(ItemCodeDescription))]
public class ItemCodeDescDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // double the property height since we will draw item code and description
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    // this actually draws the property
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that prefab override logic works on the entire property.

        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.Integer)
        {

            EditorGUI.BeginChangeCheck(); // Start of check for changed values

            // Draw item code
            var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);

            // Draw item description (use the helper method below to get the description)
            EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2), "Item Description", GetItemDescription(property.intValue));



            // If item code value has changed, then set value to new value
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = newValue;
            }


        }


        EditorGUI.EndProperty();
    }

    private string GetItemDescription(int itemCode)
    {
        ScriptableObjectItemList so_itemList;

        // get the asset programatically using the path
        so_itemList = AssetDatabase.LoadAssetAtPath("Assets/Scriptable Objects/Items/so_ItemList.asset", typeof(ScriptableObjectItemList)) as ScriptableObjectItemList;

        List<ItemDetails> itemDetailsList = so_itemList.itemDetails;

        ItemDetails itemDetail = itemDetailsList.Find(x => x.itemCode == itemCode);

        if (itemDetail != null)
        {
            return itemDetail.itemDescription;
        }
        else
        {
            return "";
        }
    }
}
