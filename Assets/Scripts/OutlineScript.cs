using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineScript : MonoBehaviour {

    GameObject hex;
    GameObject map;
	// Use this for initialization
	void Start () {
        hex = GameObject.Find("hexagon outlining2");
        map = GameObject.Find("MapParts");

        for (int i = 0; i < map.transform.childCount; i++)
        {
            var obj = map.transform.GetChild(i);
            if (obj.name != "hexagon outlining")
            {
                var hex2 = GameObject.Instantiate(hex);
                hex2.transform.parent = obj;
                hex2.transform.localPosition = new Vector3(0, 0, 0);
                hex2.transform.localScale = new Vector3(2.56f, 2.56f, 2.54f);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
