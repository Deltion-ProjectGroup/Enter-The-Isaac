using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRicoshet : MonoBehaviour
{
    LineRenderer line;
    public List<Vector3> startShape = new List<Vector3>();
    List<Vector3> newShape = new List<Vector3>();
    void Start()
    {
        line = GetComponent<LineRenderer>();
        UpdateShape();
    }

    void Update()
    {
        Test();
    }

    void UpdateShape()
    {
        startShape.Clear();
        for (int i = 0; i < line.positionCount; i++)
        {
            startShape.Add(line.GetPosition(i));
        }
    }

    Vector3 DirToDir(Vector3 dir1, Vector3 dir2)
    {
        //I modified it so its my own lol, thanks internet guy, I had a lotta trouble with this
        var inputDirection = dir1;
        var lookingDirection = dir2;

        var angle = Vector3.Angle(lookingDirection, Vector3.forward);
        var cross = Vector3.Cross(lookingDirection, Vector3.forward);
        if (cross.y < 0)
            angle = -angle;

        var lookingRotation = Quaternion.AngleAxis(angle, Vector3.up);
        var inputRotation = lookingRotation * inputDirection;
        return inputRotation;
    }

    void Test()
    {
        Vector3 currentForward = Vector3.forward;
        Vector3[] startPosWHits = startShape.ToArray();
        for (int i = 0; i < startShape.Count; i++)
        {
            startPosWHits[i] = transform.TransformDirection(startPosWHits[i]) + transform.position;
        }

        Color drawColor = Color.green;
        for (int i = 0; i < startPosWHits.Length - 1; i++)
        {
            //this makes the postions relative to the reflected normal,exept its not here yet

            Vector3 rayDir = startPosWHits[i] - startPosWHits[i + 1];
            rayDir = -rayDir;
            Debug.DrawRay(startPosWHits[i], rayDir, drawColor, Time.deltaTime);
            drawColor = Color.green;
            Debug.DrawRay(startPosWHits[i], Vector3.up, Color.red, Time.deltaTime);



            float distance = Vector3.Distance(startPosWHits[i], startPosWHits[i + 1]);


            RaycastHit[] hits = Physics.RaycastAll(startPosWHits[i], rayDir, distance, ~LayerMask.GetMask("IgnoreRaycast"), QueryTriggerInteraction.Ignore);

            if (hits.Length > 0)
            {
                bool cancel = true;
                RaycastHit closestHit = hits[0];
                for (int h = 0; h < hits.Length; h++)
                {
                    if (hits[h].transform.tag != "Player" && hits[h].point != startPosWHits[i])
                    {
                        if (Vector3.Distance(startPosWHits[i], hits[h].point) < Vector3.Distance(startPosWHits[i], closestHit.point))
                        {
                            closestHit = hits[h];
                            cancel = false;
                        }
                    }
                }
                if (closestHit.transform.tag != "Player" && closestHit.point != startPosWHits[i])
                {
                    cancel = false;
                }
                if (cancel == false)
                {
                    drawColor = Color.blue;
                    List<Vector3> newPos = new List<Vector3>();
                    newPos.AddRange(startPosWHits);
                    newPos.Insert(i + 1, closestHit.point);
                    startPosWHits = newPos.ToArray();

                    Vector3 hitDir = Vector3.zero;
                    hitDir = Vector3.Reflect(rayDir, closestHit.normal);
                    Debug.DrawRay(closestHit.point, hitDir.normalized, Color.magenta, Time.deltaTime);
                    currentForward = new Vector3(hitDir.z, hitDir.y, hitDir.x);
                }

            }

        }
    }
}
