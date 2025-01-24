using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    private Weapon[] weapons;

    private void Start()
    {
        weapons = GetComponentsInChildren<Weapon>();
        weapons[0].SetActive(true);
    }
}
