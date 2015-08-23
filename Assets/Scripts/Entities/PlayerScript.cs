using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
    public float movementSpeed;
    public int damage;
    public Vector2 whereToMove;
    bool shouldMove;
    public bool canMove = true;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (shouldMove)
            UpdatePosition();
    }

    void UpdatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, whereToMove, movementSpeed * Time.deltaTime);
        if ((transform.position.y) == whereToMove.y)
        {
            if ((transform.position.x) > whereToMove.x)
            {
                anim.Play("MoveLeft");
            }
            else if ((transform.position.x) < whereToMove.x)
            {
                anim.Play("MoveRight");
            }
        }
        else if ((transform.position.x) == whereToMove.x)
        {
            if ((transform.position.y)  > whereToMove.y)
            {
                anim.Play("MoveDown");
            }
            else if ((transform.position.y)  < whereToMove.y)
            {
                anim.Play("MoveUp");
            }
        }

        if (Vector3.Distance(transform.position, whereToMove) < 0.009f)
        {
            shouldMove = false;
            canMove = true;
            transform.position = whereToMove;
        }
    }

    public void SetToMove(Vector2 trans)
    {
        whereToMove = trans;
        shouldMove = true;
        canMove = false;
    }
}
