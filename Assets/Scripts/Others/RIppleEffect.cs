using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RIppleEffect : MonoBehaviour
{
    void Start()
    {
        transform.localScale = Vector3.one * 0.15f;
        StartCoroutine(nameof(EffectCOur));
    }

    IEnumerator EffectCOur()
    {
        while (transform.localScale.x <= .5)
        {
            transform.localScale += Vector3.one * 0.05f;
            GetComponent<SpriteRenderer>().color = new Color
            {
                r = GetComponent<SpriteRenderer>().color.r,
                g = GetComponent<SpriteRenderer>().color.g,
                b = GetComponent<SpriteRenderer>().color.b,
                a = GetComponent<SpriteRenderer>().color.a -0.1f,
            };
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(this.gameObject);
    }
}
