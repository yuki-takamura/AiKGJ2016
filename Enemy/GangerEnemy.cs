
namespace Lighthouse
{
    /// <summary>
    /// 影分身(高速横移動)をしながら
    /// プレイヤーに向かってくる敵のクラス
    /// </summary>
    class GangerEnemy : Enemy
    {
        private int enemyRad;

        protected override void EnemyMove()
        {
            cParameter.rad -= speed;

            if (isLightHit == false)
            {
                enemyRad++;
                enemyRad %= 2;

                switch (enemyRad)
                {
                    case 0:
                        cParameter.angle -= 12;
                        break;
                    case 1:
                        cParameter.angle += 12;
                        break;
                }
            }
        }
    }
}
