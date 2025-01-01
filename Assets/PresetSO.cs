using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "preset/asset", menuName = "preset")]
public class PresetSO : ScriptableObject
{
    public bool isJulia;
    public float scint;
    public Vector2 screenpos;
    public float pickoverlinear;
}

