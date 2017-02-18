using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using System.Runtime.InteropServices;

namespace WindowsGame10
{
    class Ai: Character 
    {
        ///<summary>
        ///指定範囲の乱数生成
        ///min以上からmax以下の乱数
        ///</summary>
        [DllImport("GenerateMTRandomNumber.dll")]
        static extern int GenMTRandom(int min, int max);

        public BoundingBox detectBoundingBox;
        private Vector3 boundingMin;
        private Vector3 boundingMax;
        int type;
        int typeRange;

        public Ai(Vector3 setPosition) :base(setPosition)
        {
            // 各種座標データを初期化
            InitializeCoordinateValue(position);
            // アニメーション用データを初期化
            InitializeAnimationValue();

            boundingMin = new Vector3(-40f, 0f, -160f);
            boundingMax = new Vector3(40f, 180f, 0f);

            detectBoundingBox.Min = boundingMin + position;
            detectBoundingBox.Max = boundingMax + position;
        }

        private void UpdateDetect()
        {
            int rand = GenMTRandom(0, typeRange);

            boundingMin = new Vector3(-40f, 0f, -(float)rand * 10);

            boundingMax = new Vector3(40f, 180f, -10);

            detectBoundingBox.Min = boundingMin + position;
            detectBoundingBox.Max = boundingMax + position;
        }

        public void SetType()
        {
            type = GenMTRandom(0, 2);
            switch (type)
            {
                case 0:
                    typeRange = 65;
                    break;
                case 1:
                    typeRange = 25;
                    break;
                case 2:
                    typeRange = 12;
                    break;
                default:
                    typeRange = 0;
                    break;
            }

        }

        /// <summary>
        /// モデルの座標を更新
        /// </summary>
        public override void UpdateModelCoordinates(GameTime gameTime, bool canJump)
        {
            // 移動速度として取得
            float velocity = (float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.15f;

            modelBox.Min = new Vector3(-40f, 0f, -40f) + position;
            modelBox.Max = new Vector3(40f, 180f, 40f) + position;

            UpdateDetect();

            // 計算用のベクトル
            Vector3 vec = Vector3.Zero;

            Vector3 vec2 = Vector3.One * 80;

            #region ジャンプ

            if (canJump && (onGround || onBox))
            {
                acceleration = jumpPower;
                onGround = false;
                onBox = false;
                ChangeAnimationClip("jump", true, 0);
            }

            if (onGround == false)
            {
                acceleration--;
                position.Y += acceleration;
            }

            #endregion

            if (vec == Vector3.Zero && (onGround || onBox))
            {
                ChangeAnimationClip("run", true, 0);
            }

            if (onGround == false || onGround == false)
            {
                vec.X += gravVec.X;
                vec.Y -= gravVec.Y;
                vec.Z += gravVec.Z;
            }

            // 入力があったときのみ処理する(入力がなければ長さは0.0fとなるため)
            if (vec.Length() > 0.0f)
            {
                // 移動する
                position += vec * velocity;
                modelBox.Min += vec * velocity;
                modelBox.Max += vec * velocity;

                if (position.Y < 0 && onGround == false)
                {
                    position.Y = 0;
                    onGround = true;
                    onBox = false;
                    vec = Vector3.Zero;
                }
            }

            // 回転行列の作成
            Matrix rotationMatrix = Matrix.CreateRotationY(rotation.Y);

            // 平行移動行列の作成
            Matrix translationMatrix = Matrix.CreateTranslation(position);

            // ワールド変換行列を計算する
            // モデルを拡大縮小し、回転した後、指定の位置へ移動する。
            worldMatrix = rotationMatrix * translationMatrix;

            oldKeyInput = keyInput;

            // 初期値に戻す
            if (InputManager.IsJustButtonDown(PlayerIndex.One, Buttons.RightStick)
                || InputManager.IsJustKeyDown(Keys.R))
            {
                // 各種座標データを初期化
                InitializeCoordinateValue();
            }
        }
    }
}
