using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : MonoBehaviour {

    public GameObject bullet;
    GameObject currentHolding;
    bool gun = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.G))
            shoot();
	}

    void shoot()
    {
        currentHolding = GameObject.Find("Gun");
        gun = true;
        if (gun)
        {
            var newbullet = GameObject.Instantiate(bullet);
            newbullet.transform.position = currentHolding.transform.Find("GunPoint").position;
            newbullet.transform.rotation = currentHolding.transform.Find("GunPoint").rotation;
            newbullet.GetComponent<Rigidbody>().AddForce(currentHolding.transform.Find("GunPoint").forward * -1000);
        }
    }
}
