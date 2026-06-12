# 🏛️ 프로젝트 오버뷰 (Project Overview)

## 1. 프로젝트 개요 (Introduction)
본 프로젝트는 **고전/중세 전투 테마의 2D 타워 디펜스 게임**입니다. 빈 프로젝트(Empty Project)에서 시작하여 현재의 고급 시스템(다중 적 대응, 웨이브 스폰, 유닛 블로킹 등)을 구축하는 과정을 거쳤습니다. 
사용자는 코인을 수집하고, 전략적인 위치에 타워를 건설하며, 적의 침공 웨이브를 방어해야 합니다.

## 2. 핵심 게임 루프 (Core Game Loop)
1. **자원 수집**: 게임 시작 시 주어지는 기본 자원 및 적 처치 시 획득하는 코인 경제 시스템.
2. **방어 기지 구축**: 맵 상에 지정된 빌드 사이트(Build Site)를 클릭하여 UI 팝업을 열고 타워(석궁, 투석기, 병영)를 건설.
3. **웨이브 방어**: 웨이브 매니저에 의해 일정 주기로 스폰되는 적 무리를 방어.
4. **전투 및 상성**: 타워의 속성과 적의 장갑 타입에 따른 3x3 상성 기반 자동 전투 수행.
5. **루프 재순환**: 웨이브 클리어 보상 및 적 처치 보상을 기반으로 추가 타워를 건설 및 업그레이드(예정)하여 다음 웨이브 대비.

## 3. 소프트웨어 아키텍처 (Software Architecture)
본 프로젝트는 확장성과 유지보수를 극대화하기 위해 다음과 같은 디자인 패턴과 컴포넌트 기반 아키텍처를 채택했습니다.

### 3.1. 매니저 계층 (Manager Layer)
- **싱글톤 패턴(Singleton)**: `GameManager2D`, `WaveManager2D`, `BuildManager2D` 등 게임의 전역 상태(코인, 웨이브, UI 상태)를 관리하는 객체들은 싱글톤으로 설계되어 씬 내 어디서든 쉽게 접근할 수 있습니다.
- **역할의 분리**: 코인은 `GameManager`, 적 스폰은 `WaveManager`, 건설 로직은 `BuildManager`가 각각 독립적으로 담당하여 의존성을 낮췄습니다.

### 3.2. 엔티티 및 컴포넌트 구조 (Component-based Entities)
- **인터페이스 활용 (`IDamageable`)**: 타워가 몬스터를 공격할 때 "몬스터" 클래스에 종속되지 않고 `IDamageable` 인터페이스를 통해 데미지를 입히도록 디커플링(Decoupling) 되어 있습니다. 이를 통해 민병대(아군)도 동일한 데미지 메커니즘을 공유합니다.
- **조립형 타워 컴포넌트**: 타워의 기능을 하나의 거대한 스크립트가 아닌, `TowerTargeting2D`(탐색), `TowerWeapon2D`(공격) 등으로 잘게 쪼개어 필요에 따라 조립(Composition)할 수 있도록 설계했습니다.

### 3.3. 상태 머신 (Finite State Machine)
- 아군 병사(`MilitiaUnit2D`) 및 적 AI의 경우 상태 머신(FSM)을 활용하여 이동(`Moving`), 대기(`Idling`), 교전(`Engaged`) 상태를 깔끔하게 분리하여 제어합니다.

## 4. 디렉토리 구조 (Directory Tree)
`Assets/` 폴더 내 최상위 에셋 및 세부 디렉토리 구조입니다.

```text
Assets/
├── Editor/
│   ├── AssignSlashPrefab.cs
│   ├── SetupPhaseScript.cs
│   └── SetupUIScript.cs
├── Prefabs/
│   ├── Gargoyle.prefab
│   ├── Goblin_Warrior.prefab
│   ├── Iron_Knight.prefab
│   ├── Monster_WP.prefab
│   ├── Projectile.prefab
│   ├── Slash_VFX.prefab
│   ├── Soldier.prefab
│   ├── Tower_Ballista.prefab
│   ├── Tower_Barracks.prefab
│   ├── Tower_Basic.prefab
│   └── Tower_Catapult.prefab
├── Scenes/
│   └── SampleScene.unity
├── Settings/
│   ├── Scenes/
│   │   └── URP2DSceneTemplate.unity
│   ├── Lit2DSceneTemplate.scenetemplate
│   ├── Renderer2D.asset
│   └── UniversalRP.asset
├── Sprites/
│   ├── BuildSiteSprite.png
│   ├── BushDecoSprite.png
│   ├── GrassPathSprite.png
│   ├── MonsterSprite.png
│   ├── MonsterSpritePurple.png
│   ├── PathTileSprite.png
│   ├── ProjectileSprite.png
│   ├── SlashSprite.png
│   ├── SoldierSprite.png
│   ├── TowerBaseSprite.png
│   ├── TowerHeadSprite.png
│   └── TowerSprite.png
├── TextMesh Pro/ (폰트, 리소스, 셰이더 등)
├── BuildManager2D.cs
├── BuildSite2D.cs
├── CombatEnums.cs
├── EnemySpawner2D.cs
├── GameManager2D.cs
├── HealthBar2D.cs
├── IDamageable.cs
├── MilitiaUnit2D.cs
├── MonsterAI2D.cs
├── ParticleFactory.cs
├── PathVisualizer2D.cs
├── Projectile2D.cs
├── SelfDestruct2D.cs
├── TestMonsterMovement2D.cs
├── Tower2D.cs
├── TowerBarracks2D.cs
├── TowerTargeting2D.cs
├── TowerWeapon2D.cs
├── WaveManager2D.cs
└── WaypointMovement2D.cs
```
