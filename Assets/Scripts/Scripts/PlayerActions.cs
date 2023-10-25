using UnityEngine;
using UnityEngine.Events;

public class PlayerActions : MonoBehaviour
{
    [HideInInspector] public UnityEvent updated;
    public PlayerData player;

    public void BuyBread(int cost)
    {
        if (player.cash >= cost)
        {
            player.cash -= cost;
            player.bread++;
        }
    }

    public void BuyRice(int cost)
    {
        if (player.cash >= cost)
        {
            player.cash -= cost;
            player.bread++;
        }
    }

    public void EatBread()
    {
        if (player.bread > 0)
        {
            player.bread--;
        }
    }

    public void EatRice()
    {
        if (player.bread > 0)
        {
            player.bread--;
        }
    }
}
