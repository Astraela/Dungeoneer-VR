using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR;

public class PlayerScript : MonoBehaviour {

    public AudioClip oof;
    public AudioClip teleport;
    public AudioClip switchEffect;
    public AudioClip coinsSound;

    Image RedFade;
    GameObject Book;
    private int Ammo = 200;
    public int ammo
    {
        get
        {
            return Ammo;
        }
        set
        {
            Ammo = Mathf.Clamp(value, 0, 200);
        }
    }
    private int Coins = 0;
    public int coins
    {
        get
        {
            return Coins;
        }
        set
        {
            Coins = Mathf.Clamp(value, 0, int.MaxValue);
        }
    }
    private int Health = 250;
    public int maxHealth = 250;
    public int health
    {
        get
        {
            return Health;
        }
        set 
        {
            Health = Mathf.Clamp(value, 0, maxHealth);
        }
    }
    public int stepBonus = 0;
    public int damageBonus = 0;
    void Start () {
        RedFade = transform.Find("Canvas").Find("RedFade").GetComponent<Image>();
	}
	
	void Update () {
        if (health <= 0)
        {
            if (SceneManager.GetActiveScene().name == "Toren")
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            else
                SceneManager.LoadScene("Plein");
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            GameObject.Find("Camera").GetComponent<DataHandler>().DeleteData();
            SceneManager.LoadScene("Plein");
        }
        if ((SteamVR_Input._default.inActions.GrabGrip.GetState(SteamVR_Input_Sources.LeftHand) && SteamVR_Input._default.inActions.GrabGrip.GetState(SteamVR_Input_Sources.RightHand) && SteamVR_Input._default.inActions.GrabPinch.GetState(SteamVR_Input_Sources.LeftHand) && SteamVR_Input._default.inActions.GrabPinch.GetState(SteamVR_Input_Sources.RightHand)) || SteamVR_Input._default.inActions.Menu.GetStateUp(SteamVR_Input_Sources.Any))
        {
            GameObject cam = GameObject.Find("Camera");
            if (Book == null)
                Book = Instantiate(Resources.Load("Book")) as GameObject;
            Book.transform.position = cam.transform.forward * 2 + cam.transform.position;
            Book.transform.position = new Vector3(Book.transform.position.x, GameObject.Find("[CameraRig]").transform.position.y + 1.3f, Book.transform.position.z);
            Vector3 lookat = new Vector3(cam.transform.position.x, Book.transform.position.y, cam.transform.position.z);
            Book.transform.LookAt(lookat);
            Book.transform.Rotate(30, 0, 0);
            if (Book.GetComponent<MainBook>().goingon)
            {
                Book.GetComponent<MainBook>().interacted = true;
            }
            StartCoroutine(Book.GetComponent<MainBook>().BookDelete());
        }

    }

    public void Damage(int amount)
    {
        Health = Health - amount;
        GetComponent<AudioSource>().clip = oof;
        GetComponent<AudioSource>().Play();
        StartCoroutine(Flash());
    }

    public IEnumerator DamagePotion(int ammount)
    {
        damageBonus += ammount;
        yield return new WaitForSeconds(ammount);
        damageBonus -= ammount;
    }

    public IEnumerator StepPotion(int ammount)
    {
        stepBonus += ammount;
        yield return new WaitForSeconds(ammount);
        stepBonus -= ammount;
    }


    IEnumerator Flash()
    {
        RedFade.color = new Color(1,1,1,1);
        yield return new WaitForEndOfFrame();
        while (RedFade.color.a > 0)
        {
            RedFade.color = new Color(1,1,1,RedFade.color.a - 0.01f);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator BookDelete()
    {
        yield return new WaitForSeconds(5);
        Destroy(Book);
        Book = null;
    }

    private void OnApplicationQuit()
    {
        if (!Application.isEditor)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }

    public GameObject GetEnemy(GameObject weapon) {
        GameObject enemy = null;
        GameObject oldEnemy = weapon;
        while (!oldEnemy.CompareTag("Enemy")) {
            GameObject currentEnemy = oldEnemy.transform.parent.gameObject;
            if (currentEnemy.CompareTag("Enemy"))
            {
                enemy = currentEnemy;
                break;
            }
            else {
                oldEnemy = currentEnemy;
            }
        }
        return enemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyWeapon"))
        {
            GameObject enemy = GetEnemy(other.gameObject);
            int damage = enemy.GetComponent<EnemyController>().damage;
            if (enemy.GetComponent<EnemyController>().IsAttacking() && !enemy.GetComponent<EnemyController>().hasDamagedPlayer)
            {
                enemy.GetComponent<EnemyController>().hasDamagedPlayer = true;
                Damage(damage);
            }
        }
        else if (other.CompareTag("Projectile")) {
            Damage(20);
        }
    }

}


