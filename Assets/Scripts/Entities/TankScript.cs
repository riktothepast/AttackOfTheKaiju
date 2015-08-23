using UnityEngine;
using System.Collections;

public class TankScript : EnemyScript {

	// Use this for initialization
	public override void Start () {
	    
	}

    public override void Update()
    {
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
