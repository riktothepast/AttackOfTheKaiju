using UnityEngine;
using System.Collections;

public class RocketScript : MonoBehaviour {
    public float speed;
    public Vector2 direction;
    public int damage;
	// Use this for initialization
	void Start () {
        Invoke("clear", 2f);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(direction * speed * Time.deltaTime);
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag.Equals("Building"))
        {
            this.Recycle();
        }
        if (col.tag.Equals("Player"))
        {
            col.GetComponent<PlayerScript>().DoDamage(damage);
            this.Recycle();
        }
    }

    void clear()
    {
        this.Recycle();
    }
}
