using System.Collections.Generic;
using UnityEngine;

public class SingleUnit : MonoBehaviour
{
    private Collider _collider;
    public Animator animator;

    private Vector3 _position;
    private bool isMoving = false;
    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_collider != null)
        {
            Debug.Log(collision.gameObject.name);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_collider != null)
        {
            Debug.Log(other.gameObject.name);
            if (other.CompareTag("UnitToAdd"))
            {
                PlayerCluster.Instance.AddUnit(other.transform.position);
                Destroy(other.gameObject);
                PoolManager.GetObject("JoinEffect").InitializeObject(transform.position);
            }
            if (other.CompareTag("Saw"))
            {
                PoolManager.GetObject("SawEffect").InitializeObject(transform.position);
                Die(1);
            }
            if (other.CompareTag("Mine"))
            {
                PoolManager.GetObject("MineEffect").InitializeObject(transform.position);
                other.gameObject.SetActive(false);
                List<SingleUnit> utitsToKill = new List<SingleUnit>();
                foreach (var u in PlayerCluster.Instance.unitDataList)
                {
                    if(Vector3.Distance(u.transform.position, transform.position) < 0.15f)
                    {
                        utitsToKill.Add(u);
                    }
                }
                foreach (var u in utitsToKill)
                    u.Die(0);
            }
        }
    }
    private void Update()
    {
        if (!isMoving) return;

        if (Vector3.Distance(transform.localPosition, _position) > 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _position, Time.deltaTime * moveSpeed);
        }
        else
        {
            isMoving = false;
        }
    }

    internal void MoveToPosition(Vector3 vector3)
    {
        _position = vector3;
        isMoving = true;
    }
    internal void Die(int reason)
    {
        switch (reason)
        {
            case 0:
                {
                    animator.SetTrigger("isDeath0");
                    break;
                }
            case 1:
                {
                    animator.SetTrigger("isDeath1");
                    break;
                }
            default:
                {
                    animator.SetTrigger("isDeath");
                    break;
                }
        }
        PlayerCluster.Instance.KillUnit(this);
    }
}
