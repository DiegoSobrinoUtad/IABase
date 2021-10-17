using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateChasing : StateBase
{


    public override void Enter()
    {
        nameState = "Chasing";

        m_animator.SetBool("Combat", false);
        m_animator.SetBool("Moving", true);
        unit.playerDetected = true;
        unit.looseContact = false;
        unit.myNavMeshAgent.speed = unit.chasingSpeed;
        base.Enter();

    }
    public override void Execute()
    {
        base.Execute();
        if (unit.CheckDistanceToPlayer()<=7)
        {
            fsm.ChangeState(fsm.moveCombatState);
        }
        if (!unit.CheckVisionAI())
        {
            if (unit.playerDetected)
            {
                unit.GoToPlayer();
            }
            else
            {
                fsm.ChangeState(fsm.wanderState);
            }
           
          
        }
        
      
       
        if (unit.looseContact)
        {
            
            unit.GoToDestination(unit.lastPlayerPosition);
            if (unit.playerDetected && unit.isWander)
            {
                unit.GoToPlayer();
                unit.isWander = false;
            }
        }
        else if (unit.playerDetected)
        {
            if (unit.GetComponent<NavMeshAgent>().enabled == true)
            {
                NavMeshPath path = new NavMeshPath();
                unit.myNavMeshAgent.CalculatePath(unit.m_player.transform.position, path);

                if (path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
                {
                    if (unit.CheckEnemyJump())
                    {
                        if (unit.canJump)
                        {
                            //Jump
                            
                            unit.canJump = false;
                            fsm.ChangeState(fsm.jumpingState);

                        }


                    }
                    else
                    {
                        unit.GoToPlayerInvalidNavmesh();

                    }

                }
                else
                {
                    unit.GoToPlayer();


                    //Jump Animation
                    //Jump();
                }
            }
        }
        
        
      




    

    }

   
}
