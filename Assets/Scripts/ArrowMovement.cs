using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    [SerializeField]
    private Transform _initialPos;

    [SerializeField]
    private Transform _targetPos;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private bool _activeArrow;

    public void Activate()
    {
        _activeArrow = true;     
        this.gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        _activeArrow = false;
        this.gameObject.SetActive(false);
        transform.position = _initialPos.position;
    }

    private void Update()
    {
        if (!_activeArrow) return;

        Move();                  
        if (  Vector2.Distance(transform.position, _targetPos.position) <= 1f  ) Deactivate();

    }

    public void Move() => transform.position += new Vector3(-1, 0, 0) * Time.deltaTime * _speed;

}
