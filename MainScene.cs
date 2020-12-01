using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    public void OnARClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public void OnNormalClick()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
