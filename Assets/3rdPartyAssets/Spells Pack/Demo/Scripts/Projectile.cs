﻿using UnityEngine;

namespace ZakhanSpellsPack
{
    public class Projectile : MonoBehaviour
    {
        public GameObject ExplosionPrefab;
        public float DestroyExplosion = 4.0f;
        public float DestroyChildren = 2.0f;
        public Vector2 Velocity;

        Rigidbody rb;
        void Start()
        {
            rb = gameObject.GetComponent<Rigidbody>();
            rb.linearVelocity = Velocity;

        }

        void OnCollisionEnter(Collision col)
        {
            var exp = Instantiate(ExplosionPrefab, transform.position, ExplosionPrefab.transform.rotation);
            Destroy(exp, DestroyExplosion);
            Transform child;
            child = transform.GetChild(0);
            transform.DetachChildren();
            Destroy(child.gameObject, DestroyChildren);
            Destroy(gameObject);
        }
    }
}
