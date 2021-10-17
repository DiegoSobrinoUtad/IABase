using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateWander : StateBase
{
    Vector3 lastPlayerPosition;
    
    protected override void Start() 
    {
        nameState = "Wander";
       
        base.Start();
       
      

    }
  
    public override void Enter()
    {
        Debug.Log("Player lost: Wander State");

        m_animator.GetComponent<AimHeadEnemy>().enabled = false;
        base.Enter();
        unit.isWander = true;
        

    }
    
    public override void Execute()
    {
        base.Execute();

        if (unit.CheckDistance(transform.position, lastPlayerPosition) < 0.5)
        {
            Debug.Log("Enemy arrive last player position known");
            //m_animator.SetBool("Idle", true);
        }

        if (unit.CheckVisionAI())
        {
            fsm.ChangeState(fsm.chasingState);
        }
        if (unit.damaged)
        {

            fsm.ChangeState(fsm.chasingState);
        }
        
    }
}
