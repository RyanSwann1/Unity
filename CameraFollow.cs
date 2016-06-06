using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public float m_smoothing;
    private Transform m_target;
    private Vector3 m_offSet;

    private void Awake()
    {
        m_target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        //m_offSet = m_target.position - transform.position;
        m_offSet = transform.position - m_target.position;
    }

    private void FixedUpdate()
    {
        if(m_target != null)
        {
            Vector3 targetToCamPos = m_target.position + m_offSet;

            transform.position = Vector3.Lerp(transform.position, targetToCamPos, m_smoothing * Time.fixedDeltaTime);
        }
    }

}
