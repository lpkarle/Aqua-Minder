[System.Serializable]
public class User
{
    public string name;
    public string uid;
    public float drankWeight;
}

[System.Serializable]
public class UserArrayWrapper
{
    public User[] users;
}