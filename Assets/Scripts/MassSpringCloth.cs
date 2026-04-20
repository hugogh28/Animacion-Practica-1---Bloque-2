using System.Collections.Generic;
using UnityEngine;

public class MassSpringCloth : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh; //Se guarda en la variable mesh, el mallado del objeto

        Vector3[] vertices = mesh.vertices; //Se guardan en un array todos los vértices de la mesh

        List<Node> nodes = new List<Node>(vertices.Length);

        for(int i = 0; i< vertices.Length; i++)
        {
            nodes.Add(new Node(vertices[i])); //Cada vez que se itera sobre el bucle de vértices de la mesh, se añade un nuevo nodo, cuya posición corresponde a la de su vértice
        }

        int[] triangles = mesh.triangles; //Se guardan en un array todos los triángulos de la mesh
    }

    void Update()
    {

    }
}
