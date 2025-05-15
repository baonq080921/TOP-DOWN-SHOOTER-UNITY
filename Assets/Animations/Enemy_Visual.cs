using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public enum Enemy_MeleeWeaponType
{
        OneHand, Throw

}
public class Enemy_Visual : MonoBehaviour
{
    [Header("Weapon")]

    [SerializeField] private EnemyWeapon_Model[] weaponModels;
    [SerializeField] private Enemy_MeleeWeaponType weaponType;

    public GameObject cuurrentWeaponModel {get; private set;}



    [Header("Color")]
    [SerializeField] private Texture[] colorTextures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    void Start()
    {

        weaponModels = GetComponentsInChildren<EnemyWeapon_Model>(true);
        InvokeRepeating(nameof(SetUpLook), 0, 1.5f);
    }

    public void SetUpLook()
    {
        SetUpRandom();
        SetUpRandomWeapon();
    }

    public void SetUpWeaponType(Enemy_MeleeWeaponType weaponType = Enemy_MeleeWeaponType.OneHand)
    {
        this.weaponType = weaponType;
    }

    private void SetUpRandom()
    {
        int randomIndex = Random.Range(0, colorTextures.Length);

        Material newMat = new Material(skinnedMeshRenderer.material);
        newMat.mainTexture = colorTextures[randomIndex];
        skinnedMeshRenderer.material = newMat;
    }

    private void SetUpRandomWeapon()
    {

        foreach (EnemyWeapon_Model weaponModel in weaponModels)
        {
            weaponModel.gameObject.SetActive(false);
        }

        List<EnemyWeapon_Model> filteredWeaponsModels = new List<EnemyWeapon_Model>();

        foreach (EnemyWeapon_Model weaponModel in weaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {
                filteredWeaponsModels.Add(weaponModel);
            }
        }

        int randomIndex = Random.Range(0, filteredWeaponsModels.Count);
        cuurrentWeaponModel = filteredWeaponsModels[randomIndex].gameObject;
        cuurrentWeaponModel.SetActive(true);
    }

}
