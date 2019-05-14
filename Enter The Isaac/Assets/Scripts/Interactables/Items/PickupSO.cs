using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemPickup", menuName = "New ItemPickup")]
public class PickupSO : EntitySO
{
    public GameObject entityPrefab;

    public enum ItemType
    {
        Passive, Active, Weapon
    }
}
