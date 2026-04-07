using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<Player> players = new List<Player>();
    public int currentPlayerIndex = 0;
    public bool isGameOver = false;

    public Map map;
    public CardSystem cardSystem;
    public Economy economy;

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

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        map.Initialize();
        cardSystem.Initialize();
        economy.Initialize();
        
        StartGame();
    }

    public void StartGame()
    {
        currentPlayerIndex = 0;
        isGameOver = false;
        
        foreach (Player player in players)
        {
            player.ResetPlayer();
        }
        
        UpdateCurrentPlayer();
    }

    public void RollDice()
    {
        if (!IsCurrentPlayerTurn() || isGameOver) return;

        int diceResult = Random.Range(1, 7);
        MovePlayer(diceResult);
    }

    public void MovePlayer(int steps)
    {
        Player currentPlayer = players[currentPlayerIndex];
        int newPosition = (currentPlayer.position + steps) % map.totalCells;
        
        if (newPosition < currentPlayer.position)
        {
            economy.AddGold(currentPlayer, 500);
        }
        
        currentPlayer.position = newPosition;
        TriggerCellEvent(newPosition);
    }

    public void TriggerCellEvent(int cellIndex)
    {
        Cell cell = map.GetCell(cellIndex);
        switch (cell.type)
        {
            case CellType.Start:
                break;
            case CellType.Property:
                HandlePropertyCell(cell as PropertyCell);
                break;
            case CellType.Special:
                HandleSpecialCell(cell as SpecialCell);
                break;
            case CellType.Card:
                HandleCardCell();
                break;
        }
    }

    private void HandlePropertyCell(PropertyCell property)
    {
        Player currentPlayer = players[currentPlayerIndex];
        
        if (property.owner == null)
        {
            UIManager.Instance.ShowBuyPropertyPanel(property);
        }
        else if (property.owner != currentPlayer)
        {
            int rent = economy.CalculateRent(property);
            economy.TransferGold(currentPlayer, property.owner, rent);
        }
    }

    private void HandleSpecialCell(SpecialCell specialCell)
    {
        Player currentPlayer = players[currentPlayerIndex];
        
        switch (specialCell.specialType)
        {
            case SpecialType.Hospital:
                SendToHospital(currentPlayer);
                break;
            case SpecialType.PoliceStation:
                SendToPoliceStation(currentPlayer);
                break;
            case SpecialType.Bank:
                UIManager.Instance.ShowBankPanel();
                break;
            case SpecialType.TaxOffice:
                PayTax(currentPlayer);
                break;
            case SpecialType.Park:
                EndTurn();
                break;
            case SpecialType.Chance:
                DrawChanceCard();
                break;
        }
    }

    private void HandleCardCell()
    {
        cardSystem.GiveRandomCard(players[currentPlayerIndex]);
    }

    private void SendToHospital(Player player)
    {
        if (player.hasImmunity)
        {
            player.hasImmunity = false;
            UIManager.Instance.ShowMessage($"{player.nickname} 使用免罚卡避免了被送入医院！");
        }
        else
        {
            player.isInHospital = true;
            player.skipTurns = 1;
            economy.SubtractGold(player, 200);
            UIManager.Instance.ShowMessage($"{player.nickname} 被送入医院！");
        }
    }

    private void SendToPoliceStation(Player player)
    {
        if (player.hasImmunity)
        {
            player.hasImmunity = false;
            UIManager.Instance.ShowMessage($"{player.nickname} 使用免罚卡避免了被送入警局！");
        }
        else
        {
            player.isInPoliceStation = true;
            player.skipTurns = 1;
            UIManager.Instance.ShowPoliceStationPanel(player);
        }
    }

    private void PayTax(Player player)
    {
        if (player.hasImmunity)
        {
            player.hasImmunity = false;
            UIManager.Instance.ShowMessage($"{player.nickname} 使用免罚卡豁免了税款！");
        }
        else
        {
            int tax = (int)(player.gold * 0.1f);
            economy.SubtractGold(player, tax);
            UIManager.Instance.ShowMessage($"{player.nickname} 支付了 {tax} 金币的税款！");
        }
    }

    private void DrawChanceCard()
    {
        Card card = cardSystem.GetRandomChanceCard();
        cardSystem.ExecuteCard(card, players[currentPlayerIndex]);
    }

    public void BuyProperty(PropertyCell property)
    {
        Player currentPlayer = players[currentPlayerIndex];
        
        if (economy.HasEnoughGold(currentPlayer, property.price))
        {
            economy.SubtractGold(currentPlayer, property.price);
            property.owner = currentPlayer;
            currentPlayer.properties.Add(property);
            
            UIManager.Instance.ShowMessage($"{currentPlayer.nickname} 购买了 {property.name}！");
        }
        else
        {
            UIManager.Instance.ShowError("金币不足！");
        }
    }

    public void UpgradeProperty(PropertyCell property)
    {
        Player currentPlayer = players[currentPlayerIndex];
        
        if (property.owner == currentPlayer && property.level< 3)
        {
            int upgradeCost = economy.CalculateUpgradeCost(property);
            
            if (economy.HasEnoughGold(currentPlayer, upgradeCost))
            {
                economy.SubtractGold(currentPlayer, upgradeCost);
                property.level++;
                
                UIManager.Instance.ShowMessage($"{currentPlayer.nickname} 升级了 {property.name}！");
            }
            else
            {
                UIManager.Instance.ShowError("金币不足！");
            }
        }
    }

    public void EndTurn()
    {
        if (!IsCurrentPlayerTurn() || isGameOver) return;

        Player currentPlayer = players[currentPlayerIndex];
        
        if (currentPlayer.skipTurns >0)
        {
            currentPlayer.skipTurns--;
            if (currentPlayer.skipTurns == 0)
            {
                currentPlayer.isInHospital = false;
                currentPlayer.isInPoliceStation = false;
            }
            else
            {
                NextPlayer();
                return;
            }
        }
        
        CheckBankruptcy(currentPlayer);
        NextPlayer();
    }

    private void NextPlayer()
    {
        do
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        } while (players[currentPlayerIndex].isBankrupt && !CheckGameOver());
        
        UpdateCurrentPlayer();
    }

    private void UpdateCurrentPlayer()
    {
        UIManager.Instance.UpdatePlayerPanel(players[currentPlayerIndex]);
    }

    private bool IsCurrentPlayerTurn()
    {
        Player currentPlayer = players[currentPlayerIndex];
        return !currentPlayer.isBankrupt && !currentPlayer.isInHospital && !currentPlayer.isInPoliceStation;
    }

    private void CheckBankruptcy(Player player)
    {
        if (player.gold< 0 && player.properties.Count == 0)
        {
            player.isBankrupt = true;
            UIManager.Instance.ShowMessage($"{player.nickname} 破产了！");
            
            CheckGameOver();
        }
    }

    private bool CheckGameOver()
    {
        int activePlayers = 0;
        Player winner = null;
        
        foreach (Player player in players)
        {
            if (!player.isBankrupt)
            {
                activePlayers++;
                winner = player;
            }
        }
        
        if (activePlayers<= 1)
        {
            isGameOver = true;
            UIManager.Instance.ShowGameOverPanel(winner);
            return true;
        }
        
        return false;
    }

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        players.Remove(player);
    }
}
