using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LoadSceneCommand : ICommand<LoadSceneSignal>
{
    #region Injected

    private SignalBus _signalBus;
    private SceneLoader _sceneLoader;
    private MonoBehaviourUtil _monoBehaviourUtil;

    #endregion

    public LoadSceneCommand(SignalBus signalBus, SceneLoader sceneLoader, MonoBehaviourUtil monoBehaviourUtil)
    {
        _signalBus = signalBus;
        _sceneLoader = sceneLoader;
        _monoBehaviourUtil = monoBehaviourUtil;
    }

    public void Execute(LoadSceneSignal signal)
    {
        _signalBus.Fire(new OpenScreenRequestSignal
        {
            Type = ScreenType.Loading
        });

        var loadOperation = _sceneLoader.LoadAsync(signal.Scene);
        _monoBehaviourUtil.StartCoroutine(HandleAsyncLoading(loadOperation));
    }    

    private IEnumerator HandleAsyncLoading(AsyncOperation operation)
    {
        // Yield to prevent a flashing loading screen
        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(() => operation.isDone);

        // todo: strong reference to loading screen
        _signalBus.Fire(new CloseAllScreensRequestSignal());
    }
}
