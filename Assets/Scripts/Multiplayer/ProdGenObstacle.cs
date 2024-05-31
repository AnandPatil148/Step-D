using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProdGenObstacle : MonoBehaviour
{

    public GameObject ObstaclePrefab;

    public List<GameObject> Obstacles;

    public Vector3 RandomPosition;
    public int NumberOfObstacles;
    public int minX, maxX;
    public int minZ, maxZ;


    // Start is called before the first frame update
    void Start()
    {
        InstantiateObstacles();
    }

    public void InstantiateObstacles()
    {
        for (int i = 0; i < NumberOfObstacles; i++)
        {
            
            while (true)
            {
                RandomPosition = new Vector3(Random.Range(minX, maxX), 1, Random.Range(minZ,maxZ));

                transform.position = RandomPosition + new Vector3(0,1,0);

                Ray ray = new(transform.position, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, 2))
                {
                    if (hit.collider.CompareTag("Obstacle"))
                    {
                        continue;
                    }
                    Instantiate(ObstaclePrefab, RandomPosition, Quaternion.identity);
                    break;
                }
            }

        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
