using UnityEngine;

namespace AnimalMerge
{
    [CreateAssetMenu(fileName = "AnimalData", menuName = "AnimalMerge/AnimalData")]
    public class AnimalData : ScriptableObject
    {
        public int stage;
        public string animalName;
        public float radius;
        public Color color;
        public AudioClip mergeSound;
        public Sprite sprite;
    }
}
