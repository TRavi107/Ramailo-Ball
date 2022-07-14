using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpDeleter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            BallsController other = collision.collider.GetComponent<BallsController>();
            if (other.myType != PowerUpType.none)
            {
                Destroy(other.gameObject);
                switch (other.myType)
                {
                    case PowerUpType.timeadd:
                        GameManager.instance.AddTime(5);
                        SoundManager.instance.PlaySound(SoundType.powerUps);
                        break;
                    case PowerUpType.scoreadd:
                        GameManager.instance.AddScore(100);
                        SoundManager.instance.PlaySound(SoundType.powerUps);
                        break;
                    case PowerUpType.deleteTime:
                        SoundManager.instance.PlaySound(SoundType.powerUps);
                        break;
                    case PowerUpType.none:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
