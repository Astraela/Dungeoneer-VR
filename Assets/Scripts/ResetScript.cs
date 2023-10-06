using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScript : MonoBehaviour {

    public GameObject sword1;
    public GameObject sword2;
    private Vector3 sword1Pos;
    private Vector3 sword1Rot;
    private Vector3 sword2Pos;
    private Vector3 sword2Rot;

    private Vector3 startPosition;
    private float topY;
    private GameObject currentHitbox;
    private bool beingPushed = false;
    private bool canReset = true;

    private void Start()
    {
        sword1Pos = sword1.transform.position;
        sword1Rot = sword1.transform.eulerAngles;
        sword2Pos = sword2.transform.position;
        sword2Rot = sword2.transform.eulerAngles;

        startPosition = transform.localPosition;
        topY = transform.position.y + GetComponent<MeshRenderer>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            GameObject.Find("Camera").GetComponent<DataHandler>().DeleteData();
            SceneManager.LoadScene("Plein");
        }

        if (beingPushed && currentHitbox)
        {
            //GameObject.Find("PositionBall").transform.position = new Vector3(transform.position.x, topY, transform.position.z);
            Vector3 controllerPos = currentHitbox.transform.position;
            Vector3 buttonPos = transform.position;
            float distanceY = Mathf.Clamp(topY - controllerPos.y, 0, 0.05f);
            transform.localPosition = startPosition - new Vector3(0, distanceY, 0);

            if (distanceY == 0.05f && canReset) {
                StartCoroutine(ResetDelay());
                ResetWeapons();
            }
        }
        else {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * 5);
        }
	}

    private void ResetWeapons() {
        GameObject[] controllers = GameObject.FindGameObjectsWithTag("ControllerHitbox");
        foreach (GameObject controller in controllers) {
            controller.GetComponent<PickupScript>().UnequipWeapon();
        }
        sword1.GetComponent<Rigidbody>().isKinematic = true;
        sword1.transform.position = sword1Pos;
        sword1.transform.eulerAngles = sword1Rot;
        sword1.GetComponent<Rigidbody>().isKinematic = false;
        sword2.GetComponent<Rigidbody>().isKinematic = true;
        sword2.transform.position = sword2Pos;
        sword2.transform.eulerAngles = sword2Rot;
        sword2.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ControllerHitbox") && !beingPushed) {
            beingPushed = true;
            currentHitbox = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentHitbox) {
            currentHitbox = null;
            beingPushed = false;
        }
    }

    IEnumerator ResetDelay() {
        canReset = false;
        yield return new WaitForSeconds(2);
        canReset = true;
    }
}
