/*
    - DatabaseManager manages the interaction with the Firebase Realtime Database.
    - It includes methods for creating a user, updating the database, 
        retrieving user information, and modifying user data.
*/

using Firebase.Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseManager : MonoBehaviour
{
    public InputField NameInput;
    public InputField EmailInput;
    public InputField PassInput;
    public TMP_Text NameText;
    public TMP_Text CashText;
    public TMP_Text BreadText;
    public TMP_Text RiceText;
    public string userID;

    private DatabaseReference dbReference;

    

    private void Start()
    {
        dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        userID = UserAuthentication.userID;
    }

    #region Retrieve from Database
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

        Debug.Log("Starting GetUserInfo for user ID: " + userID);

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
        var userNameData = dbReference.Child("users").Child(userID).Child("name").GetValueAsync();

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
        var userCashData = dbReference.Child("users").Child(userID).Child("cash").GetValueAsync();

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
        var breadData = dbReference.Child("users").Child(userID).Child("bread").GetValueAsync();
        var riceData = dbReference.Child("users").Child(userID).Child("rice").GetValueAsync();

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


    #region Database Interaction
    public void UpdateDatabase(PlayerData user)
    {
        if (dbReference == null)
        {
            Debug.LogError("Database reference is null.");
            return;
        }

        string json = JsonUtility.ToJson(user);

        // Update the database with the updated user data
        dbReference.Child("users").Child(userID).SetRawJsonValueAsync(json);
    }

    public void UpdateUserName()
    {

        string newUserName = NameInput.text;
        dbReference.Child("users").Child(userID).Child("name").SetValueAsync(newUserName);
        
    }

    public void EarnCash()
    {
        StartCoroutine(GetCash(userID, (int cash) =>
        {
            int addCash = cash + 50;  // Calculate the new cash amount
            CashText.text = "Cash: " + addCash.ToString();

            // Update the cash in the database
            dbReference.Child("users").Child(userID).Child("cash").SetValueAsync(addCash);
        }));
    }

    public void EatBread()
    {
        StartCoroutine(GetItems(userID, (int breadCount,int riceCount) =>
        {
            if (breadCount > 0)
            {
                breadCount--;
                BreadText.text = "Bread: " + breadCount.ToString();

                // Update the bread count in the database
                dbReference.Child("users").Child(userID).Child("bread").SetValueAsync(breadCount);
            }
            else
            {
                Debug.LogWarning("No bread to eat.");
            }
        }));
    }

    public void BuyBread()
    {
        StartCoroutine(GetCash(userID, (int cash) =>
        {
            int breadCost = 10; // Adjust the cost as needed

            if (cash >= breadCost)
            {
                cash -= breadCost;
                CashText.text = "Cash: " + cash.ToString();

                StartCoroutine(GetItems(userID, (int bread,int rice) =>
                {
                    bread++;
                    BreadText.text = "Bread: " + bread.ToString();

                    // Update the cash and bread count in the database
                    dbReference.Child("users").Child(userID).Child("cash").SetValueAsync(cash);
                    dbReference.Child("users").Child(userID).Child("bread").SetValueAsync(bread);
                }));
            }
            else
            {
                Debug.LogWarning("Not enough cash to buy bread.");
            }
        }));
    }

    public void EatRice()
    {
        StartCoroutine(GetItems(userID,(int breadCount,int riceCount) =>
        {
            if (riceCount > 0)
            {
                riceCount--;
                RiceText.text = "Rice: " + riceCount.ToString();

                // Update the rice count in the database
                dbReference.Child("users").Child(userID).Child("rice").SetValueAsync(riceCount);
            }
            else
            {
                Debug.LogWarning("No rice to eat.");
            }
        }));
    }

    public void BuyRice()
    {
        StartCoroutine(GetCash(userID, (int cash) =>
        {
            int riceCost = 15; // Adjust the cost as needed

            if (cash >= riceCost)
            {
                cash -= riceCost;
                CashText.text = "Cash: " + cash.ToString();

                StartCoroutine(GetItems(userID, (int breadCount,int riceCount) =>
                {
                    riceCount++;
                    RiceText.text = "Rice: " + riceCount.ToString();

                    // Update the cash and rice count in the database
                    dbReference.Child("users").Child(userID).Child("cash").SetValueAsync(cash);
                    dbReference.Child("users").Child(userID).Child("rice").SetValueAsync(riceCount);
                }));
            }
            else
            {
                Debug.LogWarning("Not enough cash to buy rice.");
            }
        }));
    }
    #endregion
   
}

    