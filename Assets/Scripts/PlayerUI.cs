using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
    #region Initializations
    public static PlayerUI Instance;
    [SerializeField]
    public Text EquipText;
    [SerializeField]
    private Text HealthText;
    [SerializeField]
    private Text ArmorText;
    [SerializeField]
    private Text EnergyText;
    [SerializeField]
    private Image HealthBar;
    [SerializeField]
    private Image ArmorBar;
    [SerializeField]
    public Image HitVignette;
    [SerializeField]
    private GameObject scoreBoard;
    [SerializeField]
    private Image thrusterfuelamt;
    private PlayerController controller;
    [SerializeField]
    private GameObject pauseMenu;
    public MessageManager messageManager;
    public Image Kill;
    [SerializeField]
    public Image Crosshair;
    [SerializeField]
    private Color32 DamagedCrosshairColor = new Color32(62,146,204,255);
    [SerializeField]
    private Color32 NormalCrosshairColor = new Color32(230,230,230,255);
    #endregion
    void SetFuelamt(float amt){
        thrusterfuelamt.fillAmount = amt;
        EnergyText.text = (int)(amt*100) + "%";
    }

    public void SetController(PlayerController _controller){
        controller=_controller;
    }

    void Start(){
        if(!Instance){
            Instance = this;
        }
        PauseMenu.IsOn=false;
    }
    private void Update(){
        SetFuelamt(controller.GetFuelAmt());

        if(Input.GetKeyDown(KeyCode.Tab)){
            scoreBoard.SetActive(true);
        } else if (Input.GetKeyUp(KeyCode.Tab)){
            scoreBoard.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            TogglePauseMenu();
        }
    }
    public void TogglePauseMenu(){
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn=pauseMenu.activeSelf;
    }
    public void UpdateHealthAndArmorBars(float HealthRatio, float ArmorRatio){
        HealthBar.fillAmount = HealthRatio;
        HealthText.text = (int)(HealthRatio*100.0f) + "%";
        ArmorBar.fillAmount = ArmorRatio;
        ArmorText.text = (int)(ArmorRatio*100.0f) + "%";
    }
    public void DamageVFX(){
        StartCoroutine(TimedDamageVFX());
    }
    IEnumerator TimedDamageVFX(){
        HitVignette.enabled = true;
        yield return  new WaitForSeconds(0.2f);
        HitVignette.enabled = false;
    }
    public void SwitchCrosshairColor(){
        StartCoroutine(SwitchCrosshairColorInternally());
    }
    IEnumerator SwitchCrosshairColorInternally(){
        Crosshair.color = DamagedCrosshairColor;
        yield return new WaitForSeconds(0.1f);
        Crosshair.color = NormalCrosshairColor;
    }
}