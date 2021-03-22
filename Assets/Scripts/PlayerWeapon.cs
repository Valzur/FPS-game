using UnityEngine;
public enum WeaponType{
    Primary,
    Secondary,
    Melee
}
public enum FireType{
    Automatic,
    SemiAutomatic,
    Burst
}

public class PlayerWeapon: MonoBehaviour

{
    public WeaponType weaponType = WeaponType.Primary;
    public FireType fireType = FireType.Automatic;
    public string name = "Rifle";
    public int damage = 30;
    public float range = 100f;
    public float firerate = 1.0f;
}
