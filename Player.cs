using UnityEngine;
using System.Collections;

public class Player : LivingEntity {

    public float m_movementSpeed;

    private int m_floorMask;
    private GunController m_gunController;
    private PlayerController m_playerController;

    private void Awake()
    {
        m_floorMask = LayerMask.GetMask("Floor");
        m_gunController = GetComponent<GunController>();
        m_playerController = GetComponent<PlayerController>();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        Move();
        Turn();
        Shoot();
    }

    private void Move()
    {
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * m_movementSpeed;
        m_playerController.Move(moveVelocity);
    }

    private void Turn()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit camRayHit;
        if(Physics.Raycast(camRay, out camRayHit, m_floorMask))
        {
            Vector3 playerToMouse = camRayHit.point - transform.position;
            playerToMouse.y = 0;
            m_playerController.Turn(playerToMouse);
        }
    }

    private void Shoot()
    {
        if(Input.GetButton("Fire1"))
        {
            m_gunController.Shoot();
        }
    }


    
}
