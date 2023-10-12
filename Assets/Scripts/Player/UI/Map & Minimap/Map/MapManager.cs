using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    public GameObject mainCamera;
    public GameObject mapCamera;
    public GameObject mapIconsCamera;
    public GameObject gameHUD;
    public GameObject mapHUD;
    public GameObject tagMenu;
    public InputReader input;
    public GameObject removeTagButton;
    public bool isMapOpen = false;
    public bool isTagMenuOpen = false;
    public bool isRemoveButtonOpen = false;
    public bool isHovering = false;

    [SerializeField] private LayerMask layerMask;
	[SerializeField] private LayerMask layerMaskIcons;
	private Camera cam;
    private LayerMask oldMask;
    private MoveMapCamera moveMapCamera;
    private PlayerInput inputPlayer;
    private GamepadCursor gamepadCursor;
	private IconsInfo hoverIcon;
    private MarkerManager markerManager;
    private bool isRemoveButtonMoved = false;

	private const string gamepadScheme = "Gamepad";
	private const string mouseScheme = "KeyboardMouse";

	private void Awake()
	{
        if (instance == null)
		    instance = this;
	}

	void Start()
    {
        cam = Camera.main;
        moveMapCamera = GetComponent<MoveMapCamera>();
		inputPlayer = GetComponent<PlayerInput>();
        gamepadCursor = GetComponent<GamepadCursor>();
        markerManager = GetComponent<MarkerManager>();
	}

	private void LateUpdate()
	{
		if (isMapOpen && isTagMenuOpen == false && isRemoveButtonOpen == false)
        {
		    ShootRay();
        }
	}

	public void OpenMap()
    {
        StopAllCoroutines();
        StartCoroutine(TransitionOpen(1));
        gamepadCursor.freezeCursor = false;
        oldMask = cam.cullingMask;
        cam.cullingMask = layerMask;
        mainCamera.SetActive(false);
        mapIconsCamera.SetActive(true);
        mapCamera.SetActive(true);
        gameHUD.SetActive(false);
    }

    public void CloseMap()
    {
        UnfreezeCursor();
		StopAllCoroutines();
        StartCoroutine(TransitionClose(1));
	}

    IEnumerator TransitionOpen(float duration)
    {
        yield return new WaitForSeconds(duration);
		input.EnableMapInput();
        mapHUD.SetActive(true);
		moveMapCamera.canMoveCamera = true;
	}

    IEnumerator TransitionClose(float duration)
    {
        moveMapCamera.BackToOrigin();
        while (moveMapCamera.isReturningToOrigin == true)
            yield return 0;
		cam.cullingMask = oldMask;
		mainCamera.SetActive(true);
		mapIconsCamera.SetActive(false);
		mapCamera.SetActive(false);
		gameHUD.SetActive(true);
		input.DisableMapInput();
		mapHUD.SetActive(false);
		moveMapCamera.canMoveCamera = false;
		yield return new WaitForSeconds(duration);
    }

	private void ShootRay()
	{
		Ray ray = new Ray();
		RaycastHit hit;
		
        ray = Camera.main.ScreenPointToRay(moveMapCamera.cursor.transform.position);
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMaskIcons))
		{
            isHovering = true;
			hoverIcon = hit.transform.gameObject.GetComponent<IconsInfo>();
			hoverIcon.HoverFocus();
			if (inputPlayer.currentControlScheme == gamepadScheme)
			{
				RectTransform rect = hit.transform.gameObject.GetComponent<RectTransform>();
				moveMapCamera.cursor.transform.position = Camera.main.WorldToScreenPoint(rect.position);
			}
		}
		else
		{
			if (hoverIcon != null)
            {
                isHovering = false;
				hoverIcon.HoverUnfocus();
                hoverIcon = null;
            }
		}
	}

	void OnEnable()
    {
        input.openMapEvent += OpenCloseMap;
        input.openCloseTagMenuEvent += OnOpenTagMenu;
        input.clickMapEvent += RemoveTagButton;
        input.removeTagEvent += RemoveTagButtonGamepadPress;
        input.escapeRemoveTagEvent += EscapeRemoveTag;
    }

    void OnDisable()
    {
        input.openMapEvent -= OpenCloseMap;
		input.openCloseTagMenuEvent -= OnOpenTagMenu;
		input.clickMapEvent -= RemoveTagButton;
        input.removeTagEvent -= RemoveTagButtonGamepadPress;
        input.escapeRemoveTagEvent -= EscapeRemoveTag;
	}

    void OpenCloseMap()
    {
        isMapOpen = !isMapOpen;
        if (isMapOpen == true)
            OpenMap();
        else
            CloseMap();
    }

    public void OnOpenTagMenu()
    {
        if (isMapOpen)
        {
            if (isTagMenuOpen == false)
            {
                FreezeCursor();
            }
            else
            {
                UnfreezeCursor();
            }
        }
    }

    public void FreezeCursor()
    {
		isTagMenuOpen = true;
		gamepadCursor.freezeCursor = true;
		moveMapCamera.canMoveCamera = false;
		tagMenu.SetActive(isTagMenuOpen);
	}

    public void UnfreezeCursor()
    {
		isTagMenuOpen = false;
		gamepadCursor.freezeCursor = false;
		moveMapCamera.canMoveCamera = true;
		tagMenu.SetActive(isTagMenuOpen);
	}

	public void RemoveTagButton()
	{
		if (hoverIcon != null && hoverIcon.gameObject.tag == "Marker" && isTagMenuOpen == false)
		{
            RectTransform buttonTransform = removeTagButton.GetComponent<RectTransform>();
            if (buttonTransform.position.x + buttonTransform.rect.width > Screen.width)
            {
                buttonTransform.localPosition = new Vector2(-buttonTransform.localPosition.x, buttonTransform.localPosition.y);
                isRemoveButtonMoved = true;
            }
            gamepadCursor.freezeCursor = true;
            moveMapCamera.canMoveCamera = false;
            removeTagButton.SetActive(true);
            StartCoroutine(WaitFrame());
		}
	}

    private IEnumerator WaitFrame()
    {
        yield return new WaitForEndOfFrame();
		isRemoveButtonOpen = true;
	}

    public void RemoveTagButtonGamepadPress()
    {
        if (inputPlayer.currentControlScheme == gamepadScheme)
        {
            removeTagButton.GetComponent<Button>().onClick.Invoke();
        }
    }

    public void RemoveTag()
    {
        markerManager.RemoveMarkerInList(hoverIcon.gameObject.transform.parent.gameObject, hoverIcon.gameObject.transform.parent.name);
        hoverIcon = null;
        isHovering = false;
        EscapeRemoveTag();
    }

    public void EscapeRemoveTag()
    {
		removeTagButton.SetActive(false);
		isRemoveButtonOpen = false;
		if (isRemoveButtonMoved == true)
		{
			RectTransform buttonTransform = removeTagButton.GetComponent<RectTransform>();
			buttonTransform.localPosition = new Vector2(-buttonTransform.localPosition.x, buttonTransform.localPosition.y);
		}
		UnfreezeCursor();
	}
}
