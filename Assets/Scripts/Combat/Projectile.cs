
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health target;
        [SerializeField] float speed = 5f;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect;
        [SerializeField] int maxLifetime;
        [SerializeField] GameObject[] destroyOnHit;
        [SerializeField] int lifeAfterImpact = 2;
        [SerializeField] UnityEvent onHit;
        float damage = 0;
        GameObject instigator;

        private void Start()
        {
           // GetComponent<AudioSource>().Play();
            transform.LookAt(target.GetComponent<CapsuleCollider>().bounds.center, transform.up);
        }
        
        private void Update()
        {
            if (isHoming && !target.isDead())
            transform.LookAt(target.GetComponent<CapsuleCollider>().bounds.center, transform.up);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            Destroy(gameObject, maxLifetime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.isDead()) return;
            target.TakeDamage(instigator, damage);
            onHit.Invoke();
            if (hitEffect)
            {
                GameObject hit = Instantiate(hitEffect, transform.position, transform.rotation);
                //Destroy(hit);
            }
            //foreach(GameObject obj in destroyOnHit)
            //{
            //    Destroy(obj);
            //}
            Destroy(gameObject);
           
        }
    }

}