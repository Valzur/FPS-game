using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{   
    #region Initializations
    [SerializeField]
    private GameObject [] ObjectsToDisable;
    [SerializeField]
    private Image [] ImagesToHide;
    [SerializeField]
    private bool isMessagesBoxActive = true;
    [SerializeField]
    private InputField message;
    [SerializeField]
    private Text MessageObject;
    [SerializeField]
    private GameObject MessageBox;
    [SerializeField]
    private string netId;
    public ChatSystem chatSystem;
    [SerializeField]
    private Color32 PlayerMessageColor;
    #endregion
    void Start(){
        PlayerMessageColor = new Color32(255,240,90,255);
    }
    public void SetID(string ID){
        netId = ID;
    }
    public bool GetisActive(){
        return isMessagesBoxActive;
    }
    public void ButtonSendMessage(){
        chatSystem.SendMessage(netId, message.text);
        message.Select();
        message.text = "";
    }
    public void SwitchChat(){
        if(isMessagesBoxActive){
            if(message.text ==  ""){
                //Deactivate
                foreach (GameObject Object in ObjectsToDisable)
                {
                    Object.SetActive(false);
                }
                foreach (Image image in ImagesToHide)
                {
                    image.color = new Vector4(image.color.r,image.color.b,image.color.g,40);
                }
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isMessagesBoxActive = false;
            }else{
                //Send message and deactivate
                chatSystem.SendMessage(netId , message.text);
                message.Select();
                message.text = "";
                SwitchChat();
            }
        }else{
            //Activate
            isMessagesBoxActive = true;

            foreach (GameObject Object in ObjectsToDisable)
            {
                Object.SetActive(true);
            }
            foreach (Image image in ImagesToHide)
            {
                image.color = new Vector4(image.color.r,image.color.b,image.color.g,255);
            }
            //Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            message.Select();
        }

    }
    public void AddMessage(string PlayerName, string message){
        Text messageObject = Instantiate(MessageObject, Vector3.zero,transform.rotation);
        if(chatSystem.gameObject.name == PlayerName){
            messageObject.color = PlayerMessageColor;
        }
        messageObject.text = PlayerName + ": " + message;
        messageObject.transform.SetParent(MessageBox.transform);
        messageObject.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
    }
}
