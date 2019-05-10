using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRicoshet : MonoBehaviour
{
    LineRenderer line;
    public List<Vector3> startShape = new List<Vector3>();
    List<Vector3> newShape = new List<Vector3>();

    //online code ones
    void Start()
    {
        line = GetComponent<LineRenderer>();
        UpdateShape();
    }

    void Update()
    {
         //Test();
        OnlineRicoshetCodeLol();
    }

    void OnlineRicoshetCodeLol()
    {
        /*
        
        //Debug.Log("Running");
        line.enabled = true;
        int laserReflected = 1; //How many times it got reflected
        int vertexCounter = 1; //How many line segments are there
        bool loopActive = true; //Is the reflecting loop active?
       
        Vector3 laserDirection = transform.forward; //direction of the next laser
        Vector3 lastLaserPosition = transform.localPosition; //origin of the next laser
   
        line.SetVertexCount (1);
        line.SetPosition (0, transform.position);
        RaycastHit hit;

        //neccessary values
        float laserDistance = 0;
        for (int i = 1; i < line.positionCount; i++)
        {
            laserDistance += Vector3.Distance(line.GetPosition(i), line.GetPosition(i - 1));
        }
        float maxBounce = 99;
 
        while (loopActive) {
 
            if (Physics.Raycast (lastLaserPosition, laserDirection, out hit, laserDistance) && hit.transform.gameObject.tag != "Player") {
               
                   // Debug.Log ("Bounce");
                    laserReflected++;
                    vertexCounter += 3;
                    line.SetVertexCount (vertexCounter);
                    line.SetPosition (vertexCounter - 3, Vector3.MoveTowards (hit.point, lastLaserPosition, 0.01f));
                    line.SetPosition (vertexCounter - 2, hit.point);
                    line.SetPosition (vertexCounter - 1, hit.point);
                    line.SetWidth (.1f, .1f);
                    lastLaserPosition = hit.point;
                    laserDirection = Vector3.Reflect (laserDirection, hit.normal);
                } else {
           
                   // Debug.Log ("No Bounce");
                    laserReflected++;
                    vertexCounter++;
                    line.SetVertexCount (vertexCounter);
                    Vector3 lastPos = lastLaserPosition + (laserDirection.normalized * laserDistance);
                   // Debug.Log ("InitialPos " + lastLaserPosition + " Last Pos" + lastPos);
                    line.SetPosition (vertexCounter - 1, lastLaserPosition + (laserDirection.normalized * laserDistance));
 
                    loopActive = false;
                }
            if (laserReflected > maxBounce)
                loopActive = false;
        }
        */
    
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
