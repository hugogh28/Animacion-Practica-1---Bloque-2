using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MassSpringCloth : MonoBehaviour
{
    public bool paused; //Booleano que nos servirá para pausar la animación

    public enum Integration //Los diferentes métodos de integración disponibles
    {
        ExplicitEuler = 0,
        SymplecticEuler = 1
    }

    public Integration integrationMethod; //Este será el método de integración escogido

    public float h = 0.1f; //El paso de integración

    public List<Spring> springs; //Quizás deba ponerse en el método Start()

    public List<Node> ListOfNodes;

    public Vector3 g = new Vector3(0f, 9.8f, 0f); //El valor de la gravedad aplicado al objeto masa-muelle (está en m/s)

    public float k = 100f; //Constante de rigidez de los muelles

    void Start()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh; //Se guarda en la variable mesh el mallado del objeto

        Vector3[] vertices = mesh.vertices; //Se guardan en un array todos los vértices de la mesh

        Debug.Log(vertices.Count());

        List<Node> nodes = new List<Node>(vertices.Length); //Se crea una lista de nodos cuyo tamaño sea el de los vértices de la mesh

        for(int i = 0; i < vertices.Length; i++)
        {
            nodes.Add(new Node(vertices[i])); //Cada vez que se itera sobre el bucle de vértices de la mesh, se añade un nuevo nodo, cuya posición corresponde a la de su vértice
        }

        ListOfNodes = nodes;

        int[] triangles = mesh.triangles; //Se guardan en un array todos los triángulos de la mesh

        Debug.Log(triangles.Count());

        if (nodes.Count == 0)
            Debug.Log("Son nulos");
        else
            Debug.Log(nodes.Count());

        Gizmos.color = Color.red;
        //Gizmos.matrix = transform.localToWorldMatrix;

        for (int i = 0; i < triangles.Length; i++) //Por corregir, no es una forma limpia de añadir gizmos y springs
        {
            if (!(triangles[i] == triangles.Length - 3) && !(triangles[i] == triangles.Length - 2))
            {
                //Como cada triángulo contiene 3 integers, cada uno de ellos, correspondiente a un vértice, se deberá relacionar dichos índices con los vértices

                springs.Add(new Spring(k, nodes[triangles[i]], nodes[triangles[i + 1]])); //Se deben corresponder los índices de cada triángulo con cada nodo, es decir, que se debe tener en cuenta que cada nodo puede estar en más de un triángulo
                Gizmos.DrawLine(nodes[triangles[i]].pos, nodes[triangles[i + 1]].pos); //Las líneas deben dibujarse sobre los muelles, por lo que cuando se deduzcan los nodos conectados, se pintarán sus muelles
                springs.Add(new Spring(k, nodes[triangles[i]], nodes[triangles[i + 2]]));
                Gizmos.DrawLine(nodes[triangles[i]].pos, nodes[triangles[i + 2]].pos);
            }
            else if (!(triangles[i] == triangles.Length - 2))
            {
                springs.Add(new Spring(k, nodes[i], nodes[i + 1]));
                Gizmos.DrawLine(nodes[triangles[i]].pos, nodes[triangles[i + 1]].pos);
            }
        }

    }

    private void OnDrawGizmos()
    {
        //Dibujado de los gizmos de los nodos en coordenadas globales

        Gizmos.color = Color.green;
        Gizmos.matrix = transform.localToWorldMatrix;
        foreach (Node node in ListOfNodes)
        {
            Gizmos.DrawSphere(node.pos, 0.2f);
        }

        Gizmos.color = Color.red;
        foreach(Spring spring in springs)
        {
            Gizmos.DrawLine(spring.nodeA.pos, spring.nodeB.pos);
        }

        /*foreach (Spring in springs) 
        {
            Gizmos.DrawLine();
        }*/

    }

    Vector3 VectorBetweenNodes(Node A, Node B) //Calcula la distancia entre dos nodos 
    {
        return new Vector3(B.pos.x - A.pos.x, B.pos.y - A.pos.y, B.pos.z - A.pos.z);
    }

    void Update()
    {

    }
}
