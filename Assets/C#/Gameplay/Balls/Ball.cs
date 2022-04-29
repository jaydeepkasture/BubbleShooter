using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Gameplay.Balls
{

    public class Ball : MonoBehaviour
    {
        //public GameObject parent;
        public Sprite[] sp;

        public GameObject efxPrefab;
        //this will sent from grid manager
        public int row;
        public int column;
        public int ballNumber;

        public const float POP_SPEED = 0.9f;
        public const float EXPLODE_SPEED = 5f;
        public const float KILL_Y = -30f;

        private bool efxFlag = false;
        [SerializeField] bool animStarted, falling;
        protected bool isaFireball;

        public Action onFinishedMoving;
        GridManager gridManager;
        public Square square;
        private void Start()
        {
            gridManager = GridManager.instance;
        }
        private void OnEnable()
        {
            animStarted = false;
            falling = false;
        }
        private void OnDisable()
        {
            animStarted = false;
            falling = false;
        }
        public void SetBallColour(int number)
        {
            ballNumber = number;
            animStarted = false;
            falling = false;
            GetComponent<SpriteRenderer>().sprite = sp[number];
        }
        public void SetGridManager(GridManager gm)
        {
            gridManager = gm;
        }
        public void ResetPosition()
        {
            transform.position = square.transform.position;

        }
        public void State(string state)
        {
            Collider2D cc = GetComponent<Collider2D>();
            switch (state)
            {
                case "Pop":
                    if (cc != null)
                        cc.enabled = false;

                    efxFlag = true;
                    GameObject go = Instantiate(efxPrefab);
                    go.transform.position = transform.position;
                    break;

                case "Fall":

                    if (cc != null)
                        cc.enabled = false;
                    //falling = true;
                    var ball = new GameObject();
                    ball.transform.position = this.gameObject.transform.position;
                    ball.AddComponent<SpriteRenderer>().sprite = sp[ballNumber];
                    this.gameObject.SetActive(false);

                    ball.AddComponent<Rigidbody2D>();
                    ball.AddComponent<BoxCollider2D>();
                    Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.gravityScale = 3f;

                    }
                    if (transform.position.y < KILL_Y)
                    {
                        this.ballNumber = 6;
                    }
                    Destroy(ball.gameObject, 2);
                    break;
                default:
                    break;
            }

        }
        float LAUNCH_SPEED = 30;
        public virtual IEnumerator MoveBall(List<GameObject> nearestBalls)
        {
            foreach (Vector3 pos in DottedPathCreator.instance.points)
            {
                if (pos == Vector3.zero)
                {
                    if (nearestBalls.Count == 0||nearestBalls==null)
                    {
                        Debug.Log("here");
                        onFinishedMoving?.Invoke();
                        Destroy(gameObject);
                    }
                    else AfterReachingTheEndPointOfThePath(nearestBalls);
                    yield break;
                }
                while (Vector3.Distance(transform.position, pos) > .1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, pos, LAUNCH_SPEED * Time.deltaTime);

                    yield return new WaitForEndOfFrame();
                }

            }

            if (isaFireball)
            {
                onFinishedMoving?.Invoke();
                Destroy(gameObject);
            }
        }

        public void AfterReachingTheEndPointOfThePath(List<GameObject> closestballs)
        {

            GameObject targetObject = closestballs.Count == 1 ? closestballs[0] : ActivateClosestBall(closestballs, this.gameObject);
            Ball targetBall = targetObject.GetComponent<Ball>();
            OnFinishedMoving(targetBall.row, targetBall.column);
        }
        public GameObject ActivateClosestBall(List<GameObject> balls, GameObject fromThis)
        {
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = fromThis.gameObject.transform.position;
            foreach (GameObject potentialTarget in balls)
            {
                Vector3 directionToTarget = potentialTarget.gameObject.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
            return bestTarget;
        }
        public void OnFinishedMoving(int row, int collum)
        {
            GameObject newBubble = gridManager.ActivateBall(row, collum, ballNumber);
            if (newBubble != null)
            {
                Ball gridMember = newBubble.GetComponent<Ball>();
                if (gridMember != null)
                {
                    gridManager.Seek(gridMember.column, gridMember.row, gridMember.ballNumber);
                    var HT = new Hashtable();
                    HT.Add(gameObject, gameObject);
                    HT.Add(newBubble, newBubble);
                    if (ballNumber != 6)
                        InitiateWaveEffect(transform.position, HT);

                }
            }

            onFinishedMoving?.Invoke();

            Destroy(gameObject);
        }


        #region Wave Effect
        /// <summary>
        /// This will create a wave like effect after the launching ball hits a cluster of balls
        /// </summary>
        /// <param name="newBallPos"></param>
        public void InitiateWaveEffect(Vector3 newBallPos, Hashtable animTable)
        {
            int layerMask = 1 << LayerMask.NameToLayer("Ball");
            Collider2D[] fixedBalls = Physics2D.OverlapCircleAll(transform.position, 0.5f, layerMask);
            float force = .15f;

            foreach (Collider2D obj in fixedBalls)
            {
                if (!animTable.ContainsKey(obj.gameObject) && obj.gameObject != gameObject && animTable.Count < 30)
                {
                    obj.GetComponent<Ball>()?.PlayHitAnimCorStart(newBallPos, force, animTable);
                }
            }
            if (fixedBalls.Length > 0 && !animTable.ContainsKey(gameObject))
            {
                PlayHitAnimCorStart(fixedBalls[0].gameObject.transform.position, 0, animTable);
            }
        }
        public void PlayHitAnimCorStart(Vector3 newBallPos, float force, Hashtable animTable)
        {
            if (!animStarted)
            {
                StartCoroutine(PlayHitAnimCor(newBallPos, force, animTable));
                InitiateWaveEffect(newBallPos, animTable);
            }
        }
        public IEnumerator PlayHitAnimCor(Vector3 newBallPos, float force, Hashtable animTable)
        {
            animStarted = true;
            Vector3 startPos = transform.localPosition;
            animTable.Add(gameObject, gameObject);
            if (tag == "centerball")
            {

                animStarted = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
            float dist = Vector3.Distance(transform.position, newBallPos);
            force = 1 / dist + force;
            newBallPos = transform.position - newBallPos;
            if (transform.parent == null)
            {
                animStarted = false;
                yield break;
            }
            newBallPos = Quaternion.AngleAxis(transform.parent.rotation.eulerAngles.z, Vector3.back) * newBallPos;
            newBallPos = newBallPos.normalized;
            newBallPos = transform.localPosition + (newBallPos * force / 10);

            float startTime = Time.time;
            float speed = force * 5;
            float distCovered = 0;
            while (distCovered < 1 && !float.IsNaN(newBallPos.x))
            {
                distCovered = (Time.time - startTime) * speed;
                if (this == null)
                {
                    animStarted = false;
                    falling = false;
                    yield break;
                }
                //   if( destroyed ) yield break;
                if (falling)
                {
                    animStarted = false;
                    falling = false;
                    //           transform.localPosition = startPos;
                    yield break;
                }
                transform.localPosition = Vector3.Lerp(startPos, newBallPos, distCovered);
                yield return new WaitForEndOfFrame();
            }
            Vector3 lastPos = transform.localPosition;
            startTime = Time.time;
            distCovered = 0;
            while (distCovered < 1 && !float.IsNaN(newBallPos.x))
            {
                distCovered = (Time.time - startTime) * speed;
                if (this == null)
                {
                    animStarted = false;
                    falling = false;
                    yield break;
                }
                if (falling)
                {
                    animStarted = false;
                    falling = false;
                    //      transform.localPosition = startPos;
                    yield break;
                }
                transform.localPosition = Vector3.Lerp(lastPos, startPos, distCovered);
                yield return new WaitForEndOfFrame();
            }
            transform.localPosition = startPos;
            transform.position = square.transform.position;
            animStarted = false;
            falling = false;
        }
        #endregion

    }

}