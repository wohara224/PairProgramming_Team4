# サーバー設計書

## 環境

Nugetパッケージ

- NLog : ログ出力
- NLog.Schema : ログ設定時のインテリセンス（候補入力）

## データ定義

科目：Subject

- 科目番号：Id (int)
- 名前：Name (string)
- 必須科目か？:IsRequired (bool)

生徒：Student

- 生徒番号：Id (int)
- 名前：Name (string)

成績：TestResult

- 管理番号：Id (int)
- 生徒：StudentInfo (Student)
- 科目：Subject (Subject)
- 点数：Score (int)

## リクエスト用DTO

/api/register：RegisterRequest

- 生徒ID：Student (int)
- 科目ID：Subject (int)
- 点数：Score (int)

## 200レスポンス用DTO

/api/performance?student=1：PerformanceResponse

- 生徒名：Name (string)
- 成績リスト：Subjects (List<SubjectScore>)

  - 科目成績：SubjectScore
    - 科目名：Name (string)
    - 点数：Score (int)

/api/dropout:DropoutResponse

/api/ranking?subject=1:RankingResponse

## 400/404レスポンス用DTO

エラー用オブジェクト：ErrorResponse

- エラーの種類：Error (string)
- エラーコード：Code (string)

## 画面構成

### ヘッダー

起動時

``` bash
========================================
  Server : Starting ...
  Port   : -
========================================
```

サーバー起動成功

``` bash
========================================
  Server : Succeed
  Port   : 50080  
========================================
```

ローカルホスト起動

``` bash
========================================
  Server : Warning (localhost)
  Port   : 50080 
========================================
```

サーバー起動失敗

``` bash
========================================
  Server : Failed
  Port   : -
========================================
```

### ビジネス中

通信待機中

``` bash
========================================
  Server : Succeed
  Port   : 50080  
========================================
Wait for client request ...
```

通信確立、リクエスト判別

``` bash
========================================
  Server : Succeed
  Port   : 50080  
========================================
Connecting 
```

通信確率

``` bash
========================================
  Server : Succeed
  Port   : 50080  
========================================
Connecting
```

## ログ定義

- System.*、Microsoft.*系は除外する
- 通信ログは、*Api.*を対象に出力する
- システムログは、上記以外を対象とする

通信ログ
``` text:api-20260605.log
2026-06-05 16:18:28.972|Info|GradeJudge.Server.Api.ResponseCreator|Listener起動を試行
2026-06-05 16:18:29.123|Info|GradeJudge.Server.Api.ResponseCreator|PORT:12380でListener起動を試行
2026-06-05 16:18:29.349|Error|GradeJudge.Server.Api.ResponseCreator|エラー: アクセスが拒否されました。
2026-06-05 16:18:29.443|Info|GradeJudge.Server.Api.ResponseCreator|Listenerを終了
2026-06-05 16:29:48.333|Info|GradeJudge.Server.Api.ResponseCreator|PORT:50080でListener起動を試行
2026-06-05 16:29:48.587|Info|GradeJudge.Server.Api.ResponseCreator|Listener起動
2026-06-05 16:30:28.912|Info|GradeJudge.Server.Api.ResponseCreator|通信確立
2026-06-05 16:30:29.123|Info|GradeJudge.Server.Api.ResponseCreator|GETリクエスト受信
2026-06-05 16:30:29.578|Warn|GradeJudge.Server.Api.ResponseCreator|リクエスト不正: 生徒ID:999は存在しません。
2026-06-05 16:30:29.843|Info|GradeJudge.Server.Api.ResponseCreator|レスポンス送信
```

システムログ
``` text:system-20260605.log
2026-06-05 16:18:28.882|Info|GradeJudge.Server.Program|アプリケーション起動
2026-06-05 16:18:29.591|Error|GradeJudge.Server.Program|エラー: サーバー確立失敗
2026-06-05 16:18:29.689|Info|GradeJudge.Server.Program|アプリケーション終了 ExitCode=1
2026-06-05 16:29:47.904|Info|GradeJudge.Server.Program|アプリケーション起動
```

