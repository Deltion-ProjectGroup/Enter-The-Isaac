using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PickupData", menuName = "New PickupData")]
public class PickupSO : EntitySO
{
    public GameObject entityPrefab;

    public enum ItemType
    {
        Passive, Active, Weapon
    }
}
