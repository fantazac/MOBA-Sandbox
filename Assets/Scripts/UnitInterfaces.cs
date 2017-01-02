using UnityEngine;
using System.Collections.Generic;

public interface Unit
{
    float health { get; set; }
    float attack { get; set; }
    float defense { get; set; }
    float agility { get; set; }
    float movementSpeed { get; set; }
    float attackSpeed { get; set; }

    Animation attackAnimation { get; set; }
    Animation walkAnimation { get; set; }
    Animation idleAnimation { get; set; }
    Animation deathAnimation { get; set; }
    Animation lifeAnimation { get; set; }

    void Move(Vector3 movement);
    void DecreaseHealth(double attackedAmmount);
    void HealHealth(double healAmmount);
    void IncreaseHealth(double increaseAmmount);
    void Attack(GameObject target);
}

public interface Mob : Unit
{

}

public interface SuperMob : Unit
{

}

public interface Character : Unit
{
    float mana { get; set; }
    float abilityPower { get; set; }
    float physicalPenetration { get; set; }
    float magicalPenetration { get; set; }
    List<Ability> abilities { get; set; }
    List<Equippable> equipment { get; set; }
    int money { get; set; }

    void Equip(GameObject equipable);
    void CastTargetSelectAbility(GameObject target);
    void CastAreaSelectAbility(Vector3 location);
}
