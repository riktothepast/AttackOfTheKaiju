using UnityEngine;
using System.Collections;

public class TankTurrent : MonoBehaviour {
    public GameObject rocket;
    Vector2 rocketDirection;
    public AudioClip sound;
    int dir = 0;
    void Start()
    {
        rocket.CreatePool(10);
    }

    public void RotateTurret()
    {
        transform.Rotate(Vector3.forward, 90f);
        dir++;
        if (dir > 3)
            dir = 0;
        switch(dir)
        {
            case 0:
                rocketDirection = new Vector2(0,1);
                break;
            case 1:
                rocketDirection = new Vector2(-1,0);
                break;
            case 2:
                rocketDirection = new Vector2(0,-1);
                break;
            case 3:
                rocketDirection = new Vector2(1,0);
                break;
        }
        ShootProjectile();


    }

    public void ShootProjectile()
    {
        GameObject rock = (GameObject)rocket.Spawn(transform.position);
        rock.GetComponent<RocketScript>().direction = rocketDirection;
        AudioManager.instance.PlaySound(sound);
    }
}
