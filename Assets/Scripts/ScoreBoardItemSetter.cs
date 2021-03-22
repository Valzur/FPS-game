using UnityEngine;
using UnityEngine.UI;
public class ScoreBoardItemSetter : MonoBehaviour
{
    [SerializeField]
    private Text Username;
    [SerializeField]
    private Text Kills;
    [SerializeField]
    private Text Deaths;
    

    public void SetScore(string Username, string Kills, string Deaths){
        this.Username.text = Username;
        //If it is the title, we'd change the background color.
        if(Kills == "KILLS")
            GetComponent<Image>().color = new Color32(0,0,0,0);

        this.Kills.text = Kills;
        this.Deaths.text = Deaths;
    }
    
}
