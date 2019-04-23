using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class LineHurtBox : MonoBehaviour
{
    public float damage = 1;
    public int team = 0;
    [SerializeField] bool destroyOnHit = true;
    [SerializeField] UnityEvent hitEvent;
    [SerializeField] bool useLineRenderer = true;
    [SerializeField] bool pierce = false;
    LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        for (int i = 1; i < line.positionCount; i++)
        {
            RaycastHit hit;
            Debug.DrawRay(line.GetPosition(i - 1) + transform.position, (line.GetPosition(i) + transform.position) - (line.GetPosition(i - 1) + transform.position),Color.red,Time.deltaTime);
            if (Physics.Raycast(line.GetPosition(i - 1) + transform.position, (line.GetPosition(i - 1) + transform.position) - (line.GetPosition(i) + transform.position), out hit, Vector3.Distance(line.GetPosition(i), line.GetPosition(i - 1))))
            {
                if (hit.transform.GetComponent<Hitbox>() != null)
                {
                    Hitbox hitObj = hit.transform.GetComponent<Hitbox>();
                    if (team != hitObj.team)
                    {
                        hitObj.Hit(damage);
                        hitEvent.Invoke();
                        if(destroyOnHit == true){
                            Destroy(transform.root.gameObject);
                        }
                        if (pierce == false)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
