using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelector : MonoBehaviour
{
    public GameObject player;
    public Image loadout;
    public Sprite[] loadouts;

    private void Start()
    {
        loadout.sprite = loadouts[5];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!player)
        {
            try
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            catch { }
        }
        else
        {
            try
            {
                if (player.GetComponent<Weapon>().weaponSelect == 0 && player.GetComponent<Weapon>().weaponNames.Contains("Railgun"))
                {
                    loadout.sprite = loadouts[0];
                }
                else if (player.GetComponent<Weapon>().weaponSelect == 1 && player.GetComponent<Weapon>().weaponNames.Contains("Book"))
                {
                    loadout.sprite = loadouts[1];
                }
                else if (player.GetComponent<Weapon>().weaponSelect == 2 && player.GetComponent<Weapon>().weaponNames.Contains("Stick"))
                {
                    loadout.sprite = loadouts[2];
                }
                else if (player.GetComponent<Weapon>().weaponSelect == 3 && player.GetComponent<Weapon>().weaponNames.Contains("Hammer"))
                {
                    loadout.sprite = loadouts[3];
                } 
                else if (player.GetComponent<Weapon>().weaponSelect == 4 && player.GetComponent<Weapon>().weaponNames.Contains("Firewall"))
                {
                    loadout.sprite = loadouts[4];
                } 
                else
                {
                    loadout.sprite = loadouts[5];
                }
            }
            catch {
                loadout.sprite = loadouts[5];
            }
        }
    }
}
