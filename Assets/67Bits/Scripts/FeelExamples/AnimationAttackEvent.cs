using UnityEngine;
using UnityEngine.Events;

namespace MZW.Weapons
{
    public class AnimationAttackEvent : MonoBehaviour
    {
        //[SerializeField] private WeaponController _weaponController;
        public UnityEvent attackFeel, startAttackFeel;
        public void StartAnimation()
        {
            //_weaponController.SetWeaponAnimationSpeed();
            startAttackFeel?.Invoke();
        }
        //Função para ser aplicado no Animation Event de Attack
        public void AttackAnimation()
        {
            //_weaponController.AttackEnemy();
            attackFeel?.Invoke();
        }
        public void EndAnimation()
        {
            //_weaponController.EndAttack();
        }
    }
}