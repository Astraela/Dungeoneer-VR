using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;
public class PickupScript : MonoBehaviour {

    public SteamVR_Input_Sources controller;
    PlayerScript playerScript;
    public GameObject otherController;
    private GameObject currentSelected;
    private GameObject currentHolding;
    private Transform oldParent;
    private Vector3 posVelocity;
    Vector3 eulerRotation;
    bool gun = false;
    bool holding = false;
    public GameObject bullet;
    bool gunCooldown = false;
    bool holdCD = true;
    private GameObject energy;
    int bulletcost = 35;
    private GameObject socket;
    private GameObject potionSocket;
    private GameObject lastbag;
    private string state = "off";
    GameObject Book;
    private void Start()
    {
        StartCoroutine(CalcVelocity());
        playerScript = transform.parent.parent.Find("Camera").GetComponent<PlayerScript>();
    }

    void Update () {

        //if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(SteamVR_Input_Sources.Any))


        if (SteamVR_Input._default.inActions.GrabGrip.GetStateDown(controller))
        {
            transform.GetChild(0).GetComponent<Animator>().SetBool("Fist", true);
        }

        if (SteamVR_Input._default.inActions.GrabGrip.GetStateUp(controller))
        {
            transform.GetChild(0).GetComponent<Animator>().SetBool("Fist", false);
        }
        //Shooting part
        if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown(controller))
        {
            transform.GetChild(0).GetComponent<Animator>().SetBool("Point", true);
            //print(gun + " || " + gunCooldown);
            if (gun && !gunCooldown && playerScript.ammo >= bulletcost)
            {
                energy = currentHolding.transform.Find("Canvas").Find("Energy").gameObject;
                StartCoroutine(bulletcooldown());
                var newbullet = Instantiate(bullet);
                newbullet.GetComponent<BulletScript>().controller = controller;
                newbullet.transform.position = currentHolding.transform.Find("GunPoint").position;
                newbullet.transform.rotation = currentHolding.transform.Find("GunPoint").rotation;
                newbullet.GetComponent<Rigidbody>().AddForce(currentHolding.transform.Find("GunPoint").forward * -3000);
                StartCoroutine(bulletRemover(newbullet));
                playerScript.ammo = playerScript.ammo - bulletcost;
            }
        }

        if (SteamVR_Input._default.inActions.GrabPinch.GetStateUp(controller))
        {
            transform.GetChild(0).GetComponent<Animator>().SetBool("Point", false);
        }
        if ((SteamVR_Input._default.inActions.GrabGrip.GetStateDown(controller) || SteamVR_Input._default.inActions.GrabPinch.GetStateDown(controller)) && holding == false && lastbag && state == "off")
        {
            if (Convert.ToInt32(lastbag.transform.Find("Count").GetChild(0).name) > 0)
            {
                state = "on";
                holdCD = false;
                StartCoroutine(holdingcooldown());
                if (lastbag.name == "KunaiBag")
                {
                    holding = true;
                    currentHolding = Instantiate(GameObject.Find("WeaponCat").transform.Find("Kunai").gameObject);
                    currentHolding.name = currentHolding.name.Replace("(Clone)","");
                    lastbag.transform.Find("Count").GetChild(0).name = (Convert.ToInt32(lastbag.transform.Find("Count").GetChild(0).name) - 1).ToString();
                }
                else if(lastbag.name == "RockBag")
                {

                    holding = true;
                    currentHolding = Instantiate(GameObject.Find("WeaponCat").transform.Find("Rock1").gameObject);
                    currentHolding.name = currentHolding.name.Replace("(Clone)", "");
                    lastbag.transform.Find("Count").GetChild(0).name = (Convert.ToInt32(lastbag.transform.Find("Count").GetChild(0).name) - 1).ToString();
                }
                currentHolding.GetComponent<Rigidbody>().isKinematic = true;
                if (currentHolding.transform.parent && currentHolding.transform.parent.CompareTag("BeltSocket"))
                    oldParent = null;
                else
                    oldParent = currentHolding.transform.parent;
                currentHolding.transform.parent = transform;
                currentHolding.transform.localPosition = new Vector3(0, 0, 0);
                currentHolding.transform.localEulerAngles = new Vector3(0, 90, 90);
                transform.GetChild(0).GetComponent<Animator>().SetBool("Holding", true);
                if (currentHolding.GetComponentInChildren<DamageScript>())
                    currentHolding.GetComponentInChildren<DamageScript>().controller = controller;
            }
        }
        if ((SteamVR_Input._default.inActions.GrabGrip.GetStateUp(controller) || SteamVR_Input._default.inActions.GrabPinch.GetStateUp(controller)) && holding == true && lastbag && holdCD)
        {
            state = "off";
            if (lastbag.name == "KunaiBag")
            {
                if (Convert.ToInt32(lastbag.transform.Find("Count").GetChild(0).name) < 3 && currentHolding.name == "Kunai")
                {
                    holding = false;
                    lastbag.transform.Find("Count").GetChild(0).name = (Convert.ToInt32(lastbag.transform.Find("Count").GetChild(0).name) + 1).ToString();
                    Destroy(currentHolding);
                    currentHolding = null;
                    transform.GetChild(0).GetComponent<Animator>().SetBool("Holding", false);
                    transform.GetChild(0).GetComponent<Animator>().SetBool("Gun", false);
                }
            }else if(lastbag.name == "RockBag")
            {
                if (Convert.ToInt32(lastbag.transform.Find("Count").GetChild(0).name) < 5 && (currentHolding.name == "Rock1" ||currentHolding.name == "Rock2" ))
                {
                    holding = false;
                    lastbag.transform.Find("Count").GetChild(0).name = (Convert.ToInt32(lastbag.transform.Find("Count").GetChild(0).name) + 1).ToString();
                    Destroy(currentHolding);
                    currentHolding = null;
                    transform.GetChild(0).GetComponent<Animator>().SetBool("Holding", false);
                    transform.GetChild(0).GetComponent<Animator>().SetBool("Gun", false);//
                }
            }
        }
        if ((SteamVR_Input._default.inActions.GrabGrip.GetStateDown(controller) || SteamVR_Input._default.inActions.GrabPinch.GetStateDown(controller)) && holding == false && currentSelected && !currentHolding && otherController.GetComponent<PickupScript>().HoldingObject() != currentSelected && state == "off")
        {
            state = "on";
            holdCD = false;
            StartCoroutine(holdingcooldown());
            holding = true;
            currentHolding = currentSelected;
            if (currentHolding.transform.parent && currentHolding.transform.parent.CompareTag("BeltSocket") && currentHolding.transform.parent.childCount > 1)
            {
                int price = Convert.ToInt32(currentHolding.transform.parent.Find("Price").GetComponentInChildren<Text>().text);

                if (currentHolding.name.ToLower().Contains("rock") || currentHolding.name.ToLower().Contains("kunai") || currentHolding.name == "BigHealth" || currentHolding.name == "SmallHealth" || currentHolding.name == "BigSpeed" || currentHolding.name == "SmallSpeed" || currentHolding.name == "BigDamage" || currentHolding.name == "SmallDamage")
                {
                    if (GameObject.Find("Camera").GetComponent<PlayerScript>().coins > price)
                    {
                        GameObject.Find("Camera").GetComponent<PlayerScript>().coins -= price;
                        currentHolding = Instantiate(GameObject.Find("WeaponCat").transform.Find(currentHolding.name).gameObject);
                        currentHolding.name = currentHolding.name.Replace("(Clone)", "");
                        playerScript.gameObject.GetComponent<AudioSource>().clip = playerScript.coinsSound;
                        playerScript.gameObject.GetComponent<AudioSource>().Play();
                    }
                    else
                    {
                        currentHolding = null;
                        return;
                    }
                }
                else
                {
                    if (GameObject.Find("Camera").GetComponent<DataHandler>().toolUnlocks[currentHolding.name])
                    {
                        currentHolding = Instantiate(currentHolding);
                        currentHolding.name = currentHolding.name.Replace("(Clone)", "");
                    }
                    else
                    if (GameObject.Find("Camera").GetComponent<PlayerScript>().coins > price)
                    {
                        GameObject.Find("Camera").GetComponent<PlayerScript>().coins -= price;
                        GameObject.Find("Camera").GetComponent<DataHandler>().toolUnlocks[currentHolding.name] = true;
                        currentHolding.transform.parent.Find("Price").GetComponent<Canvas>().enabled = false;
                        currentHolding = Instantiate(GameObject.Find("WeaponCat").transform.Find(currentHolding.name).gameObject);
                        currentHolding.name = currentHolding.name.Replace("(Clone)", "");
                        GameObject.Find("Camera").GetComponent<DataHandler>().saveData();
                        playerScript.gameObject.GetComponent<AudioSource>().clip = playerScript.coinsSound;
                        playerScript.gameObject.GetComponent<AudioSource>().Play();
                    }
                    else
                    {
                        currentHolding = null;
                        return;
                    }
                }

            }
            EquipWeapon(currentHolding);
        }
        else
        if ((SteamVR_Input._default.inActions.GrabGrip.GetStateUp(controller) || SteamVR_Input._default.inActions.GrabPinch.GetStateUp(controller)) && currentHolding && holding == true && holdCD)
        {
            state = "off";
            gun = false;
            holding = false;
            if (currentHolding.transform.Find("GunPoint"))
                currentHolding.transform.Find("Canvas").GetComponent<Canvas>().enabled = false;
            currentHolding.transform.parent = oldParent;
            oldParent = null;
            currentHolding.GetComponent<Rigidbody>().isKinematic = false;
            if (!currentHolding.CompareTag("Throwable"))
            {
                currentHolding.GetComponent<Rigidbody>().AddTorque(eulerRotation, ForceMode.Impulse);
                currentHolding.GetComponent<Rigidbody>().AddForceAtPosition(posVelocity, transform.position, ForceMode.Impulse);
            }
            else
            {
                currentHolding.GetComponent<Rigidbody>().AddForce(posVelocity, ForceMode.Impulse);
                //currentHolding.transform.forward = currentHolding.GetComponent<Rigidbody>().velocity;
                //currentHolding.transform.GetComponent<Rigidbody>().AddTorque(Quaternion.LookRotation(currentHolding.GetComponent<Rigidbody>().velocity).eulerAngles);
                //currentHolding.transform.rotation = Quaternion.LookRotation(currentHolding.GetComponent<Rigidbody>().velocity).normalized;
                StartCoroutine(ObjectLookAt(currentHolding));
            }
            if (socket && socket.transform.Find("GameObject").childCount == 0 && !currentHolding.CompareTag("Potion") && !currentHolding.CompareTag("Throwable"))
            {
                var v3 = currentHolding.transform.lossyScale;
                currentHolding.transform.SetParent(socket.transform.Find("GameObject").transform, false);
                currentHolding.transform.localScale = new Vector3(1 / currentHolding.transform.parent.lossyScale.x / v3.x, 1 / currentHolding.transform.parent.lossyScale.y / v3.y, 1 / currentHolding.transform.parent.lossyScale.z / v3.z);
                currentHolding.GetComponent<Rigidbody>().isKinematic = true;
                currentHolding.transform.localPosition = new Vector3(0, 0, 0);
                currentHolding.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            if (potionSocket && potionSocket.transform.Find("GameObject").childCount == 0 && currentHolding.CompareTag("Potion"))
            {
                var v3 = currentHolding.transform.lossyScale;
                currentHolding.transform.SetParent(potionSocket.transform.Find("GameObject").transform, false);
                currentHolding.transform.localScale = new Vector3(1 / currentHolding.transform.parent.lossyScale.x / v3.x, 1 / currentHolding.transform.parent.lossyScale.y / v3.y, 1 / currentHolding.transform.parent.lossyScale.z / v3.z);
                currentHolding.GetComponent<Rigidbody>().isKinematic = true;
                currentHolding.transform.localPosition = new Vector3(0, 0, 0);
                currentHolding.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            currentHolding = null;
            transform.GetChild(0).GetComponent<Animator>().SetBool("Holding", false);
            transform.GetChild(0).GetComponent<Animator>().SetBool("Gun", false);
        }

        

    }




    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent && other.transform.parent.gameObject == currentSelected)
        {
            currentSelected = null;
        }
        else if (other.gameObject == socket)
        {
            socket = null;
        }
        else if (other.gameObject == potionSocket)
        {
            potionSocket = null;
        }
        else if (other.name == "KunaiBag" || other.name == "RockBag")
        {
            lastbag = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Holdable"))
        {
            currentSelected = other.transform.parent.gameObject;
        }else if (other.CompareTag("BeltSocket"))
        {
            socket = other.gameObject;
        }else if (other.CompareTag("PotionSocket"))
        {
            potionSocket = other.gameObject;
        }else if (other.name == "KunaiBag" || other.name == "RockBag")
        {
            lastbag = other.gameObject;
        }
    }

    public GameObject HoldingObject() {
        return currentHolding;
    }

    public void EquipWeapon(GameObject weapon) {
        state = "on";
        holding = true;
        currentHolding = weapon;
        currentHolding.GetComponent<Rigidbody>().isKinematic = true;
        if (currentHolding.transform.parent && currentHolding.transform.parent.CompareTag("BeltSocket"))
            oldParent = null;
        else
            oldParent = currentHolding.transform.parent;
        currentHolding.transform.parent = transform;
        currentHolding.transform.localPosition = new Vector3(0, 0, 0);
        currentHolding.transform.localEulerAngles = new Vector3(0, 90, 90);
        transform.GetChild(0).GetComponent<Animator>().SetBool("Holding", true);
        if (currentHolding.GetComponentInChildren<DamageScript>())
            currentHolding.GetComponentInChildren<DamageScript>().controller = controller;
        if (currentHolding.GetComponentInChildren<PotionScript>())
            currentHolding.GetComponentInChildren<PotionScript>().controller = controller;
        if (currentHolding.transform.Find("GunPoint"))
        {
            gun = true;
            transform.GetChild(0).GetComponent<Animator>().SetBool("Gun", true);
            currentHolding.transform.Find("Canvas").GetComponent<Canvas>().enabled = true;
            if (controller == SteamVR_Input_Sources.RightHand)
            {
                currentHolding.transform.Find("Canvas").Find("Energy").localEulerAngles = new Vector3(0, 90, 60);
                currentHolding.transform.Find("Canvas").Find("Energy").GetComponent<Image>().fillClockwise = true;
                currentHolding.transform.Find("Canvas").Find("Backdrop").GetComponent<Image>().fillOrigin = (int)Image.Origin360.Left;
                currentHolding.transform.Find("Canvas").Find("Text").localPosition = new Vector3(0, 0.092f, -0.08f);
            }
            else
            {
                currentHolding.transform.Find("Canvas").Find("Energy").localEulerAngles = new Vector3(0, 90, 120);
                currentHolding.transform.Find("Canvas").Find("Energy").GetComponent<Image>().fillClockwise = false;
                currentHolding.transform.Find("Canvas").Find("Backdrop").GetComponent<Image>().fillOrigin = (int)Image.Origin360.Right;
                currentHolding.transform.Find("Canvas").Find("Text").localPosition = new Vector3(0, 0.092f, 0.202f);
            }
        }
    }

    public void UnequipWeapon() {
        if (currentHolding) {

            if (currentHolding.transform.Find("GunPoint"))
                currentHolding.transform.Find("Canvas").GetComponent<Canvas>().enabled = false;
            currentHolding.transform.parent = oldParent;
            oldParent = null;
            currentHolding.GetComponent<Rigidbody>().isKinematic = false;
            currentHolding = null;
            transform.GetChild(0).GetComponent<Animator>().SetBool("Holding", false);
            currentSelected = null;
            holding = false;
        }
    }
    

    IEnumerator CalcVelocity()
    {
        while (Application.isPlaying)
        {
            var oldPosition = transform.position;
            var lastFrameRotation = transform.eulerAngles;
            
            yield return new WaitForEndOfFrame();
            
            posVelocity = (transform.position - oldPosition) / Time.deltaTime;
            Vector3 deltaRotation = transform.eulerAngles - lastFrameRotation;
            eulerRotation = deltaRotation;
            eulerRotation = eulerRotation / Time.deltaTime;
        }
    }

    IEnumerator bulletRemover(GameObject bulleto)
    {
        yield return new WaitForSeconds(3);
        Destroy(bulleto);
    }

    IEnumerator bulletcooldown()
    {
        gunCooldown = true;
        yield return new WaitForSeconds(0.01f);
        gunCooldown = false;
    }

    IEnumerator holdingcooldown()
    {
        yield return new WaitForSeconds(0.25f);
        holdCD = true;
    }

    IEnumerator ObjectLookAt(GameObject throwable)
    {
        yield return new WaitForFixedUpdate();

        throwable.transform.up = posVelocity.normalized;
        yield return new WaitForFixedUpdate();
        while (throwable.GetComponent<Rigidbody>().velocity.sqrMagnitude > 0.2)
        {
            throwable.transform.up = throwable.GetComponent<Rigidbody>().velocity.normalized;
            yield return new WaitForFixedUpdate();
        }
    }
    
    
}
