using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PortalScript : MonoBehaviour
{

    public string destination = string.Empty;
    public string tower = string.Empty;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            teleport();
        }
    }

    void teleport()
    {
        GameObject.Find("Camera").GetComponent<DataHandler>().saveData();
        if (destination != string.Empty)
        {
            if (destination == "Toren" && SceneManager.GetActiveScene().name != "Toren")
                StaticClass.Origin = SceneManager.GetActiveScene().name;

            if (tower != string.Empty && tower.Length > 2)
                StaticClass.CrossSceneInformation = tower;
            else if (tower.Length == 2)
                StaticClass.CrossSceneInformation = tower + "L" + (GameObject.Find("Camera").GetComponent<DataHandler>().towerProgress[tower] + 1);

            SceneManager.LoadScene(destination);
        }
    }
}