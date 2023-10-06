using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IntroScript : MonoBehaviour {

    GameObject COF;
    GameObject Pas;
    GameObject PAC;
    GameObject Flash;
    Vector3 COFP;
    Vector3 PasP;
    bool going = false;
    bool phase2 = false;
    float speed = 40;
    // Use this for initialization
    void Start ()
    {
        COF = GameObject.Find("COF");
        Pas = GameObject.Find("Pas");
        PAC = GameObject.Find("PAC");
        Flash = GameObject.Find("Flash");
        Flash.SetActive(false);
        COFP = COF.transform.position;
        PasP = Pas.transform.position;
        COF.transform.position = COFP + new Vector3(50, 100, 0);
        Pas.transform.position = PasP - new Vector3(-25, 100, 0);
        PAC.SetActive(false);
        StartCoroutine(waiter());
        
    }
	
	// Update is called once per frame
	void Update () {
        if (!going)
            return;
        if (COF.transform.position.y > COFP.y)
        {
            COF.transform.position = COF.transform.position - new Vector3(0, Time.deltaTime * speed, 0);
        }
        if (Pas.transform.position.y < PasP.y)
        {
            Pas.transform.position = Pas.transform.position + new Vector3(0, Time.deltaTime * speed, 0);
        }
        if (COF.transform.position.x > COFP.x)
        {
            COF.transform.position = COF.transform.position - new Vector3(Time.deltaTime * (speed/2), 0, 0);
        }
        if (Pas.transform.position.x > PasP.x)
        {
            Pas.transform.position = Pas.transform.position - new Vector3(Time.deltaTime * (speed / 4), 0, 0);
        }
        if (COF.transform.position.y <= COFP.y && COF.transform.position.x <= COFP.x && Pas.transform.position.y >= PasP.y && Pas.transform.position.x <= PasP.x && !phase2)
        {
            phase2 = true;
            StartCoroutine(Flasher());
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(3);
        going = true;
    }

    IEnumerator Flasher()
    {
        float scalespeed = 30;
        //yield return new WaitForSeconds(0.2f);
        Flash.transform.localScale = new Vector3(0, 1.331982f, 0);
        Flash.SetActive(true);
        while (Flash.transform.localScale.x < 16.52)
        {
            yield return new WaitForEndOfFrame();
            Flash.transform.localScale = Flash.transform.localScale + new Vector3(Time.deltaTime * scalespeed,0,Time.deltaTime * scalespeed);
        }
        yield return new WaitForSeconds(0.2f);
        COF.SetActive(false);
        Pas.SetActive(false);
        PAC.SetActive(true);
        Material flashm = Flash.GetComponent<Renderer>().material;
        while (flashm.color.a > 0)
        {
            yield return new WaitForEndOfFrame();
            flashm.color = flashm.color - new Color(0,0,0,Time.deltaTime);
        }
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Plein");
    }
}
