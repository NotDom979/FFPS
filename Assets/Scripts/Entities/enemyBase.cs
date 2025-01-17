﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyBase : MonoBehaviour
{
    [SerializeField] public GameObject EnemyCanvas;
    [SerializeField] public GameObject rubbleParticle;
    [SerializeField] public GameObject Detector;
    [SerializeField] GameObject HeadPos;
    [SerializeField] public GameObject target;
    [SerializeField] public Animator animator;
    [SerializeField] int sightDistance;
    [SerializeField] int TargetsightDistance;
    [SerializeField] int roamDist;
    [SerializeField] public int viewAngle;
    [SerializeField] public int speedChase;
    [SerializeField] int FacePlayerSpeed;

    [SerializeField] public NavMeshAgent agent;
    [SerializeField] Renderer model;
    
    [Header("-----Item Drop-----")]
    [SerializeField] GameObject[] itemsDrops;
    [SerializeField] public int randItem;
    private int grabItem;

    [Header("-----Audios-----")]
    [SerializeField] public AudioSource footSteps;
    [SerializeField] public AudioSource attackSound;
    [SerializeField] public AudioSource hitSounds;
    [SerializeField] public AudioSource deathSound;
    [SerializeField] public AudioSource growling;


    [Header("-----Extras-----")]
    public Image enemyHpBar;
    public float maxHealth;
    public float currentHealth;

    Vector3 startPos;
    Vector3 targetDirection;
    Vector3 playerDirection;

    float stoppingDistOrigin;
    public float angle;
    public float speedPatrol;

    public bool InRadius;
    bool playerSeen;
    // Start is called before the first frame update
    virtual protected void Awake()
    {
        // EnemyCanvas = GameObject.FindGameObjectWithTag("EnemyCanvas");
        GameManager.instance.enemyNumber++;
        GameManager.instance.enemyCountText.text = GameManager.instance.enemyNumber.ToString("F0");
        currentHealth = maxHealth;
        playerSeen = false;
        stoppingDistOrigin = agent.stoppingDistance;
        target = GameObject.FindGameObjectWithTag("Target");
        Instantiate(rubbleParticle, gameObject.transform.position, transform.rotation);
        startPos = transform.position;
        //agent.SetDestination(target.transform.position);
        speedPatrol = agent.speed;
        agent.speed = speedChase;
        //Roam();
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        if (GameManager.instance.pauseMenu.activeSelf == false)
        {
            if (agent.enabled)
            {
                footSteps.enabled = true;
                growling.enabled = true;
                Detection();
                animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * 3));
                if (InRadius)
                {
                    playerDirection = GameManager.instance.player.transform.position - HeadPos.transform.position;
                    angle = Vector3.Angle(playerDirection, transform.forward);

                    CanSeePlayer();

                }
                else
                {
                    //FindTarget();
                }

            }
        }
        else
        {
            footSteps.enabled = false;
            growling.enabled = false;
        }

    }
    virtual protected void Detection()
    {
        if (gameObject.GetComponentInChildren<DetectionRadius>().inRadius == true)
        {
            InRadius = true;
            //facePlayer();
        }
        else
        {
            InRadius = false;
            agent.stoppingDistance = stoppingDistOrigin;
        }
    }

    public void faceTarget()
    {
        Quaternion rotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * FacePlayerSpeed);

    }

    public void facePlayer()
    {
        playerDirection.y = 0;
        Quaternion rotation = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * FacePlayerSpeed);
    }
    public void takeDamage(float dmg)
    {
        currentHealth -= dmg;
        enemyHpBar.fillAmount = currentHealth / maxHealth;
        hitSounds.Play();
        if (currentHealth <= 0)
        {
            StartCoroutine(death());
        }
        else
        {
           
            StartCoroutine(flashDamage());
            facePlayer();
            agent.SetDestination(GameManager.instance.player.transform.position);
            
        }
    }
    virtual protected IEnumerator death()
    {
        EnemyCanvas.SetActive(false);
        agent.speed = 0;
        agent.enabled = false;
        deathSound.Play();
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
        RandomItem();
        GameManager.instance.CheckEnemyTotal();
    }
    //virtual protected void Roam()
    //{
    //    agent.stoppingDistance = 0;
    //    agent.speed = speedPatrol;

    //    Vector3 randomDir = Random.insideUnitSphere * roamDist;
    //    randomDir += startPos;

    //    NavMeshHit hit;
    //    NavMesh.SamplePosition(randomDir, out hit, .5f, 1);
    //    NavMeshPath path = new NavMeshPath();
    //    if (hit.hit == true)
    //    {
    //        if (hit.position != null)
    //        {
    //            agent.CalculatePath(hit.position, path);
    //        }
    //    }
    //    agent.SetPath(path);
    //}
    virtual protected void CanSeePlayer()
    {
        RaycastHit hit;

        if (Physics.Raycast(HeadPos.transform.position, playerDirection, out hit, sightDistance))
        {
            Debug.DrawRay(HeadPos.transform.position, playerDirection);
            Debug.Log(angle);
            if (hit.collider.CompareTag("Player"))
            {
                playerSeen = true;
                if (angle <= viewAngle)
                {
                    //agent.speed = speedChase;
                    agent.stoppingDistance = stoppingDistOrigin;
                    //facePlayer();
                    agent.SetDestination(GameManager.instance.player.transform.position);

                }
            }



        }


    }

    public void payDay(int currency)
    {
        GameManager.instance.bankTotal += currency;
    }

    //virtual protected void FindTarget()
    //{
    //    int i = 0;
    //    RaycastHit hit;

    //    if (Physics.Raycast(HeadPos.transform.position, targetDirection, out hit, TargetsightDistance))
    //    {
    //        Debug.DrawRay(HeadPos.transform.position, targetDirection);
    //        Debug.Log(angle);
    //        if (hit.collider.CompareTag("Target"))
    //        {


    //                agent.speed = speedChase;
    //                agent.stoppingDistance = stoppingDistOrigin;
    //                agent.SetDestination(target.transform.position);


    //        }


    //    }

    //    agent.SetDestination(target.transform.position);
    //}

    public void RandomItem()
    {
        randItem = Random.Range(0, 4);



        if (randItem == 2)
        {

            Instantiate(itemsDrops[2], transform.position + (transform.up * 1.3f), Quaternion.identity);
        }
        else if (randItem == 1)
        {
            Instantiate(itemsDrops[1], transform.position + (transform.up * 1.3f), Quaternion.identity);


        }
        else if (randItem == 3)
        {

            Instantiate(itemsDrops[3], transform.position + (transform.up * 1.3f), Quaternion.identity);


        }
        else if (randItem == 0)
        {
            Instantiate(itemsDrops[0], transform.position + (transform.up * 1.3f), Quaternion.identity);
        }
        Debug.Log(itemsDrops);
    }

    virtual protected IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        agent.speed = 0;
        yield return new WaitForSeconds(.5f);
        model.material.color = Color.white;
        agent.speed = speedPatrol;
        agent.stoppingDistance = 0;
    }

   
}
