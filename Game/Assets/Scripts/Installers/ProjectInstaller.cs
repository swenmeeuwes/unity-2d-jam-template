using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private ProjectSettings _settings;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        #region Bindings

        Container
            .Bind<MonoBehaviourUtil>()
            .FromNewComponentOnNewGameObject()
            .AsSingle();

        Container
            .BindInterfacesAndSelfTo<ScreenFactory>()
            .AsSingle()
            .WithArguments(_settings.Prefabs.ScreenRoot);

        Container
            .Bind<ScreenContext>()
            .FromInstance(_settings.ScreenContext)
            .AsSingle();

        Container
            .BindInterfacesAndSelfTo<NavigationManager>()
            .AsSingle();

        Container
            .Bind<SceneLoader>()
            .ToSelf()
            .AsSingle();

        #endregion

        #region Signals

        Container.DeclareSignal<OpenScreenRequestSignal>();
        Container.DeclareSignal<CloseScreenRequestSignal>();
        Container.DeclareSignal<CloseAllScreensRequestSignal>();
        Container.DeclareSignal<ScreenStateChangedSignal>();

        #endregion

        #region Commands

        Container.DeclareSignal<LoadSceneSignal>();
        Container.Bind<LoadSceneCommand>().AsTransient();
        Container
            .BindSignal<LoadSceneSignal>()
            .ToMethod<LoadSceneCommand>(command => command.Execute)
            .FromResolve();

        #endregion
    }
}
