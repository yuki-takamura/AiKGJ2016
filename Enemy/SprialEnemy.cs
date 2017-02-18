using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lighthouse
{
    /// <summary>
    /// 螺旋を描きながら近づいてくる敵のクラス
    /// </summary>
    class SprialEnemy : Enemy
    {
        protected override void EnemyMove()
        {
            //TODO:　動き方を決める処理を作成する
            //中身を書き換えるだけ
            cParameter.rad -= speed;
        }
    }
}
