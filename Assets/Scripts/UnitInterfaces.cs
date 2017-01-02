using UnityEngine;
using System.Collections;
using System.Threading;

public interface Unit{
    public float health { get; set; }
    public float attack { get; set; }
    public float defense { get; set; }
    public float agility { get; set; }
    public float movementSpeed { get; set; }
    public float attackSpeed { get; set; }
    public Animation attackAnimation { get; set; }
    public Animation walkAnimation { get; set; }
    public Animation idleAnimation { get; set; }
    public Animation deathAnimation { get; set; }
    public Animation lifeAnimation { get; set; }

    public void Move(Vector3 movement)
    {

    }

    public void DecreaseHealth(double attackedAmmount)
    {

    }

    public void HealHealth(double healAmmount)
    {

    }

    public void IncreaseHealth(double increaseAmmount)
    {

    }

    public void Attack(GameObject target)
    {

    }
}

public interface Mob : Unit
{
    public float health { get; set; }
    public float attack { get; set; }
    public float defense { get; set; }
    public float agility { get; set; }
    public float movementSpeed { get; set; }
    public float attackSpeed { get; set; }
    public Animation attackAnimation { get; set; }
    public Animation walkAnimation { get; set; }
    public Animation idleAnimation { get; set; }
    public Animation deathAnimation { get; set; }
    public Animation lifeAnimation { get; set; }

    public void Move(Vector3 movement)
    {

    }

    public void DecreaseHealth(double attackedAmmount)
    {

    }

    public void HealHealth(double healAmmount)
    {

    }

    public void IncreaseHealth(double increaseAmmount)
    {

    }

    public void Attack(GameObject target)
    {

    }
}

public interface SuperMob : Unit
{
    public float health { get; set; }
    public float attack { get; set; }
    public float defense { get; set; }
    public float agility { get; set; }
    public float movementSpeed { get; set; }
    public float attackSpeed { get; set; }
    public Animation attackAnimation { get; set; }
    public Animation walkAnimation { get; set; }
    public Animation idleAnimation { get; set; }
    public Animation deathAnimation { get; set; }
    public Animation lifeAnimation { get; set; }

    public void Move(Vector3 movement)
    {

    }

    public void DecreaseHealth(double attackedAmmount)
    {

    }

    public void HealHealth(double healAmmount)
    {

    }

    public void IncreaseHealth(double increaseAmmount)
    {

    }

    public void Attack(GameObject target)
    {

    }
}

public interface Character : Unit
{
    public float health { get; set; }
    public float mana { get; set; }
    public float attack { get; set; }
    public float defense { get; set; }
    public float agility { get; set; }
    public float abilityPower { get; set; }
    public float movementSpeed { get; set; }
    public float attackSpeed { get; set; }
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

    public void Equip(GameObject equipable)
    {

    }

    public void Move(Vector3 movement)
    {

    }

    public void DecreaseHealth(double attackedAmmount)
    {

    }

    public void HealHealth(double healAmmount)
    {

    }

    public void IncreaseHealth(double increaseAmmount)
    {

    }

    public void Attack(GameObject target)
    {

    }

    public void CastTargetSelectAbility(GameObject target)
    {

    }

    public void CastAreaSelectAbility(Vector3 location)
    {

    }
}
