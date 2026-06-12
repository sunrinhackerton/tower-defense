using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonsterMovement2D : MonoBehaviour
{
    public float speed = 3.0f;
    public float length = 10.0f;

    private float _startX;

    void Start()
    {
        _startX = transform.position.x;
    }

    void Update()
    {
        // PingPong으로 0~length 범위를 왕복, 중심 기준으로 -length/2 ~ +length/2 이동
        float offset = Mathf.PingPong(Time.time * speed, length) - (length * 0.5f);
        transform.position = new Vector3(_startX + offset, transform.position.y, transform.position.z);
    }
}
