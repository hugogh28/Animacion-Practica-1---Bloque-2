using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Android;

public class Spring 
{
    public float k = 100f; // Constante de rigidez del muelle (N/m)
    public float length0; // Longitud natural del muelle (ahí la fuerza elástica
                          // se anula)
    public float length; // Longitud del muelle en un momento dado
    public Vector3 pos; // Posición 3D del punto medio del muelle
    public Vector3 u; // Vector unitario con la dirección del muelle que
                      // apunta de B a A
    public float defaultSize = 2f; // Longitud natural de los cilindros en
                                   // Unity (m)
    public Quaternion rotation; // Nos permitirá calcular la orientación del
                                // muelle  
    public Node nodeA; // Primer extremo del muelle
    public Node nodeB;

    public Spring(float cElasticity, Node A, Node B)
    {
        k = cElasticity;
        nodeA = A; 
        nodeB = B;
        u = VectorBetweenNodes(A, B);
        length0 = u.magnitude;
        u = Vector3.Normalize(u);
        pos = (A.pos + B.pos) / 2f;
        rotation = Quaternion.FromToRotation(Vector3.up, u);
        //Para hallar la distancia por defecto de un muelle, se debe calcular para cada uno
    }

    Vector3 VectorBetweenNodes(Node A, Node B) //Calcula el vector entre dos nodos 
    {
        return new Vector3(B.pos.x - A.pos.x, B.pos.y - A.pos.y, B.pos.z - A.pos.z);
    }

    /*float DistanceBetweenNodes() 
    {
        
    }*/
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            paused = !paused;
        }
    }

    private void FixedUpdate()
    {
        if (paused)
            return;

        switch (integrationMethod)
        {
            case Integration.ExplicitEuler:
                integrateExplicitEuler();
                break;
            case Integration.SymplecticEuler:
                integrateSymplecticEuler();
                break;
            default:
                print("ERROR METODO DE INTEGRACION DESCONOCIDO");
                break;
        }

        u = VectorBetweenNodes(nodeA, nodeB);
        length0 = u.magnitude;
        u = Vector3.Normalize(u);
        pos = (nodeA.pos + nodeB.pos) / 2f;
        rotation = Quaternion.FromToRotation(Vector3.up, u);
    }
}
