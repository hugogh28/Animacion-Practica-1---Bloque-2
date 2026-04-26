using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MassSpringCloth : MonoBehaviour
{
    public float mass = 5f;

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

    public List<Fixer> fixer = new List<Fixer>(); //Desde Unity se hará por esta línea la asignación del fixer, es decir, del cubo que fija nodos, a este script para que los nodos se fijen

    Mesh cloth;

    Vector3[] verts;



    void Start()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh; //Se guarda en la variable mesh el mallado del objeto

        cloth = mesh; //Para poder hacer las modificaciones en la malla, se guarda la mesh en una variable global

        Vector3[] vertices = mesh.vertices; //Se guardan en un array todos los vértices de la mesh

        verts = vertices; //Para poder hacer las modificaciones en la mesh, se guardan los vértices de la mesh en una variable global

        List<Node> nodes = new List<Node>(vertices.Length); //Se crea una lista de nodos cuyo tamaño sea el de los vértices de la mesh
        List<Spring> springs = new List<Spring>(); //Se crea una lista de muelles cuyo tamaño es indefinido (ya que se presupone que podemos usar cualquier bandera)

        for(int i = 0; i < vertices.Length; i++) //Se itera tantas veces como vértices hay en el array vertices
        {
            nodes.Add(new Node(vertices[i], fixer)); //Cada vez que se itera sobre el bucle de vértices de la mesh, se añade un nuevo nodo, cuya posición corresponde a la de su vértice
                                                     //Además, se comprueba, mediante la lista de fixers, si dicho nodo debe estar fijado antes de comenzar la animación
            Debug.Log("\t \t Distancia del nodo " + i + " con el primer fixer: "+nodes[i].offset[0].sqrMagnitude);
            Debug.Log("\t \t Distancia del nodo " + i + " con el segundo fixer: " + nodes[i].offset[1].sqrMagnitude);
            Debug.Log("Posición del nodo: " + nodes[i].pos);
        }
        nodeListIsFull = true; //Se activa el booleano nodeListIsFull cuando la lista de nodos se ha llenado con todos los elementos del objeto

        ListOfNodes = nodes; //Para poder hacer uso de OnDrawGizmos() se pasa la lista nodes a ListOfNodes



        int[] triangles = mesh.triangles; //Se guardan en un array todos los triángulos de la mesh

        for (int i = 0; i < triangles.Length - 3; i+=3)  //Se itera tantas veces como triángulos hay en el array triangles, es decir, que si hay 600 vértices en el array, iteramos 200 veces
        {
            springs.Add(new Spring(kT, nodes[triangles[i]], nodes[triangles[i + 1]])); //Añade un muelle entre el primer y segundo vértice del triángulo con una determinada constante de rigidez
            springs.Add(new Spring(kT, nodes[triangles[i]], nodes[triangles[i + 2]])); //Añade un muelle entre el primer y tercer vértice del triángulo con una determinada constante de rigidez
            springs.Add(new Spring(kT, nodes[triangles[i + 1]], nodes[triangles[i + 2]])); //Añade un muelle entre el segundo y tercer vértice del triángulo con una determinada constante de rigidez
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P)) // Detectamos si se ha pulsado la tecla P
        {
            // La tecla P hace de "toggle" para pausar o quitar la pausa de la
            // animación
            paused = !paused;
        }
    }

    private void FixedUpdate()
    {
        if (paused)
            // Si está pausada la animación, no hacemos nada y regresamos
            return;

        // Según el método de integración escogido, se invoca una función u otra
        switch (integrationMethod)
        {
            case Integration.ExplicitEuler:
                integrateExplicitEuler();
                break;

            case Integration.SymplecticEuler:
                integrateSymplecticEuler();
                break;
            default:
                print("ERROR METODO INTEGRACION DESCONOCIDO");
                break;
        }

        // Recorremos la lista de muelles para recalcularlos, una vez que hemos
        // calculado la nueva posición de los nodos con el método de integración
        foreach (Spring spring in ListOfSprings)
        {
            // Vector dirección del muelle, apunta de B a A            
            spring.u = spring.nodeA.pos - spring.nodeB.pos;
            // Nueva longitud del muelle 
            spring.length = spring.u.magnitude;
            // Normalizamos el vector que almacena la orientación del muelle
            spring.u = Vector3.Normalize(spring.u);
            // Posición del punto medio del muelle: media aritmética de las
            // posiciones de los dos nodos
            spring.pos = (spring.nodeA.pos + spring.nodeB.pos) / 2f;
            // Orientamos correctamente el muelle según el vector dir
            spring.rotation = Quaternion.FromToRotation(Vector3.up, spring.u);
        }
    }

    /// <summary>
    /// Método de integración de Euler Explícito
    /// </summary>
    void integrateExplicitEuler()
    {
        int i = 0;
        // Recorremos la lista de nodos para aplicar las fuerzas a cada uno de
        // ellos
        foreach (Node node in ListOfNodes)
        {
            if (!node.fixedNode) // Si el nodo no es fijo
            {
                // r_(n+1) = r_n + h * v_n

                node.pos += h * node.vel;
                this.GetComponent<MeshFilter>().mesh.vertices[i].Set(node.pos.x, node.pos.y, node.pos.z);
                this.GetComponent<MeshFilter>().mesh.RecalculateBounds();
                node.force = -(mass/*/ListOfNodes.Count*/) * g;
            }
            i++;
        }

        // Recorremos la lista de muelles para añadir a cada nodo la fuerza
        // elástica de cada muelle. Por la ley de acción y reacción, estas
        // fuerzas son iguales y de sentidos opuestos en los extremos de cada
        // muelle
        foreach (Spring spring in ListOfSprings)
        {
            spring.nodeA.force += -spring.k * (spring.length - spring.length0)
                * spring.u;
            spring.nodeB.force += spring.k * (spring.length - spring.length0)
                * spring.u;
        }

        // Recorremos de nuevo la lista de nodos para calcular la nueva
        // velocidad, una vez que ya conocemos la fuerza total en cada nodo
        foreach (Node node in ListOfNodes)
        {
            if (!node.fixedNode) // Si el nodo no es fijo
            {
                // v_(n+1) = v_n + h F_n / m
                node.vel += h * node.force / (mass/*/ListOfNodes.Count*/);
            }
        }
    }

    /// <summary>
    ///  Método de integración de Euler Simpléctico
    /// </summary>
    void integrateSymplecticEuler()
    {
        int i = 0;
        // Recorremos la lista de nodos para aplicar las fuerzas a cada uno de
        // ellos
        foreach (Node node in ListOfNodes)
        {
            node.force = -(mass) * g;
        }

        // Recorremos la lista de muelles para añadir a cada nodo la fuerza
        // elástica de cada muelle. Por la ley de acción y reacción, estas
        // fuerzas son iguales y de sentidos opuestos en los extremos de cada
        // muelle
        foreach (Spring spring in ListOfSprings)
        {
            spring.nodeA.force += -spring.k * (spring.length - spring.length0)
                * spring.u;
            spring.nodeB.force += spring.k * (spring.length - spring.length0)
                * spring.u;
        }

        // Recorremos de nuevo la lista de nodos para calcular la nueva
        // velocidad y la nueva posición, una vez que ya conocemos la fuerza
        // total en cada nodo
        foreach (Node node in ListOfNodes)
        {
            
            if (!node.fixedNode) // Si el nodo no es fijo
            {
                // v_(n+1) = v_n + h F_n / m
                node.vel += h * node.force / (mass);
                // r_(n+1) = r_n + h * v_(n+1)
                node.pos += h * node.vel;
                this.GetComponent<MeshFilter>().mesh.vertices[i].Set(node.pos.x, node.pos.y, node.pos.z);
                this.GetComponent<MeshFilter>().mesh.RecalculateBounds();
            }
            i++;
        }
    }
}
