using UnityEngine;
using System.Collections;
using Prime31.ZestKit;

public class PlayerScript : MonoBehaviour {
    public float movementSpeed;
    public int damage;
    public Vector2 whereToMove;
    bool shouldMove;
    public bool canMove = true;
    Animator anim;
    GameManager manager;

    void Start()
    {
        anim = GetComponent<Animator>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
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

    public void AttackTween(int dir)
    {
        if(dir ==0)
            transform.ZKpositionTo(transform.position + new Vector3(0, Random.Range(-0.5f, 0.5f), 0), 0.1f)
                .setLoops(LoopType.PingPong)
                .start();

        if (dir == 1)
            transform.ZKpositionTo(transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0,0), 0.1f)
                .setLoops(LoopType.PingPong)
                .start();
    }
}