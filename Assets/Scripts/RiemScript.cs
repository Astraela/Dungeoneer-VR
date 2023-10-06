using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RiemScript : MonoBehaviour {

    PlayerScript playerScript;
    public Transform Canvas;
	// Use this for initialization
	void Start () {
        playerScript = transform.parent.Find("Camera").GetComponent<PlayerScript>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        try
        {
            var p1 = Vector3.Lerp(transform.parent.position, transform.parent.Find("Camera").position, 0.53f);
            transform.position = new Vector3(transform.parent.Find("Camera").position.x, p1.y, transform.parent.Find("Camera").position.z) - transform.forward / 3.2f;
            transform.rotation = new Quaternion(0, transform.parent.Find("Camera").rotation.y, 0, transform.parent.Find("Camera").rotation.w);
            Canvas.Find("Section").Find("Health").GetComponent<Image>().fillAmount = float.Parse(playerScript.health.ToString()) / playerScript.maxHealth;
            Canvas.Find("Section").Find("Coins").GetComponent<Text>().text = playerScript.coins.ToString();
            Canvas.Find("Section").Find("Text").GetComponent<Text>().text = StaticClass.CrossSceneInformation;
        }
        catch (System.Exception)
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyWeapon"))
        {
            GameObject enemy = playerScript.GetEnemy(other.gameObject);
            int damage = enemy.GetComponent<EnemyController>().damage;
            if (enemy.GetComponent<EnemyController>().IsAttacking() && !enemy.GetComponent<EnemyController>().hasDamagedPlayer)
            {
                enemy.GetComponent<EnemyController>().hasDamagedPlayer = true;
                playerScript.Damage(damage);
            }
        }
        else if (other.CompareTag("Projectile"))
        {
            playerScript.Damage(20);
        }
    }
}
