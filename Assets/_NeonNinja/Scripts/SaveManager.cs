using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SaveHighScore(int score)
    {
        int currentHigh = GetHighScore();
        if (score > currentHigh)
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
            Debug.Log("Nuovo Record Salvato: " + score);
        }
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }
}