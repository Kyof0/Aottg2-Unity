using Characters;
using Effects;
using Settings;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Characters
{
    class APGRapidFire : ExtendedUseable
    {
        int _MaxShotCount;
        int _ShotCount = 0;
        float _waitBeforeShot = 0.15f;
        float _usedTimeLast = Time.deltaTime;
        public APGRapidFire(BaseCharacter owner, int maxShotCount) : base(owner)
        {
            _MaxShotCount = maxShotCount;
            Cooldown = 1f;
        }
        protected override float GetActiveTime()
        {
            return 0.45f;
            return CharacterData.HumanWeaponInfo["APG"]["CD"].AsFloat / 3 * _MaxShotCount;
        }
        public override bool CanUse()
        {
            var weapon = ((Human)_owner).Weapon;
            if (weapon is AmmoWeapon)
            {
                var ammo = (AmmoWeapon)weapon;
                return base.CanUse() && (ammo.RoundLeft > 0 || ammo.RoundLeft == -1);
            }
            return false;
        }
        protected override void OnUse()
        {
            base.OnUse();
            var human = (Human)_owner;
            var weapon = (AmmoWeapon)human.Weapon;
            if (weapon.RoundLeft > _MaxShotCount)
            {
                _ShotCount = weapon.RoundLeft - _MaxShotCount;
                weapon.RoundLeft -= _MaxShotCount;
            }
            else if (weapon.RoundLeft <= _MaxShotCount)
            {
                _ShotCount = weapon.RoundLeft;
                weapon.RoundLeft = 0;
            }
            //Shoot();
            //_usedTimeLast = Time.deltaTime;
        }
        protected override void ActiveFixedUpdate()
        {
            _usedTimeLast += Time.deltaTime;
            if (_usedTimeLast - _waitBeforeShot >= 0)
            {
                Shoot();
                _usedTimeLast = Time.deltaTime;
            }
        }
        private void Shoot()
        {
            var human = (Human)_owner;
            Vector3 target = human.GetAimPoint();
            Vector3 direction = (target - human.Cache.Transform.position).normalized;
            string anim;
            if (human.Grounded)
            {
                if (_ShotCount % 2 == 1)
                    anim = HumanAnimations.AHSSShootL;
                else
                    anim = HumanAnimations.AHSSShootR;
            }
            else
            {
                if (_ShotCount % 2 == 0)
                    anim = HumanAnimations.AHSSShootLAir;
                else
                    anim = HumanAnimations.AHSSShootRAir;
            }
            human.AttackAnimation = anim;
            human.CrossFade(anim, 0.05f);
            human.TargetAngle = Quaternion.LookRotation(direction).eulerAngles.y;
            human._targetRotation = Quaternion.Euler(0f, human.TargetAngle, 0f);
            human.Cache.Transform.rotation = Quaternion.Lerp(human.Cache.Transform.rotation, human._targetRotation, Time.deltaTime * 30f);
            Vector3 start = human.Cache.Transform.position + human.Cache.Transform.up * 0.8f;
            direction = (target - start).normalized;
            EffectSpawner.Spawn(EffectPrefabs.GunExplode, start, Quaternion.LookRotation(direction), 2f);
            human.PlaySound(HumanSounds.GetRandomAPGShot());
            var gunInfo = CharacterData.HumanWeaponInfo["APG"];
            if (SettingsManager.InGameCurrent.Misc.APGPVP.Value)
                gunInfo = CharacterData.HumanWeaponInfo["APGPVP"];
            var capsule = (CapsuleCollider)human.HumanCache.APGHit._collider;
            capsule.radius = gunInfo["Radius"].AsFloat;
            human.HumanCache.APGHit.transform.position = start;
            human.HumanCache.APGHit.transform.rotation = Quaternion.LookRotation(direction);
            human.HumanCache.APGHit.Activate(0f, 0.1f);
            ((InGameMenu)UIManager.CurrentMenu).HUDBottomHandler.ShootAPG();
        }
        protected override void Deactivate()
        {
            
            //for(int i = ShotCount; i>0; i--)
            //{
            //    var human = (Human)_owner;
            //    Vector3 target = human.GetAimPoint();
            //    Vector3 direction = (target - human.Cache.Transform.position).normalized;
            //    string anim;
            //    if (human.Grounded)
            //    {
            //        if (ShotCount % 2 == 1)
            //            anim = HumanAnimations.AHSSShootL;
            //        else
            //            anim = HumanAnimations.AHSSShootR;
            //    }
            //    else
            //    {
            //        if (ShotCount % 2 == 0)
            //            anim = HumanAnimations.AHSSShootLAir;
            //        else
            //            anim = HumanAnimations.AHSSShootRAir;
            //    }
            //    human.AttackAnimation = anim;
            //    human.CrossFade(anim, 0.05f);
            //    human.TargetAngle = Quaternion.LookRotation(direction).eulerAngles.y;
            //    human._targetRotation = Quaternion.Euler(0f, human.TargetAngle, 0f);
            //    human.Cache.Transform.rotation = Quaternion.Lerp(human.Cache.Transform.rotation, human._targetRotation, Time.deltaTime * 30f);
            //    Vector3 start = human.Cache.Transform.position + human.Cache.Transform.up * 0.8f;
            //    direction = (target - start).normalized;
            //    EffectSpawner.Spawn(EffectPrefabs.GunExplode, start, Quaternion.LookRotation(direction), 2f);
            //    human.PlaySound(HumanSounds.GetRandomAPGShot());
            //    var gunInfo = CharacterData.HumanWeaponInfo["APG"];
            //    if (SettingsManager.InGameCurrent.Misc.APGPVP.Value)
            //        gunInfo = CharacterData.HumanWeaponInfo["APGPVP"];
            //    var capsule = (CapsuleCollider)human.HumanCache.APGHit._collider;
            //    capsule.radius = gunInfo["Radius"].AsFloat;
            //    human.HumanCache.APGHit.transform.position = start;
            //    human.HumanCache.APGHit.transform.rotation = Quaternion.LookRotation(direction);
            //    human.HumanCache.APGHit.Activate(0f, 0.1f);
            //    ((InGameMenu)UIManager.CurrentMenu).HUDBottomHandler.ShootAPG();
            //}
            //var human = (Human)_owner;
            //Vector3 target = human.GetAimPoint();
            //Vector3 direction = (target - human.Cache.Transform.position).normalized;
            //float cross = Vector3.Cross(human.Cache.Transform.forward, direction).y;
            //string anim;
            //if (human.Grounded)
            //{
            //    anim = HumanAnimations.AHSSShootBoth;
            //}
            //else
            //{
            //    anim = HumanAnimations.AHSSShootBothAir;
            //}
            //human.State = HumanState.Attack;
            //human.AttackAnimation = anim;
            //human.CrossFade(anim, 0.05f);
            //human.TargetAngle = Quaternion.LookRotation(direction).eulerAngles.y;
            //human._targetRotation = Quaternion.Euler(0f, human.TargetAngle, 0f);
            //human.Cache.Transform.rotation = Quaternion.Lerp(human.Cache.Transform.rotation, human._targetRotation, Time.deltaTime * 30f);
            //Vector3 start = human.Cache.Transform.position + human.Cache.Transform.up * 0.8f;
            //direction = (target - start).normalized;
            //EffectSpawner.Spawn(EffectPrefabs.GunExplode, start, Quaternion.LookRotation(direction), 2f);
            //human.PlaySound(HumanSounds.GetRandomAHSSGunShotDouble());
            //var ahssInfo = CharacterData.HumanWeaponInfo["AHSS"];
            //var capsule = (CapsuleCollider)human.HumanCache.AHSSHit._collider;
            //capsule.radius = ahssInfo["Radius"].AsFloat * 2f;
            //human.HumanCache.AHSSHit.transform.position = start;
            //human.HumanCache.AHSSHit.transform.rotation = Quaternion.LookRotation(direction);
            //human.HumanCache.AHSSHit.Activate(0f, 0.1f);
            //human.Cache.Rigidbody.AddForce(-direction * ahssInfo["KnockbackForce"].AsFloat * 2f, ForceMode.VelocityChange);
            //((InGameMenu)UIManager.CurrentMenu).HUDBottomHandler.ShootAHSS(true, true);
        }
    }
}
