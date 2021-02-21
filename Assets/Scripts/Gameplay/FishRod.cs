using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRod : MonoBehaviour
{
	[SerializeField] GameObject[] m_fishes = null;
	protected void OnTriggerEnter(Collider other)
	{
		foreach (GameObject fish in m_fishes)
        {
			fish.SetActive(true);
        }
		Destroy(gameObject);
	}
}
