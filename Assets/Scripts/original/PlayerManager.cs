using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerData player;
    public PlayerActions playerActions;
    public DatabaseManager databaseManager;
    public static string eventStatus = "NoAccount";   // NoAccount, Guest, SignIn

    public void UpdatePlayerName(string newName)
    {
        player.name = newName;
        playerActions.updated.Invoke();
    }

    public void UpdatePlayerCash(int newCash)
    {
        player.cash = newCash;
        playerActions.updated.Invoke();
    }

    public void BuyBread()
    {
        int breadCost = 10;
        playerActions.BuyBread(breadCost);
        playerActions.updated.Invoke();
        databaseManager.GetUserInfo();
        
    }

    public void BuyRice()
    {
        int riceCost = 15;
        playerActions.BuyRice(riceCost);
        playerActions.updated.Invoke();
        databaseManager.GetUserInfo();
    }

    public void EatBread()
    {
        playerActions.EatBread();
        playerActions.updated.Invoke();
        databaseManager.GetUserInfo();
    }

    public void EatRice()
    {
        playerActions.EatRice();
        playerActions.updated.Invoke();
        databaseManager.GetUserInfo();
    }

}
