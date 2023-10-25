using UnityEngine;


public class PlayerLogic : MonoBehaviour
{
    public PlayerData1 playerData;

    public void SetName(string name){playerData.Name = name;}

    public void SetCash(float cash){playerData.Cash = cash;}

    public void SetBread(int bread){playerData.BreadCount = bread;}

    public void SetRice(int rice){playerData.RiceCount = rice;}
    
    public string GetName(){return playerData.Name;}

    public float GetCash(){return playerData.Cash;}

    public int GetBread(){return playerData.BreadCount;}

    public int GetRice(){return playerData.RiceCount;}

    public void AddCash(float income){
        playerData.Cash += income;}

    public void AddBread(int amount){
        playerData.BreadCount += amount;}
    
    public void AddRice(int amount){
        playerData.RiceCount += amount;}
}