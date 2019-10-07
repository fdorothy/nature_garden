using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NG
{
    public class Game : MonoBehaviour
    {
        public Plant[] plantPrefabs;
        public UnityEngine.Tilemaps.Tilemap tm;

        public List<NG.Ground> ground;

        protected NG.Ground hoverGround = null;

        protected Camera camera;

        public UnityEngine.UI.Text fertilizerText;

        public int fertilizer = 0;

        public float spawnRate = 1.0f;

        public bool isRaining = false;
        public bool isBeeing = false;
        public bool isCowing = false;

        public ParticleSystem rainSystem;
        public ParticleSystem beeSystem;
        public ParticleSystem cowSystem;

        public bool isPlaying = false;

        public Transform introTransform;
        public Transform winTransform;

        // Start is called before the first frame update
        void Start()
        {
            findGroundCells();
            camera = Camera.main;
            UpdateFertilizerText();
        }

        void Update()
        {
            Vector3 mv = camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = tm.WorldToCell(mv);
            NG.Ground oldHover = hoverGround;
            hoverGround = findGround(cell);
            if (oldHover != hoverGround)
            {
                if (oldHover != null)
                {
                    oldHover.EndHover();
                }
                if (hoverGround)
                {
                    hoverGround.StartHover();
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (hoverGround != null)
                {
                    PickPlants(hoverGround);
                }
            }

            if (fertilizer > 500) {
                WinGame();
            }
        }

        public void PickPlants(Ground g)
        {
            if (g)
            {
                fertilizer += g.PickPlant();
                UpdateFertilizerText();
            }
        }

        public void UpdateFertilizerText()
        {
            fertilizerText.text = fertilizer.ToString();
        }

        Ground findGround(Vector3Int v)
        {
            for (int i = 0; i < ground.Count; i++)
            {
                if (ground[i].cell.x == v.x && ground[i].cell.y == v.y)
                {
                    return ground[i];
                }
            }
            return null;
        }

        void findGroundCells()
        {
            ground = new List<Ground>();
            Vector3Int min = tm.origin;
            Vector3Int size = tm.size;
            for (int x = min.x; x < min.x + size.x; x++)
            {
                for (int y = min.y; y < min.y + size.y; y++)
                {
                    Vector3Int p = new Vector3Int(x, y, 0);
                    if (tm.GetSprite(p) != null)
                    {
                        Sprite s = tm.GetSprite(p);
                        addGround(p);
                    }
                }
            }
        }

        public void addGround(Vector3Int index)
        {
            Vector3 min = tm.CellToWorld(index);
            Vector3 size = tm.cellSize;
            GameObject go = new GameObject();
            NG.Ground g = go.AddComponent<NG.Ground>();
            g.Setup(index, min, min + size);
            ground.Add(g);
        }

        IEnumerator SpawnRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnRate);
                spawnPlants();
            }
        }

        public void spawnPlants()
        {
            foreach (Plant plantPrefab in plantPrefabs)
            {
                float t = Random.Range(0.0f, 1.0f);
                spawnPlant(plantPrefab, t);
            }
        }

        public void spawnPlant(Plant plantPrefab, float t)
        {
            if (t < plantPrefab.probability)
            {
                // spawn this plant somewhere
                int i = (int)Random.Range(0, ground.Count);
                Ground g = ground[i];
                if (g.plant == null)
                {
                    Plant p = (Plant)Instantiate(plantPrefab);
                    p.GetComponent<SpriteRenderer>().sortingOrder = 10;
                    g.placePlant(p);
                }
            }
        }

        public void StartRain()
        {
            if (!isRaining && fertilizer > 5)
            {
                isRaining = true;
                fertilizer -= 5;
                UpdateFertilizerText();
                StartCoroutine(RainRoutine());
            }
        }
        public void StartBees()
        {
            if (!isBeeing && fertilizer > 15)
            {
                isBeeing = true;
                fertilizer -= 15;
                UpdateFertilizerText();
                StartCoroutine(BeeRoutine());
            }
        }
        public void StartCows()
        {
            if (!isCowing && fertilizer > 25)
            {
                isCowing = true;
                fertilizer -= 25;
                UpdateFertilizerText();
                StartCoroutine(CowRoutine());
            }
        }

        public IEnumerator RainRoutine()
        {
            rainSystem.Play();
            spawnRate = 0.25f;
            yield return new WaitForSeconds(10.0f);
            spawnRate = 1.0f;
            rainSystem.Stop();
            isRaining = false;
        }
        public IEnumerator BeeRoutine()
        {
            beeSystem.Play();
            for (int i = 0; i < 10; i++)
            {
                spawnPlant(plantPrefabs[0], 0.0f);
                yield return new WaitForSeconds(1.0f);
            }
            beeSystem.Stop();
            isBeeing = false;
        }
        public IEnumerator CowRoutine()
        {
            cowSystem.Play();
            for (int i = 0; i < 10; i++)
            {
                spawnPlant(plantPrefabs[1], 0.0f);
                yield return new WaitForSeconds(1.0f);
            }
            cowSystem.Stop();
            isCowing = false;
        }

        public void StartGame()
        {
            if (!isPlaying)
            {
                introTransform.gameObject.SetActive(false);
                isPlaying = true;
                UpdateFertilizerText();
                StartCoroutine(SpawnRoutine());
            }
        }

        public void WinGame()
        {
            if (isPlaying)
            {
                winTransform.gameObject.SetActive(true);
                isPlaying = false;
                StopCoroutine(SpawnRoutine());
            }
        }

        public void RestartGame() {
            winTransform.gameObject.SetActive(false);
            introTransform.gameObject.SetActive(true);
            isPlaying = false;
            fertilizer = 0;
            UpdateFertilizerText();
        }
    }
}