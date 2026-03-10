using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{

    private void Start()
    {
        //GetComponent<NavMeshAgent2D>().destination = new Vector3(-509, 757);
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 w = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GetComponent<NavMeshAgent2D>().destination = w;
        }
    }
}