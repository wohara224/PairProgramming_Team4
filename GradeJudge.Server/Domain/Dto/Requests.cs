namespace GradeJudge.Server.Domain.Dto;

// === リクエスト本体 ===

// 成績登録
public record RegisterRequest(int StudentId, int SubjectId, int Score);