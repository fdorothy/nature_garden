using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NG
{
    public class Ground : MonoBehaviour
    {
        public Plant plant;
        public Vector3Int cell;
        public Vector3 min;
        public Vector3 max;

        public void Setup(Vector3Int cell, Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
            this.transform.position = 0.5f * (min + max);
            this.cell = cell;
        }

        public void placePlant(Plant plant)
        {
            plant.transform.position = randomLocation();
            plant.transform.parent = transform;
            this.plant = plant;
        }

        Vector3 randomLocation()
        {
            Vector3 vx = new Vector3(max.x - min.x, 0.0f, 0.0f);
            Vector3 vy = new Vector3(0.0f, max.y - min.y, 0.0f);
            float t0 = 0.5f;//Random.Range(0.0f, 1.0f);
            float t1 = 0.5f;//Random.Range(0.0f, 1.0f);
            return vx * t0 + vy * t1 + min;
        }

        public void StartHover()
        {
            this.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

        public void EndHover()
        {
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }

        public int PickPlant()
        {
            int value = 0;
            if (plant)
            {
                Stage stage = plant.getStage();
                if (stage != null)
                {
                    value += stage.value;
                }
                Destroy(plant.gameObject);
                plant = null;
            }
            return value;
        }
    }
}