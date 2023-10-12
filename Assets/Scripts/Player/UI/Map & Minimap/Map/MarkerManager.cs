using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class MarkerManager : MonoBehaviour
{
	public List<List<Object>> markers = new List<List<Object>>();
	public string[] markerNames = new string[10]{"Icon1", "Icon2", "Icon3", "Icon4", "Icon5", "Icon6", "Icon7", "Icon8", "Icon9", "Icon10"};
	[SerializeField] private int markerNumberMax;
	[SerializeField] private int markerNumber;
	[SerializeField] private string noMarkerMessageTable;
	[SerializeField] private string noMarkerMessageKey;
	[SerializeField] private GameObject messagePrefab;
	private int markerCount = 0;

	private void Start()
	{
		for (int i = 0; i < markerNumber ; i++)
		{
			List<Object> list = new List<Object>();
			markers.Add(list);
		}
		GameObject[] getMarkers = GameObject.FindGameObjectsWithTag("Marker");
		foreach (GameObject marker in getMarkers)
		{
			PutMarkerInList(marker, marker.name);
		}
	}

	public void PutMarkerInList(Object marker, string markerName)
	{
		for (int i = 0 ; i < markerNumber ; i++)
		{
			if (markerCount < markerNumberMax * markerNumber && Equals(markerName, markerNames[i]) == true && markers[i].Count < markerNumberMax)
			{
				markers[i].Add(marker);
				markerCount++;
				return;
			}
		}
		Instantiate(messagePrefab);
		messagePrefab.GetComponent<QuickMessage>().Init(noMarkerMessageTable, noMarkerMessageKey);
		Destroy(marker);
	}

	public void RemoveMarkerInList(Object marker, string markerName)
	{
		for (int i = 0; i < markerNumber ; i++)
		{
			if (markerName.Contains(markerNames[i]))
			{
				markers[i].Remove(marker);
				Destroy(marker);
				return;
			}
		}
	}
}
