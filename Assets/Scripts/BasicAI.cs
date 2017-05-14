using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson {

    public class BasicAI : MonoBehaviour {

        public NavMeshAgent agent;
        public ThirdPersonCharacter character;

        public enum State {
            PATROL,
            CHASE
        }

        public State state;
        private bool alive;

        // Variable for patrolling
        public GameObject[] wayPoints;
        private int wayPointIndex = 0;
        public float patrolSpeed = 0.5f;

        // Variable for Chasing
        public float chaseSpeed = 1f;
        public GameObject target;


        // Use this for initialization
        void Start() {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

            agent.updatePosition = true;
            agent.updateRotation = false;

            state = State.PATROL;
            alive = true;

            StartCoroutine("FSM");
            // FSM? final state machine
        }

        IEnumerator FSM() {

            while (alive) {
       
                switch (state) {
                    case State.PATROL:
                        Patrol();
                        break;
                    case State.CHASE:
                        Chase();
                        break;
                }
                yield return null;
            }

        }

        void Patrol() {
            agent.speed = patrolSpeed;
            if (Vector3.Distance(this.transform.position, wayPoints[wayPointIndex].transform.position) >= 2) {
                agent.SetDestination(wayPoints[wayPointIndex].transform.position);
                character.Move(agent.desiredVelocity, false, false);
            } else if (Vector3.Distance(this.transform.position, wayPoints[wayPointIndex].transform.position) < 2) {
                if (wayPointIndex > wayPoints.Length) {
                    wayPointIndex = 0;
                }
                wayPointIndex += 1;
                
            } else {
                // ideal animation
                character.Move(Vector3.zero, false, false);
            }
        }

        void Chase() {
            agent.speed = chaseSpeed;
            agent.SetDestination(target.transform.position);
            character.Move(agent.desiredVelocity, false, false);
        }

        private void OnTriggerEnter(Collider other) {
            Debug.Log(other.tag);
            if (other.gameObject.CompareTag("Player")) {
               
                state = State.CHASE;
                target = other.gameObject;
            }
        }


    }
}