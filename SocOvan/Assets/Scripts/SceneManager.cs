using Mono.Cecil;
using System.Linq.Expressions;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private GameObject[] Obstacles;
    private GameObject[] Boxes;
    private GameObject[] Goals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Obstacles = GameObject.FindGameObjectsWithTag("Wall");
        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
        Goals = GameObject.FindGameObjectsWithTag("Goal");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var goal in Goals)
        {
            foreach (var box in Boxes)
            {
                if (goal.transform.position.x == box.transform.position.x && goal.transform.position.y == box.transform.position.y)
                {
                    Debug.Log("¡NIVEL COMPLETADO!");
                }
            }
        }
    }
}
