using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MOBA.ExtClasses;

public class CharacterScripts : MonoBehaviour
{

    public GeneralCharacter characterAttributes;

    public Transform playerObject { get; set; }

    public int aPower;

    private bool isActivatingAbility;
    private bool characterTimer;

    #region Unity Functions
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            print("Key pressed");
            characterAttributes.Move(Input.mousePosition);
        }
    }
    #endregion


}

public class GeneralCharacter : ICharacter
{

    public float health { get; set; }
    public float aPowerk;
    public float attack { get; set; }
    public float defense { get; set; }
    public float agility { get; set; }
    public float movementSpeed { get; set; }
    public float attackSpeed { get; set; }
    public float mana { get; set; }
    public float abilityPower { get; set; }
    public float physicalPenetration { get; set; }
    public float magicalPenetration { get; set; }
    public List<Ability> abilities { get; set; }
    public List<Equippable> equipment { get; set; }
    public int money { get; set; }

    public Animation attackAnimation { get; set; }
    public Animation walkAnimation { get; set; }
    public Animation idleAnimation { get; set; }
    public Animation deathAnimation { get; set; }
    public Animation lifeAnimation { get; set; }
    #region Other Functions

    public void Move(Vector3 movement)
    {
        transform.position = Vector3.MoveTowards(transform.position, movement, Time.deltaTime * 5);
    }

    public void DecreaseHealth(float attackedAmmount)
    {

    }

    public void HealHealth(float healAmmount)
    {

    }

    public void IncreaseHealth(float increaseAmmount)
    {

    }

    public void Attack(GameObject target)
    {

    }

    public void Equip(GameObject equipable)
    {

    }

    public void CastTargetSelectAbility(GameObject target)
    {

    }

    public void CastAreaSelectAbility(Vector3 location)
    {

    }

    #endregion
}