using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Button rollDiceButton;
    public Button endTurnButton;
    public Text currentPlayerText;
    public Text currentPositionText;
    public Text goldText;
    public Text propertyCountText;
    public Text cardCountText;

    private void Start()
    {
        rollDiceButton.onClick.AddListener(() =>
        {
            GameManager.Instance.RollDice();
            rollDiceButton.interactable = false;
        });

        endTurnButton.onClick.AddListener(() =>
        {
            GameManager.Instance.EndTurn();
            rollDiceButton.interactable = true;
        });
    }

    private void Update()
    {
        UpdatePlayerInfo();
    }

    private void UpdatePlayerInfo()
    {
        if (GameManager.Instance != null && GameManager.Instance.players.Count >0)
        {
            Player currentPlayer = GameManager.Instance.players[GameManager.Instance.currentPlayerIndex];
            Cell currentCell = GameManager.Instance.map.GetCell(currentPlayer.position);
            
            currentPlayerText.text = $"当前玩家：{currentPlayer.nickname}";
            currentPositionText.text = $"位置：{currentCell.name}";
            goldText.text = $"金币：{currentPlayer.gold}";
            propertyCountText.text = $"地产：{currentPlayer.GetPropertyCount()}";
            cardCountText.text = $"道具卡：{currentPlayer.GetCardCount()}/{Player.MAX_CARDS}";

            bool isCurrentPlayerTurn = !currentPlayer.isBankrupt && !currentPlayer.isInHospital && !currentPlayer.isInPoliceStation;
            rollDiceButton.interactable = isCurrentPlayerTurn && !GameManager.Instance.isGameOver;
            endTurnButton.interactable = isCurrentPlayerTurn && !GameManager.Instance.isGameOver;
        }
    }
}
