using UnityEngine;

public class change_scenes : MonoBehaviour
{
    public string test;

    public void ChangeScene()
    {


        if (string.IsNullOrEmpty(test))
        {
            return;
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(test);
        }
    }
}