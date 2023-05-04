# PrintSolution

## 소개
원격 프린트 기능을 제공하는 솔루션 입니다.   
WebAPI를 설치한 서버로 PDF파일을 전송하여 해당 서버에 연결된 프린터로 출력하는 기능이 있습니다.

## 동작방법
1. 해당 프로젝트를 빌드 후 생성된 `PrintSolutionAPI.exe`를 실행합니다.
2. `Visual Studio Code`에 있는 `Live Server`를 통해 `SampleWeb`폴더에 있는 `Index.html`를 동작 시킵니다.
3. `Index.html`웹 페이지에서 프린터, PDF파일을 선택하고 출력버튼을 누르면 프린터가 동작합니다.

## 개발환경
 - Window 운영체제
 - Visual Studio 2022
 - Visual Studio Code   
```해당 솔루션은 Window 운영체제에서만 지원하고 있습니다.```

## 패키지
 - FreeSpire.Doc : Spire 무료 버전을 사용하고 있습니다. PDF 출력 시 3장까지만 출력이 가능합니다. 해당 재한을 해제하려면 Spire 라이센스 구매가 필요합니다.

## 기타

### 개발중
 - PDF -> PCL로 변환 -> 출력