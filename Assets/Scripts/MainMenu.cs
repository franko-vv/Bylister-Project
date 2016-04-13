using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MainMenu:MonoBehaviour
{
    public string FirstLevel;

    public void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Application.LoadLevel(FirstLevel);
    }
}
