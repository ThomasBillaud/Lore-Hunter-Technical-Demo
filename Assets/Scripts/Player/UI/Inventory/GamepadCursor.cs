using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

using Debug = UnityEngine.Debug;

public class GamepadCursor : MonoBehaviour
{
    [SerializeField] private PlayerInput input;
    [SerializeField] private GameObject cursor;
    [SerializeField] private float cursorSpeed = 1000f;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float padding = 35f;
    [SerializeField] private LayerMask layerMask;
    [HideInInspector] public bool freezeCursor = true;
    private RectTransform cursorTransform;
    private Camera camera;
    private Mouse virtualMouse;
    private Mouse currentMouse;
    private bool previousMouseState;
    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "KeyboardMouse";
    private string previousControlScheme = "";
    private MapManager mapManager;
    private MarkerManager markerManager;


    private void OnEnable()
    {
        markerManager = GetComponent<MarkerManager>();
        camera = Camera.main;
        currentMouse = Mouse.current;
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
        } else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse);
        if (cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }
        InputSystem.onAfterUpdate += UpdateMotion;
        input.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        input.onControlsChanged -= OnControlsChanged;
    }

	private void Start()
	{
		mapManager = this.GetComponent<MapManager>();
        cursorTransform = cursor.GetComponent<RectTransform>();
	}

	private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
            return;

        if (input.currentControlScheme == gamepadScheme)
        {
            Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
            deltaValue *= cursorSpeed * Time.deltaTime;
            Vector2 currentPosition = virtualMouse.position.ReadValue();
            Vector2 newPosition = currentPosition + deltaValue;

            newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
            newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

            InputState.Change(virtualMouse.position, newPosition);
            InputState.Change(virtualMouse.delta, deltaValue);

            AnchorCursor(newPosition);
        }
        else if (input.currentControlScheme == mouseScheme)
        {
			Vector2 newPosition = Mouse.current.position.ReadValue();
			newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
			newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);
			AnchorCursor(newPosition);
        }
    }

    private void AnchorCursor(Vector2 position)
    {
        if (freezeCursor == false)
        {
            Vector2 anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : camera, out anchoredPosition);
            cursorTransform.anchoredPosition = anchoredPosition;
        }
    }

    private void OnControlsChanged(PlayerInput input)
    {
        if (mapManager.isMapOpen == true)
        {
		    if (input.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
            {
			    Cursor.visible = false;
                currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
                previousControlScheme = mouseScheme;
            }
            else if (input.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
            {
                Cursor.visible = false;
                InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
                AnchorCursor(currentMouse.position.ReadValue());
                previousControlScheme = gamepadScheme;
            }
        }
    }

    public void PutMarkerOnMap(GameObject marker)
    {
        Ray ray = new Ray();
        RaycastHit hit;


        if (input.currentControlScheme == mouseScheme)
        {
            ray = Camera.main.ScreenPointToRay(cursor.transform.position);
        }
        else if (input.currentControlScheme == gamepadScheme) 
        {
            ray = Camera.main.ScreenPointToRay(cursor.transform.position);
        }
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Object newMarker = Instantiate(marker, hit.point , marker.transform.rotation);
            markerManager.PutMarkerInList(newMarker, marker.name);
            mapManager.UnfreezeCursor();
        }
    }
}
