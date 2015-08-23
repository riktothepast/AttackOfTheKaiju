using UnityEngine;
using System.Collections;
using Game.TileMapping.Unity;

public class CameraFollow : MonoBehaviour {

	public bool debugFrameRate;
	private bool hasMovedCamera;
	TileMap map;
    public Vector3 defaultPosition;
    public Vector3 offsets;
    float x, y, z;
//    GameObject player;
	// Use this for initialization
	void Awake () {
		hasMovedCamera = true;
        map = GameObject.FindGameObjectWithTag("Map").GetComponent<TileMap>();
        x = (map.Columns * map.TileWidth) / 2;
        y = (map.Rows * map.TileHeight) / 2;
        z = -10;
        transform.position = new Vector3(x + offsets.x, y + offsets.y, z + offsets.z);
	}
	
	// Update is called once per frame
	void Update () {
		if(!Application.isLoadingLevel)
		{
            transform.position = Vector3.Lerp(transform.position, new Vector3(x + offsets.x, y + offsets.y, z + offsets.z), 10f*Time.deltaTime);
            defaultPosition = transform.position;
		}

	}
}
