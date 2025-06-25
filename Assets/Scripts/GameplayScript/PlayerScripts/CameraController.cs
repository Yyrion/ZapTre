using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform PlayerCameraTransform;
    private Camera _playerCamera;
    public Transform ShoulderBone;
    public float xSensitivity = 100f;
    public float ySensitivity = 100f;

    private InputAction _lookAction;
    private InputAction _shootAction;
    private InputAction _grapplinAction;
    private InputAction _reloadAction;

    private float yRotation = 0f;
    private float xRotation = 0f;

    [Header("Shoulder Bone Rotation Settings")]
    private float _minYRotation = -90f;
    private float _maxYRotation = 90f;
    private float _baseZ = 27.5f;
    private float _minZOffset = -70f;
    private float _maxZOffset = 75f;

    [Header("Particles")]
    public GameObject FlashEffect;
    public GameObject HitEffect;
    public Transform ShootingPoint;

    [Header("Grapplin")]
    private Vector3 _grapplePoint;
    private bool isGrappling = false;

    public float grappleForce = 50f;
    public float stopDistance = 2f;

    public LayerMask grappleLayerMask;

    [Header("Grappling Line")]
    public Transform grappleOrigin;
    public LineRenderer lineRenderer;

    [Header("FOV Effects")]
    private float normalFOV = 60f;
    private float grapplinFOV = 40f;

    [Header("WeaponAttributes")]
    public WeaponSO Weapon;
    private bool canShoot = true;
    private int _ammunitionsCurrentAmount;
    private bool _isReloading;
    public int DamageModifier;
    public float TimeModifier;
    public float ReloadModifier;

    private Rigidbody _rb;
    private Animator _animator;

    [Header("UI")]
    public TextMeshProUGUI ReloadText;

    [Header("Audio")]
    private AudioSource _audioSource;
    public AudioClip ReloadClip;
    public AudioClip ShootClip;

    void Start()
    {
        // Gestion du curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Management des Inputs Action
        _lookAction = InputSystem.actions.FindAction("Look");
        _shootAction = InputSystem.actions.FindAction("Shoot");
        _grapplinAction = InputSystem.actions.FindAction("Grapplin");
        _reloadAction = InputSystem.actions.FindAction("Reload");

        xRotation = transform.rotation.eulerAngles.y;

        //Components Load
        _rb = GetComponent<Rigidbody>();
        _playerCamera = PlayerCameraTransform.GetComponent<Camera>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        //Weapon Manager Start
        _ammunitionsCurrentAmount = Weapon.cartridgeAmount;
        ReloadText.text = $"{_ammunitionsCurrentAmount} / {Weapon.cartridgeAmount}";
    }

    private void FixedUpdate()
    {
        if (isGrappling)
        {
            _rb.useGravity = false;

            Vector3 direction = (_grapplePoint - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, _grapplePoint);

            if (distance > stopDistance)
            {
                Vector3 move = direction * grappleForce * Time.fixedDeltaTime;
                _rb.MovePosition(_rb.position + move);
            }
            else
            {
                isGrappling = false;
                lineRenderer.enabled = false;
                _rb.useGravity = true;
            }
        }
        else
        {
            _rb.useGravity = true;
        }
    }


    void Update()
    {
        if (!MasterScript.Master.IsGameOver)
        {
            Vector2 look = _lookAction.ReadValue<Vector2>() * new Vector2(xSensitivity, ySensitivity) * Time.deltaTime;

            xRotation += look.x;
            yRotation -= look.y;
            yRotation = Mathf.Clamp(yRotation, -70f, 70f);

            transform.rotation = Quaternion.Euler(0f, xRotation, 0f);
            PlayerCameraTransform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);

            if (_shootAction.WasPressedThisFrame() && canShoot && !_isReloading && _ammunitionsCurrentAmount > 0) { Shoot(); StartCoroutine(WeaponShootCooldown()); } else if (_ammunitionsCurrentAmount <= 0) { Reload(); }
            if (_grapplinAction.WasPressedThisFrame()) Grapplin();
            if (_reloadAction.WasPressedThisFrame()) Reload();

            if (_playerCamera != null)
            {
                float targetFOV = isGrappling ? grapplinFOV : normalFOV;
                _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, targetFOV, Time.deltaTime * 5f);
            }
        }
    }

    void LateUpdate()
    {
        float t = Mathf.InverseLerp(_minYRotation, _maxYRotation, yRotation);
        float zOffset = Mathf.Lerp(_minZOffset, _maxZOffset, t);
        float zRotation = _baseZ + zOffset;

        Vector3 currentEuler = ShoulderBone.localEulerAngles;
        ShoulderBone.localEulerAngles = new Vector3(currentEuler.x, currentEuler.y, zRotation);

        if (isGrappling)
        {
            lineRenderer.SetPosition(0, grappleOrigin.position);
            lineRenderer.SetPosition(1, _grapplePoint);
        }
    }

    private void Shoot()
    {
        _animator.SetLayerWeight(1, 1f);
        _animator.SetTrigger("Shoot"); 

        Ray ray = new Ray(PlayerCameraTransform.transform.position, PlayerCameraTransform.transform.forward);
        GameObject flashEffect = Instantiate(FlashEffect, ShootingPoint);
        Destroy(flashEffect, 0.1f);
        _audioSource.PlayOneShot(ShootClip);
        _ammunitionsCurrentAmount -= 1;
        ReloadText.text = $"{_ammunitionsCurrentAmount} / {Weapon.cartridgeAmount}";

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            HealthManager healthManager = hit.collider.GetComponentInParent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.TakeDamage(Weapon.damage + DamageModifier);
            }
            else
            {
                GameObject effect = Instantiate(HitEffect, hit.point, Quaternion.identity);
                Destroy(effect, 0.25f);
            }
        }
    }

    private void Grapplin()
    {
        if (isGrappling) { isGrappling = false;
            lineRenderer.enabled = false;
            _rb.useGravity = true; 
            return; }
        Ray ray = new Ray(PlayerCameraTransform.transform.position, PlayerCameraTransform.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, grappleLayerMask))
        {
            _grapplePoint = hit.point;
            isGrappling = true;

            lineRenderer.positionCount = 2;
            lineRenderer.enabled = true;
        }
    }

    private IEnumerator WeaponShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSecondsRealtime(Mathf.Max(Weapon.shootCooldown - TimeModifier, 0.05f));
        canShoot = true;
    }

    private void Reload()
    {
        if (!_isReloading && _ammunitionsCurrentAmount < Weapon.cartridgeAmount)
        {
            StartCoroutine(ReloadTimeCooldown());
        }
    }


    private IEnumerator ReloadTimeCooldown()
    {
        _isReloading = true;
        canShoot = false;
        ReloadText.text = "...";
        _audioSource.PlayOneShot(ReloadClip);
        yield return new WaitForSecondsRealtime(Mathf.Max(Weapon.reloadTime - ReloadModifier, 0.1f));

        _ammunitionsCurrentAmount = Weapon.cartridgeAmount;
        ReloadText.text = $"{_ammunitionsCurrentAmount} / {Weapon.cartridgeAmount}";
        Debug.Log($"{_ammunitionsCurrentAmount}");
        _isReloading = false;
        canShoot = true;
    }

}
