﻿using IllusionInjector.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPA.Loader;

namespace IllusionInjector
{
    public class PluginComponent : MonoBehaviour
    {
        private CompositeBSPlugin bsPlugins;
        private CompositeIPAPlugin ipaPlugins;
        private bool quitting = false;

        public static PluginComponent Create()
        {
            Application.logMessageReceived += delegate (string condition, string stackTrace, LogType type)
            {
                var level = UnityLogInterceptor.LogTypeToLevel(type);
                UnityLogInterceptor.Unitylogger.Log(level, $"{condition.Trim()}");
                UnityLogInterceptor.Unitylogger.Log(level, $"{stackTrace.Trim()}");
            };

            return new GameObject("IPA_PluginManager").AddComponent<PluginComponent>();
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            bsPlugins = new CompositeBSPlugin(PluginManager.BSPlugins);
            ipaPlugins = new CompositeIPAPlugin(PluginManager.Plugins);

            // this has no relevance since there is a new mod updater system
            //gameObject.AddComponent<ModUpdater>(); // AFTER plugins are loaded, but before most things
            gameObject.AddComponent<Updating.ModsaberML.Updater>();

            bsPlugins.OnApplicationStart();
            ipaPlugins.OnApplicationStart();
            
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void Update()
        {
            bsPlugins.OnUpdate();
            ipaPlugins.OnUpdate();
        }

        void LateUpdate()
        {
            bsPlugins.OnLateUpdate();
            ipaPlugins.OnLateUpdate();
        }

        void FixedUpdate()
        {
            bsPlugins.OnFixedUpdate();
            ipaPlugins.OnFixedUpdate();
        }

        void OnDestroy()
        {
            if (!quitting)
            {
                Create();
            }
        }
        
        void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            
            bsPlugins.OnApplicationQuit();
            ipaPlugins.OnApplicationQuit();

            quitting = true;
        }

        void OnLevelWasLoaded(int level)
        {
            ipaPlugins.OnLevelWasLoaded(level);
        }

        public void OnLevelWasInitialized(int level)
        {
            ipaPlugins.OnLevelWasInitialized(level);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            bsPlugins.OnSceneLoaded(scene, sceneMode);
        }
        
        private void OnSceneUnloaded(Scene scene) {
            bsPlugins.OnSceneUnloaded(scene);
        }

        private void OnActiveSceneChanged(Scene prevScene, Scene nextScene) {
            bsPlugins.OnActiveSceneChanged(prevScene, nextScene);
        }

    }
}