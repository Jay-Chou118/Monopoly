using UnityEngine;

public class Economy : MonoBehaviour
{
    public static Economy Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        
    }

    public bool HasEnoughGold(Player player, int amount)
    {
        return player.gold >= amount;
    }

    public void AddGold(Player player, int amount)
    {
        player.gold += amount;
        UIManager.Instance.UpdatePlayerGold(player);
    }

    public void SubtractGold(Player player, int amount)
    {
        player.gold -= amount;
        UIManager.Instance.UpdatePlayerGold(player);
    }

    public void TransferGold(Player fromPlayer, Player toPlayer, int amount)
    {
        if (HasEnoughGold(fromPlayer, amount))
        {
            SubtractGold(fromPlayer, amount);
            AddGold(toPlayer, amount);
            UIManager.Instance.ShowMessage($"{fromPlayer.nickname} 向 {toPlayer.nickname} 支付了 {amount} 金币！");
        }
        else
        {
            UIManager.Instance.ShowError($"{fromPlayer.nickname} 金币不足！");
        }
    }

    public int CalculateRent(PropertyCell property)
    {
        int baseRent = property.rent;
        int levelMultiplier = 1 + property.level;
        int rent = baseRent * levelMultiplier;
        
        if (property.owner != null && property.owner.hasDoubleRent)
        {
            rent *= 2;
            property.owner.hasDoubleRent = false;
        }
        
        return rent;
    }

    public int CalculateUpgradeCost(PropertyCell property)
    {
        return property.upgradeCost * (property.level + 1);
    }

    public int CalculatePropertyValue(PropertyCell property)
    {
        int baseValue = property.price;
        int upgradeValue = property.upgradeCost * property.level;
        return baseValue + upgradeValue;
    }

    public int CalculateMortgageValue(PropertyCell property)
    {
        return CalculatePropertyValue(property) / 2;
    }

    public int CalculateRedeemCost(PropertyCell property)
    {
        return CalculateMortgageValue(property) + 10;
    }

    public void MortgageProperty(PropertyCell property)
    {
        if (property.owner != null && !property.isMortgaged)
        {
            int mortgageValue = CalculateMortgageValue(property);
            AddGold(property.owner, mortgageValue);
            property.isMortgaged = true;
            UIManager.Instance.ShowMessage($"{property.owner.nickname} 抵押了 {property.name}，获得 {mortgageValue} 金币！");
        }
    }

    public void RedeemProperty(PropertyCell property)
    {
        if (property.owner != null && property.isMortgaged)
        {
            int redeemCost = CalculateRedeemCost(property);
            if (HasEnoughGold(property.owner, redeemCost))
            {
                SubtractGold(property.owner, redeemCost);
                property.isMortgaged = false;
                UIManager.Instance.ShowMessage($"{property.owner.nickname} 赎回了 {property.name}！");
            }
            else
            {
                UIManager.Instance.ShowError("金币不足！");
            }
        }
    }

    public int CalculateTotalWealth(Player player)
    {
        int total = player.gold;
        
        foreach (PropertyCell property in player.properties)
        {
            if (!property.isMortgaged)
            {
                total += CalculatePropertyValue(property);
            }
            else
            {
                total -= CalculateRedeemCost(property);
            }
        }
        
        return total;
    }
}
