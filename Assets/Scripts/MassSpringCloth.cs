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

    public List<Spring> ListOfSprings; //Lista de muelles
    bool springListIsFull = false; //Booleano para comprobar si la lista de muelles está llena
    bool nodeListIsFull = false; //Booleano para comprobar si la lista de nodos está llena

    public List<Node> ListOfNodes; //Lista de nodos

    public Vector3 g = new Vector3(0f, 9.8f, 0f); //El valor de la gravedad aplicado al objeto masa-muelle (está en m/s)

    public float kT = 100f; //Constante de rigidez de los muelles de tracción
    public float kF = 100f; //Constante de rigidez de los muelles de flexión


    void Start()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh; //Se guarda en la variable mesh el mallado del objeto

        Vector3[] vertices = mesh.vertices; //Se guardan en un array todos los vértices de la mesh

        List<Node> nodes = new List<Node>(vertices.Length); //Se crea una lista de nodos cuyo tamaño sea el de los vértices de la mesh
        List<Spring> springs = new List<Spring>(); //Se crea una lista de muelles cuyo tamaño es indefinido (ya que se presupone que podemos usar cualquier bandera)

        for(int i = 0; i < vertices.Length; i++) //Se itera tantas veces como vértices hay en el array vertices
        {
            nodes.Add(new Node(vertices[i])); //Cada vez que se itera sobre el bucle de vértices de la mesh, se añade un nuevo nodo, cuya posición corresponde a la de su vértice
        }
        nodeListIsFull = true; //Se activa el booleano nodeListIsFull cuando la lista de nodos se ha llenado con todos los elementos del objeto

        ListOfNodes = nodes; //Para poder hacer uso de OnDrawGizmos() se pasa la lista nodes a ListOfNodes



        int[] triangles = mesh.triangles; //Se guardan en un array todos los triángulos de la mesh

        for (int i = 0; i < triangles.Length - 3; i++)  //Se itera tantas veces como vértices (de cada triángulo) hay en el array triangles
        {
            springs.Add(new Spring(kT, nodes[triangles[i]], nodes[triangles[i + 1]])); //Añade un muelle entre el primer y segundo vértice del triángulo
            springs.Add(new Spring(kT, nodes[triangles[i]], nodes[triangles[i + 2]])); //Añade un muelle entre el primer y tercer vértice del triángulo
            springs.Add(new Spring(kT, nodes[triangles[i + 1]], nodes[triangles[i + 2]])); //Añade un muelle entre el segundo y tercer vértice del triángulo
        }
        springListIsFull = true; //Se activa el booleano springListIsFull cuando la lista de muelles se ha llenado con todos los elementos del objeto

        ListOfSprings = springs; //Para poder hacer uso de OnDrawGizmos() se pasa la lista springs a ListOfSprings
    }

    private void OnDrawGizmos()
    {
        //Dibujado de los gizmos de los nodos en coordenadas globales
        Gizmos.matrix = transform.localToWorldMatrix; //Se hace el paso de coordenadas locales a globales para evitgar que los gizmos se pinten en otro lugar que no sea el nodo correspondiente
        DrawIfNotNull(); //Se trata de dibujar los gizmos
    }

    //Para evitar llamadas a elementos no existentes usamos el método DrawIfNotNull, que comprueba si las listas de nodos y muelles han sido inizialidas y llenadas para evitar
    //rellenar Gizmos que no existen
    void DrawIfNotNull()
    {
        if (nodeListIsFull) 
        {
            Gizmos.color = Color.green; //Se asigna color verde a los gizmos esféricos de los nodos
            foreach (var node in ListOfNodes) //Se recorre cada nodo de la lista
            {
                Gizmos.DrawSphere(node.pos, 0.2f); //Se pinta una esfera de radio 0.2 (unidades de Unity) sobre cada nodo de la lista
            }
        }

        if (springListIsFull)
        {
            Gizmos.color = Color.red;
            foreach (var spring in ListOfSprings) //Se recorre cada muelle de la lista
            {
                //Gizmos.color = Color.red;
                Gizmos.DrawLine(spring.nodeA.pos, spring.nodeB.pos); //Se pinta una línea sobre cada muelle de la lista
            }
        }
    }

    Vector3 VectorBetweenNodes(Node A, Node B) //Calcula la distancia entre dos nodos 
    {
        return new Vector3(B.pos.x - A.pos.x, B.pos.y - A.pos.y, B.pos.z - A.pos.z);
    }

    void Update()
    {

    }
}
