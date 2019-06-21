using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class SaveDatabase : MonoBehaviour
{
    [SerializeField] PlayerSaveData backUpSaveData;

    public List<PickupSO> weapons;
    public List<PickupSO> passiveItems;
    public List<GunType> guns;

    public void SavePlayerData(PlayerController player, Inventory playerInventory, Hitbox playerHitbox)
    {
        PlayerSaveData dataHolder = new PlayerSaveData();

        if(playerInventory.passiveItems != null)
        {
            dataHolder.passiveItems = new List<int>();
            foreach(PickupSO data in playerInventory.passiveItems)
            {
                dataHolder.passiveItems.Add(data.GetInstanceID());
            }
        }
        if (playerInventory.weaponItems != null)
        {
            dataHolder.inventoryGuns = new List<int>();
            foreach (PickupSO data in playerInventory.weaponItems)
            {
                dataHolder.inventoryGuns.Add(data.GetInstanceID());
            }
        }
        dataHolder.playerWeapons = new List<WeaponData>();
        for(int i = 0; i < player.guns.Count; i++)
        {
            dataHolder.playerWeapons.Add(new WeaponData(player.guns[i].GetInstanceID(), player.magazineStore[i], player.ammoStore[i]));
        }

        dataHolder.keys = player.keys;
        dataHolder.money = player.money;
        dataHolder.curHealth = playerHitbox.curHealth;
        dataHolder.fakeHealth = playerHitbox.fakeHealth;

        XmlSerializer playerSerializer = new XmlSerializer(typeof(PlayerSaveData));
        FileStream playerStream = new FileStream(Application.dataPath + "/PlayerData", FileMode.Create);
        playerSerializer.Serialize(playerStream, dataHolder);
        playerStream.Close();
    }
    public void SaveLevel(int levelToLoadNextTime)
    {
        XmlSerializer levelSerializer = new XmlSerializer(typeof(int));
        FileStream levelStream = new FileStream(Application.dataPath + "/LevelData", FileMode.Create);
        levelSerializer.Serialize(levelStream, levelToLoadNextTime);
        levelStream.Close();
    }
    public void LoadPlayerData(PlayerController player, Inventory playerInventory, Hitbox playerHitbox)
    {
        if(File.Exists(Application.dataPath + "/PlayerData"))
        {
            XmlSerializer playerSerializer = new XmlSerializer(typeof(PlayerSaveData));
            FileStream playerStream = new FileStream(Application.dataPath + "/PlayerData", FileMode.Open);
            PlayerSaveData dataHolder = (PlayerSaveData)playerSerializer.Deserialize(playerStream);
            playerStream.Close();

            backUpSaveData = dataHolder;

            player.guns = new List<GunType>();
            player.magazineStore = new List<int>();
            player.ammoStore = new List<int>();

            foreach (WeaponData data in dataHolder.playerWeapons)
            {
                foreach (GunType gunType in guns)
                {
                    if (gunType.GetInstanceID() == data.gunId)
                    {
                        player.guns.Add(gunType);
                        break;
                    }
                }
                player.magazineStore.Add(data.remainingAmmo);
                player.ammoStore.Add(data.magazineAmmo);
                player.gun.Start();
            }

            foreach (int id in dataHolder.passiveItems)
            {
                foreach (PickupSO passiveItem in passiveItems)
                {
                    if (passiveItem.GetInstanceID() == id)
                    {
                        playerInventory.passiveItems.Add(passiveItem);
                        Effect[] effects = passiveItem.entityPrefab.GetComponents<Effect>();
                        foreach(Effect effect in effects)
                        {
                            effect.ApplyEffect(player.gameObject);
                        }
                        break;
                    }
                }
            }

            foreach (int id in dataHolder.inventoryGuns)
            {
                foreach (PickupSO weaponItem in weapons)
                {
                    if (weaponItem.GetInstanceID() == id)
                    {
                        playerInventory.weaponItems.Add(weaponItem);
                        /*
                        Effect[] effects = weaponItem.entityPrefab.GetComponents<Effect>();
                        foreach (Effect effect in effects)
                        {
                            effect.ApplyEffect(player.gameObject);
                        }
                        '*/
                        break;
                    }
                }
            }

            player.keys = dataHolder.keys;
            player.money = dataHolder.money;
            playerHitbox.curHealth = dataHolder.curHealth;
            playerHitbox.fakeHealth = dataHolder.fakeHealth;
            File.Delete(Application.dataPath + "/PlayerData");
        }
    }
    public int LoadSavedLevel()
    {
        if(File.Exists(Application.dataPath + "/LevelData"))
        {
            XmlSerializer levelSerializer = new XmlSerializer(typeof(int));
            FileStream levelStream = new FileStream(Application.dataPath + "/LevelData", FileMode.Open);
            int levelToLoad = (int)levelSerializer.Deserialize(levelStream);
            levelStream.Close();
            File.Delete(Application.dataPath + "/LevelData");
            return levelToLoad;
        }
        return 0;
    }

    [System.Serializable]
    public struct PlayerSaveData
    {
        public List<int> passiveItems;
        public List<int> inventoryGuns;
        public List<WeaponData> playerWeapons;
        public int keys;
        public int money;
        public float curHealth, fakeHealth;

    }
    [System.Serializable]
    public struct WeaponData
    {
        public int gunId;
        public int remainingAmmo;
        public int magazineAmmo;

        public WeaponData(int gunID, int ammo, int magazineAmmo_)
        {
            gunId = gunID;
            remainingAmmo = ammo;
            magazineAmmo = magazineAmmo_;
        }
    }
}
