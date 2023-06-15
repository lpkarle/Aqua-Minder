using UnityEngine;

public static class PlayerPrefsManager
{
    private const string UserKey = "AquaMinderUsers";

    static PlayerPrefsManager()
    {
        DeleteAllPlayerPrefs();

        if (PlayerPrefs.HasKey(UserKey))
            return;

        var dummyUsers = new User[]
        {
            new User { name = "Chip 1", uid = "ab8a90b9" },
            new User { name = "Fabio Chip", uid = "6a575e9" },
            new User { name = "Fabio Card", uid = "10d57fa2" }
        };

        SetUserArray(dummyUsers);
    }

    public static User GetUserByUid(string uid)
    {
        var users = GetUserArray();

        foreach (User user in users)
        {
            if (user.uid.Equals(uid))
                return user;
        }

        return null;
    }

    public static void SetUser(User user)
    {
        var users = GetUserArray();

        for (int i = 0; i < users.Length; i++)
        {
            if (users[i].uid.Equals(user.uid))
                users[i] = user;
        }

        SetUserArray(users);
    }

    public static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    private static void SetUserArray(User[] users)
    {
        string json = JsonUtility.ToJson(new UserArrayWrapper { users = users });

        PlayerPrefs.SetString(UserKey, json);
    }

    private static User[] GetUserArray()
    {
        string json = PlayerPrefs.GetString(UserKey);

        UserArrayWrapper wrapper = JsonUtility.FromJson<UserArrayWrapper>(json);

        return wrapper.users;
    }
}
