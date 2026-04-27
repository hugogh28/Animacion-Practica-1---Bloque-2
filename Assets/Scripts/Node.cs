using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Node
{
    //public float mass = 5f;
    public bool fixedNode;

    public Vector3 pos;
    public Vector3 posGlobal;
    public Vector3 vel;
    public Vector3 force;
    public List<Vector3> offset = new List<Vector3>();
    List<float> sqrDistance = new List<float>();

    public Node(Vector3 assignedPos, List<Fixer> fixers) //Constructor de la clase, para asignar los disintos nodos a su correspondiente vértice
    {
        pos = assignedPos;
        for(int i = 0; i<fixers.Count; i++)
        {
            offset.Add(fixers[i].pos-pos);
            sqrDistance.Add(offset[i].sqrMagnitude);
            if (sqrDistance[i] <= 0.25) //Sabiendo que el fixer, un cubo, tiene un tamaño aproximado de 1 unidad de Unity,
                                     //y que la distancia se halla entre un nodo y el centro del cubo,
                                     //se asigna que la distancia mínima cuadrada (para fijar un nodo) debe ser de 0.5 al cuadrado (0.25)
            {//Se calcula la distancia entre el nodo y el fixer usando sqrMagnitude,
             //que es más eficiente que magnitude por prescindir del cáculo de la raíz cuadrada.
             //De este modo, se deduce si el nodo debe ser fijado o no si la distancia entre ambas es menor o igual a 0.5 unidades de Unity
                fixedNode = true;
            }
            else if(fixedNode == false) //Para evitar que en la segunda iteración, se registre a nodos fijos como no fijos
            {
                fixedNode = false;
            }
        }
    }
}
