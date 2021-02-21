using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
	public Transform[] pathA;
	public Transform[] pathB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //

    public GameObject SpawnShip(GameObject prefab)
    {
		GameObject ship = Instantiate(prefab, GetComponent<Transform>().position, GetComponent<Transform>().rotation);
        Ship shipScript = ship.GetComponent<Ship>();
		
		// Determine if we should select path a or B
		if (Random.Range(0, 2) == 1)
		{
			shipScript.m_pathToFollow = pathA;
		}
		else
		{
			shipScript.m_pathToFollow = pathB;
		}

		shipScript.UpdatePath();
        return ship;
    }
}
