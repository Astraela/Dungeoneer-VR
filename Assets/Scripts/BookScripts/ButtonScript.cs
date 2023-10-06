using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour
{

    public UnityEvent buttonEvent;
    private Vector3 startPosition;
    private float topY;
    private GameObject currentHitbox;
    private bool beingPushed = false;
    private bool pressedButton = false;
    private bool effectActive = false;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
        topY = transform.GetChild(0).position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (beingPushed && currentHitbox)
        {
            Vector3 controllerPos = currentHitbox.transform.position;
            float distanceY = Mathf.Clamp(topY - controllerPos.y, 0, 0.1f);
            transform.localPosition = startPosition - new Vector3(0, distanceY * 10, 0);
            if (distanceY == 0.1f && buttonEvent != null && !pressedButton) {
                pressedButton = true;
                if (!effectActive) {
                    effectActive = true;
                    StartCoroutine(PressedEffect());
                }
                GetComponent<AudioSource>().Play();
                buttonEvent.Invoke();
            }
            else if (distanceY < 0.1f && pressedButton) {
                pressedButton = false;
            }
        }
        else {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * 5);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //float distanceY = Mathf.Abs(gameObject.transform.position.y - transform.position.y);
        //float distanceXZ = Vector2.Distance(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z), new Vector2(transform.position.x, transform.position.z));
        Vector3 impactPos = other.ClosestPointOnBounds(other.transform.position) - transform.position;
        if (other.CompareTag("ControllerHitbox") && !beingPushed && (impactPos.y >= 0.5f || (transform.localPosition != startPosition && impactPos.y >= 0.4f))) {
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

    private IEnumerator PressedEffect() {
        Color oldColor = GetComponent<MeshRenderer>().material.color;
        for (float i = 0; i <= 1; i+=0.05f) {
            GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.green, oldColor, i);
            yield return new WaitForSeconds(0.01f);
        }
        GetComponent<MeshRenderer>().material.color = oldColor;
        effectActive = false;
    }
}
