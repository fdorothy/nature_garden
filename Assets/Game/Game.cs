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

        // Start is called before the first frame update
        void Start()
        {
            findGroundCells();
            camera = Camera.main;
            UpdateFertilizerText();
            StartCoroutine(SpawnRoutine());
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
                yield return new WaitForSeconds(1.0f);
                spawnPlants();
            }
        }

        public void spawnPlants()
        {
            foreach (Plant plantPrefab in plantPrefabs)
            {
                float t = Random.Range(0.0f, 1.0f);
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
        }
    }
}