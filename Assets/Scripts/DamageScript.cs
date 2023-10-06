using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class DamageScript : MonoBehaviour {

    public SteamVR_Input_Sources controller;
    public int damage = 10;
    public int minDamage = 15;
    public int maxDamage = 120;
    Vector3 oldpos;
    GameObject dText; 
    string rDamage = "";
    private Vector3 posVelocity;
    Vector3 eulerRotation;
    GameObject lastcol;
    bool cd = false;
    bool haptic = false;
    int hapticCount = 0;
    List<UnityEngine.Object> enemiesHit = new List<UnityEngine.Object>();
    List<UnityEngine.Object> enemiesEntered = new List<UnityEngine.Object>();
    // Use this for initialization
    void Start () {
        dText = GameObject.Find("Damage text");
        StartCoroutine(CalcVelocity());
	}
	
	// Update is called once per frame
	void FixedUpdate () { 

        float Damage = damage * (Mathf.Clamp(posVelocity.magnitude/3  ,0,10f)) ; // + Mathf.Clamp(eulerRotation.magnitude/100  ,0,1)
            rDamage = Mathf.Round(Mathf.Clamp(Damage, 0, maxDamage)).ToString();
        //GameObject.Find("quick text").GetComponent<TextMesh>().text = velocity.magnitude + " || " + rotvelocity.magnitude + " || " + realdamage;
        //print(velocity.magnitude + " || " + rotvelocity.magnitude + " || " + realdamage);
        if (haptic && controller != SteamVR_Input_Sources.Any)
        {
            SteamVR_Input._default.outActions.Haptic.Execute(0, Time.deltaTime, 320, 1, controller);
            //SteamVR_Input.__actions_default_out_Haptic.Execute(0, 1, 320, 1, controller);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        float n1 = 0;
        float.TryParse(rDamage, out n1);
        var collision = other.GetContact(0).otherCollider.gameObject;
        print(collision.name);
        if (n1 != 0 && collision.gameObject.CompareTag("Enemy") && !enemiesHit.Find(o => o == collision.transform.parent.gameObject) && !enemiesEntered.Find(o => o == collision.transform.parent.gameObject))
        {

            rDamage = Mathf.Round((Convert.ToInt32(rDamage) * float.Parse(collision.gameObject.name))).ToString();
            if (Convert.ToInt32(rDamage) <= minDamage)
                return;
            rDamage = (Convert.ToInt32(rDamage) + GameObject.Find("Camera").GetComponent<PlayerScript>().damageBonus).ToString();
            lastcol = collision.gameObject;
            Vector3 contact = other.contacts[0].point;//collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            //dText.GetComponent<TextMesh>().text = rDamage;

            if (collision.transform.parent && collision.transform.parent.GetComponent<EnemyController>())
            {
                collision.transform.parent.GetComponent<EnemyController>().TakeDamage(Convert.ToInt32(rDamage));
            }else if (collision.transform.parent && collision.transform.parent.parent && collision.transform.parent.parent.GetComponent<EnemyController>())
            {
                collision.transform.parent.parent.GetComponent<EnemyController>().TakeDamage(Convert.ToInt32(rDamage));
            }
            StartCoroutine(damageText(rDamage, contact));
            StartCoroutine(hapticCooldown());
            var scripto = GameObject.Find("[CameraRig]").transform.Find("Camera").GetComponent<PlayerScript>();
            scripto.ammo = scripto.ammo + 30;
            enemiesHit.Add(collision.transform.parent.gameObject);
            enemiesEntered.Add(other.gameObject);
            StartCoroutine(cooldown(collision.transform.parent.gameObject));
        }else
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemiesEntered.Add(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        float n1 = 0;
        float.TryParse(rDamage, out n1);
        if (n1 != 0 && collision.gameObject.CompareTag("Enemy") && !enemiesHit.Find(o => o == collision.transform.parent.gameObject) && !enemiesEntered.Find(o => o == collision.transform.parent.gameObject))
        {

            rDamage = Mathf.Round((Convert.ToInt32(rDamage) * float.Parse(collision.gameObject.name))).ToString();
            if (Convert.ToInt32(rDamage) <= minDamage)
                return;
            rDamage = (Convert.ToInt32(rDamage) + GameObject.Find("Camera").GetComponent<PlayerScript>().damageBonus).ToString();
            lastcol = collision.gameObject;
            Vector3 contact = Vector3.zero;

            contact = collision.gameObject.GetComponent<Renderer>().bounds.center + new Vector3(0, collision.gameObject.GetComponent<Renderer>().bounds.extents.y,0);
            //contact = collision.ClosestPointOnBounds(transform.position);
            //contact.y = transform.position.y;
            //dText.GetComponent<TextMesh>().text = rDamage;

            if (collision.transform.parent && collision.transform.parent.GetComponent<EnemyController>())
            {
                collision.transform.parent.GetComponent<EnemyController>().TakeDamage(Convert.ToInt32(rDamage));
            }
            else if (collision.transform.parent && collision.transform.parent.parent && collision.transform.parent.parent.GetComponent<EnemyController>())
            {
                collision.transform.parent.parent.GetComponent<EnemyController>().TakeDamage(Convert.ToInt32(rDamage));
            }
            StartCoroutine(damageText(rDamage, contact));
            StartCoroutine(hapticCooldown());
            var scripto = GameObject.Find("[CameraRig]").transform.Find("Camera").GetComponent<PlayerScript>();
            scripto.ammo = scripto.ammo + 30;
            enemiesHit.Add(collision.transform.parent.gameObject);
            enemiesEntered.Add(collision.transform.parent);
            StartCoroutine(cooldown(collision.transform.parent.gameObject));
        }
        else
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemiesEntered.Add(collision.transform.parent);
        }
    }
    private void OnCollisionExit (Collision other)
    {
        if (other.gameObject && enemiesEntered.Find(o=>o == other.gameObject))
        {
            lastcol = null;
            enemiesEntered.Remove(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.gameObject && enemiesEntered.Find(o => o == other.transform.parent.gameObject))
        {
            lastcol = null;
            enemiesEntered.Remove(other.transform.parent.gameObject);
        }
    }

    IEnumerator damageText(string damage, Vector3 position)
    {
        GameObject cText = GameObject.Instantiate(dText);
        cText.GetComponent<TextMesh>().text = damage;
        cText.transform.position = position;
        StartCoroutine(damagecoroutine(cText));
        yield return new WaitForSeconds(0.5f);
        GameObject.Destroy(cText);
    }

    IEnumerator damagecoroutine(GameObject text)
    {
        while (text)
        {
            text.transform.LookAt(GameObject.Find("Camera").transform);
            text.transform.position = text.transform.position + new Vector3(0, 0.02f, 0);
            text.GetComponent<TextMesh>().color = new Color(text.GetComponent<TextMesh>().color.r, text.GetComponent<TextMesh>().color.g, text.GetComponent<TextMesh>().color.b, text.GetComponent<TextMesh>().color.a - 0.02f);
            yield return new WaitForSeconds(0.5f/100);
        }
    }

    IEnumerator CalcVelocity()
    {
        var hb = transform.Find("Hitbox");
        if (hb == null)
            hb = transform.parent.Find("Hitbox");
        if (hb == null)
            hb = transform.parent.parent.Find("Hitbox");
        while (Application.isPlaying)
        {
            var oldPosition = hb.transform.position;
            var lastFrameRotation = hb.transform.rotation;
            yield return new WaitForEndOfFrame();
            posVelocity = (hb.transform.position - oldPosition) / Time.deltaTime;
            Quaternion deltaRotation = hb.transform.rotation * Quaternion.Inverse(lastFrameRotation);
            eulerRotation = new Vector3(
                0,
                0,
                Mathf.DeltaAngle(0, Mathf.Round(deltaRotation.eulerAngles.z)));
            eulerRotation = eulerRotation / Time.deltaTime;

            //var oldPosition = transform.position;
            //var lastFrameRotation = transform.rotation;
            //yield return new WaitForEndOfFrame();
            //posVelocity = (transform.position - oldPosition) / Time.deltaTime;
            //Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastFrameRotation);
            //eulerRotation = new Vector3(
            //    0,
            //    0,
            //    Mathf.DeltaAngle(0, Mathf.Round(deltaRotation.eulerAngles.z)));
            //eulerRotation = eulerRotation / Time.deltaTime;
        }
    }

    IEnumerator cooldown(UnityEngine.Object enemy)
    {
        yield return new WaitForSeconds(0.4f);
        enemiesHit.Remove(enemy);
    }

    IEnumerator hapticCooldown() {
        haptic = true;
        hapticCount += 1;
        yield return new WaitForSeconds(0.12f);
        hapticCount -= 1;
        if (hapticCount == 0)
        {
            haptic = false;
        }
    }
}
