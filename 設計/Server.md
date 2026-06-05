# サーバー設計書





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

