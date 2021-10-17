using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{

    [Header("HFSM")]

    StateChasing MoveCombatState;
    CombatStateBase currentStateCombat;

    public MeleAttackStateBase MeleAttackState;   // 3 Variaciones
    RangeAttackState RangeAttackState;  // X Variaciones 

    Hability1State Hability1State;
    Hability2State Hability2State;
    Hability3State Hability3State;
    Hability4State Hability4State;

    AngerAState AngerStateA;
    AngerBState AngerStateB;
    AngerCState AngerStateC;

    UltimateState UltimateState;

    [SerializeField] FSM fsm;


    [Header("Stats Combat")]

    [SerializeField] float[] arrayRangeMeleA;
    [SerializeField] float[] arrayRangeMeleB;

    float[] arrayRangeProyectilA;
    float[] arrayRangeProyectilB;

    //Ordenar la lista de mayor a menor. Coger el mayor.
    //Intrusión en FSM.Chasing
    
    //Consider if attack or not. 

  



    void Start()
    {
        fsm = GetComponent<FSM>();

        MoveCombatState = GetComponentInChildren<StateChasing>();

        MeleAttackState = GetComponentInChildren<MeleAttackStateBase>();
        RangeAttackState = GetComponentInChildren<RangeAttackState>();

        Hability1State = GetComponentInChildren<Hability1State>();
        Hability2State = GetComponentInChildren<Hability2State>();
        Hability3State = GetComponentInChildren<Hability3State>();
        Hability4State = GetComponentInChildren<Hability4State>();

        AngerStateA = GetComponentInChildren<AngerAState>();
        AngerStateB = GetComponentInChildren<AngerBState>();
        AngerStateC = GetComponentInChildren<AngerCState>();


        UltimateState = GetComponentInChildren<UltimateState>();

    }

    // Update is called once per frame
    void Update()
    {

        if (currentStateCombat != null)
        {
            currentStateCombat.Execute();
        }

    }

    public void ChangeState(CombatStateBase newState)
    {

        if (currentStateCombat != null)
        {
            currentStateCombat.Exit();

            currentStateCombat = newState;
            currentStateCombat.Enter();

        }

    }
    public void StartStateMachine(CombatStateBase inicialState)
    {
        
            currentStateCombat = inicialState;
            currentStateCombat.Enter();
        

    }

    public void FinishStateMachine()
    {

        currentStateCombat = null;
    }

    //Selector
    //Strategic
    //Objetives
    //Decision

}
