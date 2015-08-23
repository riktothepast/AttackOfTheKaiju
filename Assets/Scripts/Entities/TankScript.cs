using UnityEngine;
using System.Collections;

public class TankScript : EnemyScript {
    bool shouldSearchWhereToMove = false;

	// Use this for initialization
	public override void Start () {
	}

    public override void ActivateUpdate()
    {
        shouldSearchWhereToMove = true;
        Debug.Log("activating tank update");

    }

    public override void Update()
    {
        if (shouldSearchWhereToMove)
        {
            Debug.Log("im a updating tank");
            shouldMove = true;
            shouldSearchWhereToMove = false;
            whereToMove = new Vector2(transform.position.x, transform.position.y -1);
        }
        if (shouldMove)
            UpdatePosition();
    }

    public override void UpdatePosition()
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
