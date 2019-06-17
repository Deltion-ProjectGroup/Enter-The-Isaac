using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceManager : MonoBehaviour
{
    [Header("ItemPoolData")]
    public Chance.ItemPoolHolder weaponPool;
    public Chance.ItemPoolHolder itemPool;
    public Chance.ItemPoolHolder consumablePool;

    [Header("EnemyDropsData")]
    public Chance.IntAndGOHolder[] currencyDrops;
    public Chance.ItemPoolHolder enemyDropPool;
    public int baseAmmoChance, baseHealthChance, baseKeyChance;
    public GameObject ammoPickup, healthPickup, keyPickup;
    public int ammoChancePerPercentMissing, healthChancePerPercentMissing;
    public int maxKeysBeforeChanceDrop;
    public float keyChanceMultiplier;

    public DOTBuffSO fireBuffData;
    public int fireBuffChance;
    public int explosionBuffChance;
    public GameObject explosion;




    private void Start()
    {
        weaponPool.Initialize();
        itemPool.Initialize();
        consumablePool.Initialize();
        enemyDropPool.Initialize();
    }
    public GameObject getCorrespondingGem(int value)
    {
        if(currencyDrops.Length > 0)
        {
            foreach (Chance.IntAndGOHolder holder in currencyDrops)
            {
                if (value < holder.value)
                {
                    return holder.thisObject;
                }
            }
            return currencyDrops[currencyDrops.Length - 1].thisObject;
        }
        else
        {
            return null;
        }
    }
    public void ApplyBuffEffects(GameObject target)
    {
        int randomChance = Random.Range(1, 101);
        if (randomChance <= fireBuffChance)
        {
            BurnEffect burnEffect = target.AddComponent<BurnEffect>();
            burnEffect.burnData = fireBuffData;
        }
        randomChance = Random.Range(1, 101);
        if (randomChance <= explosionBuffChance)
        {
            ExplosionEffect explEffect = target.AddComponent<ExplosionEffect>();
            explEffect.explosion = explosion;
        }
    }

    public void RecalculateEnemyDropRate(int curHealth, int maxHealth, int curAmmoStore, int maxAmmoStore, int keyCount)
    {
        int missingHealth = Mathf.RoundToInt(100 - ((float)curHealth / (float)maxHealth * 100));
        int missingAmmo = Mathf.RoundToInt(100 - ((float)curAmmoStore / (float)maxAmmoStore * 100));

        float keyChance = baseKeyChance;
        if (keyCount >= maxKeysBeforeChanceDrop)
        {
            for (int i = -1; i < keyCount - maxKeysBeforeChanceDrop; i++)
            {
                keyChance = Mathf.Sqrt(keyChance);
            }
        }

        //Calculates the keyDrops
        foreach (Chance.ItemPoolData data in enemyDropPool.itemPoolData)
        {
            bool done = false;
            foreach (GameObject item in data.items)
            {
                if (item == keyPickup)
                {
                    data.chance = (int)(keyChance * keyChanceMultiplier);
                    done = true;
                    break;
                }
            }
            if (done)
            {
                break;
            }
        }

        //Calculates the healthDrops
        foreach (Chance.ItemPoolData data in enemyDropPool.itemPoolData)
        {
            bool done = false;
            foreach (GameObject item in data.items)
            {
                if (item == healthPickup)
                {
                    data.chance = baseHealthChance + (missingHealth * healthChancePerPercentMissing);
                    done = true;
                    break;
                }
            }
            if (done)
            {
                break;
            }
        }

        //Calculates the ammoDrops
        foreach (Chance.ItemPoolData data in enemyDropPool.itemPoolData)
        {
            bool done = false;
            foreach (GameObject item in data.items)
            {
                if (item == ammoPickup)
                {
                    data.chance = baseAmmoChance + (missingAmmo * ammoChancePerPercentMissing);
                    done = true;
                    break;
                }
            }
            if (done)
            {
                break;
            }
        }
        enemyDropPool.Initialize();
    }
}
namespace Chance
{
    [System.Serializable]
    public class ItemPoolData
    {
        public int unlocksAtFloor = 0;
        public int chance;
        public Vector2 percentageBorders;
        public GameObject[] items;
    }
    [System.Serializable]
    public class ItemPoolHolder
    {
        public ItemPoolData[] itemPoolData;
        public int maxPercentage;

        public void Initialize()
        {
            maxPercentage = 0;
            for (int i = 0; i < itemPoolData.Length; i++)
            {
                ItemPoolData data = itemPoolData[i];
                if (data.unlocksAtFloor <= InGameManager.floor)
                {
                    data.percentageBorders.x = maxPercentage + 1;
                    maxPercentage += data.chance;
                    data.percentageBorders.y = maxPercentage;
                }
            }
        }
        public GameObject GetInstanceFromPool()
        {
            int randomNum = Random.Range(1, maxPercentage + 1);
            foreach (ItemPoolData data in itemPoolData)
            {
                if (randomNum >= data.percentageBorders.x && randomNum <= data.percentageBorders.y)
                {
                    if (data.items.Length > 0)
                    {
                        return (data.items[Random.Range(0, data.items.Length)]);
                    }
                    break;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class ChanceIntData
    {
        public int chance;
        public Vector2 percentageBorders;
        public int amount;
    }
    [System.Serializable]
    public class IntChanceHolder
    {
        public ChanceIntData[] intPoolData;
        public int maxPercentage;

        public void Initialize()
        {
            maxPercentage = 0;
            for (int i = 0; i < intPoolData.Length; i++)
            {
                ChanceIntData data = intPoolData[i];
                data.percentageBorders.x = maxPercentage + 1;
                maxPercentage += data.chance;
                data.percentageBorders.y = maxPercentage;
            }
        }
        public int GetIntFromPool()
        {
            int randomNum = Random.Range(1, maxPercentage + 1);
            foreach (ChanceIntData data in intPoolData)
            {
                if (randomNum >= data.percentageBorders.x && randomNum <= data.percentageBorders.y)
                {
                    return data.amount;
                }
            }
            return 0;
        }
    }

    [System.Serializable]
    public struct IntAndGOHolder
    {
        public int value;
        public GameObject thisObject;
    }
}
