using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Lighthouse
{
    /// <summary>
    /// 敵の種類の列挙体
    /// 直進、カーブ、螺旋
    /// </summary>
    public enum EnemyType
    {
        Straight, Curve, Escape, Teleport, Ganger, Sprial
    }

    /// <summary>
    /// 全種の敵が持っている基本パラメータを定義
    /// </summary>
    public struct CommonParameter
    {
       public int type;
       public int modelType;
       public float life;
       public float lifeMax;
       public int power;
       public float scale;
       public float scaleMax;
       public float rad;
       public float angle;
       public float spawnTime;
       public int score;
    }

    /// <summary>
    /// ある種の敵だけが持っている個別パラメータを定義
    /// </summary>
    public struct IndividualParameter
    {
        public float speed;
        public int numberOfPoint; //曲がるポイント(通過点)の数(最大2点)
        public Position point1;
        public Position point2;
    }

    public struct EnemyModel
    {
        public Model ship;
        public Texture2D texture;

        /// <summary>
        /// 中心からの前方円中心と後方円中心の距離
        /// </summary>
        public float outsideRad;
        public float outsideRadMax;

        /// <summary>
        /// BoundingSphereひとつの半径
        /// </summary>
        public float collisionRad;
        public float collisionRadMax;
    }

    /// <summary>
    /// 敵の基本クラス
    /// </summary>
    class Enemy
    {
        public CommonParameter cParameter;
        public IndividualParameter iParameter;

        protected EnemyModel enemyModel;
        protected Matrix[] transform;
        protected Matrix world;

        /// <summary>
        /// 極座標系をデカルト座標系に変換するプロパティ
        /// </summary>
        public Vector3 Position
        { 
            get
            {
                return new Vector3(
                    (float)(cParameter.rad * Math.Cos(MathHelper.ToRadians(cParameter.angle))),
                    0f,
                    (float)(cParameter.rad * Math.Sin(MathHelper.ToRadians(cParameter.angle))));
            }
        }

        public float speed;

        protected float rotationAngle;
        protected Matrix rotation;

        protected BoundingSphere[] enemyBoundingSphere = new BoundingSphere[3];

        public float countTime;

        /// <summary>
        /// 船から灯台へのダメージ
        /// </summary>
        public int collisionDamage = 0;

        protected bool isLightHit = false;

        protected float changeAlpha;
        protected float changeAngle = 0;

        /// <summary>
        /// 敵(Straight, Curve)の初期化を行う
        /// </summary>
        /// <param name="loadModel"></param>
        /// <param name="readCParameter"></param>
        /// <param name="readIParameter"></param>
        public virtual void InitializeEnemy(EnemyModel loadModel, CommonParameter readCParameter, IndividualParameter readIParameter)
        {
            iParameter = readIParameter;
            speed = iParameter.speed;

            InitializeCommonParameter(loadModel, readCParameter);
        }

        /// <summary>
        /// 敵(Sprial)の初期化を行う
        /// </summary>
        /// <param name="loadModel"></param>
        /// <param name="readCParameter"></param>
        public void InitializeEnemy(EnemyModel loadModel, CommonParameter readCParameter)
        {
            speed = 1;

            InitializeCommonParameter(loadModel, readCParameter);
        }

        /// <summary>
        /// 全種の敵の共通パラメータを初期化する
        /// </summary>
        protected virtual void InitializeCommonParameter(EnemyModel loadModel, CommonParameter readCParameter)
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
                case 4:
                    enemyModel.outsideRad = 84;
                    enemyModel.collisionRad = 42;
                    break;
                case 5:
                    enemyModel.outsideRad = 500;
                    enemyModel.collisionRad = 250;
                    break;
                default :
                    enemyModel.outsideRad = 80;
                    enemyModel.collisionRad = 40;
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
            rotation = Matrix.CreateRotationY(MathHelper.ToRadians(- cParameter.angle - rotationAngle));

            world = Matrix.CreateScale(cParameter.scale) * rotation * Matrix.CreateTranslation(Position);
        }

        /// <summary>
        /// 敵を動かす
        /// //TODO:関数化
        /// </summary>
        public virtual void UpdateEnemy(bool lightOn, ref int score, bool useBomb, float countDownSecond,
            BoundingSphere lightBoundingSphere,
            ref bool doDrawEffect, ref bool doDrawDamage, out Vector3 boardPos, ref float boardScale,
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
                {
                    switch (cParameter.type)
                    {
                        case (int)EnemyType.Straight:
                            EnemyMove();
                            break;

                        case (int)EnemyType.Escape:
                            EnemyMove(lightPosition, lightOn, lightBoundingSphere);
                            break;

                        case (int)EnemyType.Teleport:
                            EnemyMove(lightPosition);
                            break;

                        case (int)EnemyType.Ganger:
                            EnemyMove();
                            break;

                        case (int)EnemyType.Sprial:
                            EnemyMove();
                            break;
                    }
                }

                //移動によるBoundingSphreの移動
                SetCollision();

                //当たり判定処理
                JudgeCollision(lightOn, lightBoundingSphere, ref doDrawEffect, ref score);
                doDrawDamage = isLightHit;
                if (doDrawEffect)
                    doDrawDamage = false;

                //ボム使用結果処理
                if(useBomb)
                    BombCollision(ref doDrawEffect, ref doDrawDamage, ref score);

                //灯台に当たる処理
                LighthouseCollision();

                rotation = Matrix.CreateRotationY(MathHelper.ToRadians(-cParameter.angle - rotationAngle));
                world = Matrix.CreateScale(cParameter.scale) * rotation * Matrix.CreateTranslation(Position);
            }
            else
                doDrawIcon = false;

            boardPos = new Vector3 (
                    (float)((cParameter .rad + 200 * cParameter .scale ) * Math.Cos(MathHelper.ToRadians(cParameter.angle))),
                    0f,
                    (float)((cParameter.rad + 200 * cParameter.scale) * Math.Sin(MathHelper.ToRadians(cParameter.angle))));

            boardScale = cParameter.scale;
            if (cParameter.modelType > 1)
                boardScale *= 2;

            if(cParameter.rad < 300 && doDrawDamage)
                boardPos = new Vector3 (
                    (float)(300 * Math.Cos(MathHelper.ToRadians(cParameter.angle))),
                    0f,
                    (float)(300 * Math.Sin(MathHelper.ToRadians(cParameter.angle))));

            enemyIcon.angle = cParameter.angle - 90;
            enemyIcon.position = Position;
        }

        /// <summary>
        /// 敵の動き方を決めるメソッド
        /// </summary>
        protected virtual void EnemyMove() { }

        protected virtual void EnemyMove(Position lightPosition, bool lightOn, BoundingSphere lightBoundingSphere) { }

        protected virtual void EnemyMove(Position lightPosition) { }

        /// <summary>
        /// collision(BoundingSphere)の設定
        /// </summary>
        protected virtual void SetCollision()
        {
            //船の中心
            enemyBoundingSphere[0].Center = Position;

            //船の前方
            enemyBoundingSphere[1].Center = new Vector3(
                (float)(Math.Cos(MathHelper .ToRadians (cParameter.angle - 90 + rotationAngle))),
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
        }

        /// <summary>
        /// 当たり判定の計算処理
        /// </summary>
        /// <param name="lightOn"></param>
        /// <param name="lightBoundingSphere"></param>
        /// <param name="doDrawEffect"></param>
        /// <param name="score"></param>
        protected void JudgeCollision(bool lightOn, BoundingSphere lightBoundingSphere, 
            ref bool doDrawEffect, ref int score)
        {
            if (lightOn && (lightBoundingSphere.Intersects(enemyBoundingSphere[0])
                    || lightBoundingSphere.Intersects(enemyBoundingSphere[1])
                    || lightBoundingSphere.Intersects(enemyBoundingSphere[2])))
            {
                isLightHit = true;

                cParameter.life--;
                //ダメージによる割合計算
                cParameter.scale = (cParameter.scaleMax -0.7f) * cParameter.life / cParameter.lifeMax + 0.7f;
                enemyModel.collisionRad = (enemyModel.collisionRadMax * cParameter.life) / cParameter.lifeMax;
                enemyModel.outsideRad = (enemyModel.outsideRadMax * cParameter.life) / cParameter.lifeMax;

                if (cParameter.scale <= 0.7f)
                    cParameter.scale = 0.7f;

                if (cParameter.life == 0)
                {
                    doDrawEffect = true;
                    score += cParameter.score;
                }
            }
            else
                isLightHit = false;
        }

        /// <summary>
        /// ボムによる敵動作の処理
        /// </summary>
        /// <param name="doDrawEffect"></param>
        /// <param name="score"></param>
        protected void BombCollision(ref bool doDrawEffect, ref bool doDrawDamage ,ref int score)
        {
            BoundingSphere bombBoundingSphere;
            bombBoundingSphere.Center = Vector3.Zero;
            bombBoundingSphere.Radius = 1000;

            if (bombBoundingSphere.Intersects(enemyBoundingSphere[0])
                    || bombBoundingSphere.Intersects(enemyBoundingSphere[1])
                    || bombBoundingSphere.Intersects(enemyBoundingSphere[2]))
            {
                cParameter.life = 0;
                doDrawEffect = true;
                doDrawDamage = false;
                score += cParameter.score;
            }
        }

        protected void LighthouseCollision()
        {
            BoundingSphere lighthouseBSphere;
            lighthouseBSphere.Center = Vector3.Zero;
            lighthouseBSphere.Radius = 15;

            if (lighthouseBSphere.Intersects(enemyBoundingSphere[0])
                || lighthouseBSphere.Intersects(enemyBoundingSphere[1])
                || lighthouseBSphere.Intersects(enemyBoundingSphere[2]))
            {
                cParameter.scale -= 0.01f;
                if (cParameter.scale <= 0)
                {
                    cParameter.life = 0;
                    collisionDamage = cParameter.power;
                }
            }

        }

        /// <summary>
        /// 敵を表示させる
        /// </summary>
        public void DrawEnemy(Matrix view, Matrix projection, Effect ghostShipEffect, float changeColor)
        {
            if (cParameter.life != 0 && countTime <= (180 - cParameter.spawnTime))
            {
                //船
                foreach (ModelMesh mesh in enemyModel.ship.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = ghostShipEffect;

                        ghostShipEffect.Parameters["View"].SetValue(view);
                        ghostShipEffect.Parameters["Projection"].SetValue(projection);
                        ghostShipEffect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                        ghostShipEffect.Parameters["ModelTexture"].SetValue(enemyModel.texture);
                        if (isLightHit)
                        {
                            ghostShipEffect.Parameters["ChangeColor"].SetValue(1);
                            ghostShipEffect.Parameters["ChangeAlpha"].SetValue(1);
                        }
                        else
                        {
                            ghostShipEffect.Parameters["ChangeColor"].SetValue(changeColor);
                            ghostShipEffect.Parameters["ChangeAlpha"].SetValue(changeAlpha);
                        }

                        Matrix worldInveseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                        ghostShipEffect.Parameters["WorldInverseTranspose"].SetValue(worldInveseTransposeMatrix);

                    }
                    mesh.Draw();
                }
            }
        }

        public void DrawEnemy(Matrix view, Matrix projection, float changeColor)
        {
            if (cParameter.life != 0 && countTime <= (180 - cParameter.spawnTime))
            {
                //船
                foreach (ModelMesh mesh in enemyModel.ship.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        if (isLightHit)
                        {
                            effect.LightingEnabled = true;
                            effect.AmbientLightColor = Vector3.One;
                            //effect.DiffuseColor = Vector3.One;
                            effect.Alpha = 1;
                        }
                        else
                        {
                            effect.LightingEnabled = true;
                            //effect.DiffuseColor = new Vector3 (changeColor ,changeColor ,1);
                            effect.AmbientLightColor = new Vector3(changeColor, changeColor, 1);
                            effect.Alpha = changeAlpha;
                        }

                        effect.FogEnabled = true;
                        effect.FogColor = Color.FromNonPremultiplied(new Vector4(0.4f, 0.4f, 1, 1)).ToVector3();
                        effect.FogStart = 0f;
                        effect.FogEnd = 12000f;

                        effect.View = view;
                        effect.Projection = projection;
                        effect.World = transform[mesh.ParentBone.Index] * world;
                    }
                    mesh.Draw();
                }
            }
        }

        public void DrawEnemy(Matrix view, Matrix projection, string effectTechniqueName)
        {
            if (cParameter.life != 0 && countTime <= (180 - cParameter.spawnTime))
            {
                //GraphicsDevice.BlendState = BlendState.Opaque;
                //GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                // モデルを描画します。
                foreach (ModelMesh mesh in enemyModel.ship.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        // 使用するエフェクト テクニックを指定します。
                        effect.CurrentTechnique = effect.Techniques[effectTechniqueName];

                        //Matrix localWorld = transform[mesh.ParentBone.Index] * world;

                        effect.Parameters["World"].SetValue(world);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                    }
                    //foreach (ModelMeshPart part in mesh.MeshParts)
                    //{
                    //    // 使用するエフェクト テクニックを指定します。
                    //    part.Effect = toonEffect;

                    //    //Matrix localWorld = transform[mesh.ParentBone.Index] * world;
                    //    toonEffect.CurrentTechnique = toonEffect.Techniques[effectTechniqueName];

                    //    toonEffect.Parameters["World"].SetValue(world);
                    //    toonEffect.Parameters["View"].SetValue(view);
                    //    toonEffect.Parameters["Projection"].SetValue(projection);
                    //}
                    mesh.Draw();
                }
            }
        }
    }
}
