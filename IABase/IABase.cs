using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IABase : MonoBehaviour
{
  
   
    [Header("Data")]
    [SerializeField] protected string nameUnit;
    [SerializeField] protected string version;

    [Header("Essencial Components")]
    public _Player m_player;

    //Essencial Components
    public NavMeshAgent myNavMeshAgent;
    public Rigidbody rb;
    public Animator m_animator;
    [SerializeField] LayerMask enemyLayerMask;


    FSM stateMachine;
    CombatSystem combatSystem;
    public ConsoleDisplay console;

    [Header("Movement System")]
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] public float chasingSpeed;
    [SerializeField] public float combatSpeed;

    [SerializeField] public Vector3 playerPosition, lastPlayerPosition, lastUpdatePosition, lastTrackedPosition;
    bool positionUpdated, trackedPosition;
    float timerTracking;
    [SerializeField] float trackingDuration;
    

    [SerializeField] bool haveTurn = true;
    bool turning;

    [Header("Jump")]
    private bool endJump;                            // lets us know when the jump is suppose to end
    private Vector3 jumpStartPoint;                  // the start point for jumping
    private Vector3 jumpLandPoint;                   // the landing point for jumping
    private bool startJump;                          // let us know when to start jumping
    private float startTime;                         // used in jump calulation
    private float jumpTimerReset;                    // used to calulate the jump

    public float jumpSpeed = 2.0f;                   // how fast to move while jumping in the air
    [SerializeField] float maximumHeightJump;
    [SerializeField] float maximumDistanceJump;



    public bool launched;
    public bool jumpingUp;
    public bool isJumping;
    public bool isFalling;
   
    public bool landed { get; set; } = true;
    public bool canJump { get; set; } = true;

    public float jumpCooldown;

    [Header("Perception")]

    [SerializeField] float fieldOfView;
    [SerializeField] float lateralView;
    [SerializeField] float LoosePerceptionRatio;
    [SerializeField] public bool looseContact, registred;                       //registred is a bool to seperate 1st frame from the continuos flow without vision and register lastPlayerPosition

    float timerPerception;




    [Header("Stats")]

    public int enemyLife;
    public int currentLife;

    [SerializeField] public float rangeVision;
    [SerializeField] public bool playerInSight;
    [SerializeField] public bool playerDetected;
    public bool isWander;

    bool firstTimePerception = true;

    [Header("CombatSystem")]

    public bool damaged;
    public bool isAttacking;
    public BoxCollider weaponCollider;


    protected virtual void Start()
    {
        
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        m_animator = GetComponentInChildren<Animator>();
        stateMachine = GetComponent<FSM>();
        combatSystem = GetComponent<CombatSystem>();

        console = FindObjectOfType<ConsoleDisplay>();
        m_player = FindObjectOfType<_Player>();

        //stateMachine.StartStateMachine(stateMachine.idleState);

    }
    

    protected virtual void Update()
    {
        //Movement

        //if (isJumping)
        //{
        //    TrackUpdatePosition();
        //    TrackPositionDuringTime(0.1f);
        //}



        // Get world current state
        GetWorldInfo();


        // Combat System

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.tag == "Proyectile")
        {
            playerDetected = true;   
            damaged = true;
            StartCoroutine(RecieveDamageCoroutine());
        }
    }
    


    IEnumerator RecieveDamageCoroutine()
    {


        yield return new WaitForSeconds(0.5f);
        damaged = false;
    }
    //Jump
    public void Jump()
    {
        myNavMeshAgent.enabled = false;
        rb.isKinematic = false;
        m_animator.SetBool("Jump", true);

        Vector3 directionToPlayer = CheckDirectionTarget(transform.position, m_player.transform.position);

        directionToPlayer.Normalize();
        rb.AddForce(new Vector3(directionToPlayer.x * 15000, directionToPlayer.y * 27000, directionToPlayer.z * 15000), ForceMode.Impulse);
    

    }


    public IEnumerator StartJump()
    {
       
        yield return new WaitForSeconds(0.5f);
        isJumping = true;

    }
    public IEnumerator JumpCooldown()
    {



        yield return new WaitForSeconds(5f);
        canJump = true;
    }

    public void Jump1()
    {
        jumpLandPoint = m_player.transform.position;
        jumpStartPoint = transform.position;
        // The center of the arc
        Vector3 center = (jumpStartPoint + jumpLandPoint) * 0.5F;
        // move the center a bit downwards to make the arc vertical
        center -= new Vector3(0, 1, 0);
        // Interpolate over the arc relative to center
        Vector3 riseRelCenter = jumpStartPoint - center;
        Vector3 setRelCenter = jumpLandPoint - center;
        // The fraction of the animation that has happened so far is
        // equal to the elapsed time divided by the desired time for
        // the total journey.
        float fracComplete = (jumpTimerReset - startTime) / jumpSpeed;
        jumpTimerReset += Time.deltaTime;
        transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        transform.position += center;
        float dist;
        dist = Vector3.Distance(transform.position, jumpLandPoint);
        if (dist < 0.5f)
        {
            stateMachine.ChangeState(stateMachine.chasingState);
            jumpStartPoint = jumpLandPoint;
            endJump = true;
           //StopJump
            jumpTimerReset = 0;
        }
    }

   
    public bool CheckEnemyJump()
    {
        bool valid;


       

        if (CheckDistanceToPlayer()>=maximumDistanceJump)
        {
            valid = false;
            return valid;
        }

        float yJump = transform.position.y + maximumHeightJump;
        playerPosition = m_player.transform.position;

        Vector3 positionEnemyJump = new Vector3(playerPosition.x, yJump-playerPosition.y, playerPosition.y);
        

        Vector3 randomPoint = positionEnemyJump + Random.insideUnitSphere * 2f;
        NavMeshHit hit;
       

        if (NavMesh.SamplePosition(positionEnemyJump, out hit, 5f, NavMesh.AllAreas))
        {
            Debug.DrawRay(hit.position, Vector3.down, Color.green);
            valid = true;
        }
        else
        {
            Debug.DrawRay(hit.position, Vector3.down, Color.red);
            valid = false;
        }

        return valid;
    }

   
    public void TrackPositionDuringTime(float durationTime)
    {
        bool finishedTracking = false;


        if (!trackedPosition)
        {
            finishedTracking = false;
            lastTrackedPosition = transform.position;
            trackedPosition = true;
        }
        else
        {
            timerTracking += Time.deltaTime;
            if (timerTracking > durationTime)
            {

             
                timerTracking = 0;
                trackedPosition = false;
                finishedTracking = true;
            }
        }



        if (finishedTracking==true && lastTrackedPosition.y<transform.position.y)
        {
            jumpingUp = true;
        }
        else
        {
            jumpingUp = false;
        }



      
    }
    private void TrackUpdatePosition()
    {
        if (positionUpdated == false)
        {
            lastUpdatePosition = transform.position;
            positionUpdated = true;
        }
        else
        {
            positionUpdated = false;
        }
    }

 

    public float CheckDistanceToPlayer()
    {
        float distance = Vector3.Distance(transform.position, m_player.transform.position);


        return distance;
    }
    public float CheckDistance(Vector3 point1, Vector3 point2)
    {
        float distance = Vector3.Distance(point1, point2);

        return distance;
    }

    public bool CheckGroundDetection(float rangeGroundDetection)
    {
        bool isGrounded;

        RaycastHit hit;


       //Cuando esté cayendo!!!!! A la mitad del salto
        if (Physics.Raycast(m_animator.GetBoneTransform(HumanBodyBones.LeftFoot).position, -transform.up, out hit, rangeGroundDetection))
        {
            Debug.DrawRay(m_animator.GetBoneTransform(HumanBodyBones.LeftFoot).position, -transform.up, Color.red);
           

            if (hit.transform.gameObject.tag == "Ground")
            {
                isGrounded = true;
                //rb.isKinematic = true;
            }
            else
            {
                isGrounded = false;
            }
        }
        else
        {
            isGrounded = false;
        }



        return isGrounded;
    }
    protected Vector3 CheckDirectionTarget(Vector3 position1, Vector3 position2)
    {
        Vector3 direction = position2 - position1;
        //Return direction between A-B
        return direction;
    }
    //Perception
    public virtual bool CheckVisionAI()        //Return Yes/No 
    {

        Vector3 directionToPlayer;

        //FrontView
        if (CheckDistanceToPlayer() < rangeVision)
        {
            float angle;
            angle = Vector3.Angle(m_animator.GetBoneTransform(HumanBodyBones.Head).forward, CheckDirectionTarget(m_animator.GetBoneTransform(HumanBodyBones.Head).position, m_player.transform.position));
            Debug.DrawRay(m_animator.GetBoneTransform(HumanBodyBones.Head).position, m_animator.GetBoneTransform(HumanBodyBones.Head).forward, Color.yellow);
            
            if (angle < fieldOfView)                                                                     //FrontView
            {
                RaycastHit hit;

                if (Physics.Raycast(m_animator.GetBoneTransform(HumanBodyBones.Head).position, CheckDirectionTarget(m_animator.GetBoneTransform(HumanBodyBones.Head).position, m_player.transform.position), out hit, rangeVision))
                {
                   
                    if (hit.transform.gameObject.GetComponent<_Player>())
                    {
                        Debug.DrawRay(m_animator.GetBoneTransform(HumanBodyBones.Head).position, CheckDirectionTarget(m_animator.GetBoneTransform(HumanBodyBones.Head).position, m_player.transform.position), Color.green);

                        m_animator.GetComponent<AimHeadEnemy>().enabled = true;
                        playerInSight = true;
                        firstTimePerception = false;
                        timerPerception = 0;
                        looseContact = false;
                        registred = false;

                        playerInSight = true;
                        return playerInSight;
                    }
                    else
                    {

                        if (!registred)
                        {
                            lastPlayerPosition = m_player.transform.position;
                            registred = true;
                        }
                        looseContact = true;
                        if (!isWander)
                        {
                             timerPerception += LoosePerceptionRatio * Time.deltaTime;
                        }
                       

                        Debug.DrawRay(m_animator.GetBoneTransform(HumanBodyBones.Head).position, CheckDirectionTarget(m_animator.GetBoneTransform(HumanBodyBones.Head).position, m_player.transform.position), Color.red);

                    }


                }
            }           //LateralVision
            else if (angle <fieldOfView+lateralView&&CheckDistanceToPlayer()<rangeVision/2)                 //LateralView
            {
                m_animator.GetComponent<AimHeadEnemy>().enabled = false;
                RaycastHit hit;

                if (Physics.Raycast(m_animator.GetBoneTransform(HumanBodyBones.Head).position, CheckDirectionTarget(m_animator.GetBoneTransform(HumanBodyBones.Head).position, m_player.transform.position), out hit, rangeVision))
                {
                    

                    if (hit.transform.gameObject.GetComponent<_Player>())
                    {
                        m_animator.GetComponent<AimHeadEnemy>().enabled = false;
                        Debug.DrawRay(m_animator.GetBoneTransform(HumanBodyBones.Head).position, CheckDirectionTarget(m_animator.GetBoneTransform(HumanBodyBones.Head).position, m_player.transform.position), Color.blue);
                        
                        //Fx: Turn to Player
                        TurnMethod();
                        Debug.Log("Turn");
                    }
                    else
                    {

                        m_animator.GetComponent<AimHeadEnemy>().enabled = false;
                        if (!registred)
                        {
                            lastPlayerPosition = m_player.transform.position;
                            registred = true;
                        }
                        looseContact = true;
                        if (!isWander)
                        {
                            timerPerception += LoosePerceptionRatio * Time.deltaTime;
                        }

                        Debug.DrawRay(m_animator.GetBoneTransform(HumanBodyBones.Head).position, CheckDirectionTarget(m_animator.GetBoneTransform(HumanBodyBones.Head).position, m_player.transform.position), Color.red);

                    }
                }

               

            }
            else
            {
                m_animator.GetComponent<AimHeadEnemy>().enabled = false;
                if (!playerDetected)
                {
              
                    Debug.Log("No player in Sight");
                    playerInSight = false;
                }
                
            }

            //Condición de perder al objetivo
                
            if (looseContact && !firstTimePerception)
            {
                LooseVisionContact();
            }
        }


        if (timerPerception>5 && !isWander)
        {
            timerPerception = 0;
            playerDetected = false;
            playerInSight = false;
        }
        


        return playerInSight;

    }

    public virtual void LooseVisionContact()
    {
       
        Debug.Log("Last Player Location known by IA: " + lastPlayerPosition);

    }

    public virtual void TurnMethod()
    {



        //Gestionar el Head tracking
    





        if (haveTurn)
        {
           



        }


    }

    // Movements
    public virtual void UpdateXZSpeed()
    {

        Vector3 localVelocity = transform.InverseTransformDirection(myNavMeshAgent.velocity);

        m_animator.SetFloat("XSpeed", localVelocity.x);
        m_animator.SetFloat("ZSpeed", localVelocity.z);
    }

    //Track all info
    public void GetWorldInfo()
    {



    }

    // Actions

    //Movement Actions
    public void GoToPlayer()
    {
        myNavMeshAgent.SetDestination(m_player.transform.position);
        UpdateXZSpeed();
    }
    public void GoToPlayerInvalidNavmesh()
    {
        Vector3 directionToPlayer = CheckDirectionTarget(transform.position, m_player.transform.position);
        directionToPlayer.y = 0;
        directionToPlayer.Normalize();
        Vector3 newEnemyPosition = transform.position + directionToPlayer*4;

       
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newEnemyPosition, out hit, 5f, NavMesh.AllAreas))
        {
            GoToPosition(hit.position);
            Debug.DrawRay(hit.position, Vector3.down, Color.green, 5f);
            //Debug.DrawRay(new Vector3(newEnemyPosition.x, newEnemyPosition.y + 2, newEnemyPosition.z), Vector3.down, Color.green);
        }
        else
        {
            Vector3 randomPoint = newEnemyPosition + Random.insideUnitSphere * 2f;
            if (NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas))
            {
                GoToPosition(hit.position);
                Debug.DrawRay(hit.position, Vector3.down, Color.green, 5f);
               
            }
            else
            {
                Debug.DrawRay(hit.position, Vector3.down, Color.red, 5f);
            }
      
        }

        //NavMeshHit validRaycastPosition;
        //if (NavMesh.Raycast(new Vector3(newEnemyPosition.x, newEnemyPosition.y + 2, newEnemyPosition.z), Vector3.down, out validRaycastPosition, NavMesh.AllAreas))
        //{
        //}
        //else
        //{
        //    Debug.DrawRay(new Vector3(newEnemyPosition.x, newEnemyPosition.y + 2, newEnemyPosition.z), Vector3.down, Color.red);
        //}




    }
    public void GoToPosition(Vector3 target)
    {

        myNavMeshAgent.SetDestination(target);
        UpdateXZSpeed();
    }

    public void GoToDestination(Vector3 target)
    {
        myNavMeshAgent.SetDestination(target);
        UpdateXZSpeed();
    }
    public void RunToPlayer()
    {


    }

    public void RotateToPlayer()
    {

        //-180*
    }
    public void Turn()
    {
        //+180º

    }



    //Habilidades
    public void PassiveAbility()
    {


    }
    public void NaturalAbility()
    {
        //active ability: Charge()


    }

    public void Habilidad1()
    {


    }
    public void Habilidad2()
    {

    }
    public void Habilidad3()
    {


    }

    public void Habilidad4()
    {


    }


    public void HabilidadAnger()
    {

        //Tipo A, B, C


    }


    public void Protect()
    {



    }
    public void MeleAttack()
    {



    }
    public void RangeAttack()
    {


    }

    public void HItDamage()
    {


    }

    public void Death()
    {


    }
}
