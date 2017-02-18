using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lighthouse
{
    /// <summary>
    /// 最大2点の中間地点を通り、向かってくる敵のクラス
    /// </summary>
    class CurveEnemy : Enemy
    {
        private bool passedPoint1 = false;
        private bool passedPoint2 = false;
        private int moveCount = 0;
        private float arc;
        private Vector3 position;
        private Position firstPosition;
        private Vector3 translation;
        private Vector3 translation2;

        public override void InitializeEnemy(EnemyModel loadModel, CommonParameter readCParameter, IndividualParameter readIParameter)
        {
            iParameter = readIParameter;
            switch (iParameter.numberOfPoint)
            {
                case 0:
                    passedPoint1 = true;
                    passedPoint2 = true;
                    break;
                case 1:
                    passedPoint1 = false;
                    passedPoint2 = true;
                    break;
                case 2:
                    passedPoint1 = false;
                    passedPoint2 = false;
                    break;
                default :
                    passedPoint1 = true;
                    passedPoint2 = true;
                    break;
            }

            speed = iParameter.speed;

            InitializeCommonParameter(loadModel, readCParameter);

            iParameter.point1.position = new Vector3 (
                    (float)(iParameter.point1.rad * Math.Cos(MathHelper.ToRadians(iParameter.point1.angle))),
                    0f,
                    (float)(iParameter.point1.rad * Math.Sin(MathHelper.ToRadians(iParameter.point1.angle))));
            iParameter .point2 .position = new Vector3 (
                    (float)(iParameter.point2.rad * Math.Cos(MathHelper.ToRadians(iParameter.point2.angle))),
                    0f,
                    (float)(iParameter.point2.rad * Math.Sin(MathHelper.ToRadians(iParameter.point2.angle))));

            translation = iParameter.point1.position - firstPosition.position;
            translation2 = iParameter.point2.position - iParameter.point1.position;
        }

        protected override void InitializeCommonParameter(EnemyModel loadModel, CommonParameter readCParameter)
        {
            enemyModel = loadModel;
            cParameter = readCParameter;
            switch (cParameter.modelType)
            {
                case 0:
                case 1:
                    enemyModel.outsideRad = 80;
                    enemyModel.collisionRad = 40;
                    break;
                case 2:
                case 3:
                    enemyModel.outsideRad = 212;
                    enemyModel.collisionRad = 106;
                    break;
            }
            cParameter.scaleMax = cParameter.scale;
            enemyModel.outsideRad *= cParameter.scale;
            enemyModel.outsideRadMax = enemyModel.outsideRad;
            enemyModel.collisionRad *= cParameter.scale;
            enemyModel.collisionRadMax = enemyModel.collisionRad;
            cParameter.lifeMax = cParameter.life;

            transform = new Matrix[enemyModel.ship.Bones.Count];
            enemyModel.ship.CopyAbsoluteBoneTransformsTo(transform);

            SetCollision();

            rotationAngle = 90;
            rotation = Matrix.CreateRotationY(MathHelper.ToRadians(-cParameter.angle - rotationAngle));

            position = Position;
            SetCollision();

            firstPosition.rad = cParameter.rad;
            firstPosition.angle = cParameter.angle;
            firstPosition .position = new Vector3 (
                                (float)(firstPosition.rad * Math.Cos(MathHelper.ToRadians(cParameter.angle))),
                0,
                (float)(firstPosition.rad * Math.Sin(MathHelper.ToRadians(cParameter.angle))));

            world = Matrix.CreateScale(cParameter.scale) * rotation * Matrix.CreateTranslation(Position);
        }

        public override void UpdateEnemy(bool lightOn, ref int score, bool useBomb, float countDownSecond, 
            BoundingSphere lightBoundingSphere, ref bool doDrawEffect, ref bool doDrawDamage, out Vector3 boardPos, ref float boardScale,
            Position lightPosition, float lightSpeed, ref Position enemyIcon, ref bool doDrawIcon)
        {
            countTime = countDownSecond;

            if (cParameter.life != 0 && countDownSecond <= (180f - cParameter.spawnTime))
            {
                doDrawIcon = true;

                changeAngle++;
                if (changeAngle > 359)
                    changeAngle = 0;
                changeAlpha = (float)Math.Sin(MathHelper.ToRadians(changeAngle));

                //移動
                if (cParameter.rad > 0f && countDownSecond > 0)
                    EnemyMove();

                //移動によるBoundingSphreの移動
                SetCollision();

                //当たり判定処理
                JudgeCollision(lightOn, lightBoundingSphere, ref doDrawEffect, ref score);
                doDrawDamage = isLightHit;
                if (doDrawEffect)
                    doDrawDamage = false;

                //ボム使用結果処理
                if (useBomb)
                {
                    BombCollision(ref doDrawEffect,ref doDrawDamage , ref score);
                }

                //灯台に当たる処理
                LighthouseCollision();

                rotation = Matrix.CreateRotationY(MathHelper.ToRadians(-cParameter.angle - rotationAngle));
                world = Matrix.CreateScale(cParameter.scale) * rotation * Matrix.CreateTranslation(position);
            }
            else
                doDrawIcon = false;

            boardPos = position;

            boardScale = cParameter.scale;
            if (cParameter.modelType > 1)
                boardScale *= 2;

            if (cParameter.rad < 300 && doDrawEffect)
                boardPos = new Vector3(
                    (float)(300 * Math.Cos(MathHelper.ToRadians(cParameter.angle))),
                    0f,
                    (float)(300 * Math.Sin(MathHelper.ToRadians(cParameter.angle))));

            enemyIcon.angle = rotationAngle + 90;
            enemyIcon.position = position;
        }

        protected override void EnemyMove()
        {
            if (passedPoint1 == false)
            {
                position += translation * 0.001f;
                if (moveCount == 1000)
                    passedPoint1 = true;
                moveCount++;
            }
            else if (passedPoint1 && passedPoint2 == false)
            {
                translation = iParameter.point2.position - iParameter.point1.position;
                position += translation * 0.001f;
                if (moveCount == 2000)
                {
                    passedPoint2 = true;
                }
                moveCount++;
            }
            else if(passedPoint1 == true && passedPoint2 == true)
            {
                switch (iParameter.numberOfPoint)
                {
                    case 1:
                        translation = Vector3.Normalize(Vector3.Zero - iParameter.point1.position);
                        break;
                    case 2:
                        translation = Vector3.Normalize(Vector3.Zero - iParameter.point2.position);
                        break;
                }
                position += translation;
            }

            arc = (float)Math.Atan2(translation.X, translation.Z) * (180 / (float)Math.PI);
            rotationAngle = -(arc + cParameter.angle);
        }

        protected override void SetCollision()
        {
            //船の中心
            enemyBoundingSphere[0].Center = position;

            //船の前方
            enemyBoundingSphere[1].Center = new Vector3(
                (float)(Math.Cos(MathHelper.ToRadians(cParameter.angle - 90 + rotationAngle))),
                0f,
                (float)(Math.Sin(MathHelper.ToRadians(cParameter.angle - 90 + rotationAngle)))) * enemyModel.outsideRad + position;

            //船の後方
            enemyBoundingSphere[2].Center = new Vector3(
                (float)(Math.Cos(MathHelper.ToRadians(cParameter.angle + 90 + rotationAngle))),
                0f,
                (float)(Math.Sin(MathHelper.ToRadians(cParameter.angle + 90 + rotationAngle)))) * enemyModel.outsideRad + position;

            //半径
            enemyBoundingSphere[0].Radius = enemyModel.collisionRad;
            enemyBoundingSphere[1].Radius = enemyModel.collisionRad;
            enemyBoundingSphere[2].Radius = enemyModel.collisionRad;
        }
    }
}
