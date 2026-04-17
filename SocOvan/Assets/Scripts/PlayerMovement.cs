using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private GameObject[] Obstacles;
    private GameObject[] Boxes;

    private bool ReadyToMove = true;

    [Header("Sprites jugador")]
    [SerializeField] private Sprite sUp;
    [SerializeField] private Sprite sDown;
    [SerializeField] private Sprite sLeft;
    [SerializeField] private Sprite sRight;

    private SpriteRenderer sRenderer;

    public Vector2 direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Obstacles = GameObject.FindGameObjectsWithTag("Wall");
        Boxes = GameObject.FindGameObjectsWithTag("Pushable");
        sRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movement.Normalize();
        direction = movement;

        if (movement.sqrMagnitude > 0.5)
        {
            if (ReadyToMove)
            {
                ReadyToMove = false;
                Move(movement);
            }
        }
        else
        {
            ReadyToMove = true;
        }
    }


    public bool Move(Vector2 direction)
    {
        if(Mathf.Abs(direction.x) < 0.5)
        {
            direction.x = 0;
        }
        else
        {
            direction.y = 0;
        }
        direction.Normalize();

        UpdateSprite(direction);

        if(Blocked(transform.position, direction))
        {
            return false;
        }
        else
        {
            transform.Translate(direction);
            return true;
        }
    }


    private void UpdateSprite(Vector2 dir)
    {
        if (dir.x > 0) sRenderer.sprite = sRight;
        else if (dir.x < 0) sRenderer.sprite = sLeft;
        else if (dir.y > 0) sRenderer.sprite = sUp;
        else if (dir.y < 0) sRenderer.sprite = sDown;
    }

    public bool Blocked(Vector3 pos, Vector2 direction)
    {
        Vector2 newpos = new Vector2(pos.x + direction.x, pos.y + direction.y);

        foreach (var obs in Obstacles)
        {
            if(obs.transform.position.x == newpos.x && obs.transform.position.y == newpos.y)
            {
                return true;
            }
        }

        foreach (var box in Boxes)
        {
            if (box.transform.position.x == newpos.x && box.transform.position.y == newpos.y)
            {
                Push boxObj = box.GetComponent<Push>();

                if(boxObj && boxObj.Move(direction))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        return false;
    }
}
