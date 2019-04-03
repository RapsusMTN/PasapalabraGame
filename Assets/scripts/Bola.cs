using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Clase bola, contiene el objeto de unity y su pregunta asociada
public class Bola
{
    private GameObject objetoBola;
    private Pregunta datosPregunta;

    public GameObject ObjetoBola
    {
        get { return objetoBola; }
        set { objetoBola = value; }
    }
    public Pregunta DatosPregunta
    {
        get { return datosPregunta; }
        set { datosPregunta = value; }
    }

}
