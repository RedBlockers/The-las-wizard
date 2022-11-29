using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    [Header("Other elements references")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private MoveBehaviour moveBehaviour;

    [SerializeField]
    private BasicBehaviour basicBehaviour;

    [SerializeField]
    private GameObject player;

    [Header("HEALTH")]
    [SerializeField]
    private float maxHealth;
    public float currentHealth;

    [SerializeField]
    private Image healthObjective;

    [SerializeField]
    private Image healthBarFill;

    [SerializeField]
    private float healthDecreaseRateForHungerAndThirst;

    [SerializeField]
    private Image healthBarImage;

    [SerializeField]
    private bool invincible;

    [Header("HUNGER")]
    [SerializeField]
    private float maxHunger;
    public float currentHunger;

    [SerializeField]
    private Image hungerBarFill;

    [SerializeField]
    private float hungerDecreaseRate;

    [SerializeField]
    private Image hungerBarImage;

    [Header("THIRST")]
    [SerializeField]
    private float maxThirst;
    public float currentThirst;

    [SerializeField]
    private Image thirstBarFill;

    [SerializeField]
    private float thirstDecreaseRate;

    [SerializeField]
    private Image thirstBarImage;

    [Header("STAMINA")]
    [SerializeField]
    private float maxStamina;
    public float currentStamina;

    [SerializeField]
    private Image staminaBarImage;

    [SerializeField]
    private float staminaDecreaseRate;

    [SerializeField]
    private Image staminaBarFill;

    [SerializeField]
    private float staminaInterercreaseRate;


    [HideInInspector]
    public float currentArmor;
    [HideInInspector]
    public bool isDead = false;
    public bool canSprint = true;
    public bool canJump = true;
    private bool addingStamina;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentStamina = maxStamina;
        StartCoroutine(HungerBarBipper());
        StartCoroutine(HealthBarBipper());
        StartCoroutine(ThirstBarBipper());
        StartCoroutine(StaminaBarBipper());


    }

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.K))
        {
            TakeDamage(10f);
        }
        UpdateHungerAndThurstBarFill();
        UpdateStaminaBarFill();
    }

    public void TakeDamage(float damage,bool overTime = false)
    {
        if (invincible)
        {
            return;
        }
        if (overTime)
        {
            currentHealth -= damage * Time.deltaTime;
        }
        else
        {
            currentHealth -= damage *(1 - (currentArmor/100));
        }
        UpdateHealthBarFill();
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died");
        isDead = true;
        moveBehaviour.canMove = false;

        //bloquer  la diminution des barres
        hungerDecreaseRate = 0;
        thirstDecreaseRate = 0;
        staminaDecreaseRate = 0;
        staminaInterercreaseRate = 0;
        animator.SetTrigger("Die");
    }


    public void ConsumeItem(float health, float hunger, float thirst)
    {
        currentHealth += health;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        currentHunger += hunger;

        if (currentHunger > maxHunger)
        {
            currentHunger = maxHunger;
        }

        currentThirst += thirst;

        if (currentThirst > maxThirst)
        {
            currentThirst = maxThirst;
        }

        UpdateHealthBarFill();
    }

    void UpdateHealthBarFill()
    {
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    void UpdateHungerAndThurstBarFill()
    {

        //diminue la faim et la soif au fils du temps
        currentHunger -= (addingStamina ? hungerDecreaseRate * Time.deltaTime * 4 : hungerDecreaseRate * Time.deltaTime);
        currentThirst -= (addingStamina ? thirstDecreaseRate * Time.deltaTime * 4 : thirstDecreaseRate * Time.deltaTime);


        //on empeche la valeur de passer sous 0
        currentHunger = currentHunger < 0 ? 0 : currentHunger;
        currentThirst = currentThirst < 0 ? 0 : currentThirst;

        //met a jours les barres de soif et de faim
        hungerBarFill.fillAmount = currentHunger / maxHunger;
        thirstBarFill.fillAmount = currentThirst / maxThirst;

        // si la faim ou la soif est en dessous de 0 alors on bassie la vie du personnage (*2 si les deux barres sont a 0)
        if (currentHunger <= 0 || currentThirst <= 0)
        {

            TakeDamage((currentHunger <= 0 && currentThirst <= 0 ? healthDecreaseRateForHungerAndThirst*2 : healthDecreaseRateForHungerAndThirst),true);
        }
    }

    void UpdateStaminaBarFill()
    {
        if (Input.GetKey(KeyCode.LeftShift) && player.GetComponent<Rigidbody>().velocity.magnitude > 1 && canSprint )
        {
            currentStamina -= staminaDecreaseRate * Time.deltaTime;
        }
        else
        {
            if (maxStamina <= currentStamina)
            {
                currentStamina = maxStamina;
                addingStamina = false;
            } else
            {
                addingStamina = true;
                currentStamina += staminaInterercreaseRate * Time.deltaTime;
            }
        }

        if (currentStamina < 10)
        {
            canJump = false;
        }
        else if (!canJump)
        {
            canJump = true;
        }

        if (currentStamina < 0)
        {
            canSprint = false;
        }
        else if (!canSprint && currentStamina > 50)
        {
            canSprint = true;
        }

        staminaBarFill.fillAmount = currentStamina / maxStamina;


    }



    private IEnumerator HungerBarBipper()
    {
        while (true)
        {
            if (currentHunger <= 25)
            {
                hungerBarImage.color = new Color(1,0,0,1);
                yield return new WaitForSeconds(0.3f + currentHunger / 10);
                hungerBarImage.color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(0.3f + currentHunger / 20);

            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator HealthBarBipper()
    {
        while (true)
        {
            if (currentHealth <= 25)
            {
                healthBarImage.color = new Color(1, 0, 0, 1);
                yield return new WaitForSeconds(0.3f + currentHealth / 20);
                healthBarImage.color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(0.3f + currentHealth / 20);

            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator ThirstBarBipper()
    {
        while (true)
        {
            if (currentThirst <= 25)
            {
                thirstBarImage.color = new Color(1, 0, 0, 1);
                yield return new WaitForSeconds(0.3f + currentThirst / 20);
                thirstBarImage.color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(0.3f + currentThirst / 20);

            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private IEnumerator StaminaBarBipper()
    {
        while (true)
        {
            if (currentStamina <= 25)
            {
                staminaBarImage.color = new Color(1, 0, 0, 1);
                yield return new WaitForSeconds(0.3f + currentStamina / 20);
                staminaBarImage.color = new Color(1, 1, 1, 1);
                yield return new WaitForSeconds(0.3f + currentStamina / 20);

            }
            else
            {
                yield return new WaitForSeconds(1f);
            }
        }
    }

}
