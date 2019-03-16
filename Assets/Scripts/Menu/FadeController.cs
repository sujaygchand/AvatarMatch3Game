using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{

    [SerializeField] Image s_BackgroundImage;
    [SerializeField] Animator s_Animator;

    private void Awake()
    {
        gameObject.SetActive(true);   
    }


    public void SceneTransition(string loadScene, string unloadScene)
    {
        StartCoroutine(SceneTransition_Cor(loadScene, unloadScene));
    }

    private IEnumerator SceneTransition_Cor(string loadScene, string unloadScene)
    {
        s_Animator.SetBool("DoFade", true);
        yield return new WaitUntil(() => s_BackgroundImage.color.a == 1);

        SceneManager.LoadScene(loadScene);
        //SceneManager.UnloadSceneAsync(unloadScene);
    }

}
