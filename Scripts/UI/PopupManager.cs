using UnityEngine;
using UnityEngine.UI;
using System;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPrefab;
    public Transform popupParent;

    public void ShowPopup(string title, string message, Action onConfirm = null, Action onCancel = null)
    {
        GameObject popup = Instantiate(popupPrefab, popupParent);
        Popup popupComponent = popup.GetComponent<Popup>();
        
        popupComponent.Setup(title, message, onConfirm, onCancel);
    }

    public void ShowConfirmPopup(string title, string message, Action onConfirm)
    {
        ShowPopup(title, message, onConfirm);
    }

    public void ShowInfoPopup(string title, string message)
    {
        ShowPopup(title, message, () => { });
    }

    public void ShowErrorPopup(string message)
    {
        ShowPopup("错误", message, () => { });
    }
}

public class Popup : MonoBehaviour
{
    public Text titleText;
    public Text messageText;
    public Button confirmButton;
    public Button cancelButton;

    private Action onConfirm;
    private Action onCancel;

    public void Setup(string title, string message, Action onConfirm = null, Action onCancel = null)
    {
        titleText.text = title;
        messageText.text = message;
        
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;

        confirmButton.onClick.AddListener(OnConfirm);
        
        if (onCancel != null)
        {
            cancelButton.gameObject.SetActive(true);
            cancelButton.onClick.AddListener(OnCancel);
        }
        else
        {
            cancelButton.gameObject.SetActive(false);
        }
    }

    private void OnConfirm()
    {
        onConfirm?.Invoke();
        Destroy(gameObject);
    }

    private void OnCancel()
    {
        onCancel?.Invoke();
        Destroy(gameObject);
    }
}
