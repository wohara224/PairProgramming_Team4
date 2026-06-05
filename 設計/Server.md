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
2026-06-05 16:18:29.591|Info|GradeJudge.Server.Program|アプリケーション終了 ExitCode=0
2026-06-05 16:29:47.904|Info|GradeJudge.Server.Program|アプリケーション起動
```

