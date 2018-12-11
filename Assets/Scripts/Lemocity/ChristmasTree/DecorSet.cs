using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LemonadeStore
{
    [CreateAssetMenu(fileName = "DecorationSet", menuName = "Inventory/Create Tree Decoration Set")]
    public class DecorSet : ScriptableObject
    {
        public string DecorSetKey;
        public Sprite[] Icons;
        public Sprite[] IconsLocked;
    }
}
