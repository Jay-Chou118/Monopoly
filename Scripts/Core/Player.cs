using UnityEngine;
using System.Collections.Generic;

public class Player
{
    public string nickname;
    public int gold;
    public int position;
    public bool isBankrupt;
    public bool isInHospital;
    public bool isInPoliceStation;
    public int skipTurns;
    public bool hasDoubleRent;
    public bool hasImmunity;
    
    public List<PropertyCell> properties = new List<PropertyCell>();
    public List<Card> cards = new List<Card>();
    
    public const int MAX_CARDS = 6;
    
    public Player(string nickname)
    {
        this.nickname = nickname;
        ResetPlayer();
    }
    
    public void ResetPlayer()
    {
        gold = 3000;
        position = 0;
        isBankrupt = false;
        isInHospital = false;
        isInPoliceStation = false;
        skipTurns = 0;
        hasDoubleRent = false;
        hasImmunity = false;
        
        properties.Clear();
        cards.Clear();
    }
    
    public bool HasCard(CardType type)
    {
        return cards.Exists(card => card.type == type);
    }
    
    public Card GetCard(CardType type)
    {
        return cards.Find(card => card.type == type);
    }
    
    public bool AddCard(Card card)
    {
        if (cards.Count >= MAX_CARDS)
        {
            return false;
        }
        
        cards.Add(card);
        return true;
    }
    
    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }
    
    public void RemoveCard(CardType type)
    {
        Card cardToRemove = cards.Find(card => card.type == type);
        if (cardToRemove != null)
        {
            cards.Remove(cardToRemove);
        }
    }
    
    public bool CanAfford(int amount)
    {
        return gold >= amount;
    }
    
    public void AddGold(int amount)
    {
        gold += amount;
    }
    
    public void SubtractGold(int amount)
    {
        gold -= amount;
    }
    
    public int GetPropertyCount()
    {
        return properties.Count;
    }
    
    public int GetCardCount()
    {
        return cards.Count;
    }
    
    public bool HasFreeSlots()
    {
        return cards.Count< MAX_CARDS;
    }
}
