using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

    public bool killEnemy = false;
    private List<GameObject> dummies = new List<GameObject>();
    public bool dummy;
    public bool isBoss;
    public bool canRespawn;
	private GameObject player;
	public int maxHealth;
	public int damage;
    public int reward = 50;
	private int health;
	private NavMeshAgent agent;
    private Animator animator;
	private Vector3 spawnPos;
	private bool canSlash;
	private bool canWalkRandom;
    private bool isIdleWalking = false;
    public bool hasDamagedPlayer = false;
	System.Random rand = new System.Random();
    Image healthObj;
	public enum State {
		Idle,
		Running,
		Slashing,
		Dead
	};

	public State currentState;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Camera");
        healthObj = transform.Find("Canvas").Find("Section").Find("Health").GetComponent<Image>();
        health = maxHealth;
        if (!dummy)
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            spawnPos = transform.position;
            currentState = State.Idle;
            canSlash = true;
            canWalkRandom = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (killEnemy)
        {
            TakeDamage(maxHealth / 4);
            killEnemy = false;
        }
        Vector3 playerpos;
        if (Mathf.Abs(transform.position.y - player.transform.position.y) < 3f)
            playerpos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        else
            playerpos = player.transform.position;
        float distance = Vector3.Distance(transform.position, playerpos);
        if (!dummy && currentState != State.Dead)
        {
            float angleToPlayer = Vector3.Angle((player.transform.position - new Vector3(transform.position.x, player.transform.position.y, transform.position.z)), transform.forward);
            if ((distance < 25 && angleToPlayer < 60 || distance < 8) && currentState != State.Slashing && currentState != State.Dead && !Physics.Linecast(transform.position, player.transform.position))
            {
                if (distance > 1.6f)
                {
                    if (canSlash && isBoss)
                    {
                        enemyFire();
                    }
                    else
                    {
                        GameObject closestHexagon = GetClosestHexagon(player.transform.position, player.transform.parent.Find("Controller (left)").Find("Hitbox").GetComponent<MoveScript>().currentHexagon.gameObject);
                        Vector3 finalPos = closestHexagon.transform.position;
                        //GameObject.Find("PositionBallVersion2").transform.position = finalPos + new Vector3(0, closestHexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2, 0);
                        agent.isStopped = false;
                        agent.SetDestination(finalPos);
                        animator.SetBool("Running", true);
                        isIdleWalking = true;
                        currentState = State.Running;
                    }
                }
                else if (currentState != State.Slashing && canSlash)
                {
                    enemySlash();
                    if (angleToPlayer > 60) transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position, transform.up);
                }
            }
            else if (currentState != State.Slashing && currentState != State.Dead && currentState != State.Running)
            {
                if (currentState != State.Idle)
                {
                    currentState = State.Idle;
                    agent.SetDestination(transform.position);
                    agent.isStopped = true;
                }
                WalkToRandomPos();
            }

            if (agent.remainingDistance < 0.1f && isIdleWalking) {
                animator.SetBool("Running", false);
                isIdleWalking = false;
            }
        }
        if ((health < maxHealth && !transform.Find("Canvas").GetComponent<Canvas>().enabled) || distance <= 10)
        {
            transform.Find("Canvas").GetComponent<Canvas>().enabled = true;
        }
        if (transform.Find("Canvas").GetComponent<Canvas>().enabled && distance > 10 && health >= maxHealth)
        {
            transform.Find("Canvas").GetComponent<Canvas>().enabled = false;
        }
        if (transform.Find("Canvas").GetComponent<Canvas>().enabled)
        {
            transform.Find("Canvas").LookAt(GameObject.Find("Camera").transform);
        }
        if (currentState == State.Running && agent.remainingDistance < 0.2f)
        {
            currentState = State.Idle;
        }
	}

	public void TakeDamage(int amount){
        GetComponent<AudioSource>().Play();
        if (!dummy && currentState != State.Dead)
        {
            health -= amount;
            if (currentState == State.Idle)
            {
                Vector3 finalPos = GetClosestHexagon(player.transform.position, player.transform.parent.Find("Controller (left)").Find("Hitbox").GetComponent<MoveScript>().currentHexagon.gameObject).transform.position;
                agent.isStopped = false;
                agent.SetDestination(finalPos);
                animator.SetBool("Running", true);
                isIdleWalking = true;
                currentState = State.Running;
            }
            if (health <= 0 && currentState != State.Dead)
            {
                currentState = State.Dead;
                health = 0;
                GameObject coinBag = Instantiate(Resources.Load("CoinBag")) as GameObject;
                coinBag.transform.position = transform.position + new Vector3(0,5,0);
                coinBag.GetComponent<CoinBag>().coins = reward;
                if (canRespawn)
                {
                    StartCoroutine(respawnEnemy());
                }
                else {
                    foreach (var item in dummies)
                    {
                        Destroy(item);
                    }
                    Destroy(gameObject);
                }
            }
            healthObj.fillAmount = (float)health / (float)maxHealth;
        }
        StartCoroutine(HitEffect());
    }

    private void enemySlash() {
        canSlash = false;
        //player.GetComponent<PlayerScript>().Damage(damage);
        animator.SetTrigger("Slashing");
        StartCoroutine(SlashDelay());
        currentState = State.Slashing;
    }

    private void enemyFire() {
        canSlash = false;
        currentState = State.Slashing;
        //player.GetComponent<PlayerScript>().Damage(damage);
        animator.SetTrigger("Firing");
        StartCoroutine(FireDelay());
        agent.SetDestination(transform.position);
        agent.isStopped = true;

        StartCoroutine(HandleProjectile());
    }

    private GameObject GetClosestHexagon(Vector3 position, GameObject playerHexagon) {
        GameObject[] hexagons = GameObject.FindGameObjectsWithTag("Floor");
        float closestDistance = 999f;
        GameObject closestHexagon = null;
        if (playerHexagon)
        {
            foreach (GameObject hexagon in hexagons)
            {
                float hexagonHeight = hexagon.transform.position.y + hexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2;
                Vector3 hexagonPosition = new Vector3(hexagon.transform.position.x, hexagonHeight, hexagon.transform.position.z);
                float enemyDistance = Vector3.Distance(hexagonPosition, transform.position);
                float hexagonDistance = Vector3.Distance(new Vector3(hexagon.transform.position.x, 0, hexagon.transform.position.z), new Vector3(playerHexagon.transform.position.x, 0, playerHexagon.transform.position.z));
                float distanceY = Mathf.Abs((hexagon.transform.position.y + hexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2) - (playerHexagon.transform.position.y + playerHexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2));
                if (hexagon != playerHexagon && hexagonDistance < 2 && distanceY < 2 && enemyDistance < closestDistance)
                {
                    closestDistance = enemyDistance;
                    closestHexagon = hexagon;
                }
            }
        }
        else {
            foreach (GameObject hexagon in hexagons)
            {
                float hexagonHeight = hexagon.transform.position.y + hexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2;
                Vector3 hexagonPosition = new Vector3(hexagon.transform.position.x, hexagonHeight, hexagon.transform.position.z);
                float distance = Vector3.Distance(hexagonPosition, position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHexagon = hexagon;
                }
            }
        }
        return closestHexagon;
    }

	private void WalkToRandomPos(){
		if (canWalkRandom) {
			canWalkRandom = false;
			StartCoroutine (RandomWalkDelay ());
            Vector3 randomPos = new Vector3(rand.Next((int)transform.position.x - 2, (int)transform.position.x + 2), transform.position.y, rand.Next((int)transform.position.z - 2, (int)transform.position.z + 2));
            GameObject closestHexagon = GetClosestHexagon(randomPos, null);
            Vector3 finalPos = closestHexagon.transform.position + new Vector3(0,closestHexagon.GetComponent<Renderer>().bounds.extents.y/2,0);
            //GameObject.Find("PositionBallVersion2").transform.position = finalPos + new Vector3(0, closestHexagon.GetComponent<MeshRenderer>().bounds.extents.y * 2, 0);
            agent.isStopped = false;
			agent.SetDestination (finalPos);
            animator.SetBool("Running", true);
            isIdleWalking = true;
        }
	}

	private void ResetEnemy(){
        agent.Warp(spawnPos);
		health = maxHealth;
		currentState = State.Idle;
        transform.Find("Canvas").GetComponent<Canvas>().enabled = false;
        healthObj.fillAmount = (float)health / (float)maxHealth;
    }

	IEnumerator SlashDelay(){
		yield return new WaitForSeconds (3);
		canSlash = true;
		currentState = State.Idle;
        hasDamagedPlayer = false;
	}

    IEnumerator FireDelay()
    {
        yield return new WaitForSeconds(2);
        currentState = State.Idle;
        hasDamagedPlayer = false;
        yield return new WaitForSeconds(3);
        canSlash = true;
    }

    IEnumerator RandomWalkDelay(){
		yield return new WaitForSeconds (5);
		canWalkRandom = true;
	}

    IEnumerator HitEffect()
    {
        var prevDummy = Instantiate(gameObject, new Vector3(10000, 10000, 10000), new Quaternion(0, 0, 0, 0), null);
        dummies.Add(prevDummy);
        var prevParts = prevDummy.GetComponentsInChildren<Renderer>();
        var Parts = transform.GetComponentsInChildren<Renderer>();
        prevDummy.GetComponent<EnemyController>().enabled = false;
        float lerper = 1;
        while (lerper > 0)
        {
            int count = 0;
            foreach (var obj in Parts)
            {
                var propertyBlock = new MaterialPropertyBlock();
                propertyBlock.SetColor("_Color", Color.Lerp(prevParts[count].material.color,new Color(1,0,0),lerper));
                obj.SetPropertyBlock(propertyBlock);
                count = count + 1;
            }
            lerper = lerper - 0.04f;
            yield return new WaitForFixedUpdate();
        }
        Destroy(prevDummy);
    }

    IEnumerator respawnEnemy() {
        agent.isStopped = true;
        animator.SetBool("Running", false);
        agent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        agent.Warp(new Vector3(0, 10000, 0));
        yield return new WaitForSeconds(5);
        GetComponent<Rigidbody>().isKinematic = false;
        agent.enabled = true;
        ResetEnemy();
    }

    IEnumerator HandleProjectile() {
        Vector3 shootpoint = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        GameObject projectile = Instantiate(Resources.Load("Bullet")) as GameObject;
        projectile.transform.position = shootpoint;
        projectile.transform.LookAt(player.transform);
        for (int i = 0; i < 200; i++) {
            projectile.transform.position += projectile.transform.forward * Time.deltaTime * 10;
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(projectile);
    }

    public bool IsAttacking() {
        if (currentState == State.Slashing)
        {
            return true;
        }
        else {
            return false;
        }
    }
}
