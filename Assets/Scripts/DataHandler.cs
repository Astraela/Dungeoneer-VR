using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;

public class DataHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public Dictionary<string, bool> toolUnlocks = new Dictionary<string, bool>() { { "IceSword", false },{ "Pole", false },{ "Dagger", false },{ "Sword", true },{ "MSword", false },{ "BSword", false },{ "Axe", false } };
    public Dictionary<string, int> towerProgress = new Dictionary<string, int>() { {"T1",0},{"E1",0} };
    //List<string> toolUnlocks = new List<string>() {"IceSword","Pole", "Dagger","Sword", "MSword","BSword","Axe" };
    PlayerScript player;
    AudioScript audio;
    Transform camerapos;
    void Start()
    {
        player = GameObject.Find("Camera").GetComponent<PlayerScript>();
        audio = GameObject.Find("Camera").GetComponent<AudioScript>();
        camerapos = GameObject.Find("[CameraRig]").transform;
        loadData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadData()
    {
        string filePath = Application.dataPath + "\\data.txt";
        if (File.Exists(filePath))
        {
            var data = JObject.Parse(File.ReadAllText(filePath));
            player.health = (int)data["PlayerStats"][0]["Health"];
            player.coins = (int)data["PlayerStats"][0]["Coins"];
            audio.volume = (float)data["PlayerStats"][0]["Volume"];
            towerProgress["T1"] = (int)data["PlayerStats"][0]["Torens"][0]["T1"];
            if (GameObject.Find("WeaponShop"))
            {
                var purchases = data["Purchases"][0];
                var shop = GameObject.Find("WeaponShop").GetComponentsInChildren<Rigidbody>();
                foreach (var item2 in shop)
                {
                    if (item2.transform.parent.name == "GameObject" && purchases[item2.name] != null && bool.Parse(purchases[item2.name].ToString()))
                    {
                        item2.transform.parent.Find("Price").GetComponent<Canvas>().enabled = false;
                        toolUnlocks[item2.name] = true;
                    }
                }
            }
            var equips = data["PlayerStats"][0]["Equipped"][0].Value<JObject>();
            foreach (var item in equips.Properties().Select(p => p.Name).ToList())
            {
                if (equips[item].ToString() != "null")
                {
                    var weapon = GameObject.Find("WeaponCat").transform.Find(equips[item].ToString());
                    
                    if (weapon)
                    {
                        weapon = Instantiate(weapon);
                        weapon.name = weapon.name.Replace("(Clone)", "");
                        var v3 = weapon.transform.lossyScale;
                        switch (item)
                        {
                            case "LeftWeapon":
                                weapon.transform.SetParent(GameObject.Find("belt").transform.GetChild(0).transform.Find("GameObject").transform, false);
                                weapon.transform.localScale = new Vector3(1 / weapon.transform.parent.lossyScale.x / v3.x, 1 / weapon.transform.parent.lossyScale.y / v3.y, 1 / weapon.transform.parent.lossyScale.z / v3.z);
                                weapon.GetComponent<Rigidbody>().isKinematic = true;
                                weapon.transform.localPosition = new Vector3(0, 0, 0);
                                weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                                break;
                            case "RightWeapon":
                                weapon.transform.SetParent(GameObject.Find("belt").transform.GetChild(1).transform.Find("GameObject").transform, false);
                                weapon.transform.localScale = new Vector3(1 / weapon.transform.parent.lossyScale.x / v3.x, 1 / weapon.transform.parent.lossyScale.y / v3.y, 1 / weapon.transform.parent.lossyScale.z / v3.z);
                                weapon.GetComponent<Rigidbody>().isKinematic = true;
                                weapon.transform.localPosition = new Vector3(0, 0, 0);
                                weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                                break;
                            case "LeftPotion":
                                weapon.transform.SetParent(GameObject.Find("ShoulderBelt").transform.Find("Cylinder004").Find("Socket").Find("GameObject").transform, false);
                                weapon.transform.localScale = new Vector3(1 / weapon.transform.parent.lossyScale.x / v3.x, 1 / weapon.transform.parent.lossyScale.y / v3.y, 1 / weapon.transform.parent.lossyScale.z / v3.z);
                                weapon.GetComponent<Rigidbody>().isKinematic = true;
                                weapon.transform.localPosition = new Vector3(0, 0, 0);
                                weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                                break;
                            case "RightPotion":
                                weapon.transform.SetParent(GameObject.Find("ShoulderBelt").transform.Find("Cylinder007").Find("Socket").Find("GameObject").transform, false);
                                weapon.transform.localScale = new Vector3(1 / weapon.transform.parent.lossyScale.x / v3.x, 1 / weapon.transform.parent.lossyScale.y / v3.y, 1 / weapon.transform.parent.lossyScale.z / v3.z);
                                weapon.GetComponent<Rigidbody>().isKinematic = true;
                                weapon.transform.localPosition = new Vector3(0, 0, 0);
                                weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                                break;
                            case "MiddlePotion":
                                weapon.transform.SetParent(GameObject.Find("ShoulderBelt").transform.Find("Cylinder006").Find("Socket").Find("GameObject").transform, false);
                                weapon.transform.localScale = new Vector3(1 / weapon.transform.parent.lossyScale.x / v3.x, 1 / weapon.transform.parent.lossyScale.y / v3.y, 1 / weapon.transform.parent.lossyScale.z / v3.z);
                                weapon.GetComponent<Rigidbody>().isKinematic = true;
                                weapon.transform.localPosition = new Vector3(0, 0, 0);
                                weapon.transform.localEulerAngles = new Vector3(0, 0, 0);
                                break;
                            case "RightHand":
                                GameObject.Find("Controller (right)").transform.Find("Hitbox").GetComponent<PickupScript>().EquipWeapon(weapon.gameObject);
                                break;
                            case "LeftHand":
                                GameObject.Find("Controller (left)").transform.Find("Hitbox").GetComponent<PickupScript>().EquipWeapon(weapon.gameObject);
                                break;
                        }
                    }
                    else if (item == "Stones" || item == "Kunai")
                    {
                        switch (item)
                        {
                            case "Stones":
                                GameObject.Find("RockBag").transform.Find("Count").GetChild(0).name = equips[item].ToString();
                                break;
                            case "Kunai":
                                GameObject.Find("KunaiBag").transform.Find("Count").GetChild(0).name = equips[item].ToString();
                                break;
                        }
                    }
                }
            }
            
        }
        else
        {
            saveData();
        }
    }


    public void saveData()
    {
            while (true)
            {
                if (player != null && audio != null && camerapos != null)
                {
                    break;
                }
                else
                {
                    player = GameObject.Find("Camera").GetComponent<PlayerScript>();
                    audio = GameObject.Find("Camera").GetComponent<AudioScript>();
                    camerapos = GameObject.Find("[CameraRig]").transform;
                }
            }
        GameObject.Find("DebugText").GetComponent<Text>().text = "atleast here";
        string total = "{ \"Purchases\": [{";

        int i = 1;
        foreach (var item in toolUnlocks)
        {
            total = total + "\"" + item.Key + "\": \"" + item.Value.ToString() + "\"";
            if (i != toolUnlocks.Count)
            {
                total = total + ",";
            }
            i += 1;
        }
        GameObject.Find("DebugText").GetComponent<Text>().text = "atleast here x1.2";
            total = total + "}], \"PlayerStats\": [{ \"Coins\": " + player.coins + ", \"Health\": " + player.health + ",  \"Volume\": " + audio.volume + ", \"Scene\": \"" + SceneManager.GetActiveScene().name + "\", \"Position\": [ "+ camerapos.position.x + ", " + camerapos.position.y + ", " + camerapos.position.z + "], ";
            //total = total + "}], \"PlayerStats\": [{ \"Coins\": " + player.coins + ", \"Health\": " + player.health + ", \"Volume\": " + audio.volume + ", ";
        GameObject.Find("DebugText").GetComponent<Text>().text = "atleast here x1.4";
        total = total + "\"Equipped\": [{ \"LeftWeapon\": \"";
        var riem = GameObject.Find("belt");
        GameObject.Find("DebugText").GetComponent<Text>().text = "atleast here x1.5";
        if (riem.transform.GetChild(0).Find("GameObject").childCount >= 1)
            total = total + riem.transform.GetChild(0).Find("GameObject").GetChild(0).name + "\", ";
        else
            total = total + "null" + "\", ";
        total = total + "\"RightWeapon\": \"";
        if (riem.transform.GetChild(1).Find("GameObject").childCount >= 1)
            total = total + riem.transform.GetChild(1).Find("GameObject").GetChild(0).name + "\", ";
        else
            total = total + "null" + "\", ";
        total = total + "\"LeftPotion\": \"";
        riem = GameObject.Find("ShoulderBelt");

        GameObject.Find("DebugText").GetComponent<Text>().text = "atleast here x1.6";
        if (riem.transform.Find("Cylinder004").Find("Socket").Find("GameObject").childCount >= 1)
            total = total + riem.transform.Find("Cylinder004").Find("Socket").Find("GameObject").GetChild(0).GetComponent<PotionScript>().potionSize.ToString() + riem.transform.Find("Cylinder004").Find("Socket").Find("GameObject").GetChild(0).GetComponent<PotionScript>().potionType.ToString() + "\", ";
        else
            total = total + "null" + "\", ";

        total = total + "\"MiddlePotion\": \"";
        GameObject.Find("DebugText").GetComponent<Text>().text = "atleast here x1.8";
        if (riem.transform.Find("Cylinder006").Find("Socket").Find("GameObject").childCount >= 1)
            total = total + riem.transform.Find("Cylinder006").Find("Socket").Find("GameObject").GetChild(0).GetComponent<PotionScript>().potionSize.ToString() + riem.transform.Find("Cylinder006").Find("Socket").Find("GameObject").GetChild(0).GetComponent<PotionScript>().potionType.ToString() + "\", ";
        else
            total = total + "null" + "\", ";

        total = total + "\"RightPotion\": \"";
        if (riem.transform.Find("Cylinder007").Find("Socket").Find("GameObject").childCount >= 1)
            total = total + riem.transform.Find("Cylinder007").Find("Socket").Find("GameObject").GetChild(0).GetComponent<PotionScript>().potionSize.ToString() + riem.transform.Find("Cylinder007").Find("Socket").Find("GameObject").GetChild(0).GetComponent<PotionScript>().potionType.ToString() + "\", ";
        else
            total = total + "null" + "\", ";

        total = total + "\"RightHand\": \"";


        GameObject.Find("DebugText").GetComponent<Text>().text = "atleast here x2";
        if (GameObject.Find("Controller (right)").transform.Find("Hitbox").childCount >= 2)
        {
            int child = 0;
            if (GameObject.Find("Controller (right)").transform.Find("Hitbox").GetChild(0).name.Contains("Hand"))
            {
                child = 1;
            }
            if (GameObject.Find("Controller (right)").transform.Find("Hitbox").GetChild(child).GetComponent<PotionScript>())
                total = total + GameObject.Find("Controller (right)").transform.Find("Hitbox").GetChild(child).GetComponent<PotionScript>().potionSize.ToString() + GameObject.Find("Controller (right)").transform.Find("Hitbox").GetChild(child).GetComponent<PotionScript>().potionType.ToString() + "\", ";
            else
                total = total + GameObject.Find("Controller (right)").transform.Find("Hitbox").GetChild(child).name + "\", ";
        }
        else
            total = total + "null" + "\", ";
        GameObject.Find("DebugText").GetComponent<Text>().text = "atleast here x3";
        total = total + "\"LeftHand\": \"";
        if (GameObject.Find("Controller (left)").transform.Find("Hitbox").childCount >= 2)
        {
            int child = 0;
            if (GameObject.Find("Controller (left)").transform.Find("Hitbox").GetChild(0).name.Contains("Hand"))
            {
                child = 1;
            }
            if (GameObject.Find("Controller (left)").transform.Find("Hitbox").GetChild(child).GetComponent<PotionScript>())
                total = total + GameObject.Find("Controller (left)").transform.Find("Hitbox").GetChild(child).GetComponent<PotionScript>().potionSize.ToString() + GameObject.Find("Controller (left)").transform.Find("Hitbox").GetChild(child).GetComponent<PotionScript>().potionType.ToString() + "\", ";
            else
                total = total + GameObject.Find("Controller (left)").transform.Find("Hitbox").GetChild(child).name + "\", ";
        }
        else
            total = total + "null" + "\", ";

        total = total + "\"Stones\": " + GameObject.Find("RockBag").transform.Find("Count").GetChild(0).name + ",";
        total = total + "\"Kunai\": " + GameObject.Find("KunaiBag").transform.Find("Count").GetChild(0).name + "";
        total = total + "}], ";
        GameObject.Find("DebugText").GetComponent<Text>().text = "gOT HERE YA DINGUS REEEE";
        total = total + "\"Torens\": [{ \"T1\": " + towerProgress["T1"] + "}]";


        total = total + "}]";



        total = total + "}";
        string filePath = Application.dataPath + "\\data.txt";
        File.WriteAllText(filePath, total);
        
    }

    public void DeleteData()
    {
        string filePath = Application.dataPath + "\\data.txt";
        File.Delete(filePath);
    }
    public void ChangeKey(string key, int value)
    {
        towerProgress[key] = value;
    }
}



/*[
  
  {
	"PlayerStats":[{
		"Coins": 0,
		"Health": 500,
        "Scene": "Plein",
		"Position": [
			1,2,3
		],
		"Volume": 1,
		"Equipped": [{
          "Stones": 0,
          "Kunai": 0,
          "LeftWeapon": "BSword",
          "RightWeapon": "MSword",
          "LeftPotion": "BigHealth",
          "MiddlePotion": "NormalDamage",
          "RightPotion":  "BigStep"
		}]
	}],
    "Purchases": [{
      "IceSword": false,
      "Pole": false,
      "Dagger": false,
      "Sword": true,
      "MSword": false,
      "BSword": false,
      "Axe": false
    }]
  }
]*/
