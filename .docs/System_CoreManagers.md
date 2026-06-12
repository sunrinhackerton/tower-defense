# ⚙️ 코어 시스템 (Core Systems)

본 문서는 게임의 전반적인 상태를 관장하는 매니저 클래스들을 분석합니다.

## 1. GameManager2D.cs
게임의 전체적인 경제(Economy)와 전역 상태를 주관하는 싱글톤 클래스입니다.
- **핵심 변수**: 
  - `public static GameManager2D Instance`: 전역 접근용 인스턴스
  - `public int startingCoins`: 게임 시작 시 지급되는 초기 재화
  - `private int _currentCoins`: 현재 보유 재화 상태 관리
  - `public TMP_Text coinText`: UI 연동 텍스트 컴포넌트
- **핵심 기능**:
  - `AddCoins(int amount)`: 몬스터 처치 시 코인을 증가시키고 UI를 즉시 갱신합니다.
  - `UseCoins(int amount) -> bool`: 타워 건설 시 재화를 차감합니다. 보유량이 부족하면 `false`를 반환하여 건설을 차단합니다.

## 2. WaveManager2D.cs
웨이브 데이터 배열을 바탕으로 적의 스폰 타이밍을 조절하는 매니저입니다.
- **구조체 정의**: `WaveData` (적 프리팹, 생성 마리 수, 생성 간격)
- **핵심 기능**:
  - `WaveLoop() -> IEnumerator`: `waves` 배열을 순차적으로 순회하며 정의된 마리 수만큼 `SpawnEnemy`를 호출합니다.
  - 한 웨이브 스폰이 완료되면 맵 상에 살아있는 적(`_activeEnemies.Count`)이 0이 될 때까지 대기(Wait) 상태에 머무릅니다.
  - 모든 적이 소탕되면 `waveDelay`(대기 시간) 이후 다음 웨이브로 넘어갑니다.

## 3. BuildManager2D.cs 및 BuildSite2D.cs
사용자가 맵에 타워를 건설할 수 있도록 돕는 UI-인게임 상호작용 시스템입니다.
- **BuildSite2D (빌드 사이트)**:
  - 맵에 배치되는 타워 건설 기반 타일. `BoxCollider2D`를 소유하고 있습니다.
  - `OnMouseDown()`: 클릭 시 `BuildManager`에게 UI 오픈을 요청합니다.
  - `BuildTower(GameObject prefab)`: 넘겨받은 프리팹을 타일 중앙에 Instantiate하고 자신(빈 타일)의 클릭 감지를 비활성화합니다.
- **BuildManager2D (빌드 매니저)**:
  - 건설 UI 팝업(`BuildUI_Panel`)을 화면 중앙에 띄웁니다.
  - 버튼 이벤트(예: `OnBallistaButtonClicked`)와 연동되어 `GameManager`를 통해 코인을 검사한 후 타워 건설을 지시합니다.
