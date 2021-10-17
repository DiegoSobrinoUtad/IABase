using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStateBase : StateBase
{
    


    protected override void Start()
    {
        base.Start();
        
    

    }
    public override void Enter()
    {

        nameState = "Combat System: " + nameState ;
        base.Enter();


    }
    public override void Execute()
    {

        Debug.Log("State " + nameState + " execution");
        base.Execute();


    }


}
