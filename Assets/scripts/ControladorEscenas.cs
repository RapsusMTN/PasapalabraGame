using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorEscenas : MonoBehaviour {

    public void Inicio()
    {
        SceneManager.LoadScene("Intro");
    }

	public void NuevoJuego()    
    {
        SceneManager.LoadScene("Juego");
    }

    public void Ranking()
    {
        SceneManager.LoadScene("Ranking");
    }

    public void Salir()
    {
        Application.Quit();
    }

}
