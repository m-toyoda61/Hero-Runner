using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Engine/Super")]
public class SuperEngine : Engine {
    public int Test;
    public override float GetEnergy() {
        return m_Speed;
    }
}
