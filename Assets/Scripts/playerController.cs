﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("-----PlayerStats-----")]
    [SerializeField] public CharacterController controller;
    [SerializeField] float playerSpeed = 2.0f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float gravityValue = -9.81f;
    [SerializeField] int jumpsMax;
	[SerializeField] public int HP;
    int HPOrigin;
    private Vector3 playerVelocity;
    private int timesJumped;

    [Header("-----GunStats-----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDmg;
    [SerializeField] GameObject bullet;
    [SerializeField] List<gunStats> gunstats = new List<gunStats>();
	[SerializeField] GameObject model;
	public GameObject hitEffect;
    public GameObject muzzleFlash;
    public AudioSource gunShot;
    public GameObject shotPoint;

    bool isShooting;
    //bool isReloading = false;
    public int selectGun;

    public int maxAmmo;
    public int currentAmmo;
    public int reloadTime;
    private GameObject mfClone;
	private GameObject spClone;
	private GameObject hitEffClone;


    private void Start()
    {
        selectGun = 0;
        HPOrigin = HP;
        respawn();
        currentAmmo = maxAmmo;
        GameManager.instance.AmmoCount.text = currentAmmo.ToString("F0");
    }

    void Update()
    {
        StartCoroutine(reloadGun());
        movement();
        StartCoroutine(shoot());
        gunselect();
    }
    void movement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            timesJumped = 0;
        }
        Vector3 move = (transform.right * Input.GetAxis("Horizontal")) +
        (transform.forward * Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);
        if (Input.GetButton("Sprint"))
        {
            controller.Move(move * Time.deltaTime * (playerSpeed * 1.50f));
        }


        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && timesJumped < jumpsMax)
        {
            timesJumped++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    IEnumerator shoot()
    {
        if ((Input.GetButtonDown("Shoot") || Input.GetButton("Shoot")) && !isShooting)
        {
            if (currentAmmo >= 1)
            {
                isShooting = true;
                mfClone = Instantiate(muzzleFlash, shotPoint.transform.position, transform.rotation);
                mfClone.SetActive(true);
                gunShot.Play();
                StartCoroutine(muzzleWait());
                currentAmmo--;
                GameManager.instance.AmmoCount.text = currentAmmo.ToString("F0");
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
                {
	                //Instantiate(bullet, hit.point, transform.rotation);
	                hitEffClone = Instantiate(hitEffect, hit.point, transform.rotation);
                    if (hit.collider.GetComponent<IDamage>() != null)
                    {
	                    hitEffClone.SetActive(true);
                        hit.collider.GetComponent<IDamage>().takeDamage(shootDmg);
                    }
	                hitEffClone.SetActive(false);
                }

                Debug.Log("ZipBang");
                if (shootRate <= 1)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(shootRate);
                isShooting = false;
                gunShot.Stop();
	            Destroy(mfClone);
	            Destroy(hitEffClone);
            }
        }

    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
	    updatePLayerHud();
	    if (HP >= 0)
	    {
		    StartCoroutine(GameManager.instance.playerDamage());
	    }
	    else if (HP <= 0)
        {
            GameManager.instance.playerDeadMenu.SetActive(true);
            GameManager.instance.cursorLockPause();
        }
    }
    public void gunPickup(gunStats stats)
    {
        shootRate = stats.shootRate;
        shootDist = stats.shootDist;
        shootDmg = stats.shootDamage;
        currentAmmo = stats.ammoCount;
        maxAmmo = stats.maxAmmo;
        muzzleFlash = stats.muzzleEffect;
	    shotPoint.transform.localPosition = stats.shotPoint.transform.localPosition;
	    hitEffect = stats.hitEffect;
        model.GetComponent<MeshFilter>().sharedMesh = stats.gunModel.GetComponent<MeshFilter>().sharedMesh;
        model.GetComponent<MeshRenderer>().sharedMaterial = stats.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        gunstats.Add(stats);
        GameManager.instance.AmmoCount.text = currentAmmo.ToString("F0");
    }
    void gunselect()
    {
        if (gunstats.Count > 1)
        {
            if ((Input.GetKeyDown("q") || Input.GetAxis("Mouse ScrollWheel") > 0) && selectGun < gunstats.Count - 1)
            {
                selectGun++;
                changeGun();
            }
            if ((Input.GetKeyDown("e") || Input.GetAxis("Mouse ScrollWheel") < 0) && selectGun > 0)
            {
                selectGun--;
                changeGun();
            }
        }
        GameManager.instance.AmmoCount.text = currentAmmo.ToString("F0");
    }
    void changeGun()
    {
        shootRate = gunstats[selectGun].shootRate;
        shootDist = gunstats[selectGun].shootDist;
        shootDmg = gunstats[selectGun].shootDamage;
        currentAmmo = gunstats[selectGun].ammoCount;
        muzzleFlash = gunstats[selectGun].muzzleEffect;
	    shotPoint.transform.localPosition = gunstats[selectGun].shotPoint.transform.localPosition;
	    hitEffect = gunstats[selectGun].hitEffect;

        model.GetComponent<MeshFilter>().sharedMesh = gunstats[selectGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        model.GetComponent<MeshRenderer>().sharedMaterial = gunstats[selectGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void updatePLayerHud()
    {
        GameManager.instance.playerHpBar.fillAmount = (float)HP / (float)HPOrigin;
    }
    public void respawn()
    {
        controller.enabled = false;
        HP = HPOrigin;
        updatePLayerHud();
        transform.position = GameManager.instance.spawnPoint.transform.position;
        GameManager.instance.playerDeadMenu.SetActive(false);
        controller.enabled = true;
    }
    IEnumerator reloadGun()
    {

        if (Input.GetKeyDown("r"))
        {
            Debug.Log("Reload");
            yield return new WaitForSeconds(reloadTime);
            currentAmmo = maxAmmo;
        }

    }
    IEnumerator muzzleWait()
    {
        yield return new WaitForSeconds(0.01f);
        mfClone.SetActive(false);
    }
}
