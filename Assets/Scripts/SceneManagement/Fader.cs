using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvas;
        Coroutine currentActiveFade;
        // Start is called before the first frame update
        void Awake()
        {
            canvas = GetComponent<CanvasGroup>();
        }
        public void FadeOutImmidiate()
        {
            canvas.alpha = 1;
        }
        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);

        }
        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }
        public Coroutine Fade(float target, float time)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return currentActiveFade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while(!Mathf.Approximately(canvas.alpha, target))
            {
                canvas.alpha = Mathf.MoveTowards(canvas.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}
