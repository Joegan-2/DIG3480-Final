using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : MonoBehaviour
{
    public AudioClip getClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController ruby = other.GetComponent<RubyController>();

        if (ruby != null)
        {

            ruby.speed = 3.5f;
            Destroy(gameObject);

            ruby.PlaySound(getClip);

        }
    }
}
