﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour, IDataPersistence
{
    public static GameManager instance;
    [Header("-----GameGoal------")]
    public int flag;
    public int enemyNumber;
    public int bankTotal;
	public GameObject shop;
	public Shop shopScript;
    [Header("-----Player Relations------")]
    public GameObject player;
    public playerController playerScript;
    public GameObject spawnPoint;
    public GameObject checkPoint;
    [Header("-----MENUS-----")]
    public GameObject pauseMenu;
    public GameObject currMenu;
	public GameObject optionMenu;
    public GameObject winMenu;
    public GameObject playerDeadMenu;
	public GameObject playerLoseMenu;
    [Header("-----UI-----")]
    public GameObject damageFlash;
	public GameObject bleedFlash;
	public GameObject poisonFlash;
	public GameObject pressf;
	public GameObject PoisonAlert;
	public GameObject reloadAlert;
	public GameObject BleedAlert;
	public GameObject SniperScope;
    public TextMeshProUGUI enemyCountText;
    public TextMeshProUGUI flagCountText;
    public TextMeshProUGUI AmmoCount;
	public TextMeshProUGUI AmmoClip;
    public TextMeshProUGUI bankAccount;
    public TextMeshProUGUI LethalCount;
    public TextMeshProUGUI bankRupt;
    public TextMeshProUGUI pressFtoInteract;
    public Image playerHpBar;
    public Image playerArmorBar;
	public bool isPaused;
	public int WaveCounter;
    // Start is called before the first frame update
    void Awake()
    {
	    // StartCoroutine(pressF());
        bankRupt.enabled = false;
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
	    spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
	    bankTotal = 50;
	    shop = GameObject.FindGameObjectWithTag("Shop");
	    shopScript = shop.GetComponent<Shop>();
	    shop.SetActive(false);
	    
	    

    }

    // Update is called once per frame
    void Update()
	{
		if (damageFlash.activeSelf == false)
			{
	    checkWin();
			if (Input.GetButtonDown("Cancel") && !playerDeadMenu.activeSelf && !winMenu.activeSelf && !optionMenu.activeSelf && !playerLoseMenu.activeSelf)
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
            if (isPaused)
            	{
                	cursorLockPause();
            	}
            else
        		{
        	    	cursorUnLockUnPause();
            	}
        }
			else if(Input.GetButtonDown("Cancel"))
			{
				if (optionMenu.activeSelf)
				{
					optionMenu.SetActive(false);
					cursorUnLockUnPause();
				}
				else if (playerDeadMenu.activeSelf == true || winMenu.activeSelf == true || playerLoseMenu.activeSelf == true)
				{
					GameManager.instance.cursorUnLockUnPause();
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
					playerDeadMenu.SetActive(false);
					playerLoseMenu.SetActive(false);
					winMenu.SetActive(false);
				}
			}
			}
		
    }
    public void cursorLockPause()
	{
		damageFlash.SetActive(false);
		bleedFlash.SetActive(false);
		poisonFlash.SetActive(false);
		SniperScope.SetActive(false);
        reloadAlert.SetActive(false);
        pressf.SetActive(false);
		Time.timeScale = 0;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;			
		

    }
    public void cursorUnLockUnPause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
        pauseMenu.SetActive(isPaused);
    }
    public IEnumerator playerDamage()
    {
        if (playerScript.HP >= 2)
        {
            damageFlash.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            damageFlash.SetActive(false);
        }

    }
	public IEnumerator bleedflash()
	{
		if (playerScript.HP >= 2)
		{
			bleedFlash.SetActive(true);
			yield return new WaitForSeconds(0.1f);
			bleedFlash.SetActive(false);
		}
	}
	public IEnumerator poisonflash()
	{
		if (playerScript.HP >= 2)
		{
			poisonFlash.SetActive(true);
			yield return new WaitForSeconds(0.1f);
			poisonFlash.SetActive(false);
		}
	}

    public void CheckBankTotal()
    {
        if (bankTotal < 0)
        {
            bankTotal = 0;
        }

        bankAccount.text = bankTotal.ToString("F0");

    }
    public void CheckEnemyTotal()
    {
	    enemyNumber--;
	    if (enemyNumber == -1)
	    {
	    	enemyNumber = 0;
	    }
	    enemyCountText.text = enemyNumber.ToString("F0");

    }
    public void WinCondition()
    {
        flag++;
        flagCountText.text = flag.ToString("F0");
        if (flag == 3 && enemyNumber == 0)
        {
            winMenu.SetActive(true);
            cursorLockPause();
        }
    }
    public void checkWin()
    {
        if (flag == 1 && enemyNumber == 0)
        {
            winMenu.SetActive(true);
            cursorLockPause();
        }
    }

    IEnumerator pressF()
    {

        pressFtoInteract.enabled = true;
        yield return new WaitForSeconds(2);
        pressFtoInteract.enabled = false;
    }

    public void LoadData(GameData data)
    {
        this.bankTotal = data.moneySave;
       
    }

    public void SaveData(GameData data)
    {
        data.moneySave = this.bankTotal;
    }
}
