using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneLoader
{
    #region Injected

    private readonly MonoBehaviourUtil _monoBehaviourUtil;

    #endregion

    public AsyncOperation CurrentAsyncOperation { get; private set; }

    private Scenes _loadingScreenScene = Scenes.LoadingScreen;

    public SceneLoader(MonoBehaviourUtil monoBehaviourUtil)
    {
        _monoBehaviourUtil = monoBehaviourUtil;
    }

    public AsyncOperation LoadAsync(Scenes scene)
    {
        CurrentAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
        _monoBehaviourUtil.StartCoroutine(HandleAsyncLoading(CurrentAsyncOperation));

        return CurrentAsyncOperation;
    }

    private IEnumerator HandleAsyncLoading(AsyncOperation operation)
    {        
        operation.allowSceneActivation = false;

        // To prevent a flashing loading screen
        yield return new WaitForSeconds(0.2f);

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                // When 'allowSceneActivation' is false it will not load beyond 0.9f (even is the the operation is done)
                operation.allowSceneActivation = true;
                CurrentAsyncOperation = null;
            }

            yield return null;
        }
    }
}
