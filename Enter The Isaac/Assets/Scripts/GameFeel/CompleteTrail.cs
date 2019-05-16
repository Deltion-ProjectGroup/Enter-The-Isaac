using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteTrail : MonoBehaviour
{
    [SerializeField] TrailRenderer trail;
    void OnDestroy()
    {
        if (trail != null)
        {
            trail.transform.parent = null;
            trail.autodestruct = true;
            trail = null;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
