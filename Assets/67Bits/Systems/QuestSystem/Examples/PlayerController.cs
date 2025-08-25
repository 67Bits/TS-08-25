using UnityEngine;

namespace SSB.Quests.Examples
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        private void Awake()
        {
            GameReferences.PlayerTransform = this.transform;
        }
        void Update()
        {
            // Recebe a entrada do teclado
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            // Calcula o movimento
            Vector3 move = new Vector3(moveX, 0, moveY) * moveSpeed * Time.deltaTime;

            // Move o personagem
            transform.Translate(move, Space.World);
        }
    }
}