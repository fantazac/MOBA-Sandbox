using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MOBA.ExtClasses;

public class CharacterScripts : GeneralCharacter
{
    private bool isActivatingAbility;
    private bool characterTimer;
    private float zCameraOffset;

    #region Unity Functions
    // Use this for initialization
    void Start()
    {
        SetTargetPosition(new Vector3(211, 2, 226));
        zCameraOffset = playerCamera.transform.position.z - playerTransform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Move(ray);
        }
        playerTransform.position = Vector3.MoveTowards(playerTransform.position, GetTargetPosition(), Time.deltaTime * movementSpeed);
        playerCamera.transform.position = Vector3.MoveTowards(playerCamera.transform.position, new Vector3() { x = GetTargetPosition().x, z = GetTargetPosition().z+ zCameraOffset, y = playerCamera.transform.position.y }, Time.deltaTime * movementSpeed);
    }
    #endregion


}
namespace MOBA.ExtClasses
{
    public class GeneralCharacter : MonoBehaviour
    {

        public float health;
        public float aPowerk;
        public float attack;
        public float defense;
        public float agility;
        public float movementSpeed;
        public float attackSpeed;
        public float mana;
        public float abilityPower;
        public float physicalPenetration;
        public float magicalPenetration;
        public List<Ability> abilities;
        public List<Equippable> equipment;
        public int money;

        public Animation attackAnimation;
        public Animation walkAnimation;
        public Animation idleAnimation;
        public Animation deathAnimation;
        public Animation lifeAnimation;

        public Transform playerTransform;
        public Camera playerCamera;
        private Vector3 targetPosition;
        #region Other Functions


        public Vector3 GetTargetPosition()
        {
            return targetPosition;
        }


        public void SetTargetPosition(Vector3 newPosition)
        {
            targetPosition = newPosition;
        }

        public void Move(Ray ray)
        {
            Plane playerPlane = new Plane(Vector3.up, playerTransform.position);
            float hitdist = 0.0f;

            if (playerPlane.Raycast(ray, out hitdist))
            {

                var targetPoint = ray.GetPoint(hitdist);
                targetPosition = ray.GetPoint(hitdist);
                var targetRotation = Quaternion.LookRotation(targetPoint - playerTransform.position);
                playerTransform.rotation = targetRotation;
            }
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
}