//using UnityEditor.Timeline;
using UnityEngine;
//using UnityEngine.UIElements;

public class Push : MonoBehaviour
{

    private GameObject[] Obstacles;
    private GameObject[] Boxes;
    private GameObject[] Portals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Obstacles = GameObject.FindGameObjectsWithTag("Wall");
        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
        Portals = GameObject.FindGameObjectsWithTag("Entry Portal");
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

        bool blocked = false;

        foreach (var obs in Obstacles)
        {
            if (obs.transform.position.x == newpos.x && obs.transform.position.y == newpos.y)
            {
                blocked = true;
                break;
            }
        }

        if (!blocked)
        {
            foreach (var box in Boxes)
            {
                if (box != this)
                {
                    if (box.transform.position.x == newpos.x && box.transform.position.y == newpos.y)
                    {
                        blocked = !box.GetComponent<Push>().Move(direction);
                        break;
                    }
                }
            }
        }

        if (!blocked)
        {
            foreach (var inPortal in Portals)
            {
                GameObject portalBox = inPortal.GetComponent<EntryPortal>().BoxAtExit;
                if (portalBox != null) {
                    if (inPortal.transform.position.x == newpos.x && inPortal.transform.position.y == newpos.y)
                    {
                        
                        blocked = !portalBox.GetComponent<Push>().Move(direction);
                        if (!blocked)
                        {
                            inPortal.GetComponent<EntryPortal>().ExitPortal.GetComponent<ExitPortal>().BoxOn = null;
                        }
                        else
                        {
                            blocked = false;
                        }
                        break;
                    }
                }
                
            }
        }

        return blocked;
    }
}
