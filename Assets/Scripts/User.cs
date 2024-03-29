using UnityEngine.Serialization;

[System.Serializable]
public class User
{
    public string name;
    public string uid;
    public float drankWeight = 0;
    public float bottleTareWeight;
    public float mostRecentRawWeight = 0;
    public float colorRed = 0;
    public float colorGreen = 0;
    public float colorBlue = 0;
}

[System.Serializable]
public class UserArrayWrapper
{
    public User[] users;
}