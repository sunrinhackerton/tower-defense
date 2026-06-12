# 🏹 전투 및 타워 시스템 (Combat & Tower Systems)

타워의 타겟팅, 무기 발사, 민병대 초소 메커니즘을 상세히 기술합니다.

## 1. TowerTargeting2D.cs
타워의 "두뇌" 역할을 하며 반경 내 최적의 타겟을 찾습니다.
- **필터링 메커니즘 (`IsTargetValid`)**:
  - 대상이 범위(`range`) 밖으로 나갔는지 실시간으로 검사합니다.
  - `IDamageable.IsDead()`를 통해 대상이 죽었는지 확인합니다.
  - `canTargetFlying` 및 `canTargetGround` 플래그를 통해 타워가 공격할 수 없는 몬스터(예: 투석기의 가고일 락온)를 무시합니다.
- **타겟 서치 (`FindBestTarget`)**:
  - 맵 상의 "Monster" 태그를 스캔하고 유효성 검증을 거쳐 가장 가까운 적(`Closest`)을 타겟으로 고정합니다.

## 2. TowerWeapon2D.cs
`TowerTargeting2D`가 찾아낸 타겟을 향해 지정된 주기로 발사체를 생성합니다.
- **핵심 변수**: `attackDamage`, `attackDelay`, `damageType`, `splashRadius`
- **발사 메커니즘 (`Fire`)**:
  - 내장된 타이머(`_attackTimer`)가 `attackDelay`를 초과할 때마다 Muzzle 위치에서 `projectilePrefab`을 인스턴스화합니다.
  - 생성된 탄환(`Projectile2D`)에 데미지, 타입, 스플래시 범위를 주입(`Setup`)합니다.

## 3. Projectile2D.cs
무기에서 발사되어 적에게 날아가는 투사체 로직입니다.
- **유도 추적 로직**: 타겟(`_target`)의 실시간 위치(`_lastKnownPos`)를 계속 갱신하며 `Vector3.MoveTowards`를 사용해 날아갑니다.
- **적중 로직 (`HitTarget`)**:
  - 타겟과의 거리가 `hitRadius` 이내가 되면 폭발합니다.
  - `splashRadius > 0`인 경우: `Physics2D.OverlapCircleAll`을 사용해 반경 내 모든 "Monster"를 찾아 각각 데미지를 줍니다.
  - 스플래시가 0인 경우: 단일 타겟의 `TakeDamage`만 호출합니다.

## 4. TowerBarracks2D.cs
공격용 발사체를 쏘는 대신 잔딧길 위에 민병대(아군)를 직접 스폰하여 길을 막는 유틸리티 타워입니다.
- **집결지 연산 (`CalculateRallyPoint`)**:
  - 맵에 설치된 전체 웨이포인트(`Path_Waypoints`)의 자식 노드들을 검색하여 타워 반경 내에서 가장 가까운 포인트의 좌표를 추출합니다.
- **라이프사이클 (Respawn System)**:
  - 타워가 지어지면 지정된 수(예: 2기)의 병사를 집결지로 보냅니다.
  - 병사가 전사하여 리스트(`_activeSoldiers`)에서 소멸하면, `respawnDelay`(예: 10초) 이후 새로운 병사를 보충합니다.
