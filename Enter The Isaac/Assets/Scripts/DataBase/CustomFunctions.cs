using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomFunctions
{
    public static GameObject GetAbsoluteParent(this GameObject objectToCheck)
    {
        if(objectToCheck.transform.parent != null)
        {
            return objectToCheck.transform.parent.gameObject.GetAbsoluteParent();
        }
        else
        {
            return objectToCheck;
        }
    }
}
