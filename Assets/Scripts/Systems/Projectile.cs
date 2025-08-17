using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public float fireRate;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

    private GameObject Player;
    [HideInInspector]
    public float attackDamage = 0f;

    private PlayerStatistics stats;

    void Start()
    {
        GameObject statManager = GameObject.FindGameObjectWithTag("Stat Manager");
        if (statManager != null)
        {
            stats = statManager.GetComponent<PlayerStatistics>();
        }

        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var psMuzzle = muzzleVFX.GetComponent<ParticleSystem>();
            if (psMuzzle != null)
            {
                Destroy(muzzleVFX, psMuzzle.main.duration);
            }
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }

        Player = GameObject.FindGameObjectWithTag("Player");
        attackDamage = stats.damage;

        if (stats != null)
        {
            if (Random.value <= stats.critChance)
            {
                attackDamage *= stats.critMultiplier;
                Debug.Log($"CRIT! {attackDamage} dmg");
            }
        }
    }

    void Update()
    {
        if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("No Speed");
        }
    }

    void OnCollisionEnter(Collision co)
    {
        speed = 0;

        ContactPoint contact = co.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot);
            hitVFX.transform.SetParent(co.transform);
            var psHit = hitVFX.GetComponent<ParticleSystem>();
            if (psHit != null)
            {
                Destroy(hitVFX, psHit.main.duration);
            }
            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
        }
        Destroy(gameObject);
    }
}