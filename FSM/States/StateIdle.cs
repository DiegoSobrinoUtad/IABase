using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : StateBase
{
    public override void Enter()
    {
        nameState = "Idle";
        base.Enter();

    }

    public override void Execute()
    {
        base.Execute();

        if (unit.CheckVisionAI())
        {
            unit.playerInSight = true;
            fsm.ChangeState(fsm.chasingState);
          
        }
        
      
    }
   
}
