using System;
using System.Linq;
using UnityEngine;
using Zenject;

public class ScreenFactory : IInitializable, IDisposable
{
    private Transform _screenRoot;

    #region Injected

    private readonly GameObject _screenRootPrefab;
    private readonly DiContainer _container;
    private readonly ScreenContext _screenContext;

    #endregion

    public ScreenFactory(GameObject screenRootPrefab, DiContainer container, ScreenContext screenContext)
    {
        _screenRootPrefab = screenRootPrefab;
        _container = container;
        _screenContext = screenContext;
    }

    public void Initialize()
    {
        // todo: not sure if this should be done in the 'ScreenFactory', maybe pass in the parent transform in the 'Create' method
        _screenRoot = GameObject.Instantiate(_screenRootPrefab).transform;
        GameObject.DontDestroyOnLoad(_screenRoot);
    }

    public void Dispose()
    {
        GameObject.Destroy(_screenRoot);
        _screenRoot = null;
    }

    public ScreenController Create(ScreenType screenType)
    {
        var screenPrefab = _screenContext.Map.First(item => item.Type == screenType).Transform;
        var screenController = _container.InstantiatePrefabForComponent<ScreenController>(screenPrefab, _screenRoot);
        if (!screenController)
        {
            Debug.LogWarningFormat("The screen prefab of type '{0}' has no screen controller attached to it! Fix the prefab used in the screen context asset.", screenType);
            return null;
        }

        return screenController;
    }
}