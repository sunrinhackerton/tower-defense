# Deep Interview Context Snapshot: Tower Interactions

- Timestamp UTC: 20260616T145059Z
- Task statement: Unity tower-defense project에서 Tower - Player Control, Mob - WaveController, WaveController - Coin - Tower 상호작용 연결 기능을 구현하기 위한 프로젝트 분석 및 개발 계획 수립.
- Desired outcome: 현재 프로젝트 구조를 근거로 상호작용 흐름을 명확히 하고 실행 가능한 개발 계획/요구사항 명세로 수렴.
- Stated solution: 세 관계를 연결: (1) Tower와 Player Control, (2) Mob과 WaveController, (3) WaveController-Coin-Tower.
- Probable intent hypothesis: 클릭/빌드/웨이브/보상/타워 구매 루프가 이미 일부 존재하나, 흐름이 완전히 통합·검증되지 않았거나 요구 명칭과 코드 명칭이 어긋남.

## Known facts/evidence
- Unity version: 6000.4.5f1. Packages include Input System 1.19.0, UGUI 2.0.0, URP 17.4.0, TextMesh Pro.
- Active scene: Assets/Scenes/SampleScene.unity, root objects include Main Camera, Canvas, EventSystem, GameManager, Path_Waypoints, Build_Site_1~3, WaveManager, BuildManager.
- Tags include Monster and Player, but no current scene GameObject tagged Monster before runtime.
- BuildManager scene object has buildUIPanel=BuildUI_Panel and tower prefabs Tower_Ballista/Catapult/Barracks with costs 100/150/200.
- WaveManager scene object has goblin/knight/gargoyle prefabs, waypointsParent=Path_Waypoints, waveText=WaveText.
- GameManager scene object has startingCoins=300 and coinText=CoinText.
- BuildSite2D uses IPointerClickHandler.OnPointerClick to open BuildManager2D UI and tracks HasTower; BuildManager2D deducts GameManager2D coins then instantiates a tower and marks site built.
- MonsterAI2D implements IDamageable, has coinReward, and on Die() calls GameManager2D.Instance.AddCoins(coinReward).
- WaveManager2D auto-builds waves in Awake(), spawns enemies along waypoints, tracks _activeEnemies, and waits until all active enemies are destroyed before next wave.
- TowerTargeting2D searches GameObjects tagged Monster, filters range/dead/flying-ground; TowerWeapon2D fires Projectile2D; Projectile2D applies IDamageable damage.
- Barracks spawns Soldier prefab; MilitiaUnit2D blocks non-flying monsters via WaypointMovement2D.isBlocked and deals melee damage.
- No PlayerController/PlayerControl script found; “Player Control” currently appears to mean click/UI interaction via BuildSite2D + EventSystem + BuildManager2D unless user intends a new controller.
- Documentation under .docs describes the same manager/component architecture but some Korean text encoding is corrupted. It still identifies GameManager/WaveManager/BuildManager singleton layer, tower composition, combat matrix, and core loop.
- Console currently has PathVisualizer missing sprite warning and MCP connection warnings/errors; no compile error was retrieved in latest console page.

## Constraints
- Deep-interview mode: no direct implementation until requirements are clarified and artifacts are written.
- Prefer existing components/patterns: Singleton managers, component-based towers, IDamageable, UGUI/InputSystem click flow.
- No new dependencies requested.

## Unknowns/open questions
- Whether “Player Control” means existing mouse/touch build-site click/UI control, or a new PlayerController/input abstraction.
- Whether wave progression should remain automatic or become player-started/controlled.
- Whether coin rewards should be awarded per monster kill only, per wave clear, or both.
- Whether tower construction should be blocked/enabled based on wave state, coin state, site state, or all of these.
- Desired acceptance criteria and out-of-scope boundaries.

## Decision-boundary unknowns
- May agent refactor BuildSite2D's unused towerPrefab/towerCost versus BuildManager centralized costs?
- May agent add events/callbacks between WaveManager, GameManager, BuildManager, and MonsterAI2D?
- May agent change scene/prefab serialized references and UI button wiring?

## Likely codebase touchpoints
- Assets/BuildSite2D.cs
- Assets/BuildManager2D.cs
- Assets/GameManager2D.cs
- Assets/WaveManager2D.cs
- Assets/MonsterAI2D.cs
- Assets/TowerTargeting2D.cs
- Assets/TowerWeapon2D.cs
- Assets/Projectile2D.cs
- Assets/MilitiaUnit2D.cs
- Assets/WaypointMovement2D.cs
- Assets/Prefabs/Tower_*.prefab
- Assets/Prefabs/Goblin_Warrior.prefab, Iron_Knight.prefab, Gargoyle.prefab
- Assets/Scenes/SampleScene.unity

## Relevant repo docs/rules/context inspected
- .docs/ProjectOverview.md
- .docs/GameDesign.md
- .docs/System_CoreManagers.md
- .docs/System_Combat.md
- .docs/System_Entities.md
- .docs/EditorSetupGuide.md
- .omx state/cache discovered; no prior matching context snapshot found in initial scan.

## Terminology/doc-code notes
- User term “Mob” maps to code/doc “Monster” tag and MonsterAI2D.
- User term “WaveController” likely maps to WaveManager2D.
- User term “Coin” maps to GameManager2D economy.
- User term “Player Control” has no direct class; code currently implements player interaction through BuildSite2D.OnPointerClick + BuildManager2D + Canvas/EventSystem.

## Prompt-safe initial-context summary status
not_needed
