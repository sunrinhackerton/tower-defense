# 🛡️ 엔티티 및 유닛 시스템 (Entities & Unit Systems)

게임 내에서 능동적으로 이동하고 상호작용하는 액터(몬스터, 민병대)들의 구조를 다룹니다.

## 1. IDamageable.cs (인터페이스)
피해를 입을 수 있는 모든 엔티티(몬스터, 병사, 본진 타워 등)가 상속받는 추상화 인터페이스입니다.
- `TakeDamage(int amount, WeaponDamageType type)`: 타격받았을 때 호출됩니다.
- `IsDead() -> bool`: 해당 엔티티가 사망 상태인지 반환하여 타겟팅 시스템에서 걸러내도록 돕습니다.

## 2. MonsterAI2D.cs
모든 적 몬스터 프리팹에 부착되어 체력, 방어 타입, 피격 연산을 담당합니다.
- **핵심 변수**: `maxHP`, `armorType(Light, Heavy, Flying)`, `rewardCoins`, `hpFill`(체력바).
- **피격 메커니즘 (`TakeDamage`)**:
  - `damageType`(Pierce/Splash/Melee)과 자신의 `armorType`을 비교하여 **방어력 공식**을 적용합니다. (예: Heavy는 Pierce를 맞으면 10%만 적용)
  - 연산된 최종 데미지로 체력을 차감하고 붉은색 HitFlash 연출을 가동합니다.
  - 사망 시 `GameManager`를 호출해 보상(`rewardCoins`)을 지급하고 객체를 파괴합니다.

## 3. MilitiaUnit2D.cs
Barracks(민병대 초소)에서 스폰되어 몬스터를 몸으로 막아내는 아군 보병 AI입니다.
- **상태 머신 (State Machine)**: `Moving`, `Idling`, `Engaged`.
- **블로킹 및 전투 (`Engage` & `CombatLoop`)**:
  - `OnTriggerEnter2D`로 몬스터 접촉 시 즉시 이동을 멈추고 적을 붙잡습니다(`isBlocked = true`).
  - 1초 간격으로 검기 파티클(`Slash_VFX`)을 소환하며 서로 10 / 20 데미지를 교환합니다.
- **사망 처리**: 체력이 다하면 블로킹을 해제(`isBlocked = false`)하여 몬스터가 다시 이동하도록 놔주고 파괴됩니다.

## 4. WaypointMovement2D.cs
적 유닛과 투사체가 정해진 길을 따라가게 만드는 경로 탐색 컴포넌트입니다.
- **이동 로직**: 맵 상의 `Transform[] waypoints` 배열을 순서대로 순회합니다.
- **가고일(공중) 비행 이동**: 기존에 일직선으로 화면 밖을 가로지르던 로직을 폐기하고, 공중 판정(`isFlying = true`)만 남겨둔 채 정상적으로 구불구불한 잔딧길을 따라 비행하도록 설계되었습니다. (단, 지상군과 충돌 판정이 없어 병사와 부딪히지 않고 무시합니다.)
