using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private static SaveManager m_instance;
    public static SaveManager Instance => m_instance;
    
    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");
    
    public SaveData Data { get; private set; }
    
    // --------------------------------------------
    //               INITIALIZATION
    // --------------------------------------------
    
    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Debug.LogWarning("Multiple SaveManager instances in scene!");
            Destroy(gameObject);
            return;
        }

        m_instance = this;
    }

    private void Start()
    {
        Load();
    }
    
    // --------------------------------------------
    //                  FUNCTIONS
    // --------------------------------------------

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            Data = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Data = new SaveData();
            Save();
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(SavePath, json);
    }
    
    public static void DeleteSave()
    {
        
    }
}