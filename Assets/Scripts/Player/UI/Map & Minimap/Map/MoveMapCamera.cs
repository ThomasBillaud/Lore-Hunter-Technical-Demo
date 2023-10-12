using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveMapCamera : MonoBehaviour
{
    public new GameObject camera;
    public bool canMoveCamera;
	public GameObject cursor;
    public float backToOriginSpeed = 1000f;
    public bool isReturningToOrigin = false;
	[SerializeField] private float edgeSize = 30f;
	[SerializeField] private float moveAmount = 100f;
    [SerializeField] private Transform limitMin;
    [SerializeField] private Transform limitMax;
	[SerializeField] private LayerMask layerMask;

	private bool edgeScrolling;
    private PlayerInput input;
    private Vector3 cameraFollowingPosition;
    private bool isMoving = false;

	private const string gamepadScheme = "Gamepad";
	private const string mouseScheme = "KeyboardMouse";

	// Start is called before the first frame update
	void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        isMoving = false;
		if (isReturningToOrigin)
		{
			camera.transform.localPosition = Vector3.MoveTowards(camera.transform.localPosition, new Vector3(0, 100, 0), Time.deltaTime * backToOriginSpeed);
			if (camera.transform.localPosition == new Vector3(0, 100, 0))
				isReturningToOrigin = false;
			return;
		}
		if (canMoveCamera == true)
		{
			MoveCamera();
		}

    }

    private void MoveCamera()
    {
		if (input.currentControlScheme == mouseScheme)
		{
			if (Mouse.current.position.ReadValue().x > Screen.width - edgeSize)
			{
				Debug.Log("Prout souris");
				cameraFollowingPosition.x = 1f;
				isMoving = true;
			}
			else if (Mouse.current.position.ReadValue().x < edgeSize)
			{
				Debug.Log("Caca souris");
				cameraFollowingPosition.x = -1f;
				isMoving = true;
			}
			if (Mouse.current.position.ReadValue().y > Screen.height - edgeSize)
			{
				Debug.Log("Pipi souris");
				cameraFollowingPosition.z = 1f;
				isMoving = true;
			}
			else if (Mouse.current.position.ReadValue().y < edgeSize)
			{
				Debug.Log("Vomi souris");
				cameraFollowingPosition.z = -1f;
				isMoving = true;
			}
		}
		else if (input.currentControlScheme == gamepadScheme)
		{
			if (cursor.transform.position.x > Screen.width - edgeSize)
			{
				Debug.Log("Prout manette");
				cameraFollowingPosition.x = 1f;
				isMoving = true;
			}
			else if (cursor.transform.position.x < edgeSize)
			{
				Debug.Log("Caca manette");
				cameraFollowingPosition.x = -1f;
				isMoving = true;
			}
			if (cursor.transform.position.y > Screen.height - edgeSize)
			{
				Debug.Log("Pipi manette");
				cameraFollowingPosition.z = 1f;
				isMoving = true;
			}
			else if (cursor.transform.position.y < edgeSize)
			{
				Debug.Log("Vomi manette");
				cameraFollowingPosition.z = -1f;
				isMoving = true;
			}
		}
		if (isMoving == false)
		{
			cameraFollowingPosition = new Vector3(0, 0, 0f);
		}
	}

	private void LateUpdate()
	{
        if (isMoving == true)
        {
            Vector3 moveDir = transform.forward * cameraFollowingPosition.z + transform.right * cameraFollowingPosition.x;
            camera.transform.localPosition += moveDir * moveAmount * Time.deltaTime;
        }
        if (canMoveCamera == true && isMoving == true)
            camera.transform.position = new Vector3(Mathf.Clamp(camera.transform.position.x, limitMin.position.x, limitMax.position.x), camera.transform.position.y, Mathf.Clamp(camera.transform.position.z, limitMin.position.z, limitMax.position.z));
	}

    public void BackToOrigin()
    {
        isReturningToOrigin = true;
    }
}
