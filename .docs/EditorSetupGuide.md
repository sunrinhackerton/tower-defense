# 🎨 에디터 및 UI 셋업 가이드 (Editor & UI Setup)

유니티 에디터 상에서 UI, 렌더링 순서, 씬 셋업을 어떻게 구성했는지 정리한 가이드입니다.

## 1. UI Canvas 시스템 (Screen Space)
화면 전체를 덮는 메인 UI입니다.
- **GameManager의 코인 텍스트**: 화면 좌측 상단(Top-Left)에 Anchor 및 Pivot(0, 1)을 고정하여 화면 해상도가 변하더라도 위치가 깨지지 않게 설정.
- **BuildUI_Panel (건설 팝업)**: 화면 중앙에 Anchor(0.5, 0.5)를 두고 평소에는 `SetActive(false)`로 숨김. 빌드 사이트를 클릭하면 스크립트에서 활성화시킵니다.

## 2. World Space UI (체력바 시스템)
캐릭터의 머리 위를 따라다니는 체력 게이지입니다.
- **구조**: `Monster` 및 `Soldier` 프리팹 최상단 자식으로 `HP_Canvas` 오브젝트 생성.
- **세팅**:
  - `RenderMode`를 `World Space`로 설정. 
  - CanvasScaler 및 EventSystem의 광선 추적(Raycaster)을 무력화하여 성능 최적화.
- **슬라이더(Fill) 메커니즘**: Fill 이미지의 `Pivot`을 좌측(0, 0.5)으로 설정하여 스크립트에서 `localScale.x` 값을 깎을 때 게이지가 우측에서 좌측으로 자연스럽게 줄어들도록 셋업.

## 3. SpriteRenderer 정렬 레이어 (Sorting Order)
2D 게임의 특성 상 원근감을 표현하기 위한 Z-Order(그리기 순서) 규칙입니다.
- **`Order 0`**: 지형의 바닥 (타일, 잔딧길 배경)
- **`Order 5`**: 타워의 기둥 (Tower Base)
- **`Order 6`**: 타워의 포탑 머리부 (Tower Head, Base 위로 렌더링)
- **`Order 10`**: 유닛 (몬스터, 민병대)
- **`Order 20`**: 파티클 효과 (발사체, 타격 섬광, 폭발 이펙트)

이 엄격한 규칙을 통해 공중에 떠다니는 탄환이 타워에 파묻히거나 적이 길바닥 아래로 숨는 그래픽 오류를 원천 차단했습니다.
