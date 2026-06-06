using System;
using System.Collections.Generic;
using System.Text;
using static Program;

namespace GradeJudge.Client.Model
{
    // 生徒と点数
    public class Student
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
    // 科目ごとの生徒ランキング
    public class SubjectGrade
    {
        public string Name { get; set; }
        public List<Student> Students { get; set; }
    }

}
