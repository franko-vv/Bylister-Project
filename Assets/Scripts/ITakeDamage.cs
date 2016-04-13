using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface ITakeDamage
{
    void TakeDamage(int damage, GameObject instigator);
}
