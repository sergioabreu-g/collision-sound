using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text currentMaterial;

    public void changeMaterial(string material) {
        currentMaterial.text = material.ToUpper();
    }
}
