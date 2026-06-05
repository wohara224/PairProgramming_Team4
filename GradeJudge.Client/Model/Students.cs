using System;
using System.Collections.Generic;
using System.Text;

namespace GradeJudge.Client.Model
{
    // 科目と点数
    public class Subject
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
    // 生徒ごとの各科目の点数
    public class StudentGrade
    {
        public string Name { get; set; }
        public List<Subject> Subjects { get; set; }
    }
}
