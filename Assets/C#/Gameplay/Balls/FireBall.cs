using System;
using UnityEngine;

namespace Gameplay.Balls
{
    public class FireBall : Ball
    {
        private void Start()
        {
            isaFireball = true;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var ball = collision.GetComponent<Ball>();
            if (ball == null) return;
            GridManager.instance.DeactivateBall(ball.row, ball.column);

        }

    }
}
