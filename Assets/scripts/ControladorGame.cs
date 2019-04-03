using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text;

public class ControladorGame : MonoBehaviour
{

    public InputField textoRespuesta;
    public Text textoPregunta;
    public Text textoPresentador;
    public Sprite amarillo;
    public Sprite rojo;
    public Sprite verde;
    public Sprite azul;
    public GameObject concursante;
    public GameObject presentador;
    public GameObject bolaUnity;
    public GameObject botonAceptar;
    public GameObject botonPasapalabra;
    public GameObject botonGuardar;
    public GameObject botonMenu;
    public Text temporizador;//texto del temporizador en pantalla
    public Text aciertos;//texto de los aciertos en la partida
    public ControladorEscenas cambiarescena;

    private List<Bola> listaBolas = new List<Bola>();
    private List<Pregunta> Datos = new List<Pregunta>();

    [SerializeField]
    private List<Pregunta> DatosSinOrdenar = new List<Pregunta>();

    [SerializeField]
    private List<Puntuaciones> Punts = new List<Puntuaciones>();

    private Bola bolaActiva;
    private float tiempoInicial = 150;//Tiempo inicial del temporizador
    private int contadorPreguntas = 0;
    private int juegoFinal = 0;
    private float ContAnimacion;


    Puntuaciones punts = new Puntuaciones();

    // Use this for initialization
    void Start()
    {
        DeserializarPreguntas();
        OrdenarPreguntas();
        DeserializarRanking();
        CrearCirculos();
        CalcularCirculo(listaBolas, Vector3.zero, 170f);
    }

    // Update is called once per frame
    void Update()
    {

        if (IsJuegoFinalizado())
        {
            DesactivarJuego();
        }
        else
        {
            UpdateBola();
            Cronometro();
            if (Input.GetKeyDown(KeyCode.Return)) //al pulsar ENTER (ACEPTAR)
            {
                ComprobarPregunta(1);
            }
        }

    }

    public void ObtenerFocoTextRespuesta()
    {
        textoRespuesta.ActivateInputField();
    }

    private bool IsJuegoFinalizado()
    {
        if (tiempoInicial <= 0 || juegoFinal >= listaBolas.Count)
        {
            return true;
        }

        return false;
    }

    private void DesactivarJuego()//Se ejecuta al finalizar el juego para desactivar los componentes
    {
        botonAceptar.SetActive(false);
        botonPasapalabra.SetActive(false);
        botonGuardar.SetActive(true);
        botonMenu.SetActive(true);

        textoPregunta.text = "Juego finalizado";
        if (textoRespuesta.text.Length <= 0)
        {
            textoPresentador.text = "Debes introducir tu nombre para guardar tu puntuación!";
        }
        else
        {
            textoPresentador.text = "¿Guardar tu puntuación en el ranking?";
        }
        punts.tiempoRestante = (int)tiempoInicial;

    }

    public void BtnGuardar()
    {
        if (textoRespuesta.text.Length > 0)
        {
            punts.nombre = textoRespuesta.text;
            Punts.Add(punts);//añade a la lista
            SerializarRanking();
            cambiarescena = new ControladorEscenas();
            cambiarescena.Ranking();
        }

    }

    private void DeserializarPreguntas()
    {
        var textfile = Resources.Load<TextAsset>("Text/datosPreguntas");
        Pregunta[] mispreguntas = JsonHelper.FromJson<Pregunta>(textfile.text);
        foreach (Pregunta item in mispreguntas)
        {
            DatosSinOrdenar.Add(item);
        }
    }
    
    private void OrdenarPreguntas()
    {
        List<Pregunta> DatosAux = new List<Pregunta>();

        for (int i = 0; i < 27; i++)
        {

            DatosAux.Clear();

            foreach (Pregunta p in DatosSinOrdenar)
            {
                if (p.IdPregunta == i)
                {
                    DatosAux.Add(p);
                }
            }

            int total = DatosAux.Count;
            int numRand = Random.Range(0, total);
            Datos.Add(DatosAux[numRand]);
        }
    }

    //private void BorrarUltimaFila(string ruta)
    //{
    //    List<string> lines = File.ReadAllLines(ruta).ToList();
    //    File.WriteAllLines(ruta, lines.GetRange(0, lines.Count - 1).ToArray());
    //}

    private void SerializarRanking()
    {
        File.Delete(Application.persistentDataPath + "/datosRanking.json");
        FileStream archivo = File.Create(Application.persistentDataPath + "/datosRanking.json");
        string json = JsonHelper.ToJson<Puntuaciones>(Punts.ToArray());
        byte[] b = new UTF8Encoding(true).GetBytes(json);
        archivo.Write(b, 0, b.Length);

    }

    private void DeserializarRanking()
    {
        Debug.Log(Application.persistentDataPath + "/datosRanking.json");

        if (File.Exists(Application.persistentDataPath + "/datosRanking.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/datosRanking.json");
            Puntuaciones[] mispreguntas = JsonHelper.FromJson<Puntuaciones>(json);
            foreach (Puntuaciones item in mispreguntas)
            {
                Punts.Add(item);
            }

        }
        else
        {
            FileStream archivo = File.Create(Application.persistentDataPath + "/datosRanking.json");
            Punts = new List<Puntuaciones>();
            string json = JsonHelper.ToJson<Puntuaciones>(Punts.ToArray());
            byte[] b = new UTF8Encoding(true).GetBytes(json);
            archivo.Write(b, 0, b.Length);

        }
    }
    
    private void CrearCirculos()
    {
        for (int i = 0; i < 27; i++)
        {
            GameObject bolaObject = GameObject.Instantiate(bolaUnity, gameObject.transform);
            bolaObject.GetComponentInChildren<Text>().text = Datos[i].Letra;
            listaBolas.Add(new Bola() { ObjetoBola = bolaObject, DatosPregunta = Datos[i] });
        }
        bolaActiva = listaBolas[0];
        textoPregunta.text = bolaActiva.DatosPregunta.Enunciado.ToString();
        textoPresentador.text = bolaActiva.DatosPregunta.Presentador.ToString();
    }

    private void Cronometro()
    {
        //Temporizador cuenta atras!
        if (tiempoInicial > 0)
        {
            tiempoInicial -= Time.deltaTime;
            temporizador.text = tiempoInicial.ToString("f0");
        }

    }

    private void CalcularCirculo(List<Bola> Obj, Vector3 centro, float radio)
    {
        float cantidad = 360.0f / Obj.Count;
        float posInicial = 90.0f;
        for (int i = 0; i < Obj.Count; i++)
        {
            Vector3 pos = Obj[i].ObjetoBola.transform.localPosition;
            pos.x = (radio * Mathf.Cos(posInicial * Mathf.PI / 180)) + centro.x;
            pos.y = (radio * Mathf.Sin(posInicial * Mathf.PI / 180)) + centro.y;
            pos.z = 0f;
            Obj[i].ObjetoBola.transform.localPosition = pos;

            posInicial -= cantidad;
        }
    }

    private void UpdateBola()
    {
        if (bolaActiva.DatosPregunta.Estado == 0)
        {
            ContAnimacion += Time.deltaTime;
            if (ContAnimacion > 0.5f)
            {
                ContAnimacion = 0;
                if (bolaActiva.ObjetoBola.gameObject.GetComponent<Image>().sprite == azul)
                {
                    bolaActiva.ObjetoBola.gameObject.GetComponent<Image>().sprite = amarillo;
                }
                else
                {
                    bolaActiva.ObjetoBola.gameObject.GetComponent<Image>().sprite = azul;
                }
            }
        }

    }

    private void ComprobarPregunta(int btn)
    {
        //Boton Aceptar
        if (btn == 1)
        {
            ComprobarRespuesta();
            textoRespuesta.text = "";
            textoRespuesta.Select();
        }
        //Boton Pasapalabra
        else if (btn == 2)
        {
            textoRespuesta.text = "";
            textoRespuesta.Select();
            if (juegoFinal < 27)
            {
                bolaActiva.ObjetoBola.gameObject.GetComponent<Image>().sprite = azul;
            }
        }


        ComprobarEstado();

        if (juegoFinal < 27)
        {
            bolaActiva = listaBolas[contadorPreguntas];
            textoPregunta.text = bolaActiva.DatosPregunta.Enunciado.ToString();
            textoPresentador.text = bolaActiva.DatosPregunta.Presentador.ToString();
        }

    }

    private bool RespuestaMultiple()
    {
        string respuestas = bolaActiva.DatosPregunta.Respuesta.ToLower();
        string[] tokens = respuestas.Split('#');
        foreach (string resp in tokens)
        {
            if (textoRespuesta.text.ToLower().Equals(resp))
            {
                return true;
            }
        }
        return false;
    }

    private void ComprobarRespuesta()
    {
        textoRespuesta.Select();
        if (RespuestaMultiple())
        {
            //Acertado        
            bolaActiva.DatosPregunta.Estado = 1;
            bolaActiva.ObjetoBola.gameObject.GetComponent<Image>().sprite = verde;
            concursante.GetComponent<Animator>().Play("AnimaAcertar");
            presentador.GetComponent<Animator>().Play("PresenAcertar");
            punts.aciertos++;//Guarda los aciertos en la clase puntuaciones
            aciertos.text = punts.aciertos.ToString();
        }
        else
        {
            //Fallado
            bolaActiva.DatosPregunta.Estado = 2;
            bolaActiva.ObjetoBola.gameObject.GetComponent<Image>().sprite = rojo;
            concursante.GetComponent<Animator>().Play("AnimaFallar");
            presentador.GetComponent<Animator>().Play("PresenFallar");
            punts.fallos++;//Guarda los fallos en la clase puntuaciones
        }
        juegoFinal++;

    }

    private void ComprobarEstado()
    {
        bool bandera = false;
        if (juegoFinal < 27)
        {
            while (!bandera)
            {
                contadorPreguntas++;
                if (contadorPreguntas > 26) contadorPreguntas = 0;
                if (listaBolas[contadorPreguntas].DatosPregunta.Estado == 0)
                {
                    bolaActiva = listaBolas[contadorPreguntas];
                    bandera = true;
                }
            }
        }
    }

}
