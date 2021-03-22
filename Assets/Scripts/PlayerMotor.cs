using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioManager))]
public class PlayerMotor : MonoBehaviour
{
    #region Initializations
    private WeaponManager weaponManager;
    private AudioManager audioManager;
    private Vector3 Velocity=Vector3.zero;
    private Rigidbody rb;
    private Vector3 rotation=Vector3.zero;
    private float CamRotX= 0f;
    private float currentCamRot=0f;
    private Vector3 thrusterForce = Vector3.zero;
    private bool UsedThruster = false;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float CameraRotationLimit=85f;
    public bool isMoving = true;
    private bool isRotating = true;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        weaponManager = GetComponent<WeaponManager>();
        rb = GetComponent<Rigidbody>();
        audioManager = GetComponent<AudioManager>();
    }

    public void Move(Vector3 vel){
        Velocity=vel;
    }
    void FixedUpdate()
    {
        if(isMoving)
            PerformMovement();
        if(isRotating)
            PerformRotation();
    }

    void PerformMovement(){
        if(Velocity!=Vector3.zero){
            if(weaponManager.GetcurrentWeapon())
                weaponManager.GetcurrentWeapon().GetComponent<Animator>().SetBool("IsMoving", true);
            rb.MovePosition(rb.position+Velocity*Time.fixedDeltaTime);
        }
        else
        {
            if(weaponManager.GetcurrentWeapon())
                weaponManager.GetcurrentWeapon().GetComponent<Animator>().SetBool("IsMoving", false);
        }
        if(thrusterForce!=Vector3.zero)
        {
            rb.AddForce(thrusterForce*Time.fixedDeltaTime, ForceMode.Acceleration);
            if(!UsedThruster){
            UsedThruster = true;
            audioManager.Play("Jump");
            }
        }else{
            UsedThruster = false;
            audioManager.Stop("Jump");
        }
    }

    public void Rotate(Vector3 rot){
        rotation=rot;
    }
    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation*Quaternion.Euler(rotation*Time.fixedDeltaTime));
        currentCamRot-= CamRotX*Time.fixedDeltaTime;
        currentCamRot = Mathf.Clamp(currentCamRot,-CameraRotationLimit,CameraRotationLimit);
        cam.transform.localEulerAngles= new Vector3(currentCamRot,0f,0f);
    }
    public void RotateCamera(float rotcamX){
        CamRotX=rotcamX;
    }
    public void ApplyThruster(Vector3 TF){
        thrusterForce=TF;
    }
    
    public void AdvRotate(Vector3 rot){
        Vector3 ModifiedRotation = rot;
        if(rot.x==-1)
            ModifiedRotation = new Vector3(rb.rotation.x,rot.y,rot.z);

        if(rot.y==-1)
            ModifiedRotation = new Vector3(ModifiedRotation.x,rb.rotation.y,rot.z);

        if(rot.z==-1)
            ModifiedRotation = new Vector3(ModifiedRotation.x,ModifiedRotation.y,rb.rotation.z);

        rotation = ModifiedRotation;

    }

    public void MoveCamera(Vector3 pos){
        cam.transform.position = pos;
    }
    public void RotateCamera(Quaternion rot){
        cam.transform.rotation = rot;
    }
    public void SetMovement(bool isMoving){
        this.isMoving = isMoving;
    }
    public void SetRotation(bool isRotating){
        this.isRotating = isRotating;
    }
}
