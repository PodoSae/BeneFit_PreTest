# PreTest
### 마이베네핏 VM 개발팀 입사 지원 사전 과제

## 과제 요구사항
기존 내용 유지

---

## 구현 내용

### 1. 데이터 로드 및 초기화

#### 요구사항
- Resources/Items.json 로드
- machineId, status, products 파싱

#### 구현 내용
- DataManager에서 Items.json을 로드하고 VendingMachineData로 파싱
- persistentDataPath/Items.json이 존재하면 수정된 데이터를 우선 로드
- 없을 경우 Resources/Items.json을 기본 데이터로 사용

#### 추가 구현
- 상품 구매로 재고가 변경되면 Items.json을 persistentDataPath에 저장
- 저장 시 updatedAt 값을 현재 UTC 시간으로 갱신

---

### 2. UI 상태 및 데이터 바인딩

#### 요구사항
- machineId 표시
- status에 따라 Power Light 색상 변경
- 상품 리스트 동적 생성

#### 구현 내용
- UI_VMInfo에서 machineId와 status UI 처리
- UI_ProductList에서 products 데이터를 기반으로 상품 아이템 동적 생성
- ProductViewData를 사용해 UI 표시 데이터와 원본 데이터를 분리

#### 추가 구현
- 상품 구매 후 해당 상품의 재고 UI만 Refresh
- Dictionary 기반으로 productId에 해당하는 UI 아이템을 빠르게 갱신

---

### 3. 핵심 비즈니스 로직

#### 요구사항
- 재화 획득
- 상품 구매
- 상품 소비

#### 구현 내용
- MoneyManager에서 현재 금액 관리
- InventoryManager에서 구매 상품 관리
- VmController에서 사용자 입력을 받아 Manager와 UI를 연결

#### 추가 구현
- 최대 금액 10,000원 제한
- 상품 구매 시 금액 차감, 재고 감소, 인벤토리 추가 처리
- 유저 정보 변경 시 UserData.json 을 저장
- 앱 재실행 시 UserData.json을 통해 money와 inventory 복원

---

### 4. 로그 시스템

#### 요구사항
- 재화 획득, 상품 구매, 상품 소비 로그 출력

#### 구현 내용
- LogManager에서 로그 이벤트 관리
- UI_LogList에서 로그 프리팹을 생성하여 ScrollView에 표시

#### 추가 구현
- 실패 로그와 차단 로그를 LogState.Error로 구분
- Inactive 상태에서 사용자 행동 차단 시 로그 출력
---

## 추가 구현 기능

### 1. Architecture

본 프로젝트는 MVC 기반 구조에 Singleton Manager를 결합하여 구현했습니다.

- `VmController`
  - UI 이벤트 연결
  - 사용자 입력 처리
  - Manager 호출 및 UI 갱신
- `DataManager`
  - Items.json / UserData.json 로드 및 저장
  - 상품 데이터, 사용자 데이터 관리
  - 상품 이미지 로드
- `MoneyManager`
  - 현재 금액 상태 관리
  - 금액 추가 / 차감 / 최대 금액 제한
- `InventoryManager`
  - 구매한 상품 인벤토리 상태 관리
  - 상품 소비 처리
- `LogManager`
  - 주요 사용자 행동 로그 전달
- `UI_*`
  - 화면 표시 및 사용자 입력 이벤트 전달

---

### 2. 데이터 저장 구조
- PC 나 Mobile 환경에서 파일 변경 시 저장
- UserData (Money, Inventory) 저장 기능 추가
- ItemsData 재고 및 updateAt 저장 기능 추가

Application.persistentDataPath
├─ Items.json
├─ UserData.json
└─ images


### 3. 외부 이미지 로드
- Items.json의 imageUrl 값을 기준으로 persistentDataPath에서 이미지 우선 검색
- 이미지가 없으면 Resources의 기본 이미지 사용
- 빌드 이후에도 json과 이미지 파일 교체를 통해 상품 이미지 변경 가능

### 4. 이벤트 기반 UI 갱신
- MoneyManager.OnMoneyChanged
- InventoryManager.OnAddItem
- LogManager.OnAddLog
각 Manager 의 상태 변경을 UI 가 이벤트로 받아 갱신하도록 구성했습니다.
---
