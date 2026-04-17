using UnityEngine;

public class EntryPortal : MonoBehaviour
{
    private GameObject[] Boxes;
    public GameObject Player;
    public GameObject ExitPortal;
    public GameObject BoxAtExit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
        BoxAtExit = ExitPortal.GetComponent<ExitPortal>().BoxOn;
    }

    // Update is called once per frame
    void Update()
    {
        BoxAtExit = ExitPortal.GetComponent<ExitPortal>().BoxOn;
        GameObject box_on = BoxOnPortal();

        if (box_on != null && BoxAtExit == null)
        {
            box_on.transform.position = ExitPortal.transform.position;
            ExitPortal.GetComponent<ExitPortal>().BoxOn = box_on;
        }
        else if (Player.transform.position == this.transform.position)
        {
            if(BoxAtExit != null)
            {
                var movement = BoxAtExit.GetComponent<Push>();
                Vector2 dir = Player.GetComponent<PlayerMovement>().direction;
                if (movement.Move(dir))
                {
                    ExitPortal.GetComponent<ExitPortal>().BoxOn = null;
                    Player.transform.position = ExitPortal.transform.position;
                }

            }
            else
            {
                Player.transform.position = ExitPortal.transform.position;
            }
        }
    }

    GameObject BoxOnPortal()
    {
        foreach (var box in Boxes)
        {
            if (box.transform.position == this.transform.position)
            {
                return box;
            }
        }

        return null;
    }
}
