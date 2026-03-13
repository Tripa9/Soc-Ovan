using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") < 0)
            {
                this.transform.Translate(-1.0f, 0.0f, 0.0f);
            }
            else if (Input.GetAxis("Horizontal") > 0)
            {
                this.transform.Translate(1.0f, 0.0f, 0.0f);
            }

        } else if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                this.transform.Translate(0.0f, 1.0f, 0.0f);
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                this.transform.Translate(0.0f, -1.0f, 0.0f);
            }
            
        }
        
    }
}
