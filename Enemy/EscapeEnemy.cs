using System;
using Microsoft.Xna.Framework;

namespace Lighthouse
{
    /// <summary>
    /// ライトを近づけるとよける敵のクラス
    /// </summary>
    class EscapeEnemy : Enemy
    {
        /// <summary>
        /// 検知用バウンディングスフィア
        /// </summary>
        private BoundingSphere detectSphere;
        private bool getLightDetect = false;
        private float initialSpeed;

        protected override void  EnemyMove(Position lightPosition, bool lightOn, BoundingSphere lightBoundingSphere)
        {
            if (getLightDetect == false)
            {
                initialSpeed = speed;
            }

            cParameter.rad -= speed;

            if (lightOn && lightBoundingSphere.Intersects(detectSphere))
                getLightDetect = true;
            else
                getLightDetect = false;

            //検知時の幽霊船の挙動
            if (getLightDetect)
            {
                speed = initialSpeed * 2;

                if (lightPosition.angle < cParameter.angle
                    || lightPosition.angle == cParameter.angle && cParameter.spawnTime % 2 == 0)
                {
                    cParameter.angle += 0.3f;
                }
                else if (lightPosition.angle > cParameter.angle
                    || lightPosition.angle == cParameter.angle && cParameter.spawnTime % 2 == 1)
                {
                    cParameter.angle -= 0.3f;
                }
            }
            else
                speed = initialSpeed;
        }

        /// <summary>
        /// 当たり判定を設定
        /// </summary>
        protected override void SetCollision()
        {
            //船の中心
            enemyBoundingSphere[0].Center = Position;

            //船の前方
            enemyBoundingSphere[1].Center = new Vector3(
                (float)(Math.Cos(MathHelper.ToRadians(cParameter.angle - 90 + rotationAngle))),
                0f,
                (float)(Math.Sin(MathHelper.ToRadians(cParameter.angle - 90 + rotationAngle)))) * enemyModel.outsideRad + Position;

            //船の後方
            enemyBoundingSphere[2].Center = new Vector3(
                (float)(Math.Cos(MathHelper.ToRadians(cParameter.angle + 90 + rotationAngle))),
                0f,
                (float)(Math.Sin(MathHelper.ToRadians(cParameter.angle + 90 + rotationAngle)))) * enemyModel.outsideRad + Position;

            //半径
            enemyBoundingSphere[0].Radius = enemyModel.collisionRad;
            enemyBoundingSphere[1].Radius = enemyModel.collisionRad;
            enemyBoundingSphere[2].Radius = enemyModel.collisionRad;

            //検知用当たり判定
            detectSphere.Center = Position;
            detectSphere.Radius = 300;
        }
    }
}
