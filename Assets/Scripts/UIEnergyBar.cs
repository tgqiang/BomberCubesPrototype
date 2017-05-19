using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Energy))]
public class UIEnergyBar : MonoBehaviour
{
	public Rect m_Rectangle = new Rect(0, 0, 50, 4);
	public Vector2 m_Offset = new Vector2(-25, -25);
	
	Texture2D m_Background;
	Texture2D m_Foreground;
	
	Energy energy;

	// Use this for initialization
	void Start ()
	{
		// Use 1x1 pixels to draw the health bars
		m_Background = new Texture2D(1,1);
		m_Background.SetPixel(0,0, Color.black);
		m_Background.Apply();
		m_Foreground = new Texture2D(1,1);
		m_Foreground.SetPixel(0,0, Color.blue);
		m_Foreground.Apply();
		
		energy = GetComponent<Energy>();
	}
	
	void OnGUI ()
	{
		m_Rectangle.x = Camera.main.WorldToScreenPoint(transform.position).x + m_Offset.x;
		m_Rectangle.y = Screen.height - Camera.main.WorldToScreenPoint(transform.position).y + m_Offset.y;
			
		GUI.DrawTexture(m_Rectangle, m_Background);
		
		Rect partialRect = m_Rectangle;
		partialRect.width = m_Rectangle.width * (energy.FractionEnergy);
		partialRect.x = m_Rectangle.x;
		
		GUI.DrawTexture(partialRect, m_Foreground);
	}
}
