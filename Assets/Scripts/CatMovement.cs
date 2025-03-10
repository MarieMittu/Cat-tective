using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    public Animator playerAnim;
    public Rigidbody playerRigid;
    public float walk_speed, walkback_speed, oldwalk_speed, run_speed, rotate_speed;
    public bool walking;
    public Vector3 jump;
    public Vector3 jumpOff;
    public Vector3 jumpBack;
    public Vector3 jumpForward;
    public float jumpForce = 2.0f;
    public bool isGrounded;
    public bool isOnWall;
    private bool isVaulting = false;
    private bool isClimbing = false;
    private bool isSunDrunk = false;
    private bool isPlaying = false;
    private bool isScared = false;
    private bool readyToInvestigate = false;
    private bool isInvestigating = false;
    private GameObject currentInvestigationArea;

    public Transform playerTrans;
    public Transform cameraTrans;
    public Transform cameraPivot;

    private int vaultLayer;
    public Camera catMainCam;
    private float catHeight = 2.5f;
    private float catRadius = 0.5f;

    public float mouseSensitivity = 2.0f;
    public float verticalRotationLimit = 80f;
    private float verticalRotation = 0f;
    public float cameraResetSpeed = 5f;

    private DateTime lastKeyPressTime;
    private TimeSpan timeBetweenKeyPresses;
    private bool chainStarted = false;
    public float soberUpTime = 5f;
    private float initialSoberTime;
    private float timeToRecover = 0.5f;
    private float timeSinceLastPress = 0f;
    private float keyPressThreshold = 0.2f;
    public float beSeriousTime = 5f;
    private float initialBeSeriousTime;

    private Vector3 gizmoTargetPos;
    private bool showGizmo = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerRigid = GetComponent<Rigidbody>();
        vaultLayer = LayerMask.NameToLayer("VaultLayer");
        vaultLayer = ~vaultLayer;
        jump = Vector3.up;
        jumpOff = Vector3.down;
        jumpBack = Vector3.back;
        jumpForward = Vector3.forward;
        initialSoberTime = soberUpTime;
        initialBeSeriousTime = beSeriousTime;
    }

    void FixedUpdate()
    {
        if (isVaulting) return;
        if (isScared)
        {
            playerRigid.velocity = new Vector3(0, playerRigid.velocity.y, 0); // Prevent moving forward but allow gravity
            return; // Skip movement logic
        }

        Vector3 velocity = playerRigid.velocity; // Preserve current velocity

        if (isOnWall)
        {
            playerRigid.useGravity = false;

            if (Input.GetKey(KeyCode.W))
            {
                velocity = new Vector3(velocity.x, 0, velocity.z); 
                playerRigid.velocity = velocity;

                playerRigid.AddForce(transform.up * walk_speed, ForceMode.Force);
                isClimbing = true; // Set climbing flag to true
                TutorialManager.sharedInstance.startClimbing = true;
            }
            else
            {
                if (isClimbing)
                {
                    velocity = new Vector3(velocity.x, 0, velocity.z); 
                    playerRigid.velocity = velocity; 
                }
                isClimbing = false; 
            }
        }
        else
        {
            if (isGrounded)
            {
                if (!isSunDrunk && !isPlaying && !isScared && !isInvestigating)
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        velocity.x = transform.forward.x * walk_speed * Time.deltaTime;
                        velocity.z = transform.forward.z * walk_speed * Time.deltaTime;
                    }
                    if (Input.GetKey(KeyCode.S))
                    {
                        velocity.x = -transform.forward.x * walkback_speed * Time.deltaTime;
                        velocity.z = -transform.forward.z * walkback_speed * Time.deltaTime;
                    }
                }
                
            }

            playerRigid.velocity = velocity;
            playerRigid.useGravity = true; 
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded)
        {
            if (collision.gameObject.CompareTag("Climb"))
            {
               
                isOnWall = true;
                isGrounded = false; 
                Debug.Log("Start climb");
                
                   
            }
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            
            
            isGrounded = true;
            playerRigid.useGravity = true;

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            playerRigid.useGravity = true;
        }

        if (!isGrounded)
        {
            if (collision.gameObject.CompareTag("Climb"))
            {
                isOnWall = true;
                isGrounded = false;
                //playerRigid.useGravity = false;
                Debug.Log("Is climbing");
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Climb"))
        {
            isOnWall = false;
            isGrounded = false; 
            playerRigid.useGravity = true; 
            Debug.Log("Stop climb");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Top"))
        {
            Vault();
        }

        if (other.gameObject.CompareTag("Sun"))
        {
            isSunDrunk = true;
            TutorialManager.sharedInstance.startChilling = true;
            //trigger chill anim
            playerAnim.SetTrigger("SitDown");
            playerAnim.SetTrigger("Sitting");
        }

        if (other.gameObject.CompareTag("Play"))
        {
            isPlaying = true;
            //other.gameObject.SetActive(false);
            TutorialManager.sharedInstance.startPlaying = true;
            //trigger play anim
            playerAnim.SetTrigger("SitDown");
            playerAnim.SetTrigger("Sitting");
        }

        if (other.gameObject.CompareTag("Snake"))
        {
            isScared = true;
            TutorialManager.sharedInstance.isScared = true;

            Vector3 direction = (transform.position - other.transform.position).normalized;
            playerRigid.AddForce(direction * 15.0f, ForceMode.Impulse);
        }

        if (other.gameObject.CompareTag("Search"))
        {
            readyToInvestigate = true;
            currentInvestigationArea = other.gameObject;
        }

        if (other.gameObject.CompareTag("Closet"))
        {
            TutorialManager.sharedInstance.foundWatch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Sun"))
        {
            isSunDrunk = false;
            TutorialManager.sharedInstance.startChilling = false;
            soberUpTime = initialSoberTime;
        }
       
        if (other.gameObject.CompareTag("Snake"))
        {
            isScared = false;
            TutorialManager.sharedInstance.isScared = false;

        }

        if (other.gameObject.CompareTag("Search"))
        {
            readyToInvestigate = false;
            currentInvestigationArea = null; 
        }
    }

    private IEnumerator LerpVault(Vector3 targetPos, float duration)
    {
        float time = 0;
        Vector3 startPos = transform.position;
        playerRigid.isKinematic = true;
        isVaulting = true;

        Collider playerCollider = GetComponent<Collider>();
        Collider[] allColliders = FindObjectsOfType<Collider>();
        foreach (var collider in allColliders)
        {
            if (collider != playerCollider)
            {
                Physics.IgnoreCollision(playerCollider, collider, true);  // Ignore collisions with all other colliders
            }
        }

        gizmoTargetPos = targetPos;
        showGizmo = true;
        Debug.Log("START LERP");
        while (time < duration)
        {
            float lerpRatio = time / duration;
            Vector3 lerpedPosition = Vector3.Lerp(startPos, targetPos, lerpRatio);

            // Use Rigidbody to move the player while lerping
            playerRigid.MovePosition(lerpedPosition);
            time += Time.deltaTime;
            yield return null;
        }
        Debug.Log("FINISH LERP");
        playerRigid.MovePosition(targetPos);
        foreach (var collider in allColliders)
        {
            if (collider != playerCollider)
            {
                Physics.IgnoreCollision(playerCollider, collider, false);
            }
        }

        Invoke("UndoKinematic", 0.2f);
        //showGizmo = false;
    }

    private void UndoKinematic()
    {
        playerRigid.isKinematic = false;
        isVaulting = false;
        showGizmo = false;
    }

    private void Vault()
    {
        if (Physics.Raycast(catMainCam.transform.position, catMainCam.transform.forward, out var firstHit, 1f, vaultLayer))
        {
            if (Physics.Raycast(firstHit.point + (catMainCam.transform.forward * catRadius * 0.5f) + (Vector3.up * 0.1f * catHeight), Vector3.down, out var secondHit, catHeight))
            {
                isVaulting = true;
                StartCoroutine(LerpVault(secondHit.point, 0.5f));
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(gizmoTargetPos, 0.02f); // Draw a small sphere at the target position
            Gizmos.DrawLine(transform.position, gizmoTargetPos); // Draw a line from the current position to target
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (isVaulting) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (walking)
        {
            playerTrans.Rotate(0, mouseX, 0);
            Quaternion targetRotation = Quaternion.LookRotation(playerTrans.forward);
            cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, targetRotation, Time.deltaTime * cameraResetSpeed);
        }
        else
        {
            cameraPivot.Rotate(0, mouseX, 0, Space.World);
        }

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);
        cameraTrans.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        if (!isSunDrunk && !isPlaying && !isInvestigating)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isGrounded)
                {
                    playerRigid.velocity = new Vector3(playerRigid.velocity.x, 0, playerRigid.velocity.z);
                    playerRigid.AddForce(jump * jumpForce, ForceMode.Impulse);
                    isGrounded = false;
                }
                else if (isOnWall)
                {
                    isOnWall = false;
                    playerRigid.useGravity = true;
                    playerRigid.AddForce(jumpOff * jumpForce, ForceMode.Impulse);
                }
            }


            if (isGrounded) MoveOnGround();
            if (isOnWall) MoveOnWall();
        } else if (isSunDrunk)
        {
            ReturnToNormalState(ref soberUpTime, initialSoberTime, ref isSunDrunk, () => SoberUp());
        } else if (isPlaying)
        {
            ReturnToNormalState(ref beSeriousTime, initialBeSeriousTime, ref isPlaying, () => BecomeSerious());
        }

        if (readyToInvestigate && currentInvestigationArea != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isInvestigating = true;
                TutorialManager.sharedInstance.isSearching = true;
                AreaActions areaScript = currentInvestigationArea.GetComponent<AreaActions>();
                if (areaScript != null)
                {
                    areaScript.ActivateView();
                }
            }
        }

        if (catMainCam.isActiveAndEnabled) isInvestigating = false;
    }

    void MoveOnGround()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerAnim.SetTrigger("Walk");
            playerAnim.ResetTrigger("Idle");
            walking = true;
            //steps1.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            playerAnim.ResetTrigger("Walk");
            playerAnim.SetTrigger("Idle");
            walking = false;
            //steps1.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            playerAnim.SetTrigger("Walk");
            playerAnim.ResetTrigger("Idle");
            //steps1.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            playerAnim.ResetTrigger("Walk");
            playerAnim.SetTrigger("Idle");
            //steps1.SetActive(false);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerTrans.Rotate(0, -rotate_speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerTrans.Rotate(0, rotate_speed * Time.deltaTime, 0);
        }
        if (walking)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                //steps1.SetActive(false);
                //steps2.SetActive(true);
                walk_speed = walk_speed + run_speed;
                //playerAnim.SetTrigger("run");
                //playerAnim.ResetTrigger("walk");
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                //steps1.SetActive(true);
                //steps2.SetActive(false);
                walk_speed = oldwalk_speed;
                //playerAnim.ResetTrigger("run");
                //playerAnim.SetTrigger("walk");
            }

            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftCommand))
            {
                walk_speed = walk_speed / 2;
            }
            if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftCommand))
            {
                walk_speed = oldwalk_speed;
            }
        }
        
    }

    void MoveOnWall()
    {
       
    }

    private void ReturnToNormalState(ref float timer, float initialTimer, ref bool state, Action onZero)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!chainStarted)
            {
                lastKeyPressTime = DateTime.Now;
                chainStarted = true;
                timeSinceLastPress = 0f;
            }
            else
            {
                TimeSpan timeBetweenPresses = DateTime.Now - lastKeyPressTime;
                if (timeBetweenPresses.TotalSeconds < 0.2f) // Rapid press threshold
                {
                    timer -= 0.2f;
                    timer = Mathf.Max(timer, 0f);
                }
                lastKeyPressTime = DateTime.Now;
            }
        }
        else
        {
            timeSinceLastPress += Time.deltaTime;

            if (timeSinceLastPress > 0.5f) // Recovery start time
            {
                timer += Time.deltaTime * 0.2f;
                timer = Mathf.Min(timer, initialTimer);
            }

            if (timeSinceLastPress > 2f)
            {
                chainStarted = false;
            }
        }

        if (timer <= 0)
        {
            onZero?.Invoke(); // Trigger event when timer hits zero
            state = false;
        }
    }

    void SoberUp()
    {
        playerAnim.SetTrigger("StandUp");
        playerAnim.SetTrigger("Idle");
    }

    void BecomeSerious()
    {
        playerAnim.SetTrigger("StandUp");
        playerAnim.SetTrigger("Idle");
        isPlaying = false;
        
        TutorialManager.sharedInstance.startPlaying = false;
        beSeriousTime = initialBeSeriousTime;
    }
}
