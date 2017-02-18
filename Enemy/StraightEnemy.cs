
namespace Lighthouse
{
    /// <summary>
    /// 直進してくる敵のクラス
    /// </summary>
    class StraightEnemy : Enemy
    {
        protected override void EnemyMove()
        {
            cParameter.rad -= speed;
        }
    }
}
