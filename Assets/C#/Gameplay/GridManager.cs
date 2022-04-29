using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System;
using Gameplay.Balls;

namespace Gameplay
{
    public class GridManager : MonoBehaviour
    {
        [Header("Grid")]
        public int columns;
        public int rows;
        public int MoveAfter = 5;
        public GameObject bubblePrefab, squarePrefab;
        public Transform group;
        Vector3 initialPos;
        [Range(0, 1.5f)]
        public float gapx, gapy = 0.5369999f;

        [Header("Game")]
        public GameObject youWin;
        //public GameObject youLose;
        public string GridData = "Assets/Data/level1.data";

        [Header("Fall down controller")]
        public GameObject[,] grid;
        public static GridManager instance;
        public string level;
        public int countMoves;

        private void Awake()
        {
            instance = this;
        }

        public float SPEED = 1f;
        public float MoveByValue = .5f;
        public int currentLevel;

        GameObject squaresParent;
        private void Start()
        {
            squaresParent = new GameObject();
            currentLevel = LocalPlayer.CurrentLevel;
            //gap = bubble.transform.localScale.x ;
            grid = new GameObject[columns, rows];
            initialPos = group.position;
            level = LoadLevel();
            ArragementBubble(level);
            countMoves = MoveAfter;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;


            Gizmos.color = Color.yellow;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    Vector3 position = new Vector3(c * gapx, -r * gapy, 0f) + group.position;
                    if (r % 2 != 0)
                    {
                        position.x += 0.5f * gapx;
                    }

                    Gizmos.DrawWireSphere(position, 0.1f);
                }

            }
        }

        //here the kind no 6 is used for the empty space on the grid
        //kind is the ball no
        private void ArragementBubble(string level)
        {
            int levelpos = 0;
            int index = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    Vector3 position = new Vector3(c * gapx, -r * gapy, 0f) + initialPos;
                    index++;
                    if (r % 2 != 0)
                        position.x += 0.5f * gapx;

                    int kind = 0;

                    if (level.Length <= levelpos)
                    {
                        kind = 6;
                        Create(position, kind, r, c, index);
                        continue;
                    }
                    if (level[levelpos] == '\n' || level[levelpos] == '\r' || level[levelpos] == ' ')
                    {
                        continue;
                    }


                    if (level[levelpos] == '0')
                    {
                        kind = 0;
                    }

                    if (level[levelpos] == '1')
                        kind = 1;

                    if (level[levelpos] == '2')
                        kind = 2;

                    if (level[levelpos] == '3')
                        kind = 3;

                    if (level[levelpos] == '4')
                        kind = 4;

                    if (level[levelpos] == '5')
                        kind = 5;

                    if (level[levelpos] == '6')
                        kind = 6;
                    Create(position, kind, r, c, index);
                    levelpos++;
                }
            }


        }
        public void Create(Vector2 position, int ballNumber, int row, int column, int index)
        {
            Vector3 snappedPosition = position;

            GameObject bubbleClone = Instantiate(bubblePrefab);
            GameObject squre = Instantiate(squarePrefab);
            squre.transform.position = snappedPosition;
            squre.transform.parent = squaresParent.transform;
            squre.name = "Square" + index;
            squre.GetComponent<Square>().ball = bubbleClone.GetComponent<Ball>();
            bubbleClone.GetComponent<Ball>().square = squre.GetComponent<Square>();
            bubbleClone.name = "Bubble" + index;
            bubbleClone.transform.parent = group;
            bubbleClone.transform.position = snappedPosition;
            try
            {
                grid[column, row] = bubbleClone;
            }
            catch (IndexOutOfRangeException)
            {
                Destroy(bubbleClone);
                return;
            }

            //This addeds offset colliders
            //
            if (row % 2 == 0 && column == columns - 1)
            {
                BoxCollider2D bc;
                bubbleClone.AddComponent<BoxCollider2D>();
                bc = bubbleClone.GetComponents<BoxCollider2D>()[0];
                bc.size = new Vector2(0.65f, 0.65f);
                bc.offset = new Vector2(0.3350024f, 0f);
            }
            else if (row % 2 != 0 && column == 0)
            {
                BoxCollider2D bc;
                bubbleClone.AddComponent<BoxCollider2D>();
                bc = bubbleClone.GetComponents<BoxCollider2D>()[0];
                bc.size = new Vector2(.65f, .65f);
                bc.offset = new Vector2(-0.3350024f, 0f);
            }

            Ball ball = bubbleClone.GetComponent<Ball>();
            ball.SetGridManager(this);
            if (ball != null)
            {
                //gridMember.parent = gameObject;
                ball.row = row;
                ball.column = column;
                ball.ballNumber = ballNumber;
                SpriteRenderer spriteRenderer = bubbleClone.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                    spriteRenderer.sprite = ball.sp[ball.ballNumber];

            }
            if (ballNumber == 6)
            {
                bubbleClone.gameObject.SetActive(false);
            }
            else
                bubbleClone.SetActive(true);

        }

        bool allow = true;

        public void Seek(int column, int row, int kind)//to popthe balls
        {
            StartCoroutine(seek(column, row, kind));
        }
        IEnumerator seek(int column, int row, int kind)//to popthe balls
        {


            int[] pair = new int[2] { column, row };

            bool[,] visited = new bool[columns, rows];

            visited[column, row] = true;

            int[] deltax = { -1, 0, -1, 0, -1, 1 };
            int[] deltaxprime = { 1, 0, 1, 0, -1, 1 };
            int[] deltay = { -1, -1, 1, 1, 0, 0 };


            Queue<int[]> queue = new Queue<int[]>();
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            queue.Enqueue(pair);

            int count = 0;
            while (queue.Count != 0)
            {
                int[] top = queue.Dequeue();
                GameObject gtop = grid[top[0], top[1]];
                if (gtop.activeSelf)
                {
                    objectQueue.Enqueue(gtop);
                }
                count += 1;
                for (int i = 0; i < 6; i++)
                {
                    int[] neighbor = new int[2];
                    if (top[1] % 2 == 0)
                        neighbor[0] = top[0] + deltax[i];
                    else
                        neighbor[0] = top[0] + deltaxprime[i];

                    neighbor[1] = top[1] + deltay[i];
                    try
                    {
                        GameObject g = grid[neighbor[0], neighbor[1]];
                        if (g.activeSelf)
                        {
                            Ball gridMember = g.GetComponent<Ball>();
                            if (gridMember != null && gridMember.ballNumber == kind)
                            {
                                if (!visited[neighbor[0], neighbor[1]])
                                {
                                    visited[neighbor[0], neighbor[1]] = true;
                                    queue.Enqueue(neighbor);
                                }
                            }
                        }
                    }
                    catch (IndexOutOfRangeException) { }
                }
            }

            if (count >= 3)
            {
                while (objectQueue.Count != 0)
                {
                    yield return new WaitForSeconds(0.05f);
                    GameObject g = objectQueue.Dequeue();

                    Ball gm = g.GetComponent<Ball>();
                    SpriteRenderer sp = g.GetComponent<SpriteRenderer>();
                    if (gm != null)
                    {
                        //grid[gm.column, gm.row] = null;
                        gm.State("Pop");
                        sp.sprite = gm.sp[6];
                        gm.ballNumber = 6;
                        gm.gameObject.SetActive(false);
                        if (allow)
                        {
                            countMoves += 2;

                            allow = false;
                        }

                    }

                }

                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                    audioSource.Play();
            }

            CheckCeiling(0);
        }


        public void CheckCeiling(int ceiling)//to fall the bubbles from grid
        {

            bool[,] visited = new bool[columns, rows];

            Queue<int[]> queue = new Queue<int[]>();

            int[] deltax = { -1, 0, -1, 0, -1, 1 };
            int[] deltaxprime = { 1, 0, 1, 0, -1, 1 };
            int[] deltay = { -1, -1, 1, 1, 0, 0 };

            for (int i = 0; i < columns; i++)
            {
                int[] pair = new int[2] { i, ceiling };
                if (grid[i, ceiling].activeSelf)
                {
                    visited[i, ceiling] = true;
                    queue.Enqueue(pair);
                }
            }

            int count = 0;
            while (queue.Count != 0)
            {
                int[] top = queue.Dequeue();
                count += 1;
                for (int i = 0; i < 6; i++)
                {
                    int[] neighbor = new int[2];
                    if (top[1] % 2 == 0)
                    {
                        neighbor[0] = top[0] + deltax[i];
                    }
                    else
                    {
                        neighbor[0] = top[0] + deltaxprime[i];
                    }
                    neighbor[1] = top[1] + deltay[i];
                    try
                    {
                        GameObject g = grid[neighbor[0], neighbor[1]];
                        if (g.activeSelf)
                        {
                            if (!visited[neighbor[0], neighbor[1]])
                            {
                                visited[neighbor[0], neighbor[1]] = true;
                                queue.Enqueue(neighbor);
                            }
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }
                }
            }

            if (count == 0)
            {
                if (youWin != null)
                {
                    youWin.SetActive(true);
                    //this will avoid path creation after the youwin popup enable
#if UNITY_ANDROID
#endif
                }
            }

            bool allow = true;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (grid[c, r].activeSelf && !visited[c, r])
                    {
                        GameObject g = grid[c, r];
                        Ball gm = g.GetComponent<Ball>();
                        SpriteRenderer sp = g.GetComponent<SpriteRenderer>();
                        if (gm != null)
                        {
                            //grid[gm.column, gm.row] = null;
                            gm.State("Fall");
                            sp.sprite = gm.sp[6];
                            gm.ballNumber = 6;
                            gm.gameObject.SetActive(false);

                            if (allow)
                            {
                                countMoves += 2;
                                allow = false;
                            }

                        }
                    }
                }
            }
        }


        public GameObject ActivateBall(int row, int column, int ballNumber)
        {
            grid[column, row].GetComponent<Ball>().ballNumber = ballNumber;
            grid[column, row].GetComponent<Ball>().GetComponent<Collider2D>().enabled = true;
            grid[column, row].GetComponent<SpriteRenderer>().sprite = grid[column, row].GetComponent<Ball>().sp[ballNumber];
            grid[column, row].SetActive(true);
            grid[column, row].GetComponent<Ball>().ResetPosition();
            countMoves--;

            return grid[column, row].gameObject;
        }
        public void DeactivateBall(int row, int column)
        {
            grid[column, row].GetComponent<Ball>().ResetPosition();
            grid[column, row].SetActive(false);
            countMoves++;
            Seek(row, column, -1);

        }
        #region pirvate function 
        private string LoadLevel()
        {

            string bubblePlaceMent = "12315423551"
                                    + "12154233551"
                                    + "42154233551"
                                    + "12154233351"
                                    + "12154233553"
                                    + "12154233153"
                                    + "12154233153"
                                    + "12154233153"
                                    + "12154233551";
            return bubblePlaceMent;
        }

        #endregion



    }
}