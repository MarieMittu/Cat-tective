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
    public float jumpForce = 2.0f;
    public bool isGrounded;

    public Transform playerTrans;
    public Transform cameraTrans;
    public Transform cameraPivot;

    public float mouseSensitivity = 2.0f;
    public float verticalRotationLimit = 80f;
    private float verticalRotation = 0f;
    public float cameraResetSpeed = 5f;

    void Start()
    {
        playerRigid = GetComponent<Rigidbody>();
        jump = Vector3.up;
    }

    void FixedUpdate()
    {
        Vector3 velocity = playerRigid.velocity; // Preserve current velocity

        if (Input.GetKey(KeyCode.W))
        {
            velocity.x = transform.forward.x * walk_speed * Time.deltaTime;
            velocity.z = transform.forward.z * walk_speed * Time.deltaTime;
        }

        playerRigid.velocity = velocity; // Apply velocity without resetting Y-axis
    }


    void OnCollisionStay()
    {
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
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
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    playerAnim.SetTrigger("walkback");
        //    playerAnim.ResetTrigger("idle");
        //    //steps1.SetActive(true);
        //}
        //if (Input.GetKeyUp(KeyCode.S))
        //{
        //    playerAnim.ResetTrigger("walkback");
        //    playerAnim.SetTrigger("idle");
        //    //steps1.SetActive(false);
        //}
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
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRigid.velocity = new Vector3(playerRigid.velocity.x, 0, playerRigid.velocity.z);
            playerRigid.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

    }
}
