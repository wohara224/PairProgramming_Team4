# API仕様書

## 処理の流れ

サーバー開設

1. 通信立上げ
2. クライアントリクエスト待ち受け

【必須】成績登録：POST

1. JSONから登録情報を受け取る
2. バリデーションOKなら200、NGなら400

    - 正常なリクエスト：200（追加しました）
    - 正常で既存データがあった：200（更新しました）
    - JSONが不正；400エラー（リクエストが不正です）
    - 生徒が存在しない：400エラー（存在しない生徒が指定されました）
    - 科目が存在しない：400エラー（存在しない科目が指定されました）
    - 点数が範囲外：400エラー（点数は0-100の範囲です）

【必須】個人成績取得：GET

1. JSON内から付加情報を取り出す
2. 生徒IDをもらってデータを返す

    - 正常なリクエスト：200（全科目分返す）
    - IDが不正：400エラー（生徒IDが不正です）
    - 生徒が存在しない：404エラー（生徒ID:**は存在しません）

【オプション】落第者通知：GET

1. JSON内から付加情報を取り出す
2. 必修科目の成績が60点未満の生徒一覧を返す

    - 正常なリクエスト：200（一覧を返す）
    - 正常なリクエスト（落第者0）：200（空リストを返す）

【オプション】科目別ランキング：GET

1. JSON内から付加情報を取り出す
2. 科目に従って上位5名分のデータを返す。

    - 正常なリクエスト：200（5名分のその科目の結果を返す）
    - IDが不正：400エラー（科目IDが不正です）
    - 科目が存在しない：404エラー（科目ID:**は存在しません）

## API仕様

接続URL：http://xxx.xxx.xxx.xxx:50080

成績登録<POST>：/api/register?id=*

個人成績取得<GET>：/api/performance

落第者通知<GET>：/api/dropout

科目別ランキング<GET>：/api/ranking?subject=*

## リクエストボディ

/api/register <POST>
``` json
{
  "studentId": 1,
  "subjects" : [
    { "subjectId": 1, "score": 94 },
    { "subjectId": 2, "score": 47 },
    { "subjectId": 3, "score": 83 }
  ]
}
```

/api/performance?student=1 <GET>
``` json
{}
```

/api/dropout <GET>
``` json
{}
```

/api/ranking?subject=1 <GET>
``` json
{}
```

## レスポンスボディ (200)

/api/register <POST>
``` json
{}
```

/api/performance?student=1 <GET>
``` json
{
  "name": "田中",
  "subjects": [
    { "name": "数学", "score": 94 },
    { "name": "英語", "score": 47 },
    { "name": "物理", "score": 83 }
  ]
}
```

/api/dropout <GET>
``` json
[
  {
    "name": "伊藤",
    "subjects": [
      { "name": "数学", "score": 42 },
      { "name": "物理", "score": 38 }
    ]
  },
  {
    "name": "鈴木",
    "subjects": [
      { "name": "英語", "score": 56 }
    ]
  }
]
```

/api/ranking?subject=1 <GET>
``` json
{
  "subject": "数学",
  "students": [
    { "name": "山本", "score": 98 },
    { "name": "田中", "score": 94 },
    { "name": "柏", "score": 93 },
    { "name": "宇田", "score": 89 },
    { "name": "藤田", "score": 87 }
  ]
}
```

## レスポンスボディ (400)

/api/register <POST> / リクエスト不正
``` json
{
  "error": "Bad Request",
  "code": "INVALID_REQUEST"
}
```

/api/register <POST> / 生徒が存在しない
``` json
{
  "error": "Bad Request",
  "code": "STUDENT_NOT_EXIST"
}
```

/api/register <POST> / 科目が存在しない
``` json
{
  "error": "Bad Request",
  "code": "SUBJECT_NOT_EXIST"
}
```

/api/register <POST> / 点数が範囲外
``` json
{
  "error": "Bad Request",
  "code": "SCORE_OUT_OF_RANGE"
}
``` 

/api/performance?student=abc <GET> / 生徒ID不正
``` json
{
  "error": "Bad Request",
  "code": "INVALID_STUDENT_ID"
}
```

/api/ranking?subject=abc <GET> / 科目ID不正
``` json
{
  "error": "Bad Request",
  "code": "INVALID_SUBJECT_ID"
}
```

## レスポンスボディ (404)

/api/performance?student=999 <GET> / 生徒が存在しない
``` json
{
  "error": "Not Found",
  "code": "STUDENT_NOT_FOUND"
}
```

/api/ranking?subject=999 <GET> / 科目が存在しない
``` json
{
  "error": "Not Found",
  "code": "SUBJECT_NOT_FOUND"
}
```
