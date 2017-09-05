이벤트 소싱 예제 웹 응용프로그램입니다.

# 구동 방법

## 이벤트 허브 설정

SocialFake 응용프로그램은 Azure Event Hubs를 사용합니다. 응용프로그램이 동작하려면 호스트 응용프로그램의 Web.config 파일에 Azure Event Hubs 연결을 설정해야 합니다.

### Identity\SocialFake.Identity.Domain.Host\Web.config

```xml
<connectionStrings>
  <add name="MessagingNamespace" connectionString="Microsoft.EventHub/namespaces 연결 문자열" />
  <add name="MessagingStorage" connectionString="이벤트 처리기 저장소 계정 연결 문자열" />
</connectionStrings>
<appSettings>
  <add key="MessageStream" value="Microsoft.EventHub/namespaces/eventhubs 인스턴스 이름" />
  <add key="DomainConsumerGroup" value="Identity 도메인 소비자 그룹 이름" />
</appSettings>
```

### Facade\SocialFake.Facade.Host\Web.config

```xml
<connectionStrings>
  <add name="MessagingNamespace" connectionString="Microsoft.EventHub/namespaces 연결 문자열" />
  <add name="MessagingStorage" connectionString="이벤트 처리기 저장소 계정 연결 문자열" />
</connectionStrings>
<appSettings>
  <add key="MessageStream" value="Microsoft.EventHub/namespaces/eventhubs 인스턴스 이름" />
  <add key="FacadeConsumerGroup" value="Facade 소비자 그룹 이름" />
</appSettings>
```

## 시작 프로젝트 설정

SocialFake.Identity.Domain.Host 프로젝트와 SocialFake.Facade.Host 프로젝트를 시작 프로젝트로 설정합니다.

1. 솔루션 탐색기의 솔루션 항목에서 오른쪽 버튼 클릭
1. 'Set Startup Projects...' 메뉴 선택
1. 다이얼로그에서 'Multiple startup projects' 라디오 버튼 선택
1. SocialFake.Identity.Domain.Host 프로젝트와 SocialFake.Facade.Host 프로젝트의 Action을 Start로 설정
1. 'OK' 버튼 클릭
