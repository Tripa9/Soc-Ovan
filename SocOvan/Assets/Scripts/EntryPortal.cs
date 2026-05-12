using UnityEngine;

public class EntryPortal : MonoBehaviour
{
    private GameObject[] Boxes;
    public GameObject Player;
    public GameObject ExitPortal;
    public GameObject BoxAtExit;

    public AudioSource portalAudio;

    // Variable para evitar comprobar todo el rato si el jugador puede pasar por un portal si lo ha pisado, no ha podido pasar y no se ha movido del sitio
    private bool notStaying;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
        BoxAtExit = ExitPortal.GetComponent<ExitPortal>().BoxOn;
        notStaying = true;
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
            portalAudio.Play();
        }
        else if (Player.transform.position.x == this.transform.position.x && Player.transform.position.y == this.transform.position.y)
        {
            if (notStaying) {
                notStaying = false;
                
                if (BoxAtExit != null)
                {
                    var movement = BoxAtExit.GetComponent<Push>();
                    Vector2 dir = Player.GetComponent<PlayerMovement>().direction;

                    if (movement.Move(dir))
                    {
                        ExitPortal.GetComponent<ExitPortal>().BoxOn = null;
                        Player.transform.position = ExitPortal.transform.position;
                        portalAudio.Play();
                    }


                }
                else
                {
                    Player.transform.position = ExitPortal.transform.position;
                    portalAudio.Play();
                }
            }
            
        }
        else
        {
            notStaying = true;
        }
    }

    
    GameObject BoxOnPortal()
    {
        foreach (var box in Boxes)
        {
            if (box.transform.position.x == this.transform.position.x && box.transform.position.y == this.transform.position.y)
            {
                return box;
            }
        }

        return null;
    }
}
