using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BulletScript : MonoBehaviour {

    bool haptic = false;
    int hapticCount = 0;
    GameObject dText;
    bool cd = false;
    System.Random rnd = new System.Random();
    public SteamVR_Input_Sources controller;
    // Use this for initialization
    void Start ()
    {
        dText = GameObject.Find("Damage text");
        StartCoroutine(hapticCooldown());
    }
	
	// Update is called once per frame
	void Update () {
        if (haptic && controller != SteamVR_Input_Sources.Any)
        {
            SteamVR_Input._default.outActions.Haptic.Execute(0, 1, 320, 1, controller);
            //SteamVR_Input.__actions_default_out_Haptic.Execute(0, 1, 320, 1, controller);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && cd == false)
        {
            
            Vector3 contact = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            //dText.GetComponent<TextMesh>().text = rDamage;
            var damage = Math.Floor(rnd.Next(23, 28) * float.Parse(collision.gameObject.name));
            collision.transform.parent.parent.GetComponent<EnemyController>().TakeDamage(Convert.ToInt32(damage));
            StartCoroutine(damageText(damage.ToString(), contact));
            StartCoroutine(cooldown());
        }
    }

    public IEnumerator hapticCooldown()
    {
        haptic = true;
        hapticCount += 1;
        yield return new WaitForSeconds(0.06f);
        hapticCount -= 1;
        if (hapticCount == 0)
        {
            haptic = false;
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
            yield return new WaitForSeconds(0.5f / 100);
        }
    }

    IEnumerator cooldown()
    {
        cd = true;
        yield return new WaitForSeconds(0.2f);
        cd = false;
    }
}
