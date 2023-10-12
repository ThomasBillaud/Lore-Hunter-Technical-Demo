using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    int isWeaponDrawHash;
    int isWeaponMartaxeHash;
    int primaryButtonPressedHash;
    int secondaryButtonPressedHash;
    int specialButtonPressedHash;
    int isPressPossibleHash;
    int isHammerHash;
    float animOriginalSpeed;

    [SerializeField] private InputReader _inputReader = default;
    [SerializeField] private HeadTracking headTrack;
    [SerializeField] private PlayerStats stats;
    PlayerWeaponInventory weaponInventory;
    WeaponItem weapon;
    Animator animator;

    bool isDrawn = false;

    int nbCharge = 0;

    GameObject currentHitbox;

    // Start is called before the first frame update
    void Start()
    {
        weaponInventory = GetComponent<PlayerWeaponInventory>();
        weapon = weaponInventory.weapon;
        animator = GetComponent<Animator>();

        isWeaponDrawHash = Animator.StringToHash("isWeaponDraw");
        isWeaponMartaxeHash = Animator.StringToHash("isWeaponMartaxe");
        primaryButtonPressedHash = Animator.StringToHash("primaryButtonPressed");
        secondaryButtonPressedHash = Animator.StringToHash("secondaryButtonPressed");
        specialButtonPressedHash = Animator.StringToHash("specialButtonPressed");
        isPressPossibleHash = Animator.StringToHash("isPressPossible");
        isHammerHash = Animator.StringToHash("isHammer");
    }

    void DetectWeapon()
    {
        if (weapon.type == WeaponItem.weaponType.Martaxe)
        {
            animator.SetBool(isWeaponMartaxeHash, true);
        }
    }

    public void LoadWeapon()
    {
        weaponInventory.DrawWeapon();
    }

    public void UnloadWeapon()
    {
        weaponInventory.SheathWeapon();
    }

    void OnEnable()
    {
        _inputReader.drawWeaponEvent += OnDraw;
        _inputReader.sheathWeaponEvent += OnSheath;
        _inputReader.primaryAttackEvent += OnPrimaryAttackButton;
        _inputReader.primaryAttackHoldEvent += OnPrimaryAttackButtonHold;
        _inputReader.secondaryAttackEvent += OnSecondaryAttackButton;
        _inputReader.specialMoveEvent += OnSpecialAttackButton;
    }

    void OnDisable()
    {
        _inputReader.drawWeaponEvent -= OnDraw;
        _inputReader.sheathWeaponEvent -= OnSheath;
        _inputReader.primaryAttackEvent -= OnPrimaryAttackButton;
        _inputReader.primaryAttackHoldEvent += OnPrimaryAttackButtonHold;
        _inputReader.secondaryAttackEvent -= OnSecondaryAttackButton;
        _inputReader.specialMoveEvent -= OnSpecialAttackButton;
    }

    private void OnDraw()
    {
        if (animator.GetBool(isPressPossibleHash) == true)
        {
            animator.SetBool(isWeaponDrawHash, true);
            isDrawn = true;
            DetectWeapon();
            headTrack.StopHeadRig();
        }
    }

    private void OnSheath()
    {
		if (animator.GetBool(isPressPossibleHash) == true)
        {
            animator.SetBool(isWeaponDrawHash, false);
            animator.SetBool(isWeaponMartaxeHash, false);
            isDrawn = false;
            headTrack.StartHeadRig();
        }
    }

    private void OnPrimaryAttackButton()
    {
        if(isDrawn && animator.GetBool(isPressPossibleHash) == true)
        {
            animator.SetTrigger(primaryButtonPressedHash);
        }
    }

    private void OnPrimaryAttackButtonHold(bool isHold)
    {
        animator.SetBool("isPrimaryHold", isHold);
    }

    private void OnSecondaryAttackButton()
    {
        if(isDrawn && animator.GetBool(isPressPossibleHash) == true)
        {
            animator.SetTrigger(secondaryButtonPressedHash);
        }
    }

    private void OnSpecialAttackButton()
    {
        if(isDrawn && animator.GetBool(isPressPossibleHash) == true)
        {
            animator.SetTrigger(specialButtonPressedHash);
        }   
    }

    void CanPressForCombo()
    {
        animator.SetBool(isPressPossibleHash, true);
    }

    void DoingCombo()
    {
        animator.ResetTrigger(primaryButtonPressedHash);
        animator.ResetTrigger(secondaryButtonPressedHash);
        animator.ResetTrigger(specialButtonPressedHash);
        animator.SetBool(isPressPossibleHash, false);
    }

    void SwitchMartaxe()
    {
        animator.SetBool(isHammerHash, !animator.GetBool(isHammerHash));
        weaponInventory.GetWeaponModel().transform.Rotate(0, 0, 180);
    }

    void ChargeAnim(int chargeLevel)
    {
        float duration = 0.75f;
        if(animator.GetBool("isPrimaryHold") == true)
        {
            if (nbCharge + 1 == chargeLevel)
            {
                nbCharge += 1;
                animOriginalSpeed = animator.speed;
                animator.speed = 0;
                Debug.Log(animOriginalSpeed);
                StartCoroutine(FreezeAnim(duration));
            }
        }
        if (chargeLevel == 3)
            nbCharge = 0;
    }

    IEnumerator FreezeAnim(float duration)
    {
        for (float i = duration; i > 0; i = i - 0.1f)
        {
            Debug.Log("i = " + i);
            if (i <= 0.1f)
                animator.speed = animOriginalSpeed;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void HitboxAppear(GameObject hitbox)
    {
        currentHitbox = Instantiate(hitbox, weaponInventory.GetWeaponModel().transform);
    }

    public void HitboxDisappear()
    {
        Destroy(currentHitbox);
    }


    public void InflictDamage(Hurtbox target, MonsterHealth health, PlayerHitbox.attackType hitType, int motionValue)
    {
        int damage = 0;

        //Debug.Log("TargetResistance = " + target.bluntHitzoneWeakness);
        //Debug.Log("MonsterHealth = " + health.health);
        //Debug.Log("Attack = " + stats.attack);
        //Debug.Log("MotionValue = " + motionValue);
        if (hitType == PlayerHitbox.attackType.Blunt)
        {
            damage = Mathf.RoundToInt((stats.attack * (motionValue / 100f)) * (target.bluntHitzoneWeakness / 100f));
            if (Random.Range(0f, 100f) < stats.critChance)
            {
                damage = Mathf.RoundToInt(damage * 1.5f);
            }
            health.health -= damage;
            Debug.Log("Damage = " + damage);
        }
    }

}
