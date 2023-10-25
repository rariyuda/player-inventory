using UnityEngine;
using Firebase.Database;
using System.Collections;
using System;
using TMPro;

public class DbManager : MonoBehaviour
{
    DatabaseReference dbReference;
    public PlayerData1 playerData;
    public TMP_Text NameText;
    public TMP_Text CashText;
    public TMP_Text BreadText;
    public TMP_Text RiceText;
    public string userID;

    void Start()
    {
        // Initialize Firebase Realtime Database
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        userID = UserAuthentication.userID;
    }

    #region Retrieve from Database
    // public void GetUserInfo(string userID)
    public void GetUserInfo()
    {
        if (string.IsNullOrEmpty(userID))
        {
            Start();
            if(string.IsNullOrEmpty(userID))
            {
                Debug.LogWarning("User ID is null or empty. Cannot retrieve user info.");
                return;
            }
        }

        StartCoroutine(GetName(userID,(string name) =>
        {
            NameText.text = "Name: " + name;
        }));

        StartCoroutine(GetCash(userID,(int cash) =>
        {
            CashText.text = "Cash: " + cash.ToString();
        }));

        StartCoroutine(GetItems(userID, (int bread, int rice) =>
        {
            BreadText.text = "Bread: " + bread.ToString();
            RiceText.text = "Rice: " + rice.ToString();
        }));
    }

    public IEnumerator GetName(string userID, Action<string> onCallback)
    {
        var userNameData = dbReference.Child("users").Child(userID).Child("Name").GetValueAsync();

        yield return new WaitUntil(() => userNameData.IsCompleted);

        if (userNameData != null && userNameData.Result.Exists)
        {
            DataSnapshot snapshot = userNameData.Result;
            onCallback.Invoke(snapshot.Value.ToString());
        }
        else
        {
            Debug.Log("User not found for ID: " + userID);
            onCallback.Invoke("User not found");
        }
    }

    public IEnumerator GetCash(string userID, Action<int> onCallback)
    {
        var userCashData = dbReference.Child("users").Child(userID).Child("Cash").GetValueAsync();

        yield return new WaitUntil(() => userCashData.IsCompleted);

        if (userCashData != null)
        {
            DataSnapshot snapshot = userCashData.Result;
            int cash = Convert.ToInt32(snapshot.Value);
            onCallback.Invoke(cash);
        }
    }

    public IEnumerator GetItems(string userID, Action<int, int> onCallback)
    {
        var breadData = dbReference.Child("users").Child(userID).Child("BreadCount").GetValueAsync();
        var riceData = dbReference.Child("users").Child(userID).Child("RiceCount").GetValueAsync();

        yield return new WaitUntil(() => breadData.IsCompleted && riceData.IsCompleted);

        if(breadData != null && riceData!=null)
        {
            DataSnapshot breadSnapshot = breadData.Result;
            DataSnapshot riceSnapshot = riceData.Result;

            int bread = Convert.ToInt32(breadSnapshot.Value);
            int rice = Convert.ToInt32(riceSnapshot.Value);

            onCallback.Invoke(bread,rice);
        }
    }
    #endregion 

    // public void UpdatePlayerData(string userId)
    public void UpdatePlayerData()
    {
        if (dbReference != null)
        {
            // Construct the path to the player's data in the database
            string path = "users/" + userID;
            string json = JsonUtility.ToJson(playerData);
            dbReference.Child(path).SetRawJsonValueAsync(json).ContinueWith(updateTask =>
            {
                if (updateTask.IsFaulted || updateTask.IsCanceled)
                {
                    Debug.LogError("Failed to update player data: " + updateTask.Exception);
                }
                else
                {
                    Debug.Log("Player data updated in the database.");
                }
            });
        }
        else
        {
            Debug.LogError("Firebase Realtime Database not initialized.");
        }
    }
}