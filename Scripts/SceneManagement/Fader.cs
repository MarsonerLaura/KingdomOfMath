using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Coroutine currentActiveFade = null;

        #region Basic Unity Methods

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        #endregion

        #region Main Methods

        public Coroutine FadeOut(float time)
        {
            return Fade(time, 1f);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(time, 0);
        }

        public Coroutine Fade(float time, float alphaTarget)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(time,alphaTarget));
            return currentActiveFade;
        }
        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }


        private IEnumerator FadeRoutine(float time, float alphaTarget)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha,alphaTarget))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha,alphaTarget, Time.unscaledDeltaTime / time);
                yield return null; //wait for 1 frame
            }
        }

        #endregion
        
    }
}
