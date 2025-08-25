using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SSB.Quests
{
    [CreateAssetMenu(fileName = "New Objective", menuName = "67Bits/Quest/Objective", order = 0)]
    public class Objective : ScriptableObject
    {
        [field: SerializeField, HideLabel, PreviewField(100, Alignment = ObjectFieldAlignment.Center)] public Sprite Icon { get; private set; }
        [HorizontalGroup()]public List<Quest> LinkedQuests = new();
        [HorizontalGroup()]public List<QuestObject> LinkedObjects = new();

        public void UpdateProgression(int value)
        {
            for (int i = 0; i < LinkedQuests.Count; i++)
            {
                Quest quest = LinkedQuests[i];
                quest.UpdateProgression(value);
            }
        }
    }
}
