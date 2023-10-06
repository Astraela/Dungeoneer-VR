using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBag : MonoBehaviour
{

    bool picked = false;
    public int coins = 0;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("ControllerHitbox") && picked == false)
        {
            picked = true;
            StartCoroutine(effect());
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("ControllerHitbox") && picked == false)
        {
            picked = true;
            GameObject.Find("Camera").GetComponent<PlayerScript>().coins += coins;
            GetComponent<Renderer>().enabled = false;
            GetComponent<AudioSource>().Play();
            StartCoroutine(effect());
        }
    }

    IEnumerator effect()
    {
        transform.Find("ParticleEmitter").GetComponent<ParticleSystem>().Emit(40);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
