using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    [SerializeField] private float speed;
    //[SerializeField] private ParticleSystem destroyParticle;

    void Update()
    {
        transform.Translate(speed * Vector2.up * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick") || collision.gameObject.CompareTag("DeathWall") || collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
