using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    IABase IAbase;

    public StateIdle idleState;
    public StateWander wanderState;
    public StateChasing chasingState;
    public StateJump jumpingState;
    public StateMoveCombat moveCombatState;


    StateBase currentState;
  


    ConsoleDisplay console;

    void Start()
    {
        IAbase = GetComponent<IABase>();

        idleState = GetComponentInChildren<StateIdle>();
        wanderState = GetComponentInChildren<StateWander>();
        chasingState = GetComponentInChildren<StateChasing>();
        jumpingState = GetComponentInChildren<StateJump>();
        moveCombatState = GetComponentInChildren<StateMoveCombat>();

        console = FindObjectOfType<ConsoleDisplay>();


        // Start Machine Inicio
        currentState = idleState;
        StartStateMachine(idleState);
     
       
   
    }


    void Update()
    {
       
        if (currentState != null)
        {
            currentState.Execute();
        }
        
    }
  
    public void ChangeState(StateBase newState)
    {

        if (currentState != null)
        {
            currentState.Exit();

            currentState = newState;
            currentState.Enter();
            
        }

    }
    public void ExitCurrentState(StateBase exitState) 
    {
       
        
    
    } 
    public void StartStateMachine(StateBase inicialState)
    {
            
                currentState = inicialState;
                currentState.Enter();
            

    }
    public void FinishStateMachine()
    {

        currentState = null;
    }
}
