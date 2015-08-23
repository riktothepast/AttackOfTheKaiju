using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{
    public float movementSpeed;
    public int damage;
    public Vector2 whereToMove;
    public bool shouldMove;
    public bool canMove = true;

    // Use this for initialization
    public virtual void Start()
    {

    }

    public virtual void ActivateUpdate()
    { 
        
    }

    public virtual void Update()
    {
        if (shouldMove)
            UpdatePosition();
    }

    public virtual void UpdatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, whereToMove, movementSpeed * Time.deltaTime);


        if (Vector3.Distance(transform.position, whereToMove) < 0.009f)
        {
            shouldMove = false;
            canMove = true;
            transform.position = whereToMove;
        }
    }
}
