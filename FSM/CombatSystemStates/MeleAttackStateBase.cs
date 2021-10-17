using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleAttackStateBase : CombatStateBase
{
    public override void Enter()
    {
        m_animator.SetTrigger("M1");
        nameState = "MeleAttack";
        unit.isAttacking = true;
        base.Enter();

      


    }
    public override void Execute()
    {

        Debug.Log("State " + nameState + " execution");
        base.Execute();

        if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("MoveCombat") && unit.isAttacking ==false)
        {
            fsm.StartStateMachine(fsm.moveCombatState);
            combatSystem.FinishStateMachine();

        }


    }
    public override void Exit()
    {

        Debug.Log("State " + nameState + " exit");
        base.Exit();

     
  

    }


}
