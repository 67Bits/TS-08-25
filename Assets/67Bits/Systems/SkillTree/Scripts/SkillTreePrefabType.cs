using UnityEngine;
using Upgrades;
[System.Serializable]
public class SkillTreePrefabType
{
    public SkillTreePrefabType(string name) => Name = name;
    [HideInInspector] public string Name;
    public SkillTreeUpgradeType Type;
    public GameObject Prefab;
}
