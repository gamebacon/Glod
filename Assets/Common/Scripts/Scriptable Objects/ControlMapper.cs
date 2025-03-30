using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Controlmapper", menuName = "ScriptableObjects/ControlMapper")]
public class ControlMapper : ScriptableObject
{
    [SerializeField] public List<Control> controls;
}