using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Gameplay;

public class BubbleRowManager : MonoBehaviour
{

    /*
     this class used to calculate the bubbleParent(Group) position 
     such that there will be only 10 rows on the screen of our mobile phone and the remaining bubbles will be above the screen
     as a result we can only see only half of our screen will fill with bubbles i.e only 110 bubble at a time
     and we have to maintain this number by changing the position of the Group GameObject which is the parent of bubbles in the scen
         */




    private const int MAX_ROWS_ON_SCREEN = 10;
    private const float VERTICAL_DIFFERENCE_BETWEEN_TWO_BUBBLES_ON_Y_AXIS=.8F;

    public float initialSpeed = 10,Speed=5;
    private bool Load;


    [SerializeField] private GameObject BubbleGroup,InitialGroupPostion;






    public static BubbleRowManager instance;
    
    private void Awake()
    {
        instance = this;   
    }

    private void Start()
    {
        BubbleGroup.transform.position = new Vector3(0, InitialGroupPostion.transform.position.y, 0);
        CaculateGroupPos();
        Load = true;

    }

    /*
     As name suggested this func is to calc the group pos
     by going through the instance grid which is in the GridManager
     and it is called from GridManage.CheckCeiling()
         */
    public void CaculateGroupPos()
    {
        return;

        int target = 0;


        for (int r = 0; r < GridManager.instance.rows; r++)
        {
            for (int c = 0; c < GridManager.instance.columns; c++)
            {
                if (GridManager.instance.grid[c, r])
                if (GridManager.instance.grid[c, r].activeSelf)
                {
                    target= r;
                    //SwitchBubbles.next.Add(GridManager.instance.grid[c, r].GetComponent<Ball>().ballNumber);
                    //    if(r== GridManager.instance.rows)
                    //    {
                    //        GameSceneUiManager.instance.outOfRowsPanel.SetActive(true);
                    //    }
                    
                    }
                
            }
        }

        //SwitchBubbles.next.Distinct();

        int calucatedRowNo = 0;
        if(MAX_ROWS_ON_SCREEN< target)
        {
            calucatedRowNo =Mathf.Abs( MAX_ROWS_ON_SCREEN - target);
            Debug.Log("max row " + calucatedRowNo);
            //change the postion if y postion is greter then zero
            if (calucatedRowNo > 0)
            {
                StartCoroutine( BubbleGroupChangePosition(new Vector3(BubbleGroup.transform.position.x,
                    calucatedRowNo * VERTICAL_DIFFERENCE_BETWEEN_TWO_BUBBLES_ON_Y_AXIS,
                    BubbleGroup.transform.position.z),initialSpeed));
            }
        }
        else
        {
            if (target == 10)
            {

            StartCoroutine(BubbleGroupChangePosition(new Vector3(BubbleGroup.transform.position.x,
                 0, BubbleGroup.transform.position.z), initialSpeed));
            }
            else if (target == 9)
            {
                StartCoroutine(BubbleGroupChangePosition(new Vector3(BubbleGroup.transform.position.x,
                 -0.8f, BubbleGroup.transform.position.z), initialSpeed));

            }
            else if (target == 8)
            {
                StartCoroutine(BubbleGroupChangePosition(new Vector3(BubbleGroup.transform.position.x,
                 -1.6f, BubbleGroup.transform.position.z), initialSpeed));

            }
            else
            {
                StartCoroutine(BubbleGroupChangePosition(new Vector3(BubbleGroup.transform.position.x,
                -2.4f, BubbleGroup.transform.position.z), initialSpeed));

            }
        }
        initialSpeed = Speed;//change initial speed to normal speed
    }

    /*
     this func is used to change the position after each launch
         */
    private IEnumerator BubbleGroupChangePosition(Vector3 targetpos, float speed=1f)
    {



        while (Vector3.Distance(BubbleGroup.transform.position, targetpos) > .1f)
        {
            BubbleGroup.transform.position = Vector3.MoveTowards(BubbleGroup.transform.position, targetpos, speed * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        BubbleGroup.transform.position = targetpos;
        if (Load)
        {
            Load = false;//enable launcher after very first animation
        }



    }
}
