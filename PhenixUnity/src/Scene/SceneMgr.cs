using System.Collections;
using UnityEngine;
using Phenix.Unity.Pattern;
using UnityEngine.SceneManagement;

namespace Phenix.Unity.Scene
{
    /// <summary>
    /// 场景管理
    /// </summary>
    [AddComponentMenu("Phenix/Scene/SceneMgr")]
    public class SceneMgr : Singleton<SceneMgr>
    {
        AsyncOperation _asyncOpt;

        [SerializeField]
        LoadSceneMode _mode = LoadSceneMode.Single;        

        public float SceneLoadProgress { get { return _asyncOpt.progress; } }

        public void LoadScene(int sceneIdx, bool allowSceneActivation = true, 
            System.Action<float> onLoading = null, System.Action onReady = null, System.Action onCompleted = null)
        {
            Resources.UnloadUnusedAssets();
            _asyncOpt = SceneManager.LoadSceneAsync(sceneIdx, _mode);
            _asyncOpt.allowSceneActivation = allowSceneActivation;
            if (onLoading != null)
            {
                StartCoroutine(Loading(onLoading, onReady));
            }
            if (onCompleted != null)
            {
                _asyncOpt.completed += ((x) => { onCompleted.Invoke(); OnEnterScene(sceneIdx); });
            }
        }

        public void LoadScene(string sceneName, bool allowSceneActivation = true,
            System.Action<float> onLoading = null, System.Action onReady = null, System.Action onCompleted = null)
        {
            Resources.UnloadUnusedAssets();
            _asyncOpt = SceneManager.LoadSceneAsync(sceneName, _mode);
            _asyncOpt.allowSceneActivation = allowSceneActivation;
            if (onLoading != null)
            {
                StartCoroutine(Loading(onLoading, onReady));
            }
            if (onCompleted != null)
            {
                _asyncOpt.completed += ((x) => { onCompleted.Invoke(); OnEnterScene(sceneName); });
            }
        }

        IEnumerator Loading(System.Action<float> onLoading, System.Action onReady)
        {
            // 无论allowSceneActivation何值，新scene资源加载时_asyncOpt.progress == 0.9f，这时如果allowSceneActivation为true，则开始激活并显示
            // 新场景，_asyncOpt.progress会变成1，触发_asyncOpt.completed事件，否则就一直停留为0.9，直到allowSceneActivation为true
            while (IsSceneReady() == false)
            {
                onLoading.Invoke(_asyncOpt.progress);
                yield return new WaitForEndOfFrame();
            }
            if (onReady != null)
            {
                onReady.Invoke();
            }
        }

        /// <summary>
        /// 激活load之后，资源已经全部载入的scene（即_asyncOpt.progress == 0.9f）
        /// </summary>
        public void ActivateSceneOnReady()
        {
            if (IsSceneReady())
            {
                _asyncOpt.allowSceneActivation = true;
            }
        }

        public bool IsSceneReady()
        {
            return _asyncOpt.progress >= 0.9f;
        }      

        public void UnloadScene(int sceneIdx)
        {
            SceneManager.UnloadSceneAsync(sceneIdx);
        }

        public void UnloadScene(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }

        void OnEnterScene(int sceneIdx)
        {

        }

        void OnEnterScene(string sceneName)
        {

        }
    }
}