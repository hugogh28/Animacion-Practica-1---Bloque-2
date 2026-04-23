using UnityEngine;

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

        //Para hallar la distancia por defecto de un muelle, se debe calcular para cada uno
    }

    /*float DistanceBetweenNodes() 
    {
        
    }*/

}
