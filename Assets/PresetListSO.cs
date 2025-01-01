using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "presetlist/asset", menuName = "presetlist")]
public class PresetListSO : ScriptableObject
{
    public List<PresetSO> presets;
}
