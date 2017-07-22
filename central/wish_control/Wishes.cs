using UnityEngine;

public class Wishes : MonoBehaviour
{

    private static Wishes instance;

    public static Wishes Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("wishes got destroyeed\n");
            Destroy(gameObject);
        }
        Instance = this;
    }
}