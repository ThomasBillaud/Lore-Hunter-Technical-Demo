using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    //Everything related to animation
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    int isRollingHash;
    int isPickingUpHash;
    int isChoppingHash;
    int isMiningHash;
    int isInteractingHash;
    int isUsingItemHash;
    int useBandageHash;
    int velocityHash;
    int isExhaustedHash;
    int isPressPossibleHash;

    //Everything related to the inputs
    [Header("Inputs")]
    [SerializeField] private InputReader _inputReader = default;
    Vector2 currentMovement;
    bool movementPressed;
    bool runPressed;
    bool rollPressed;
    bool hotbarActive;
    bool isRotatable = true;
    public float acceleration = 0.1f;
    public float deceleration = 0.5f;

    [Header("Life and Stamina")]
    public Stamina stamina;

    //Everything related to the inventory
    [Header("Inventory")]
    public Inventory inventory;
    public GameObject itemPrefab;

    //Everything related to the head rig
    [Header("Head rig")]
    public Camera camera;
    public GameObject fovStartPoint;
    public float lookSpeed = 200f;
    public float maxAngle = 75f;
    float rotationFactorPerFrame = 10.0f;

    //The tools the player can use in animations
    [Header("Tools")]
    public GameObject axe;
    public GameObject pickaxe;

    //Everything related to the climbing
    [Header("Climb settings")]
    [SerializeField] private float _wallAngleMax;
    [SerializeField] private float _groundAngleMax;
    [SerializeField] private LayerMask _layerMaskClimb;
    [SerializeField] private Vector3 _endOffset;
    [SerializeField] private Vector3 _climbOriginDown;
    private CharacterController _player;
    bool isClimbing = false;
    private Vector3 _endPosition;
    private RaycastHit _downRaycastHit;
    private RaycastHit _forwardRaycastHit;
    private Vector3 _matchTargetPosition;
    private Quaternion _matchTargetRotation;
    private Quaternion _forwardNormalXZRotation;
    private MatchTargetWeightMask _weightMask = new MatchTargetWeightMask(Vector3.one, 1);
    private float _matchTargetBegin;
    private float _matchTargetEnd;

    [Header("Heights")]
    [SerializeField] private float _overpassHeight;
    [SerializeField] private float _bigClimbHeight;
    [SerializeField] private float _smallClimbHeight;

    [Header("Climb animation settings")]
    public CrossFadeSettings _bigClimbSettings;
    public CrossFadeSettings _smallClimbSettings;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        _player = GetComponent<CharacterController>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isRollingHash = Animator.StringToHash("isRolling");
        isPickingUpHash = Animator.StringToHash("isPickingUp");
        isChoppingHash = Animator.StringToHash("isChopping");
        isMiningHash = Animator.StringToHash("isMining");
        isInteractingHash = Animator.StringToHash("isInteracting");
        isUsingItemHash = Animator.StringToHash("isUsingItem");
        useBandageHash = Animator.StringToHash("useBandage");
        velocityHash = Animator.StringToHash("velocity");
        isExhaustedHash = Animator.StringToHash("isExhausted");
        isPressPossibleHash = Animator.StringToHash("isPressPossible");
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        handleRotation();
    }

    void onMovementInput (InputAction.CallbackContext ctx)
    {
        currentMovement = ctx.ReadValue<Vector2>();
        movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
    }

    void handleMovement() 
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRolling = animator.GetBool(isRollingHash);
        bool isInteracting = animator.GetBool(isInteractingHash);
        bool isPickingUp = animator.GetBool(isPickingUpHash);
        bool isUsingItem = animator.GetBool(isUsingItemHash);
        bool isExhausted = animator.GetBool(isExhaustedHash);

        if (!isInteracting || isPickingUp) 
        {
            animator.SetFloat("velocityX", currentMovement.x);
            animator.SetFloat("velocityY", currentMovement.y);
            if (!movementPressed && isWalking)
                animator.SetBool(isWalkingHash, false);
            if (!isExhausted)
            {
                if (movementPressed && !isWalking)
                    animator.SetBool(isWalkingHash, true);
                if ((movementPressed && runPressed) && stamina.value > 0)
                {
                    stamina.regenStam = false;
                    stamina.value -= 0.25f;
                    if (!isRunning)
                        animator.SetBool(isRunningHash, true);
                }
                if ((!movementPressed || !runPressed) && isRunning)
                {
                    Debug.Log("Stamina value = " + stamina.value);
                    if (stamina.value == 0) {
                        animator.SetBool(isExhaustedHash, true);
                    }
                    stamina.regenStam = true;
                    animator.SetBool(isRunningHash, false);
                }
                if (movementPressed && !isClimbing)
                {
                    if (CanClimb(out _downRaycastHit, out _forwardRaycastHit, out _endPosition))
                    {
                        Debug.Log("Je peux grimper en face !");
                        InitiateClimb();
                    }
                }
            }
        }
    }

    void handleRotation() 
    {
        if (isRotatable)
        {
            Vector3 positionToLookAt;
            positionToLookAt.x = currentMovement.x;
            positionToLookAt.y = 0f;
            positionToLookAt.z = currentMovement.y;
            Quaternion currentRotation = transform.rotation;
            if (movementPressed) {
                Quaternion targetRotation = Quaternion.LookRotation(Quaternion.Euler(0, camera.transform.eulerAngles.y, 0) * positionToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
            }
        }
    }

    void handleInteraction()
    {
        float range = 2f;
        string[] validTags = {"Object", "Tree", "Mineral", "NPC", "ItemBox"};
        Collider[] itemsAroundThePlayer = Physics.OverlapSphere(transform.position, 2f);
        itemsAroundThePlayer = itemsAroundThePlayer.Where(val => validTags.Contains(val.tag)).ToArray();
        GameObject closestItem = null;
        foreach (var hitCollider in itemsAroundThePlayer)
        {
            float dist = Vector3.Distance(new Vector3(hitCollider.transform.position.x, transform.position.y, hitCollider.transform.position.z), transform.position);
            if (dist < range && isItemInFieldOfView(fovStartPoint, hitCollider.gameObject))
            {
                range = dist;
                closestItem = hitCollider.gameObject;
            }
        }
        Debug.Log("tag = " + closestItem.tag);
        if (closestItem != null)
        {
            if (closestItem.tag == "Object") {
                animator.SetBool(isInteractingHash, true);
                ObtainItem(closestItem.GetComponent<LootTable>());
                animator.SetLayerWeight(1, 1f);
                animator.SetBool(isPickingUpHash, true);
            } else if (closestItem.tag == "Tree") {
                animator.SetBool(isInteractingHash, true);
                ObtainItem(closestItem.GetComponent<LootTable>());
                animator.SetBool(isChoppingHash, true);
            } else if (closestItem.tag == "Mineral") {
                animator.SetBool(isInteractingHash, true);
                ObtainItem(closestItem.GetComponent<LootTable>());
                animator.SetBool(isMiningHash, true);
            } else if (closestItem.tag == "NPC" && closestItem.GetComponent<DialogueTrigger>() != null) {
                closestItem.GetComponent<DialogueTrigger>().TriggerDialogue();
            } else if (closestItem.tag == "ItemBox") {
                FindObjectOfType<InventoryManager>().OnItemBoxInventory();
            }
        }
    }

    public void handleItemUsageAnimation(ItemData.consumeAnim animation)
    {
        switch(animation)
        {
            case ItemData.consumeAnim.None:
                break;
            case ItemData.consumeAnim.Bandage:
                animator.SetLayerWeight(2, 1f);
                animator.SetBool(isUsingItemHash, true);
                animator.SetBool(useBandageHash, true);
                break;
            default:
                break;
        }
    }

    private bool CanClimb(out RaycastHit downRaycastHit, out RaycastHit forwardRaycastHit, out Vector3 endPosition)
    {
        endPosition = Vector3.zero;
        downRaycastHit = new RaycastHit();
        forwardRaycastHit = new RaycastHit();

        bool _downHit;
        bool _forwardHit;
        bool _overpassHit;
        float _climbHeight;
        float _groundAngle;
        float _wallAngle;

        RaycastHit _downRaycastHit;
        RaycastHit _forwardRaycastHit;
        RaycastHit _overpassRaycastHit;

        Vector3 _endPosition;
        Vector3 _forwardDirectionXZ;
        Vector3 _forwardNormalXZ;

        Vector3 _downDirection = Vector3.down;
        Vector3 _downOrigin = transform.TransformPoint(_climbOriginDown);

        _downHit = Physics.Raycast(_downOrigin, _downDirection, out _downRaycastHit, _climbOriginDown.y - _smallClimbHeight, _layerMaskClimb);
        Debug.DrawRay(_downOrigin, _downDirection, Color.blue);
        if (_downHit)
        {
            float _forwardDistance = _climbOriginDown.z;
            Vector3 _forwardOrigin = new Vector3(transform.position.x, _downRaycastHit.point.y - 0.1f, transform.position.z);
            Vector3 _overpassOrigin = new Vector3(transform.position.x, _overpassHeight, transform.position.z);

            _forwardDirectionXZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            _forwardHit = Physics.Raycast(_forwardOrigin, _forwardDirectionXZ, out _forwardRaycastHit, _forwardDistance, _layerMaskClimb);

            Debug.DrawRay(_forwardOrigin, _forwardDirectionXZ, Color.red);
            _overpassHit = Physics.Raycast(_overpassOrigin, _forwardDirectionXZ, out _overpassRaycastHit, _forwardDistance, _layerMaskClimb);
            Debug.DrawRay(_overpassOrigin, _forwardDirectionXZ, Color.green);
            _climbHeight = _downRaycastHit.point.y - transform.position.y;

            if (_forwardHit)
            {
                if (_overpassHit || _climbHeight < _overpassHeight)
                {
                    _forwardNormalXZ = Vector3.ProjectOnPlane(_forwardRaycastHit.normal, Vector3.up);
                    _groundAngle = Vector3.Angle(_downRaycastHit.normal, Vector3.up);
                    _wallAngle = Vector3.Angle(-_forwardNormalXZ, _forwardDirectionXZ);

                    if (_wallAngle <= _wallAngleMax)
                    {
                        if (_groundAngle <= _groundAngleMax)
                        {
                            Vector3 _vectSurface = Vector3.ProjectOnPlane(_forwardDirectionXZ, _downRaycastHit.normal);
                            _endPosition = _downRaycastHit.point + Quaternion.LookRotation(_vectSurface, Vector3.up) * _endOffset;

                            Collider _colliderB = _downRaycastHit.collider;
                            bool _penetrationOverlap = Physics.ComputePenetration(
                                colliderA: _player,
                                positionA: _endPosition,
                                rotationA: transform.rotation,
                                colliderB: _colliderB,
                                positionB: _colliderB.transform.position,
                                rotationB: _colliderB.transform.rotation,
                                direction: out Vector3 _penetrationDirection,
                                distance: out float _penetrationDistance);
                            if (_penetrationOverlap)
                                _endPosition += _penetrationDirection * _penetrationDistance;
                            
                            float _inflate = -0.05f;
                            float _upSweepDistance = _downRaycastHit.point.y - transform.position.y;
                            Vector3 _upSweepDirection = transform.up;
                            Vector3 _upSweepOrigin = transform.position;
                            bool _upSweepHit = CharacterSweep(
                                position: _upSweepOrigin,
                                rotation: transform.rotation,
                                direction: _upSweepDirection,
                                distance: _upSweepDistance,
                                layerMask: _layerMaskClimb,
                                inflate: _inflate);
                            
                            Vector3 _forwardSweepOrigin = transform.position + _upSweepDirection * _upSweepDistance;
                            Vector3 _forwardSweepVector = _endPosition - _forwardSweepOrigin;
                            bool _forwardSweepHit = CharacterSweep(
                                position: _forwardSweepOrigin,
                                rotation: transform.rotation,
                                direction: _forwardSweepVector.normalized,
                                distance: _forwardSweepVector.magnitude,
                                layerMask: _layerMaskClimb,
                                inflate: _inflate);
                            
                            if (!_upSweepHit && !_forwardSweepHit)
                            {
                                endPosition = _endPosition;
                                downRaycastHit = _downRaycastHit;
                                forwardRaycastHit = _forwardRaycastHit;
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool CharacterSweep(Vector3 position, Quaternion rotation, Vector3 direction, float distance, LayerMask layerMask, float inflate)
    {
        float _heightScale = Mathf.Abs(transform.lossyScale.y);
        float _radiusScale = Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));

        float _radius = _player.radius * _radiusScale;
        float _totalHeight = Mathf.Max(_player.height * _heightScale, _radius * 2);

        Vector3 _capsuleUp = rotation * Vector3.up;
        Vector3 _center = position + rotation * _player.center;
        Vector3 _top = _center + _capsuleUp * (_totalHeight / 2 - _radius);
        Vector3 _bottom = _center - _capsuleUp * (_totalHeight / 2 - _radius);

        bool _sweepHit = Physics.CapsuleCast(
            point1: _bottom,
            point2: _top,
            radius: _radius,
            direction: direction,
            maxDistance: distance,
            layerMask: layerMask);

        return _sweepHit;
    }

    private void InitiateClimb()
    {
        isClimbing = true;
        //_player.enabled = false;

        float _climbHeight = _downRaycastHit.point.y - transform.position.y;
        Vector3 _forwardNormalXZ = Vector3.ProjectOnPlane(_forwardRaycastHit.normal, Vector3.up);
        _forwardNormalXZRotation = Quaternion.LookRotation(-_forwardNormalXZ, Vector3.up);

        if (_climbHeight > _bigClimbHeight)
        {
            Debug.Log("Grand");
            _matchTargetPosition = _endPosition;
            _matchTargetRotation = _forwardNormalXZRotation;
            _matchTargetBegin = 0f;
            _matchTargetEnd = 0.9f;
            animator.CrossFadeInFixedTime(_bigClimbSettings);
        }
        else if (_climbHeight > _smallClimbHeight)
        {
            Debug.Log("Moyen");
            _matchTargetPosition = _endPosition;
            _matchTargetRotation = _forwardNormalXZRotation;
            _matchTargetBegin = 0f;
            _matchTargetEnd = 0.65f;
            animator.CrossFadeInFixedTime(_smallClimbSettings);
        }
        else
        {
            isClimbing = false;
            _player.enabled = true;
        }
    }

    bool isItemInFieldOfView(GameObject looker, GameObject item)
    {
        Vector3 targetDir = item.transform.position - looker.transform.position;
        float angle = Vector3.Angle(targetDir, looker.transform.forward);
    
        if (angle < maxAngle)
            return true;
        else
            return false;
    }

    public void PickaxeDisappear()
    {
        if (!animator.GetBool(isInteractingHash) && !animator.GetBool(isMiningHash))
            pickaxe.SetActive(false);
    }

    public void ObtainItem(LootTable lootTable)
    {
        Loot item = lootTable.DiceRoll();
        int quantity = Random.Range(item.minQuantity, item.maxQuantity);
        inventory.AddItem(item.item, quantity);
        
    }

    public void EndRoll()
    {
        stamina.regenStam = true;
        animator.SetBool(isRollingHash, false);
    }

    public void EndPickingUp()
    {
        animator.SetBool(isInteractingHash, false);
        animator.SetBool(isPickingUpHash, false);
        animator.SetLayerWeight(1, 0f);
    }

    public void EndUseBandage()
    {
        animator.SetBool(isUsingItemHash, false);
        animator.SetBool(useBandageHash, false);
        animator.SetLayerWeight(2, 0f);
    }

    public void EndChopping()
    {
        animator.SetBool(isInteractingHash, false);
        animator.SetBool(isChoppingHash, false);
    }

    public void BeginClimbing()
    {
        animator.MatchTarget(_matchTargetPosition, _matchTargetRotation, AvatarTarget.Root, _weightMask, _matchTargetBegin, _matchTargetEnd);
        EndRoll();
        CannotRotate();
    }

    public void EndClimbing()
    {
        isClimbing = false;
        CanRotate();
        //_player.enabled = true;
    }

    public void AxeAppear()
    {
        axe.SetActive(true);
    }

    public void AxeDisappear()
    {
        axe.SetActive(false);
    }

    public void EndMining()
    {
        animator.SetBool(isInteractingHash, false);
        animator.SetBool(isMiningHash, false);
    }

    public void PickaxeAppear()
    {
        pickaxe.SetActive(true);
    }

    public void CanRotate()
    {
        isRotatable = true;
    }

    public void CannotRotate()
    {
        isRotatable = false;
    }

    public void EndExhaust()
    {
        animator.SetBool(isExhaustedHash, false);
    }

    void OnEnable()
    {
        _inputReader.moveEvent += OnMove;
        _inputReader.runEvent += OnRun;
        _inputReader.rollEvent += OnRoll;
        _inputReader.interactEvent += OnInteract;
    }

    void OnDisable()
    {
        _inputReader.moveEvent -= OnMove;
        _inputReader.runEvent -= OnRun;
        _inputReader.rollEvent -= OnRoll;
        _inputReader.interactEvent -= OnInteract;
    }

    //Events listener
    private void OnMove(Vector2 movement)
    {
        currentMovement = movement;
        movementPressed = currentMovement.x != 0 || currentMovement.y != 0;
    }

    private void OnRun(bool isRunning)
    {
        runPressed = isRunning;
    }

    private void OnRoll()
    {
        if (stamina.value > 25 && animator.GetBool(isPressPossibleHash) == true)
        {
            stamina.value -= 25;
            stamina.regenStam = false;
            animator.SetBool(isRollingHash, true);
        }
    }

    private void OnInteract()
    {
        handleInteraction();
    }

}
