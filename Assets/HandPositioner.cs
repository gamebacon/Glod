using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HandPositioner : MonoBehaviour
{

    [SerializeField] private List<HandItemPosition> handPositions; 


    [System.Serializable]
    public class HandItemPosition
    {
        public ItemType type;
        public Vector3 rotation;
        public Vector3 position;

        public HandItemPosition(ItemType type, Vector3 rotation, Vector3 position)
        {
            this.type = type;
            this.rotation = rotation;
            this.position = position;
        }   
    }

    public void AddPosition(ItemType type)
    {

        HandItemPosition position = new HandItemPosition(
            type,
            transform.localEulerAngles,
            transform.localPosition
        );

        handPositions.Add( position );
    }

    internal void Position(HandItem item)
    {
        HandItemPosition position = handPositions.Find(p => p.type == item.GetItemType());

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

            if (GUILayout.Button("Copy tranform ref"))
            {
                HandPositioner positioner = GameObject.FindGameObjectWithTag("HandPosRef").GetComponent<HandPositioner>();
                HandItem item = target.GetComponentInChildren<HandItem>();
                // item.SetPos(posRef.localPosition, posRef.localEulerAngles);
                positioner.AddPosition(item.GetItemType());
            }
        }
    }

}
