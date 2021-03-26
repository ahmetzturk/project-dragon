using GameDevTV.Utils;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 5f;
        [SerializeField] private float aggroCooldownTime = 5f;
        [SerializeField] private PatrolPath patrolPath = null;
        [SerializeField] private float waypointTolerance = 1f;
        [SerializeField] private float waypointDwellTime = 5f;
        [Range(0,1)]
        [SerializeField] private float patrolSpeedFraction = 0.2f;
        [SerializeField] private float shoutDistance = 5f;

        private GameObject player;
        private Fighter fighter;
        private Mover mover;
        private Health health;
        private Vector3 guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = 0;
        private float timeSinceAggravated = Mathf.Infinity;
        private int currentWaypointIndex = 0;

        private void Awake()
        {          
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
        }

        private void Start()
        {
            guardPosition = transform.position;
        }

        private void Update()
        {
            if (health.IsDead()) return;
            if (IsAggravated() && fighter.CanAttack(player.gameObject))
            {
                timeSinceLastSawPlayer = 0;
                AttackBehaviour();             
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspiciousBehaviour();
            }
            else
            {              
                PatrolBehaviour();
            }
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        public void Aggravate()
        {
            timeSinceAggravated = 0;          
        }

        private void AttackBehaviour()
        {
            fighter.Attack(player);
            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies()
        {
            /*
            AIController[] AIControllers = FindObjectsOfType<AIController>();
            foreach (var aiController in AIControllers)
            {
                if (Vector3.Distance(gameObject.transform.position, aiController.transform.position) < chaseDistance)
                {
                    aiController.timeSinceAggravated = 0;
                }
            }
            */

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (var hit in hits)
            {
                AIController aiController = hit.collider.GetComponent<AIController>();
                if(aiController != null)
                {
                    aiController.Aggravate();
                }
            }
        }

        private void SuspiciousBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition;
            if (patrolPath != null)
            {
                nextPosition = GetNextWaypoint();
            }
            mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            if (AtWayPoint())
            {
                if (timeSinceArrivedAtWaypoint > waypointDwellTime)
                {
                    currentWaypointIndex++;
                    timeSinceArrivedAtWaypoint = 0;
                }
                timeSinceArrivedAtWaypoint += Time.deltaTime;
            }
        }

        private bool AtWayPoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetNextWaypoint());
            return distanceToWaypoint < waypointTolerance;    
        }
        private Vector3 GetNextWaypoint()
        {
            if (patrolPath == null) return Vector3.positiveInfinity;
            return patrolPath.GetWaypoint(currentWaypointIndex % patrolPath.transform.childCount);
        }
        private bool IsAggravated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance || timeSinceAggravated < aggroCooldownTime;                   
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
