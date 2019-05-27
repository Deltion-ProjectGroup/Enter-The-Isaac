using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonConnectionPoint : MonoBehaviour
{
    public BaseRoom ownerRoom;
    public GameObject pointConnectedTo;
    public GameObject objectToReplace;
    public ConnectionDirection direction;
    public int id;


    public enum ConnectionDirection {Left, Down, Right, Up }

    public DungeonConnectionPoint(GameObject parentedConnectionPoint)
    {
        pointConnectedTo = parentedConnectionPoint;
    }
}
