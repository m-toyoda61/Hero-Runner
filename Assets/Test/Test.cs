using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    private float gasorin = 50.0f;
    private Rigidbody m_Rigidbody;
    public Engine m_Engine;
    
    
    private void Start() {
        m_Rigidbody = this.GetComponent<Rigidbody>();
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            Run();
        }
    }

    public void Run() {
        // エンジンからエネルギー取得
        float energy = m_Engine.GetEnergy();

        // 受け取ったエネルギーから速度計算
        m_Rigidbody.velocity = Vector3.forward * energy;
    }
}
