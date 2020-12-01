using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class SaveManger : MonoBehaviour
{
    public GameObject saveMenu;
    public GameObject comfirmMenu;

    public InputField FileNameInput;

    public Transform saveList;
    public GameObject savePrefab;

    private int saveCounter = 0;

    private bool isSaving;

    private Dictionary< string,int> saves ;

    private void Start()
    {
        RefreshSaves();
    }
    private void RefreshSaves()
    {
        saves = new Dictionary<string, int>();
        saveCounter = 0;
        while (PlayerPrefs.HasKey(saveCounter.ToString()))
        {
            string name = PlayerPrefs.GetString(saveCounter.ToString());
            saves.Add(name.Split('%')[0], saveCounter);
            saveCounter++;
        }
    }

    public void OnSaveMenuClick()
    {
        saveMenu.SetActive(true);
        RefreshSaveList();
    }
    public void onSaveClick()
    {
        saveMenu.SetActive(false);
        comfirmMenu.SetActive(true);
       
        isSaving = true;
    }
    public void onLoadClick()
    {
        saveMenu.SetActive(false);
        comfirmMenu.SetActive(true);
        isSaving = false;
    }
    public void OnClickCancel()
    {
        saveMenu.SetActive(false);
    }
    public void OnComfirmOK()
    {
        if (isSaving)
            Save();
        else
            load();

        comfirmMenu.SetActive(false);
    }
    public void OnComfirmDelete()
    {
        comfirmMenu.SetActive(false);
    }

    public void OnDelete()
    {
        string fileName = FileNameInput.text;
        int k;
        saves.TryGetValue(fileName, out k);

        if (!saves.ContainsValue(k))
        {
            Debug.Log("unable to find file");
            return;
        }

        PlayerPrefs.DeleteKey(k.ToString());
        saveCounter--;
        while (PlayerPrefs.HasKey((k + 1).ToString()))
        {
            string data = PlayerPrefs.GetString((k + 1).ToString());
            PlayerPrefs.SetString(k.ToString(), data);
            PlayerPrefs.DeleteKey((k + 1).ToString());
            k++;
        }
        RefreshSaves();
        saveMenu.SetActive(false);
    }
     
    private void Save()
    {
        string fileName = FileNameInput.text;
        bool isUsed = (saves.ContainsKey(fileName));

        if (string.IsNullOrEmpty(fileName))
            fileName = saveCounter.ToString();


        string savedata = fileName+'%';
        Block[,,] b = GameManager.Instance.blocks;


        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                for (int k = 0; k < 20; k++)
                {
                    Block currentBlock = b[i, j, k];
                    if (currentBlock == null)
                        continue;

                    savedata += i.ToString() + "|" +
                             j.ToString() + "|" +
                             k.ToString() + "|" +
                             ((int)currentBlock.color).ToString() + "%";
                }
            }
        }
        if (isUsed)
        {
            //overwrite
            int k;
            saves.TryGetValue(fileName, out k);
            PlayerPrefs.SetString(k.ToString(), savedata);
        }
        else
        {
            saves.Add(fileName, saveCounter);
            PlayerPrefs.SetString(saveCounter.ToString(), savedata);
           
                saveCounter++;
        }
       
        
    }
    private void load()
    {
        string fileName = FileNameInput.text;
        int k;
        saves.TryGetValue(fileName, out k);

        if (!saves.ContainsValue(k))
        { Debug.Log("unable to find file");
            return;
        }

       string save= PlayerPrefs.GetString(k.ToString());
        string[] blockData = save.Split('%');

        GameManager.Instance.RefreshGrid();

        for(int i=1;i<blockData.Length-1; i++)
        {
            string[] currentBlock = blockData[i].Split('|');
            int x = int.Parse(currentBlock[0]);
            int y = int.Parse(currentBlock[1]);
            int z = int.Parse(currentBlock[2]);

            int c = int.Parse(currentBlock[3]);

            Block b = new Block() { color = (BlockColor)c };
            GameManager.Instance.CreateBlock(x, y, z, b);
        }
    }

    private void RefreshSaveList()
    {
        foreach (Transform t in saveList)
            Destroy(t.gameObject);
        for (int i = 0; i < saveCounter; i++)
        {
            GameObject go = Instantiate(savePrefab) as GameObject;
            go.transform.SetParent(saveList);

            string[] saveData = PlayerPrefs.GetString(i.ToString()).Split('%');
            go.GetComponentInChildren<Text>().text = saveData[0];

            string s = saveData[0];
            go.GetComponent<Button>().onClick.AddListener(() => OnSaveClick(s));
        }

    }
    private void OnSaveClick(string name)
    {
        FileNameInput.text = name;
    }
}
