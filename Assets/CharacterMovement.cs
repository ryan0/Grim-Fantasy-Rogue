using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 0.5f; // Speed at which the character moves
    private bool isMoving;
    private Vector3 targetPosition;
    private TileData[,] tileDataMatrix; // Reference to your tile data matrix
    public int mapWidth = 50;
    public int mapHeight = 50;

    private void Start()
    {
        tileDataMatrix = TileMapGenerator.tileDataMatrix;
    }

    private void Update()
    {
        if (isMoving)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                isMoving = false;

            }
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        int xDir = 0;
        int yDir = 0;

        if (Input.GetKeyDown(KeyCode.UpArrow)) yDir = 1;
        if (Input.GetKeyDown(KeyCode.DownArrow)) yDir = -1;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) xDir = -1;
        if (Input.GetKeyDown(KeyCode.RightArrow)) xDir = 1;

        if (xDir != 0 || yDir != 0)
        {
            int x = Mathf.FloorToInt(transform.position.x) + xDir;
            int y = Mathf.FloorToInt(transform.position.y) + yDir;

            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight && tileDataMatrix[x, y].CanWalk)
            {
                targetPosition = new Vector3(x, y, transform.position.z);
                isMoving = true;
            }
        }
    }
}

