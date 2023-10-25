using UnityEngine;
using UnityEngine.Events;

public class PlayerManager1 : MonoBehaviour
{       
    public DbManager dbManager;
    public PlayerLogic playerLogic;
    public InterfaceManager uiManager;
    [HideInInspector] public UnityEvent updated;

    public void GetPlayerData()
    {
        dbManager.GetUserInfo();
    }

    public void BuyBread(){
        int amount = 1;
        float breadPrice = 10;
        playerLogic.AddCash(-breadPrice);
        playerLogic.AddBread(amount);
        updated.Invoke();
        uiManager.UpdateCashText();
        uiManager.UpdateBreadText();
    }

    public void BuyRice(){
        int amount = 1;
        float breadPrice = 15;
        playerLogic.AddCash(-breadPrice);
        playerLogic.AddRice(amount);
        updated.Invoke();
    }

    public void ConsumeBread(){
        int amount = 1;
        playerLogic.AddBread(-amount);
    }

    public void ConsumeRice(){
        int amount = 1;
        playerLogic.AddRice(-amount);
    }

    public void EarnCash(){
        float cash = 50;
        playerLogic.AddCash(cash);
    }

    public void InitializePlayerData(GameObject playerObject)
    {
        playerLogic = playerObject.GetComponent<PlayerLogic>();

        if (playerLogic != null)
        {
            playerLogic.SetName("GuestUser");
            playerLogic.SetCash(200.0f); // Set the cash to 100
            playerLogic.SetBread(15); // Set the bread count to 5
            playerLogic.SetRice(13); 
        }
        else
        {
            Debug.LogError("PlayerLogic script not found on the provided playerObject.");
        }
    }
}
