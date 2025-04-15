using UnityEngine;
using UnityEngine.TextCore.Text;

public class WeaponSway : InterfaceItem
{

    CharacterController cc;
    PlayerMovement mover;

    [Header("Sway")]
    [SerializeField] float step = 0.01f;
    [SerializeField] float maxStepDistance = 0.06f;
    Vector3 swayPos;

    [Header("Sway Rotation")]
    [SerializeField] float rotationStep = 4f;
    [SerializeField] float maxRotationStep = 5f;
    Vector3 swayEulerRot;

    [SerializeField] float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    [SerializeField] float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    [SerializeField] Vector3 travelLimit = Vector3.one * 0.025f;
    [SerializeField] Vector3 bobLimit = Vector3.one * 0.01f;
    Vector3 bobPosition;

    [SerializeField] float bobExaggeration;

    [Header("Bob Rotation")]
    [SerializeField] Vector3 multiplier;
    Vector3 bobEulerRotation;

    protected override void Unsubscribe(PlayerBody body)
    {
        var mover = body.PlayerMovement;
        if (mover != null)
        {
            mover.OnAimUpdated -= UpdateLookInput;
            mover.OnMoveUpdated -= UpdateWalkInput;
        }
    }

    protected override void Subscribe(PlayerBody body)
    {
        var mover = body.PlayerMovement;
        mover.OnAimUpdated += UpdateLookInput;
        mover.OnMoveUpdated += UpdateWalkInput;

        this.mover = mover;
        cc = mover.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();

        CompositePositionRotation();
    }

    public void UpdateWalkInput(Vector3 moveInput)
    {
        walkInput = new Vector2(moveInput.x, moveInput.z);
    }

    public void UpdateLookInput(Vector2 lookInput)
    {
        this.lookInput = lookInput;
    }


    Vector2 walkInput;
    Vector2 lookInput;

    //void GetInput()
    //{
    //    walkInput.x = Input.GetAxis("Horizontal");
    //    walkInput.y = Input.GetAxis("Vertical");
    //    walkInput = walkInput.normalized;

    //    lookInput.x = Input.GetAxis("Mouse X");
    //    lookInput.y = Input.GetAxis("Mouse Y");
    //}


    void Sway()
    {
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation()
    {
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation), Time.deltaTime * smoothRot);
    }

    void BobOffset()
    {
        if (cc == null) return;
        speedCurve += Time.deltaTime * (cc.isGrounded ? (walkInput.x + walkInput.y) * bobExaggeration : 1f) + 0.01f;

        bobPosition.x = (curveCos * bobLimit.x * (cc.isGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x);
        bobPosition.y = (curveSin * bobLimit.y) - (walkInput.y * travelLimit.y);
        bobPosition.z = -(walkInput.y * travelLimit.z);
    }

    void BobRotation()
    {
        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0);
    }

}