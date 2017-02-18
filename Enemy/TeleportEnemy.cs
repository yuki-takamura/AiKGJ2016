
namespace Lighthouse
{
    class TeleportEnemy : Enemy
    {
        private bool firstHit = false;

        protected override void EnemyMove(Position lightPosition)
        {
            cParameter.rad -= speed;

            if (firstHit == false && isLightHit)
            {
                firstHit = true;

                cParameter.angle = lightPosition.angle + 180;
                cParameter.rad += 200;
            }
            if (firstHit)
                changeAlpha = 1;
        }
    }
}
