using UnityEngine;

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
        
    }
}
