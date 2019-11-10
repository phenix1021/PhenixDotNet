using UnityEngine;
using UnityEngine.UI;
using Phenix.Unity.Extend;

namespace Phenix.Unity.UI
{
    /// <summary>
    /// 利用RenderTexture显示动态画面，类似电视机播放的效果
    /// </summary>
    [AddComponentMenu("Phenix/UI/TVShow")]
    public class TVShow : MonoBehaviour
    {
        public Canvas canvas;
        public UnityEngine.Camera tvCamera;     // 照着被播放对象的摄像机
        public RawImage tvScreen;               // 要显示画面的地方

        RenderTexture _renderTexture;

        private void OnEnable()
        {
            UpdateScreen();
        }

        private void OnDisable()
        {
            if (tvCamera)
            {
                tvCamera.enabled = false;
            }

            if (_renderTexture)
            {
                _renderTexture.Release();
                Destroy(_renderTexture);
                _renderTexture = null;
            }            
        }

        private void OnRectTransformDimensionsChange()
        {
            if (isActiveAndEnabled == false)
            {
                return;
            }

            UpdateScreen();
        }

        void UpdateScreen()
        {
            if (_renderTexture)
            {
                _renderTexture.Release();
                Destroy(_renderTexture);
            }

            Vector2 tvScreenSize = tvScreen.rectTransform.rect.size * canvas.scaleFactor;
            _renderTexture = new RenderTexture((int)tvScreenSize.x, (int)tvScreenSize.y, 16, RenderTextureFormat.ARGB32);
            if (QualitySettings.antiAliasing > 0 )
            {
                _renderTexture.antiAliasing = QualitySettings.antiAliasing;
            }

            tvCamera.enabled = true;
            tvCamera.targetTexture = _renderTexture;
            tvScreen.texture = _renderTexture;
        }
    }
}
