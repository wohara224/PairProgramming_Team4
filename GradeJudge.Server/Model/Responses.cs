namespace GradeJudge.Server.Model;

// === レスポンス本体 ===

// 個人成績
public record PerformanceResponse(string Name, List<SubjectScore> Subjects);

// 落第者
public record DropoutResponse(List<DropoutStudent> DropoutStudents);

// ランキング
public record RankingResponse(string Subject, List<StudentScore> Students);

// 汎用エラー
public record ErrorResponse(string Error, string Code);


// === 内部レコード ===

// 科目ごとの点数
public record SubjectScore(string Name, int Score);

// 落第者の指定科目における成績
public record DropoutStudent(string Name, List<SubjectScore> Subjects);

// 指定科目における生徒の成績
public record StudentScore(string Name, int Score);