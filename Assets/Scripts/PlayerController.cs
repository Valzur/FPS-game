using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(AudioManager))]
public class PlayerController : MonoBehaviour
{
    #region Initializations
    [SerializeField]
    private GameObject Thrust;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity=3f;
    [SerializeField]
    private float thrusterForce=1000f;
    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float Fuelamt=1;
    [Header("Spring Settings:")]
    [SerializeField]
    private float JointSpring=20f;
    [SerializeField]
    private float JointMaxForce= 40f;

    private ConfigurableJoint Joint;
    private PlayerMotor motor;
    private Animator animator;
    private Player player;
    [SerializeField]
    private WeaponManager weps;
    private GameObject wepl;
    private AudioManager audioManager;
    [HideInInspector]
    private float ControlTime;
    public const float InitialControlTime = 0.4f;
    public PlayerUI playerUI;
    #endregion
    void Start() {
        ControlTime = InitialControlTime;
        motor=GetComponent<PlayerMotor>();
        Joint=GetComponent<ConfigurableJoint>();
        SetJointSettings(JointSpring);
        player=GetComponent<Player>();
        audioManager = GetComponent<AudioManager>();
    }
    void FixedUpdate() {
        if(Cursor.lockState!=CursorLockMode.Locked){
            Cursor.lockState=CursorLockMode.Locked;
        }
        
        if(PauseMenu.IsOn || playerUI.messageManager.GetisActive()){
            if(Cursor.lockState!=CursorLockMode.None){
                Cursor.lockState=CursorLockMode.None;
            }
            if(Cursor.visible!=true){
                Cursor.visible = true;
            }

            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(0f);

            return;
        }

        

        RaycastHit _hit;
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 100f)){
            Joint.targetPosition=new Vector3 (0f,0f-_hit.point.y,0f);
        } else {
            Joint.targetPosition= new Vector3(0f,0f,0f);
        }

        float xMove=Input.GetAxis("Horizontal");
        float zMove=Input.GetAxis("Vertical");

        Vector3 movHorizontal = transform.right*xMove;
        Vector3 movVertical = transform.forward*zMove;

        Vector3 Velocity=(movHorizontal+movVertical).normalized*speed;

        // Animation Variable
        wepl=weps.GetWepInstance();
        if(animator){
            animator = wepl.GetComponent<Animator>();
            animator.SetBool("IsMoving", zMove!=0 );
        }

        
        motor.Move(Velocity);

        DoRotation();

        Vector3 ThrustF=Vector3.zero;
        if(Input.GetButton("Jump") && Fuelamt>0f){
            Fuelamt-=thrusterFuelBurnSpeed*Time.deltaTime;
            if(Fuelamt>0.01f){
            ThrustF=Vector3.up*thrusterForce;
            SetJointSettings(0f);
            Thrust.SetActive(true);
            player.CmdJumpStart();
            }else{
                if(ControlTime == InitialControlTime){
                    audioManager.PlayOneShot("PowerDown");
                    ControlTime -=Time.deltaTime;
                }else if(ControlTime <= 0){
                    ControlTime = InitialControlTime;
                }else{
                    ControlTime-=Time.deltaTime;
                }
            }
        } else {
            Fuelamt+= thrusterFuelRegenSpeed*Time.fixedDeltaTime;

            SetJointSettings(JointSpring);
            Thrust.SetActive(false);
            player.CmdJumpStop();
        }
        
        Fuelamt = Mathf.Clamp(Fuelamt,0,1);

        motor.ApplyThruster(ThrustF);
    }

    public float GetFuelAmt(){
        return Fuelamt;
    }
    private void SetJointSettings(float _JointSpring){
        Joint.yDrive= new JointDrive {
            positionSpring=_JointSpring,
            maximumForce=JointMaxForce
        };
    }
    private void DoRotation(){
        float yRot=Input.GetAxis("Mouse X");
        float xRot=Input.GetAxis("Mouse Y");

        Vector3 rotation= new Vector3(0f, yRot, 0f)*lookSensitivity;
        motor.Rotate(rotation);

        float CamRotationX = xRot*lookSensitivity;
        motor.RotateCamera(CamRotationX);
    }
}
