using UnityEditor;
using UnityEngine;
namespace Gameplay
{
    public class DottedPathCreator : MonoBehaviour
    {
        public static int maxReflectionCount = 4;
        public static int maxDotsInAStepCount = 100;
        public static float maxStepDistance = 500;
        [SerializeField] public GameObject dot, dotsParent, bubble, LastWall;
        static GameObject[,] dots = new GameObject[maxReflectionCount, maxDotsInAStepCount];



        [HideInInspector] public Vector3[] points = new Vector3[maxReflectionCount];//this vector is use to get the position of the path 
        [HideInInspector] public GameObject destinationBall;//Tt stores the ball which raycast hits
        public GameObject BackWall, frontWall;
        public static DottedPathCreator instance;


        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            dotsParent.SetActive(false);

            for (int i = 0; i < dots.GetLength(0); i++)
            {
                for (int j = 0; j < dots.GetLength(1); j++)
                {
                    dots[i, j] = Instantiate(dot);
                    dots[i, j].transform.position = transform.position;
                    dots[i, j].transform.parent = dotsParent.transform;
                }
            }
            string[] layers = new string[]
            {
                "Ball",
                "Wall",
                "FireBall"
            };
            OnNormalBallSelected();
        
        }
        private void FixedUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                mangaeDotsPoints();
               
            }

        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.ArrowHandleCap(0, transform.position + transform.forward * 0.25f, transform.rotation, 0.5f, EventType.Repaint);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.25f);
            DrawPredictedReflectionPattern(transform.position, transform.up, maxReflectionCount);
        }

#endif

        private void DrawPredictedReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining)
        {
            if (reflectionsRemaining == 0)
                return;
            Vector3 startingPosition = position;

            Ray ray = new Ray(position, direction);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxStepDistance, layerMask);
            if (hit)
            {
                direction = Vector2.Reflect(direction, hit.normal);
                position = hit.point + new Vector2(direction.x, direction.y) * 0.01f;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(position, 0.25f);
            }
            else
                position += direction * 0.01f * maxStepDistance;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startingPosition, position);

            DrawPredictedReflectionPattern(position, direction, reflectionsRemaining - 1);
        }



        public LayerMask layerMask;

        public void mangaeDotsPoints()
        {
            ManageDotsPath(transform.position, transform.up, maxReflectionCount);
        }

        void ManageDotsPath(Vector2 position, Vector2 direction, int reflectionsRemaining,
             bool hideLeftoverDots = false)
        {

            points = new Vector3[maxReflectionCount];
            Debug.Log(maxReflectionCount);
            while (reflectionsRemaining > 0)
            {

                Vector2 startingPosition = position;
                Ray ray = new Ray(position, direction);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, maxStepDistance, layerMask);

                if (hit)
                {
                    direction = Vector2.Reflect(direction, hit.normal);
                    position = hit.point + direction * 0.01f;
                }
                else
                    position += direction * 0.01f * maxStepDistance;

                for (int i = 0; i < dots.GetLength(1); i++)
                {

                    Vector2 AB = position - startingPosition;
                    AB = AB.normalized;
                    float step = i / 2.8f;
                    if (step < (position - startingPosition).magnitude && !hideLeftoverDots)
                    {
                        dots[maxReflectionCount - reflectionsRemaining, i].SetActive(true);
                        dots[maxReflectionCount - reflectionsRemaining, i].transform.position = startingPosition + step * AB;
                        points[maxReflectionCount - reflectionsRemaining] = position;

                    }
                    else
                    {
                        dots[maxReflectionCount - reflectionsRemaining, i].SetActive(false);
                    }
                }

                if (hideLeftoverDots == false)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject == LastWall)
                        {
                            hideLeftoverDots = true;
                        }
                        if (hit.collider.CompareTag("bubble"))
                        {
                            hideLeftoverDots = true;
                        }
                        else if (hit.collider.gameObject == BackWall)
                        {
                            hideLeftoverDots = true;
                        }
                        destinationBall = hit.collider.gameObject;
                    }
                }
                reflectionsRemaining -= 1;
            }


        }

        public void SetDotsColors(Color color)
        {
            foreach (Transform dots in dotsParent.transform)
            {
                dots.GetComponent<SpriteRenderer>().color = color;
            }
        }


        public void OnFireBallSelected()
        {
            layerMask =
                 (1 << LayerMask.NameToLayer(Layers.Fireball.ToString()))
                | (1 << LayerMask.NameToLayer(Layers.Wall.ToString()))
                ;
            maxReflectionCount = 1;
        } 
        public void OnNormalBallSelected()
        {
            layerMask =
                 (1 << LayerMask.NameToLayer(Layers.Ball.ToString()))
                | (1 << LayerMask.NameToLayer(Layers.Wall.ToString()))
                ;
            maxReflectionCount = 4;
        }

    }
}
