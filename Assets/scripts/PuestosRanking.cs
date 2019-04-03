using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PuestosRanking : MonoBehaviour
{
    public GameObject rankingVacio;
    public GameObject fila;
    private List<GameObject> filasRanking = new List<GameObject>();

    [SerializeField]
    private List<Puntuaciones> Punts = new List<Puntuaciones>();


    // Use this for initialization
    void Start()
    {
        DeserializarRanking();
        OrdenarRanking();
        GenerarRanking();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DeserializarRanking()
    {
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

    private void OrdenarRanking()
    {
        Punts.Sort(
            delegate (Puntuaciones p1, Puntuaciones p2)
            {
                if (p2.aciertos != p1.aciertos)
                {
                    return p2.aciertos.CompareTo(p1.aciertos);
                }
                else if (p2.aciertos == p1.aciertos && p2.fallos != p1.fallos)
                {
                    return p2.fallos.CompareTo(p1.fallos);
                }
                return p2.tiempoRestante.CompareTo(p1.tiempoRestante);

            }
        );
    }

    private void GenerarRanking()
    {
        if (Punts.Count == 0)
        {
            GameObject rvacio = GameObject.Instantiate(rankingVacio, gameObject.transform);
            rvacio.transform.localPosition = new Vector3(-191f, 173f, 0f);
            rvacio.GetComponentInChildren<Text>().text = "El Ranking Está Vacío";
        }

        for (int i = 0; i < Punts.Count && i < 5; i++)
        {
            filasRanking.Add(GameObject.Instantiate(fila, gameObject.transform));
        }

        for (int i = 0; i < Punts.Count && i < 5; i++)
        {
            Component[] array = filasRanking[i].GetComponentsInChildren<Image>();
            array[0].GetComponentInChildren<Text>().text = (i + 1).ToString();
            array[1].GetComponentInChildren<Text>().text = Punts[i].nombre;
            array[2].GetComponentInChildren<Text>().text = Punts[i].aciertos.ToString();
            array[3].GetComponentInChildren<Text>().text = Punts[i].fallos.ToString();
        }

        float cantidad = 90f;
        float posInicial = 30f;
        for (int i = 0; i < filasRanking.Count; i++)
        {
            Vector3 pos = filasRanking[i].transform.localPosition;
            pos.x = 30;
            pos.y = posInicial;
            pos.z = 0;
            filasRanking[i].transform.localPosition = pos;

            posInicial -= cantidad;
        }

    }

}

