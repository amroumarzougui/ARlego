using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScene : MonoBehaviour
{
    public Material[] mats;
    public GameObject previewCountair;
    public Text previewName;
    public GameObject blockPrefab;
    private void Start()
    {
        buildPreview(0);
    }

    private void Update()
    {
        RotatePreview();   
    }

    private void buildPreview(int key)
    {
        if (!PlayerPrefs.HasKey(key.ToString()))
        {
            return;
        }
        string data = PlayerPrefs.GetString(key.ToString());


       
        string[] blockData = data.Split('%');


        previewName.text = blockData[0];
        for (int i = 1; i < blockData.Length - 1; i++)
        {
            string[] currentBlock = blockData[i].Split('|');
            int x = int.Parse(currentBlock[0]);
            int y = int.Parse(currentBlock[1]);
            int z = int.Parse(currentBlock[2]);

            int c = int.Parse(currentBlock[3]);

            Block b = new Block() { color = (BlockColor)c };
            GameObject go = Instantiate(blockPrefab) as GameObject;
            go.transform.SetParent(previewCountair.transform);
            go.transform.position = new Vector3(x, y, z);
            go.GetComponent<Renderer>().material = mats[(int)c];
        }


    }
    private void RotatePreview()
    {
        previewCountair.transform.RotateAround(new Vector3(5,0,5),Vector3.up,  35 * Time.deltaTime);
    }
    public void OnplayClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
