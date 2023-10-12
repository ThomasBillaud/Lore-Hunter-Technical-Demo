using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamepadGridInteract : MonoBehaviour
{
	[SerializeField] private GridLayoutGroup gridLayout;
	[SerializeField] private RectTransform highlight;
	[SerializeField] private InputReader inputReader;
	[SerializeField] private PlayerInput input;
	private Transform selectedItem;
	private int selectedIndex = 0;
	private int width = 0;
	private int height = 0;

	private const string gamepadScheme = "Gamepad";
	private const string mouseScheme = "KeyboardMouse";

	private void Start()
	{
		GetColumnAndRow(gridLayout, out height, out width);
		SelectChild(selectedIndex);
	}

	private void Update()
	{
		if (input.currentControlScheme == mouseScheme)
		{
			highlight.gameObject.SetActive(false);
		} else
		{
			highlight.gameObject.SetActive(true);
		}
	}

	private void SelectChild(int childIndex)
	{
		selectedIndex = childIndex;
		selectedItem = gridLayout.transform.GetChild(childIndex);
		highlight.localPosition = selectedItem.localPosition;
	}

	void OnEnable()
	{
		inputReader.navigateEvent += NavigateGrid;
		inputReader.navigateMapEvent += NavigateGrid;
		inputReader.useTagEvent += ClickButton;
	}

	void OnDisable()
	{
		inputReader.navigateEvent -= NavigateGrid;
		inputReader.navigateMapEvent -= NavigateGrid;
		inputReader.useTagEvent -= ClickButton;
	}

	public void NavigateGrid(Vector2 value)
	{
		if (value.x > 0)
		{
			if ((selectedIndex + 1) % width == 0)
			{
				selectedIndex = selectedIndex + 1 - width;
			} else
			{
				selectedIndex += 1;
			}
		} else if (value.x < 0)
		{
            if (selectedIndex % width <= 0)
            {
                selectedIndex = selectedIndex - 1 + width;
            } else
			{
				selectedIndex -= 1;
			}
		}
		if (value.y > 0)
		{
			if (selectedIndex - width < 0)
			{
				selectedIndex = (selectedIndex % width) + (width * (height - 1));
			} else
			{
				selectedIndex -= width;
			}
		} else if (value.y < 0)
		{
			if (selectedIndex + width >= width * height)
			{
				selectedIndex = selectedIndex % width;
			} else
			{
				selectedIndex += width;
			}
		}
		SelectChild(selectedIndex);
	}

	public void ClickButton()
	{
		if (input.currentControlScheme == gamepadScheme)
		{ 
			selectedItem.gameObject.GetComponent<Button>().onClick.Invoke();
		}
	}

	private void GetColumnAndRow(GridLayoutGroup glg, out int column, out int row)
	{
		column = 0;
		row = 0;

		if (glg.transform.childCount == 0)
			return;

		//Column and row are now 1
		column = 1;
		row = 1;

		//Get the first child GameObject of the GridLayoutGroup
		RectTransform firstChildObj = glg.transform.
			GetChild(0).GetComponent<RectTransform>();

		Vector2 firstChildPos = firstChildObj.anchoredPosition;
		bool stopCountingRow = false;

		//Loop through the rest of the child object
		for (int i = 1; i < glg.transform.childCount; i++)
		{
			//Get the next child
			RectTransform currentChildObj = glg.transform.
		   GetChild(i).GetComponent<RectTransform>();

			Vector2 currentChildPos = currentChildObj.anchoredPosition;

			//if first child.x == otherchild.x, it is a column, ele it's a row
			if (firstChildPos.x == currentChildPos.x)
			{
				column++;
				//Stop couting row once we find column
				stopCountingRow = true;
			}
			else
			{
				if (!stopCountingRow)
					row++;
			}
		}
	}
}
