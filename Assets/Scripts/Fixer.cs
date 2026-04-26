using UnityEngine;

public class Fixer : MonoBehaviour
{
    public Vector3 pos; //Posición que tendrá el fixer en el mundo
    public bool hidden; //Variable booleano que marcará si se oculta el fixer (true) o no (false)

    void Start()
    {
        pos = GetComponentInParent<Transform>().localPosition; //Se asigna la posición (al fixer) que tiene el objeto (en las coordenadas locales del objeto Tela)
                                                               //Es importante que el (o los) fixer se encuentre como hijo del objeto que tenga el script MassSpringCloth
                                                               //De lo contrario, no sucederá lo que se busca
        this.GetComponent<MeshRenderer>().enabled = true; //Se muestra el fixer en primera instancia, para ocultarlo se podrá pulsar la tecla 'H' o presionar sobre el tick del panel
        Debug.Log("Posición del fixer: " + pos); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S)){ //Al pulsar la tecla H se podrá ocultar o mostrar todos los fixers
            hidden = !hidden;
            Hide();
        }
        Hide(); //Esto permitirá manejar que se muestre (u oculte) solo un fixer, usando el tick de su interfaz
                //esta función puede desincronizar el cómo se ven los fixers

    }

    void Hide() //Mediante este método, se gestiona el ocultamiento del fixer
    {
        if(hidden) //Si se le ha indicado que debe ocultarse, se ocultará
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        else //Si se le ha indicado que debe mostrarse, se mostrará
            gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
}
