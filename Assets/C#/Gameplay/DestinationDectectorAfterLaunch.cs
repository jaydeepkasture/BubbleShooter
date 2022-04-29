using Gameplay.Balls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{/// <summary>
/// This Class to detect all the bubbles that needs to pop after collision
/// </summary>
    public class DestinationDectectorAfterLaunch : MonoBehaviour
    {

        //here kind no 6 is for deactivated bubble
        //this is the key func to calc the loction where the bubble going to be just after the launch
        public static List<GameObject> CalculateIndex(GameObject target)
        {
            List<GameObject> balls = new List<GameObject>();
            List<int[]> indexs = new List<int[]>();
            int x, y, rows, columns, deactivatedBallNumber = 6;
            if (target.CompareTag("bubble"))
            {
                rows = GridManager.instance.GetComponent<GridManager>().rows;
                columns = GridManager.instance.GetComponent<GridManager>().columns;
                x = target.GetComponent<Ball>().column;
                y = target.GetComponent<Ball>().row;
            }
            else
            {
                Debug.LogError("raycast Tagger null");
                return null;
            }
            //Debug.Log($"name {GridManager.instance.grid[x, y].gameObject.name}  r {y} c {x}");

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {

                    if (x == 0 && y == 0)
                    {

                        print("$1");

                        /*
                         |$0OOOOOO|//IF $ is the target bubble the zero is the empty space then the launch bubble will 
                         |OOOOOOOO|
                         |OOOOOOOO|
                         |OOOOOOOO|
                         |OOOOOOOO|
                         |OOOOOOOO|

                         */
                        if (i == 1 && j == 0 || i == 0 && j == 1)
                        {

                            if (GridManager.instance.grid[x + i, y + j].GetComponent<Ball>().ballNumber == deactivatedBallNumber)
                            {
                                int[] index = { x + i, y + j };
                                indexs.Add(index);
                                //Debug.Log($"before collision x {x + i}  y {y + j}");
                                balls.Add(GridManager.instance.grid[x + i, y + j].gameObject);
                            }
                        }

                    }
                    else if (x == columns - 1 && y == 0)
                    {
                        print("$2");

                        if (i == -1 && j == 0 || i == 0 && j == 1 || i == -1 && j == 1)
                        {

                            if (GridManager.instance.grid[x + i, y + j].GetComponent<Ball>().ballNumber == deactivatedBallNumber)
                            {
                                int[] index = { x + i, y + j };
                                indexs.Add(index);
                                balls.Add(GridManager.instance.grid[x + i, y + j].gameObject);
                            }
                        }

                    }

                    else if (x < columns && x > 0 && y == 0)

                    {
                        print("$3");

                        if (i == 1 && j == -1 || i == 1 && j == 1 || i == -1 && j == -1 || i == 0 && j == -1)
                            continue;
                        {

                            if (GridManager.instance.grid[x + i, y + j].GetComponent<Ball>().ballNumber == deactivatedBallNumber)
                            {
                                int[] index = { x + i, y + j };
                                indexs.Add(index);

                                balls.Add(GridManager.instance.grid[x + i, y + j].gameObject);
                            }
                        }

                    }

                    else if (x == 0 && y < rows && y > 0)
                    {

                        print("$4");

                        if (y % 2 != 0)
                        {

                            if (i == -1)
                                continue;


                            if (GridManager.instance.grid[x + i, y + j].GetComponent<Ball>().ballNumber == deactivatedBallNumber)
                            {
                                int[] index = { x + i, y + j };
                                indexs.Add(index);
                                balls.Add(GridManager.instance.grid[x + i, y + j].gameObject);
                            }

                        }
                        else
                        {

                            if (i > -1 && j == 0 || i == 0 && j != 0)
                            {

                                if (GridManager.instance.grid[x + i, y + j].GetComponent<Ball>().ballNumber == deactivatedBallNumber)
                                {
                                    int[] index = { x + i, y + j };
                                    indexs.Add(index);
                                    balls.Add(GridManager.instance.grid[x + i, y + j].gameObject);
                                }
                            }


                        }

                    }
                    else if (x == columns - 1 && y < rows && y > 0)
                    {
                        print("$5");

                        if (y % 2 == 0)
                        {

                            if (i == 1)
                                continue;


                            if (GridManager.instance.grid[x + i, y + j].GetComponent<Ball>().ballNumber == deactivatedBallNumber)
                            {
                                int[] index = { x + i, y + j };
                                indexs.Add(index);
                                balls.Add(GridManager.instance.grid[x + i, y + j].gameObject);
                            }

                        }
                        else
                        {


                            if (i < 1 && j == 0 || i == 0 && j != 0)
                            {

                                if (GridManager.instance.grid[x + i, y + j].GetComponent<Ball>().ballNumber == deactivatedBallNumber)
                                {
                                    int[] index = { x + i, y + j };
                                    indexs.Add(index);
                                    balls.Add(GridManager.instance.grid[x + i, y + j].gameObject);
                                }
                            }


                        }

                    }
                    else if (x > 0 && x < columns && y > 0 && y < rows)
                    {
                        print("$6");


                        if (y % 2 != 0)
                        {
                            if (i == -1 && j == -1 || i == -1 && j == 1)
                                continue;

                            if (GridManager.instance.grid[x + i, y + j].GetComponent<Ball>().ballNumber == deactivatedBallNumber)
                            {
                                int[] index = { x + i, y + j };
                                indexs.Add(index);
                                balls.Add(GridManager.instance.grid[x + i, y + j].gameObject);
                            }
                        }
                        else
                        {
                            if (i == 1 && j == -1 || i == 1 && j == 1)
                                continue;

                            if (GridManager.instance.grid[x + i, y + j].GetComponent<Ball>().ballNumber == deactivatedBallNumber)
                            {

                                int[] index = { x + i, y + j };
                                indexs.Add(index);
                                balls.Add(GridManager.instance.grid[x + i, y + j].gameObject);
                            }
                        }

                    }
                }
            }
            return balls;
        }


    }
}