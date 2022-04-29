using Gameplay.Balls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class Launcher : MonoBehaviour
    {
        public Transform ballSpawnPositon;
        public GameObject BallPrefab, curretBubbleParent;
        Ball currentBall;
        public GameObject nextBubble;
        public GameObject lastWall, dragArea;
        public float LAUNCH_SPEED = 10f;
        public float fireRate = 0.5F;
        public int maxBubbles = 0;
        [Header("Falling")]
        public bool atuoFalling = true;
        public float falldownDistance = .25f;
        public bool startShooting = true, allow;
        public bool stopBall = false, drragging;
        private GridManager grid;
        private float nextFire = 0.0F;
        private Ball gotemp;
        [SerializeField] public Color[] dotSpriteColor;
        Touch touch;
        [Header("BubbleSprite")]
        public Sprite[] bubbleSprite;
        public GameObject backWall;
        public GameObject frontWall;
        public GameObject swapbutton;


        private void Start()
        {
            LoadBall();
        }
        private void Update()
        {
            //this only detect drag
            if (Application.isEditor)
                ForEditor();
            else
                ForTouchDevices();
        }
        private void ForEditor()
        {
            if (Input.GetMouseButton(0))
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layerMask = 1 << LayerMask.NameToLayer("DragArea");
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity,layerMask);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == dragArea)
                    {
                        drragging = true;
                        Vector2 mousePos = new Vector2();
                        Vector2 delta;
                        float clampValue = 0;
                        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        delta = mousePos - new Vector2(transform.position.x, transform.position.y);
                        clampValue = Mathf.Clamp(-Mathf.Rad2Deg * Mathf.Atan2(delta.x, delta.y), -60, 60);
                        DottedPathCreator.instance.dotsParent.SetActive(true);
                        DottedPathCreator.instance.transform.rotation = Quaternion.Euler(0f, 0f, clampValue); ;
                    }
                }


            }


            if (Input.GetMouseButtonUp(0) && Time.time > nextFire && startShooting && maxBubbles > 0 && drragging)
            {
                DottedPathCreator.instance.dotsParent.SetActive(false);

                Collider2D collider = currentBall.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.isTrigger = true;
                    collider.enabled = true;
                }
                /// Shoot cool down
                nextFire = Time.time + fireRate;
                startShooting = false;
                drragging = false;
                Fire();

            }
            if (Input.GetMouseButtonDown(0))
                SwitchBallColor();
        }
        private void ForTouchDevices()
        {
            if (Input.touchCount == 1 && allow)
            {

                int layerMask = 1 << LayerMask.NameToLayer("DragArea");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, layerMask);
                //RaycastHit2D hit = Physics2D.CircleCast(ray.origin, .2f, ray.direction);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == dragArea)
                    {
                        drragging = true;

                        Vector2 mousePos = new Vector2();
                        Vector2 delta;
                        float clampValue = 0;

                        touch = Input.GetTouch(0);
                        //mousePos = new Vector2(touch.position.x, touch.position.y);
                        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        delta = mousePos - new Vector2(transform.position.x, transform.position.y);
                        clampValue = Mathf.Clamp(-Mathf.Rad2Deg * Mathf.Atan2(delta.x, delta.y), -60, 60);
                        DottedPathCreator.instance.transform.rotation = Quaternion.Euler(0f, 0f, clampValue);
                        DottedPathCreator.instance.dotsParent.SetActive(true);
                    }
                }
                else
                {
                    drragging = false;
                    DottedPathCreator.instance.dotsParent.SetActive(false);

                }




                if (touch.phase == TouchPhase.Ended && Time.time > nextFire && drragging && startShooting && maxBubbles > 0)
                {
                    DottedPathCreator.instance.dotsParent.SetActive(false);

                    CircleCollider2D collider = currentBall.GetComponent<CircleCollider2D>();
                    if (collider != null)
                    {
                        collider.isTrigger = true;
                        collider.enabled = true;
                    }
                    /// Shoot cool down
                    nextFire = Time.time + fireRate;
                    startShooting = false;
                    Fire();
                    /// Show ball sprite
                    gotemp.GetComponent<SpriteRenderer>().enabled = true;

                }
                else
                {
                    if (maxBubbles <= 0)
                    {
                        DottedPathCreator.instance.dotsParent.SetActive(false);
                        allow = false;

                    }
                }


            }
            else
            {
                DottedPathCreator.instance.dotsParent.SetActive(false);
                drragging = false;
            }


            if (Input.GetMouseButtonDown(0))
                SwitchBallColor();
        }
        public void LoadBall()
        {
            allow = true;

            var ball = Instantiate(BallPrefab, ballSpawnPositon.position, Quaternion.identity);
            ball.name = "Luncher Ball " + maxBubbles;
            currentBall = ball.GetComponent<Ball>();
            ball.GetComponent<BoxCollider2D>().enabled = false;
            int ballNumber = (int)Random.Range(0f, 6f);

            currentBall.ballNumber = ballNumber;
            currentBall.SetBallColour(ballNumber);
            currentBall.transform.parent = curretBubbleParent.transform;
            DottedPathCreator.instance.SetDotsColors(dotSpriteColor[ballNumber]);
            currentBall.gameObject.SetActive(true);
            gotemp = currentBall;

     
            if (currentBall != null)
                currentBall.transform.parent = transform;
            //nextBubble.GetComponent<SpriteRenderer>().sprite = bubbleSprite[SwitchBubbles.instance.SetBubbleColor(ref currentBubbleKind)];

        }
        public void SwitchBallColor()
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            ////RaycastHit2D hit = Physics2D.CircleCast(ray.origin, .2f, ray.direction);
            //if (hit.collider != null)
            //{
            //    if (hit.collider.gameObject == swapbutton)
            //    {
            //        SwitchBubbles.instance.GetSwitchBubbleIndex(out int current, out int next);
            //        nextBubble.GetComponent<SpriteRenderer>().sprite = bubbleSprite[next];
            //        currentBall.GetComponent<SpriteRenderer>().sprite = bubbleSprite[current];
            //        currentBall.ballNumber = current;
            //        DottedPathCreator.instance.SetDotsColors(dotSpriteColor[current]);
            //        currentBubbleKind = next;
            //    }
            //}


        }
        #region Private function
        void Fire()
        {
            maxBubbles -= 1;
            //GameSceneUiManager.instance.maxBubble.text = maxBubbles.ToString();
            List<GameObject> nearestBalls = new List<GameObject>();
            if (DottedPathCreator.instance.destinationBall != null)
            {
                //if Hitting the group of bubbles
                if (DottedPathCreator.instance.destinationBall.CompareTag("bubble"))
                {
                    nearestBalls = DestinationDectectorAfterLaunch.CalculateIndex(DottedPathCreator.instance.destinationBall);
                }//if hitting the last wall
                else if (lastWall == DottedPathCreator.instance.destinationBall)
                {
                    

                    for (int i = 0; i < GridManager.instance.columns; i++)
                    {
                        nearestBalls.Add(GridManager.instance.grid[i, 0]);
                    }

                }//if hitting the wall below the projectile
                else if (backWall == DottedPathCreator.instance.destinationBall)
                {
                    Destroy(currentBall.gameObject);
                    LoadBall();
                    startShooting = true;
                    Debug.LogError("No target found is null");
                }else if (frontWall == DottedPathCreator.instance.destinationBall)
                {
                    nearestBalls.Clear();
                }
            }
            else
            {
                Destroy(currentBall.gameObject);
                LoadBall();
                startShooting = true;
                Debug.LogError("No target found is null");
            }

            StartCoroutine(currentBall.MoveBall(nearestBalls));
            currentBall.onFinishedMoving += () =>
            {
                LoadBall();
                startShooting = true;
            };

        }



        #endregion
    }
}