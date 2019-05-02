using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeatUI : MonoBehaviour
{

    List<RectTransform> hearts = new List<RectTransform>();
    [Header("script scales its ui children to make heartbeat effect")]
    [SerializeField] float rate = 0.1f;
	[SerializeField] float scale = 1.25f;
	Vector3 startScale;

    void Start()
    {
        hearts.Clear();
        hearts.AddRange(transform.GetComponentsInChildren<RectTransform>());
        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i].transform.parent != transform)
            {
                hearts.RemoveAt(i);
                i = 0;
            }
        }
		startScale = hearts[0].localScale;
        InvokeRepeating("ScaleHeart", 0, rate);
    }

    void ScaleHeart()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].localScale = startScale * scale;
        }
    }

	void Update(){
		 for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].localScale = Vector3.Lerp(hearts[i].localScale,startScale,Time.deltaTime * 10);
        }
	}
}
