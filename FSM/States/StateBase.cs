using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase : MonoBehaviour
{
    public IABase unit;
    public FSM fsm;
    protected CombatSystem combatSystem;
    [SerializeField]protected Animator m_animator;

    protected string nameState = "Inicio";
    protected ConsoleDisplay console;

    protected virtual void Start()
    {
        unit = GetComponentInParent<IABase>();
        fsm = GetComponentInParent<FSM>();
        combatSystem = GetComponentInParent<CombatSystem>();
        console = FindObjectOfType<ConsoleDisplay>();
        m_animator = unit.GetComponentInChildren<Animator>();
        
    }
    public virtual void Enter()
    {
        //ConsoleDisplay
        if (console.ConsoleActivated)
        {
            console.InserteConsole("State: " + nameState);
        }
       
        Debug.Log("State " + nameState + " has begin");

    }
    public virtual void Execute()
    {

        Debug.Log("State " + nameState + " execution");
    }

    public virtual void FixedExecute()
    {



    }
    public virtual void Exit()
    {


        Debug.Log("State " + nameState + " exit");

    }
}
