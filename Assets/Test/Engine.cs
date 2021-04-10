using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : ScriptableObject {

    public float m_Speed = 0.0f;

    public float m_Gasorin = 100.0f;

    public virtual float GetEnergy() {
        if (m_Gasorin > 0.0f) {
            return m_Speed;
        }
        else {
            return 0.1f;
        }
    }
}