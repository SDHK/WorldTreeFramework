using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleToScreen : MonoBehaviour
{
	private const int maxLines = 50;
	private const int maxLineLength = 120;
	private string _logStr = "";

	private readonly List<string> _lines = new List<string>();

	public int fontSize = 15;

	private void OnEnable()
	{ Application.logMessageReceived += Log; }

	private void OnDisable()
	{ Application.logMessageReceived -= Log; }

	public void Log(string logString, string stackTrace, LogType type)
	{
		foreach (var line in logString.Split('\n'))
		{
			if (line.Length <= maxLineLength)
			{
				_lines.Add(line);
				continue;
			}
			var lineCount = line.Length / maxLineLength + 1;
			for (int i = 0; i < lineCount; i++)
			{
				if ((i + 1) * maxLineLength <= line.Length)
				{
					_lines.Add(line.Substring(i * maxLineLength, maxLineLength));
				}
				else
				{
					_lines.Add(line.Substring(i * maxLineLength, line.Length - i * maxLineLength));
				}
			}
		}
		if (_lines.Count > maxLines)
		{
			_lines.RemoveRange(0, _lines.Count - maxLines);
		}
		_logStr = string.Join("\n", _lines);
	}

	private void OnGUI()
	{
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
		   new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));

		GUIStyle uIStyle = new GUIStyle() { fontSize = Math.Max(10, fontSize) };
		uIStyle.normal.textColor = Color.green;

		GUI.Label(new Rect(10, 10, 800, 370), _logStr, uIStyle);
	}
}