using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HandPositioner : MonoBehaviour
{


    public void AddOrUpdatePosition(Item item)
    {
        // add safety checks!
        HandItemPosition position = new HandItemPosition(
            transform.localEulerAngles,
            transform.localPosition
        );

        item.itemInfo.SetHandPosition( position );
     
    }

    internal void Position(Item item)
    {
        HandItemPosition position = item.itemInfo.GetHandItemPosition();
        item.itemInfo.ApplyFingers();

        if (position != null)
        {
            transform.localPosition = position.position;
            transform.localEulerAngles = position.rotation;
        } else
        {
            Debug.LogWarning($"{item.GetItemType()} was not found!");
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }

        item.transform.parent = transform;
        item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    [CustomEditor(typeof(HandPositioner))]
    class CopyTranfrom : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Save item hand position"))
            {
                HandPositioner positioner = GameObject.FindGameObjectWithTag("HandPosRef").GetComponent<HandPositioner>();
                Item item = target.GetComponentInChildren<Item>();
                if (item)
                {
                    positioner.AddOrUpdatePosition(item);
                } else
                {
                    Debug.Log("Not holding any item!");
                }
            }
        }
    }
}

    [System.Serializable]
    public class HandItemPosition
    {
        public Vector3 rotation;
        public Vector3 position;

        public HandItemPosition(Vector3 rotation, Vector3 position)
        {
            this.rotation = rotation;
            this.position = position;
        }   
    }

