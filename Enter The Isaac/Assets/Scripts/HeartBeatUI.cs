using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeatUI : MonoBehaviour
{

    [SerializeField] RectTransform[] hearts;
    [SerializeField] float rate = 0.1f;
	[SerializeField] float scale = 1.25f;
	Vector3 startScale;

    void Start()
    {
		startScale = hearts[0].localScale;
        InvokeRepeating("ScaleHeart", 0, rate);
    }

    void ScaleHeart()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].localScale = startScale * scale;
        }
    }

	void Update(){
		 for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].localScale = Vector3.Lerp(hearts[i].localScale,startScale,Time.deltaTime * 10);
        }
	}
}
