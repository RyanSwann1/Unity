using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private Vector3 m_moveVelocity;
    private Quaternion m_turnPoint;
    private Rigidbody m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        m_rigidbody.MovePosition(transform.position + m_moveVelocity * Time.fixedDeltaTime);
        m_rigidbody.MoveRotation(m_turnPoint);
    }

    public void Move(Vector3 newVelocity)
    {
        m_moveVelocity = newVelocity;
    }

    public void Turn(Vector3 newTurnPoint)
    {
        m_turnPoint = Quaternion.LookRotation(newTurnPoint);
    }
}
