using System;
using UnityEngine;

namespace Gameplay
{
    public class Dot : MonoBehaviour
    {
        float time = .4f;

        public LeanTweenType easetype; 
       
        void AnimateScale()
        {
            if (transform.GetSiblingIndex() % 2 == 0)
            {

                gameObject.LeanScale(Vector3.one * 0.7f, time).setOnComplete(() =>
                {
                    gameObject.LeanScale(Vector3.one, time).setEase(easetype).setOnComplete(AnimateScale);
                }).setEase(easetype);
            }
            else
            {
                gameObject.LeanScale(Vector3.one, time).setOnComplete(() =>
               {
                   gameObject.LeanScale(Vector3.one * 0.7f, time).setEase(easetype).setOnComplete(AnimateScale);
               });
            }
        }

        private void OnDisable()
        {
            LeanTween.cancel(gameObject);
        }
        private void OnEnable()
        {

            AnimateScale();
        }
    }
}
