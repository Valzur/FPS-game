using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoomButton : MonoBehaviour
{
    
    public void ButtonCreateRoom(){
        HostGame.instance.CreateRoom();
    }
}
