using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPercentBar : MonoBehaviour {

	float maxScale;
	public float curPercent = 100;
	RectTransform rect;
	[SerializeField] float lerpSpeed = 1;
	[SerializeField] bool useWitdth = false;

	void Awake () {
		rect = GetComponent<RectTransform> ();
		if (useWitdth == false) {
			maxScale = rect.localScale.x;
		} else {
			maxScale = rect.sizeDelta.x;
		}
	}

	void Update () {
		curPercent = Mathf.Max (curPercent, 0);
		if (useWitdth == false) {
			rect.localScale = new Vector3 (Mathf.Lerp (rect.localScale.x, maxScale * (curPercent / 100), lerpSpeed * Time.unscaledDeltaTime), rect.localScale.y, rect.localScale.z);
		} else {
			rect.sizeDelta = new Vector2(Mathf.MoveTowards (rect.localScale.x, maxScale * (curPercent / 100),Time.deltaTime * lerpSpeed),rect.sizeDelta.y);
		}
	}

	public void SetPercent (float newPercent) {
		curPercent = newPercent;
	}
}