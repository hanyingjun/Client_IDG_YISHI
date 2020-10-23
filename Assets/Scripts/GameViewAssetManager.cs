using UnityEngine;

public class GameViewAssetManager : MonoBehaviour
{
    public static GameViewAssetManager instance;
    public SkillAssets skillAssets;
    public WeaponAssets weaponAssets;

    private void Awake()
    {
        instance = this;
    }
}
