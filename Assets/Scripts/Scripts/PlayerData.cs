
[System.Serializable]
public class PlayerData
{
    public string name;
    public int cash;
    public int bread;
    public int rice;

    public PlayerData(string name, int cash, int bread, int rice)
    {
        this.name = name;
        this.cash = cash;
        this.bread = bread;
        this.rice = rice;
    }
}