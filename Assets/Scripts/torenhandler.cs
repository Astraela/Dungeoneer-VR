using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class torenhandler : MonoBehaviour
{
    private int enemycount = 0;
    private GameObject portal;
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;
    public GameObject Boss;
    public GameObject E1;
    void Start()
    {
        portal = GameObject.Find("Portal2");
        portal.SetActive(false);
        GameObject.Find("Portal1").transform.Find("Cube (3)").GetComponent<PortalScript>().destination = StaticClass.Origin;
        GameObject.Find("Portal1").transform.Find("Cube (6)").Find("Canvas").Find("Text").GetComponent<Text>().text = StaticClass.Origin;
        switch (StaticClass.CrossSceneInformation)
        {
            case "T1L1":
                GameObject.Find("Camera").GetComponent<DataHandler>().ChangeKey("T1", 1);
                GameObject.Find("Camera").GetComponent<DataHandler>().saveData();
                StartCoroutine(spawn(Enemy1, GameObject.Find("hexagon (160)")));



                portal.transform.Find("Cube (6)").Find("Canvas").Find("Text").GetComponent<Text>().text = "Level 2";
                portal.transform.Find("Cube (3)").GetComponent<PortalScript>().tower = "T1L2";
                break;
            case "T1L2":
                GameObject.Find("Camera").GetComponent<DataHandler>().ChangeKey("T1", 1);
                GameObject.Find("Camera").GetComponent<DataHandler>().saveData();

                StartCoroutine(spawn(Enemy1, GameObject.Find("hexagon (85)")));
                StartCoroutine(spawn(Enemy1, GameObject.Find("hexagon (82)")));



                portal.transform.Find("Cube (6)").Find("Canvas").Find("Text").GetComponent<Text>().text = "Level 3";
                portal.transform.Find("Cube (3)").GetComponent<PortalScript>().tower = "T1L3";
                break;
            case "T1L3":
                GameObject.Find("Camera").GetComponent<DataHandler>().towerProgress["T1"] = 2;
                GameObject.Find("Camera").GetComponent<DataHandler>().saveData();

                StartCoroutine(spawn(Enemy1, GameObject.Find("hexagon (85)")));
                StartCoroutine(spawn(Enemy1, GameObject.Find("hexagon (82)")));
                StartCoroutine(spawn(Enemy2, GameObject.Find("hexagon (160)")));



                portal.transform.Find("Cube (6)").Find("Canvas").Find("Text").GetComponent<Text>().text = "Level 4";
                portal.transform.Find("Cube (3)").GetComponent<PortalScript>().tower = "T1L4";
                break;
            case "T1L4":
                GameObject.Find("Camera").GetComponent<DataHandler>().towerProgress["T1"] = 3;
                GameObject.Find("Camera").GetComponent<DataHandler>().saveData();

                StartCoroutine(spawn(Enemy2, GameObject.Find("hexagon (13)")));
                StartCoroutine(spawn(Enemy2, GameObject.Find("hexagon (4)")));
                StartCoroutine(spawn(Enemy2, GameObject.Find("hexagon (34)")));
                StartCoroutine(spawn(Enemy3, GameObject.Find("hexagon (160)")));



                portal.transform.Find("Cube (6)").Find("Canvas").Find("Text").GetComponent<Text>().text = "Level 5";
                portal.transform.Find("Cube (3)").GetComponent<PortalScript>().tower = "T1L5";
                break;
            case "T1L5":
                GameObject.Find("Camera").GetComponent<DataHandler>().towerProgress["T1"] = 4;
                GameObject.Find("Camera").GetComponent<DataHandler>().saveData();

                StartCoroutine(spawn(Enemy2, GameObject.Find("hexagon (13)")));
                StartCoroutine(spawn(Enemy1, GameObject.Find("hexagon (4)")));
                StartCoroutine(spawn(Enemy2, GameObject.Find("hexagon (34)")));
                StartCoroutine(spawn(Enemy3, GameObject.Find("hexagon (159)")));
                StartCoroutine(spawn(Enemy3, GameObject.Find("hexagon (164)")));



                portal.transform.Find("Cube (6)").Find("Canvas").Find("Text").GetComponent<Text>().text = "Level 6";
                portal.transform.Find("Cube (3)").GetComponent<PortalScript>().tower = "T1L6";
                break;
            case "T1L6":
                GameObject.Find("Camera").GetComponent<DataHandler>().towerProgress["T1"] = 5;
                GameObject.Find("Camera").GetComponent<DataHandler>().saveData();

                StartCoroutine(spawn(Enemy2, GameObject.Find("hexagon (13)")));
                StartCoroutine(spawn(Enemy1, GameObject.Find("hexagon (4)")));
                StartCoroutine(spawn(Enemy2, GameObject.Find("hexagon (34)")));
                StartCoroutine(spawn(Enemy3, GameObject.Find("hexagon (159)")));
                StartCoroutine(spawn(Enemy3, GameObject.Find("hexagon (164)")));



                portal.transform.Find("Cube (6)").Find("Canvas").Find("Text").GetComponent<Text>().text = "Level 7";
                portal.transform.Find("Cube (3)").GetComponent<PortalScript>().tower = "T1L7";
                break;
            case "T1L7":
                GameObject.Find("Camera").GetComponent<DataHandler>().towerProgress["T1"] = 6;
                GameObject.Find("Camera").GetComponent<DataHandler>().saveData();

                StartCoroutine(spawn(Boss, GameObject.Find("hexagon (160)")));



                portal.transform.Find("Cube (6)").Find("Canvas").Find("Text").GetComponent<Text>().text = "Level 7";
                portal.transform.Find("Cube (3)").GetComponent<PortalScript>().tower = "T1L7";
                break;
            case "E1L1":

                E1.SetActive(true);
                break;
        }

    }

    IEnumerator spawn(GameObject enemy, GameObject hexagon)
    {
        enemycount += 1;
        enemy = Instantiate(enemy);
        enemy.transform.position = hexagon.transform.position + new Vector3(0, hexagon.GetComponent<Renderer>().bounds.extents.y * 2, 0);
        enemy.SetActive(true);
        while (enemy.GetComponent<EnemyController>().currentState.ToString() != "Dead")
        {

            yield return new WaitForFixedUpdate();
        }
        enemycount -= 1;
        if (enemycount <= 0)
        {
            portal.SetActive(true);
        }
    }
    
}
