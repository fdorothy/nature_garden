using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NG
{
    public class Plant : MonoBehaviour
    {
        protected SpriteRenderer sr;
        public Stage[] stages;
        public float probability;
        public int currentStage = 0;

        // Start is called before the first frame update
        void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            setStageProps();
            if (stages != null && stages.Length > 0)
            {
                Invoke("Morph", stages[0].duration);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void Morph()
        {
            currentStage += 1;
            setStageProps();
            if (currentStage < stages.Length)
            {
                Invoke("Morph", stages[currentStage].duration);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void setStageProps()
        {
            if (stages != null && currentStage < stages.Length)
            {
                sr.sprite = stages[currentStage].sprite;
            }
        }

        public Stage getStage()
        {
            if (stages != null && currentStage < stages.Length)
            {
                return stages[currentStage];
            }
            else
            {
                return null;
            }
        }
    }
}