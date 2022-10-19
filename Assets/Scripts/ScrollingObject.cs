using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingObject : MonoBehaviour
{
    public float speed = 10f;
    public float distance = 4f;
    private float width;

    void Awake()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        width = box.size.x;
    }
    void Update()
    {
        if (NetworkManager.gamestartcheck == true)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            if (transform.position.x <= -width * 2)
            {
                Reposition();
            }
        }
    }

    void Reposition()
    {
        Debug.Log(width);
        Vector2 offset = new Vector2(width * distance, 0);
        transform.position = (Vector2)transform.position + offset;
    }
}
