using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public Text messageText;
    public Text errorText;
    public GameObject messagePanel;
    public GameObject errorPanel;
    public GameObject buyPropertyPanel;
    public GameObject bankPanel;
    public GameObject policeStationPanel;
    public GameObject fixedDicePanel;
    public GameObject gameOverPanel;
    public GameObject targetPlayerPanel;
    public GameObject targetPropertyPanel;

    public Text buyPropertyNameText;
    public Text buyPropertyPriceText;
    public Button buyButton;
    public Button cancelButton;

    public Button mortgageButton;
    public Button redeemButton;

    public Button payBailButton;
    public Button stayButton;

    public Button[] diceButtons;

    public Text gameOverWinnerText;

    private PropertyCell currentProperty;
    private Player currentPlayer;
    private Action<Player> targetPlayerCallback;
    private Action<PropertyCell> targetPropertyCallback;

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
        HideAllPanels();
        
        buyButton.onClick.AddListener(() =>
        {
            GameManager.Instance.BuyProperty(currentProperty);
            HideBuyPropertyPanel();
        });
        
        cancelButton.onClick.AddListener(HideBuyPropertyPanel);
        
        payBailButton.onClick.AddListener(() =>
        {
            Economy.Instance.SubtractGold(currentPlayer, 300);
            currentPlayer.isInPoliceStation = false;
            currentPlayer.skipTurns = 0;
            HidePoliceStationPanel();
            GameManager.Instance.EndTurn();
        });
        
        stayButton.onClick.AddListener(() =>
        {
            HidePoliceStationPanel();
            GameManager.Instance.EndTurn();
        });
        
        for (int i = 0; i< diceButtons.Length; i++)
        {
            int diceValue = i + 1;
            diceButtons[i].onClick.AddListener(() =>
            {
                GameManager.Instance.MovePlayer(diceValue);
                HideFixedDicePanel();
            });
        }
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);
        Invoke("HideMessagePanel", 3f);
    }

    public void HideMessagePanel()
    {
        messagePanel.SetActive(false);
    }

    public void ShowError(string error)
    {
        errorText.text = error;
        errorPanel.SetActive(true);
        Invoke("HideErrorPanel", 2f);
    }

    public void HideErrorPanel()
    {
        errorPanel.SetActive(false);
    }

    public void ShowBuyPropertyPanel(PropertyCell property)
    {
        currentProperty = property;
        buyPropertyNameText.text = property.name;
        buyPropertyPriceText.text = $"价格：{property.price} 金币";
        buyPropertyPanel.SetActive(true);
    }

    public void HideBuyPropertyPanel()
    {
        buyPropertyPanel.SetActive(false);
    }

    public void ShowBankPanel()
    {
        bankPanel.SetActive(true);
    }

    public void HideBankPanel()
    {
        bankPanel.SetActive(false);
    }

    public void ShowPoliceStationPanel(Player player)
    {
        currentPlayer = player;
        policeStationPanel.SetActive(true);
    }

    public void HidePoliceStationPanel()
    {
        policeStationPanel.SetActive(false);
    }

    public void ShowFixedDicePanel()
    {
        fixedDicePanel.SetActive(true);
    }

    public void HideFixedDicePanel()
    {
        fixedDicePanel.SetActive(false);
    }

    public void ShowGameOverPanel(Player winner)
    {
        gameOverWinnerText.text = $"{winner.nickname} 获胜！";
        gameOverPanel.SetActive(true);
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowTargetPlayerPanel(Action<Player> callback)
    {
        targetPlayerCallback = callback;
        targetPlayerPanel.SetActive(true);
    }

    public void HideTargetPlayerPanel()
    {
        targetPlayerPanel.SetActive(false);
    }

    public void ShowTargetPropertyPanel(Action<PropertyCell> callback)
    {
        targetPropertyCallback = callback;
        targetPropertyPanel.SetActive(true);
    }

    public void HideTargetPropertyPanel()
    {
        targetPropertyPanel.SetActive(false);
    }

    public void UpdatePlayerPanel(Player player)
    {
        
    }

    public void UpdatePlayerGold(Player player)
    {
        
    }

    public void HideAllPanels()
    {
        messagePanel.SetActive(false);
        errorPanel.SetActive(false);
        buyPropertyPanel.SetActive(false);
        bankPanel.SetActive(false);
        policeStationPanel.SetActive(false);
        fixedDicePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        targetPlayerPanel.SetActive(false);
        targetPropertyPanel.SetActive(false);
    }
}
