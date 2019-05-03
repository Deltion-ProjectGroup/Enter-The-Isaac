using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(LineRenderer))]
public class LineHurtBox : MonoBehaviour
{
    public float damage = 1;
    public int team = 0;
    [Header("frames are relative to 60 fps, and is not dependant on actual framerate")]
    public int activeFrames = 100;
    public int damageFrames = 10;
    public bool destroyOnHit = true;
    [SerializeField] UnityEvent hitEvent;
    public bool pierce = false;
    LineRenderer line;
    List<Vector3> lastPositions = new List<Vector3>();
    bool shouldUpdate = false;
    [SerializeField] GameObject particles;
    [SerializeField] int particlesToSpawn = 10;
    [SerializeField] GameObject destroyParticles;
    [SerializeField] int _particlesToSpawn = 5;

    void Start()
    {
        StartStuff();
    }

    void StartStuff()
    {
        line = GetComponent<LineRenderer>();
        UpdateShape();
        InvokeRepeating("FrameChecker", ((float)activeFrames / (float)damageFrames) / 60, ((float)activeFrames / (float)damageFrames) / 60);
    }

    void Update()
    {
        if (shouldUpdate == true)
        {
            line.SetPositions(lastPositions.ToArray());
            shouldUpdate = false;
        }
        CompareShape();
        LineHurtbox(true);
    }

    void FrameChecker()
    {
        //also check for destroy stuff lol
        line.SetPositions(lastPositions.ToArray());
        LineHurtbox(false);
        SpawnABunchaParticles();
    }

    void SpawnABunchaParticles()
    {// I love the name sorry for not being proffesional lol
        SpawnParticles(_particlesToSpawn, particles);
    }

    void LineHurtbox(bool ignoreDamage)
    {
        //from linePos 0---1 to 1---2 etc.
        for (int i = 1; i < line.positionCount; i++)
        {
            Ray ray = new Ray(transform.TransformPoint(line.GetPosition(i - 1)), (transform.TransformPoint(line.GetPosition(i)) - transform.TransformPoint(line.GetPosition(i - 1))));
            RaycastHit[] hit = Physics.RaycastAll(ray.origin, ray.direction, Vector3.Distance(line.GetPosition(i), line.GetPosition(i - 1)) * transform.localScale.x);
            if (hit.Length > 0)
            {
                for (int r = 0; r < hit.Length; r++)
                {
                    if (hit[r].transform != null && hit[r].transform.GetComponent<Hitbox>() != null)
                    {
                        Hitbox hitObj = hit[r].transform.GetComponent<Hitbox>();
                        if (team != hitObj.team)
                        {
                            if (ignoreDamage == false)
                            {
                                hitObj.Hit(damage);
                                hitEvent.Invoke();
                                if (destroyOnHit == true)
                                {
                                    Destroy(transform.root.gameObject);
                                }
                            }
                            if (pierce == false)
                            {
                                UpdateShape();
                                line.SetPosition(i, transform.InverseTransformPoint(hit[r].point));
                                for (int i2 = i; i2 < line.positionCount; i2++)
                                {
                                    line.SetPosition(i2, line.GetPosition(i));
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    void CompareShape()
    {
        Vector3[] posses = new Vector3[line.positionCount];
        line.GetPositions(posses);
        for (int i = 0; i < lastPositions.Count; i++)
        {
            if (posses[i] != lastPositions.ToArray()[i])
            {
                UpdateShape();
                break;
            }
        }
    }

    void UpdateShape()
    {
        lastPositions.Clear();
        for (int i = 0; i < line.positionCount; i++)
        {
            lastPositions.Add(line.GetPosition(i));
        }
        shouldUpdate = true;
    }

    void SpawnParticles(float amount, GameObject toSpawn)
    {
        float totalLength = 0;
        for (int i = 1; i < line.positionCount; i++)
        {
            totalLength += Vector3.Distance(line.GetPosition(i - 1), line.GetPosition(i));
        }

        Vector3[] posses = new Vector3[line.positionCount];
        line.GetPositions(posses);
        for (int i = 0; i < amount; i++)
        {
            float percent = (totalLength - (totalLength / amount) * i) / totalLength;
            Vector3 spawnPos = FindPoint(posses, totalLength, percent);
            spawnPos = transform.TransformPoint(spawnPos);
            Instantiate(toSpawn, spawnPos, transform.rotation);
        }
    }



    //this function is copy pasted from the internet
    Vector3 FindPoint(Vector3[] vectors /* your vector array */, float length /* length of curve */, float p /* percentage along the line 0 to 1 */)
    {
        if (vectors.Length < 1)
        {
            return Vector3.zero; // if the list is empty
        }
        else if (vectors.Length < 2)
        {
            return vectors[0]; //if there is only one point in the list
        }

        float dist = length * Mathf.Clamp(p, 0, 1);
        Vector3 pos = vectors[0];

        for (int i = 0; i < vectors.Length - 1; i++)
        {
            Vector3 v0 = vectors[i];
            Vector3 v1 = vectors[i + 1];
            float this_dist = (v1 - v0).magnitude;

            if (this_dist < dist)
            {
                dist -= this_dist; //if the remaining distance is more than the distance between these vectors then minus the current distance from the remaining and go to the next vector
                continue;
            }

            return Vector3.Lerp(v0, v1, dist / this_dist); //if the distance between these vectors is more or equal to the remaining distance then find how far along the gap it is and return
        }
        return vectors[vectors.Length - 1];
    }
}
