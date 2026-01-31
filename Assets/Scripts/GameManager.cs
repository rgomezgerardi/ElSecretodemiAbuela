using UnityEngine;

public class GameManager: MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;

        if (instance != null)
        {
          DontDestroyOnLoad(gameObject);
        }
    }
}
