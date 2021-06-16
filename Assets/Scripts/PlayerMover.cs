using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerMover : MonoBehaviour
{
    [SerializeField] float trust = 150f;
    [SerializeField] ParticleSystem particle;
    bool isMove = true;
    bool isAlive = true;

    VariableJoystick joystick;

    Transform upPoint;
    Transform lowPoint;

    Rigidbody rb;
    Vector3 direction;
    float xClamp;

    void Start()
    {
        EventsManager.current.onGameStateChangeTrigger += GameStateChange;

        upPoint = CameraMover.current.TopPoint;
        lowPoint = CameraMover.current.LowerPoint;

        xClamp = GameManager.current.LevelWidth;
        rb = GetComponent<Rigidbody>();
        joystick = FindObjectOfType<VariableJoystick>();

        OnOffParticle(true);
    }

    private void OnDestroy()
    {
        EventsManager.current.onGameStateChangeTrigger -= GameStateChange;
    }

    private void GameStateChange(EventsManager.GameState state)
    {
        switch (state)
        {
            case EventsManager.GameState.Start:
                isMove = true;
                break;

            case EventsManager.GameState.Win:
                isMove = false;
                break;

            case EventsManager.GameState.GameOver:
                isMove = false;
                break;

            default:
                break;
        }
    }

    void Update()
    {
        if (isMove)
        {
            float horizontal = joystick.Horizontal;
            float vertical = joystick.Vertical;

            if (horizontal > 0 || vertical > 0)
            {
                var em = particle.emission;
                em.enabled = true;
            }
            else
            {
                var em = particle.emission;
                em.enabled = false;
            }

            direction = new Vector3(horizontal, 0f, vertical) * trust;

            //Debug.Log($"horizontal {horizontal}    vertical {vertical}");
        }
    }

    private void FixedUpdate()
    {
        if (isMove)
        {
            rb.velocity = direction * Time.deltaTime;
        }

        float posX = transform.position.x;
        float posZ = transform.position.z;
        transform.position = new Vector3(Mathf.Clamp(posX, -xClamp, xClamp), transform.position.y, Mathf.Clamp(posZ, lowPoint.position.z, upPoint.position.z));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAlive)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                isMove = false;
                OnOffParticle(isMove);
                isAlive = isMove;
                EventsManager.current.GameStateChangeTrigger(EventsManager.GameState.GameOver);
            }
            else if (other.gameObject.CompareTag("End"))
            {
                isMove = false;
                OnOffParticle(isMove);
                isAlive = isMove;
                EventsManager.current.GameStateChangeTrigger(EventsManager.GameState.Win);
            }
        }
    }

    void OnOffParticle(bool isEnabled)
    {
        if (isEnabled)
        {
            var em = particle.emission;
            em.enabled = true;
        }
        else
        {
            var em = particle.emission;
            em.enabled = false;
        }
    }
}
