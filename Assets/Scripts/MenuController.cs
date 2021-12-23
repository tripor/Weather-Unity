using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private List<Button> ChangeSceneButtons;

    public void ChangeScene(int sceneIdentifier)
    {
        SetInteractableChangeSceneButtons(false);
        StartCoroutine(LoadSceneAsync(sceneIdentifier));
    }

    private IEnumerator LoadSceneAsync(int sceneIdentifier)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIdentifier, LoadSceneMode.Single);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        SetInteractableChangeSceneButtons(true);
    }

    private void SetInteractableChangeSceneButtons(bool status)
    {
        foreach (var button in ChangeSceneButtons)
        {
            button.interactable = status;
        }
    }

    public void ExitApplication()
    {
        Application.Quit(0);
    }
}
