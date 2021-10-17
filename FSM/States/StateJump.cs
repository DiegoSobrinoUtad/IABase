using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateJump : StateBase
{
    float timerJump;
    public override void Enter()
    {
        nameState = "Jump";
        unit.myNavMeshAgent.enabled = false;
        unit.rb.isKinematic = false;
        unit.Jump();
        StartCoroutine(unit.StartJump());
        base.Enter();

    }

    public override void Execute()
    {

        base.Execute();
        if (unit.isJumping && unit.CheckGroundDetection(0.5f))
        {
            fsm.ChangeState(fsm.chasingState);
        }

      


    }
    public override void Exit()
    {
      
        unit.myNavMeshAgent.enabled = true;
        unit.rb.isKinematic = true;
        unit.isJumping = false;
        m_animator.SetBool("Jump", false);
        StartCoroutine(unit.JumpCooldown());
   


    }

    void Jump()
    {
        if (unit.myNavMeshAgent.isOnOffMeshLink)
        {


            //JUMP
            if (unit.isJumping == false && unit.landed && unit.canJump)
            {

                unit.canJump = false;
                unit.isJumping = true;
                unit.landed = false;

                unit.m_animator.SetBool("Jump", true);

            }


        }


        Debug.Log("lastposition " + unit.lastUpdatePosition.y);
        Debug.Log("position " + unit.transform.position.y);

        if (unit.lastTrackedPosition.y < unit.transform.position.y && unit.isJumping && !unit.landed && !unit.canJump && unit.jumpingUp)
        {
            unit.launched = true;
        }
        //Caida Jump
        if (unit.isJumping == true && unit.lastUpdatePosition.y > unit.transform.position.y && unit.landed == false && unit.canJump == false)
        {
            Debug.Log("Falling");
            unit.isFalling = true;

        }

        //Land
        if (unit.isJumping == true && unit.isFalling)
        {
            if (unit.CheckGroundDetection(1.5f))
            {
                unit.isFalling = false;


                Debug.Log("Enemy is falling");
            }


        }
        else if (unit.isJumping && !unit.isFalling && unit.launched == true)
        {
            if (unit.CheckGroundDetection(0.75f))
            {

                //Landing

                unit.launched = false;
                unit.landed = true;
                unit.isJumping = false;
                unit.m_animator.SetBool("Jump", false);
            }
        }

        if (!unit.isJumping && unit.landed == true && !unit.canJump)
        {
            timerJump += Time.deltaTime;
        }

        if (timerJump > unit.jumpCooldown)
        {
            timerJump = 0;
            unit.canJump = true;
        }

        //if (unit.myNavMeshAgent.isOnNavMesh)
        //{
        //    unit.m_animator.SetBool("Jump", false);
        //}
    }
}
