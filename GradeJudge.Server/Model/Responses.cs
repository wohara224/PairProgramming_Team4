namespace GradeJudge.Server.Model;

// === レスポンス本体 ===

// 個人別スコア
public record StudentScoresResponse(string Name, List<SubjectScore> Subjects);

// 科目別成績リスト
public record SubjectScoresResponse(string Name, List<StudentScore> Students);

// 汎用エラー
public record ErrorResponse(List<ErrorItem> Errors);


// === 内部レコード ===

// 科目ごとの点数
public record SubjectScore(string Name, int Score);

// 指定科目における生徒の成績
public record StudentScore(string Name, int Score);

// エラーメッセージの本体
public record ErrorItem(string Message);