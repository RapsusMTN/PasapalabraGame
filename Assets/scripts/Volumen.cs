using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volumen : MonoBehaviour {

    public Slider slider;
    public AudioSource volumenAudio;

    public void ValueChangeCheck()
    {
        //Volumen de la musica controlado por el slider
        AudioListener.volume = slider.value;
    }

    // Use this for initialization
    void Start () {
        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
	}

    // Update is called once per frame
    void Update () {
		
	}
}
