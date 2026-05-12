# 동물 수박게임 (Animal Merge Puzzle)

## 프로젝트 개요
Unity 6 (6000.3.15f1) + URP 2D 기반 물리 퍼즐 게임.
상단에서 동물을 떨어뜨려 같은 종류끼리 합쳐 더 큰 동물을 만들고, 최종 단계인 **사자**를 완성하는 게임.

GDD 원본: `C:\Users\YOONJISU\Downloads\Animal_Suika_Game_GDD_v2.pdf`

---

## 동물 진화 단계

| 단계 | 동물 | 비고 |
|------|------|------|
| 1 | 햄스터 | 초기 등장 (1~5번째 투하: 1~2단계만) |
| 2 | 병아리 | 초기 등장 |
| 3 | 토끼 | |
| 4 | 고양이 | |
| 5 | 시바견 | |
| 6 | 양 | |
| 7 | 돼지 | |
| 8 | 얼룩말 | |
| 9 | 곰 | |
| 10 | 사자 | 최종 단계 |

그래픽: 현재는 색깔 원(Circle)으로 프로토타입. 나중에 동물 스프라이트로 교체 예정.

---

## 핵심 메커닉

### 투하 시스템
- 마우스 클릭 후 뗄 때 투하 (터치 지원 예정)
- 짧은 쿨타임으로 연사 방지
- 처음 3회 투하까지만 바닥까지 이어지는 점선 가이드라인 표시

### 합성 로직
- 동일 단계 동물 2개 충돌 → 두 동물의 **정중앙**에서 다음 단계 생성
- 합성 시 해당 동물 울음소리 SFX 1회 재생
- 사자(10단계) 합성 시 특별 연출

### 물리 설정
- 중간 정도의 마찰력(Friction)과 탄성(Bounciness)
- 적당히 구르면서도 쌓이는 느낌

### 게임 오버
- 데드라인(컨테이너 상단) 초과 시 **5초 유예 카운트다운** 후 종료

---

## 점수 시스템
- 합성 시 점수 획득: `다음 단계 번호 × 기본 점수`
- 높은 단계일수록 가산점 증가
- 최고 점수(Best Score) PlayerPrefs에 저장

---

## 화면 구성
- 해상도: 모바일 세로 모드 **9:16** (기준 1080×1920)
- Camera: Orthographic 2D

### UI 요소
- 현재 점수 (Current Score)
- 최고 점수 (Best Score)
- Next 동물 미리보기
- After Next 동물 미리보기
- 게임 오버 패널

---

## 스크립트 구조 (예정)

```
Assets/Scripts/
├── Data/
│   └── AnimalData.cs          # ScriptableObject - 동물 정보
├── Game/
│   ├── AnimalController.cs    # 개별 동물 물리/합성 로직
│   ├── AnimalSpawner.cs       # 마우스 입력, 투하 처리
│   └── GameManager.cs         # 게임 상태, 점수, 게임오버
└── UI/
    └── UIManager.cs           # 점수판, Next 표시, 게임오버 패널
```

---

## 개발 환경

| 항목 | 값 |
|------|-----|
| Unity 버전 | 6000.3.15f1 |
| 렌더 파이프라인 | URP 2D (17.3.0) |
| 코드 포맷터 | CSharpier 1.2.6 (PostToolUse 훅 자동 실행) |
| 물리 | Physics 2D (Rigidbody2D + CircleCollider2D) |

---

## 코딩 컨벤션
- C# 네임스페이스: `AnimalMerge`
- ScriptableObject는 `Assets/Data/` 폴더
- 씬 파일: `Assets/Scenes/`
- 상수는 `static readonly` 또는 ScriptableObject로 분리
- CSharpier 자동 포맷 적용 (Edit/Write 후 자동 실행)
