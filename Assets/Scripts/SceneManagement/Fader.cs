using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvas;
        // Start is called before the first frame update
        void Awake()
        {
            canvas = GetComponent<CanvasGroup>();
        }
        public void FadeOutImmidiate()
        {
            canvas.alpha = 1;
        }
        IEnumerator FadeOutIn()
        {
            yield return FadeOut(3f);
            yield return FadeIn(1f);
        }
        public IEnumerator FadeOut(float time) 
        {
            while(canvas.alpha < 1)
            {
                canvas.alpha += Time.deltaTime / time;
                yield return null;
            }
        }
        public IEnumerator FadeIn(float time)
        {
            while (canvas.alpha > 0)
            {
                canvas.alpha -= Time.deltaTime / time;
                yield return null;
            }
        }
    }
}
