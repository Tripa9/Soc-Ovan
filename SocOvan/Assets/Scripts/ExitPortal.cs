using UnityEngine;
using UnityEngine.UIElements;
//using static Unity.Collections.AllocatorManager;

public class ExitPortal : MonoBehaviour
{
    private GameObject[] Boxes;

    public GameObject BoxOn = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var box in Boxes) 
        {
            if (box.transform.position.x == this.transform.position.x && box.transform.position.y == this.transform.position.y)
            {
                BoxOn = box;
            }
        }

        if(BoxOn != null)
        {
            if (BoxOn.transform.position.x != this.transform.position.x && BoxOn.transform.position.y != this.transform.position.y)
            {
                BoxOn = null;
            }
        }
    }
}
