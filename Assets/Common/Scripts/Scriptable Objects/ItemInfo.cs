using NUnit.Framework.Interfaces;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New item", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemInfo : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private ItemType _itemType;
    [SerializeField] private Sprite _icon;
    [SerializeField] private HandItemPosition _handPosition; 


    public void SetHandPosition(HandItemPosition handPosition)
    {
        this._handPosition = handPosition;

        #if UNITY_EDITOR
            Debug.Log("Unity editor!");
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        #endif

        Debug.Log($"Saved hand positions for {this._name}");
    }

    public HandItemPosition GetHandItemPosition()
    {
        return this._handPosition;
    }

    public string GetItemName()
    {
        return _name;
    }
    public string GetItemDescription()
    {
        return _description;
    } 
    public ItemType GetItemType()
    {
        return _itemType;
    } 

}

public enum ItemInteractionType
{
    VIEW,
    ATTACK,
    USE,
}