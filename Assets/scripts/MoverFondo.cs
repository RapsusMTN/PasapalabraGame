using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoverFondo : MonoBehaviour {

    private Image miImage;
	// Use this for initialization
	void Start () {
        miImage = this.GetComponent<Image>();
        Material m = Instantiate(miImage.material);
        miImage.material = m;
	}
	
	// Update is called once per frame
	void Update () {
        miImage.material.mainTextureOffset = new Vector2(Time.time * 0.03f * 1f, 0f);
    }
}
