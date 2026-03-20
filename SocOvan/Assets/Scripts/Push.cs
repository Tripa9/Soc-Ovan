using UnityEditor.Timeline;
using UnityEngine;

public class Push : MonoBehaviour
{

    private GameObject[] Obstacles;
    private GameObject[] Boxes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Obstacles = GameObject.FindGameObjectsWithTag("Wall");
        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Move(Vector2 direction)
    { 
        if(BoxBlocked(transform.position, direction))
        {
            return false;
        }
        else
        {
            transform.Translate(direction);
            return true;
        }
    }

    public bool BoxBlocked(Vector2 pos, Vector2 direction)
    {
        Vector2 newpos = new Vector2(pos.x + direction.x, pos.y + direction.y);

        foreach (var obs in Obstacles)
        {
            if (obs.transform.position.x == newpos.x && obs.transform.position.y == newpos.y)
            {
                return true;
            }
        }

        foreach (var box in Boxes)
        {
            if (box.transform.position.x == newpos.x && box.transform.position.y == newpos.y)
            {
                return true;
            }
        }

        return false;
    }
}
