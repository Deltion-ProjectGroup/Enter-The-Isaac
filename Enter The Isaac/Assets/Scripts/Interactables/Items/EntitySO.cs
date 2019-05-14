using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EntityData", menuName = "New EntityData")]
public class EntitySO : ScriptableObject
{
    public string entityName;
    public Sprite entityImage;

    public string funText;
    public string description;
}
