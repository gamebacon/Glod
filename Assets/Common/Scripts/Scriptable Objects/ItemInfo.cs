using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "New item", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemInfo : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private ItemType _itemType;
    [SerializeField] private Sprite _icon;
    [SerializeField] private HandItemPosition _handPosition; 
    [SerializeField] private Fingers fingers; 


    public void SetHandPosition(HandItemPosition handPosition)
    {
        // this._handPosition = handPosition;
        this.fingers = SaveFingers();

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

    // shouldn't be here
    public void ApplyFingers()
    {
        foreach (FingerRotation fingerRotation in fingers.fingerRotations)
        {
            GameObject boneObject = GameObject.Find(fingerRotation.name);
            if (boneObject != null)
            {
                boneObject.transform.localRotation = fingerRotation.rotation;
            }
            else
            {
                Debug.LogWarning($"Bone not found: {fingerRotation.name}");
            }
        }
    }

    // shouldn't be here
    public void ResetFingers()
    {
        foreach (FingerRotation fingerRotation in fingers.fingerRotations)
        {
            GameObject boneObject = GameObject.Find(fingerRotation.name);
            if (boneObject != null)
            {
                boneObject.transform.localRotation = Quaternion.identity; 
            }
            else
            {
                Debug.LogWarning($"Bone not found: {fingerRotation.name}");
            }
        }
    }

    private Fingers SaveFingers()
    {
        string[][] hand =
        {
            new string[] {"thumb1.R", "thumb2.R", "thumb3.R"},
            new string[] {"index1.R", "index2.R", "index3.R"},
            new string[] {"long1.R", "long2.R", "long3.R"},
            new string[] {"ring1.R", "ring2.R", "ring3.R"},
            new string[] {"pinky1.R", "pinky2.R", "pinky3.R"},
        };

        Fingers fingers = new Fingers();

        for (int i = 0; i < hand.Length; i++)
        {
            for (int j = 0; j < hand[i].Length; j++)
            {
                string boneName = hand[i][j];
                GameObject boneObject = GameObject.Find(boneName);

                if (boneObject != null)
                {
                    Quaternion rotation = boneObject.transform.localRotation;
                    fingers.fingerRotations.Add(new FingerRotation(boneName, rotation));
                    Debug.Log($"Saved {boneName} rotation: {rotation.eulerAngles}");
                }
                else
                {
                    Debug.LogWarning($"Bone not found: {boneName}");
                }
            }
        }

        return fingers;
    }
}

[System.Serializable]
public class Fingers
{
    public List<FingerRotation> fingerRotations = new List<FingerRotation>();

    public Quaternion GetFingerRotation(string fingerName)
    {
        foreach (var finger in fingerRotations)
        {
            if (finger.name == fingerName)
                return finger.rotation;
        }
        return Quaternion.identity;
    }
}

[System.Serializable]
public class FingerRotation
{
    public string name;
    public Quaternion rotation;

    public FingerRotation(string name, Quaternion rotation)
    {
        this.name = name;
        this.rotation = rotation;
    }
}
public enum ItemInteractionType
    {
        VIEW,
        ATTACK,
        USE,
    }



