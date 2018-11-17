using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Holder", menuName = "Inventory/Holder")]
public class PrefabHolder : ScriptableObject
{
    public GameObject Coin;
    public bool[] States;
    public int CurrentGrade;
    public string Key;
}
