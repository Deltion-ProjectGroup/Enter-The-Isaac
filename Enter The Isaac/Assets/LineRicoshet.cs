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
        //set startshape to current shape
        //UpdateShape();
        line.positionCount = startShape.Count;
        line.SetPositions(startShape.ToArray());
        // UpdateShape();
        //clear last newshape, dont need it anymore

        newShape.Clear();

        Vector3 currentForward = Vector3.forward;


        newShape.Add(startShape[0]);
        List<Vector3> startWithInbetweens = startShape;
        for (int i = 1; i < startWithInbetweens.ToArray().Length; i++)
        {
            Ray ray = new Ray(transform.TransformPoint(startWithInbetweens[i - 1]), (transform.TransformPoint(startWithInbetweens[i]) - transform.TransformPoint(startWithInbetweens[i - 1])));
            Debug.DrawRay(ray.origin, Vector3.up, Color.green, Time.deltaTime);
            RaycastHit[] hit = Physics.RaycastAll(ray.origin, ray.direction, Vector3.Distance(startWithInbetweens[i], startWithInbetweens[i - 1]) * transform.localScale.x);
            Vector3 helper = Vector3.forward * Vector3.Distance(line.GetPosition(i), startWithInbetweens[i - 1]) * transform.localScale.x;
            //above this line chunck works
            bool shouldIPlus = false;
            if (hit.Length > 0)
            {
                for (int i2 = 0; i2 < hit.Length; i2++)
                {
                    if (hit[i2].transform.GetComponent<Hitbox>() == null && hit[i2].transform.GetComponent<Hurtbox>() == null && hit[i2].transform.GetComponent<LineHurtBox>() == null)
                    {
                        startWithInbetweens.Insert(i2, transform.InverseTransformPoint(hit[i2].point));
                        shouldIPlus = true;
                        /*
                        Vector3 hitDir = Vector3.Reflect(ray.direction, hit[i2].normal);
                        Debug.DrawRay(hit[i2].point, hitDir, Color.red, Time.deltaTime);
                        currentForward = hitDir;

                        float distanceLeft = Vector3.Distance(line.GetPosition(i), line.GetPosition(i - 1)) * transform.localScale.x - Vector3.Distance(hit[i2].point, line.GetPosition(i - 1) + transform.position);
                        hitDir *= distanceLeft;

                        newShape.Add(transform.InverseTransformPoint(hit[i2].point));
                        helper = transform.InverseTransformPoint(hit[i2].point + hitDir);
                        i2 = hit.Length;
                        */
                    }
                }
            }
            //raycast end    
            newShape.Add(startShape[i]);
            startWithInbetweens[i] = newShape[i];
            if(shouldIPlus == true){
                i++;
            }

        }

        print(newShape.Count);
        line.positionCount = newShape.ToArray().Length;
        line.SetPositions(newShape.ToArray());
    }
}
