﻿using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] private float maxSpeed = 6f;

        private NavMeshAgent navMeshAgent;
        private Animator animator;     

        // Start is called before the first frame update
        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            navMeshAgent.enabled = !GetComponent<Health>().IsDead();
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            speed = Math.Abs(speed);
            animator.SetFloat("forwardSpeed", speed);
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            //GetComponent<Fighter>().Cancel();
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.SetDestination(destination);
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        

    }
}