using UnityEngine;

public class Node
{
    public float mass = 5f;
    public bool fixedNode;

    public Vector3 pos;
    public Vector3 vel;
    public Vector3 force;

    public Node(Vector3 assignedPos) //Constructor de la clase, para asignar los disintos nodos a su correspondiente vértice
    {
        pos = assignedPos;
    }

    /*void Start()
    {
        pos = transform.position;
    }

    void Update()
    {
        transform.position = pos;
    }*/
}
