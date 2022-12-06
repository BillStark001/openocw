using AngleSharp.Dom;
using Oocw.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Crawler.Models;

public class SyllabusRecord
{
    public class NoteRecord
    {
        public string Title { get; set; } = "";
        public string LectureType { get; set; } = "";
        public string LectureDate { get; set; } = "";
        public string[] Links { get; set; } = new string[] { };
    }

    public class DetailRecord
    {
        [MatchKey("Course description and aims")]
        [MatchKey("講義の概要とねらい")]
        public string Description { get; set; } = "";

        [MatchKey("Student learning outcomes")]
        [MatchKey("到達目標")]
        public string Outcomes { get; set; } = "";

        [MatchKey("Keywords")]
        [MatchKey("キーワード")]
        public string Keywords { get; set; } = "";

        [MatchKey("Class flow")]
        [MatchKey("授業の進め方")]
        public string ClassFlow { get; set; } = "";

        [MatchKey("Out-of-Class Study Time (Preparation and Review)")]
        [MatchKey("授業時間外学修（予習・復習等）")]
        public string OocStudyTime { get; set; } = "";

        [MatchKey("Textbook(s)")]
        [MatchKey("教科書")]
        public string Textbooks { get; set; } = "";

        [MatchKey("Reference books, course materials, etc.")]
        [MatchKey("参考書、講義資料等")]
        public string ReferenceBooks { get; set; } = "";

        [MatchKey("Assessment criteria and methods")]
        [MatchKey("成績評価の基準及び方法")]
        public string Assessment { get; set; } = "";

        [MatchKey("Related courses")]
        [MatchKey("関連する科目")]
        public string Related { get; set; } = "";

        [MatchKey("Prerequisites (i.e., required knowledge, skills, courses, etc.)")]
        [MatchKey("履修の条件(知識・技能・履修済科目等)")]
        public string Prerequisites { get; set; } = "";

        [MatchKey("Course taught by instructors with work experience")]
        [MatchKey("実務経験のある教員等による授業科目等")]
        public string InstructorExperience { get; set; } = "";

        [MatchKey("Contact information (e-mail and phone)    Notice : Please replace from \"[at]\" to \"@\"(half-width character).")]
        [MatchKey("Contact information (e-mail and phone)")]
        [MatchKey("連絡先（メール、電話番号）    ※”[at]”を”@”(半角)に変換してください。")]
        [MatchKey("連絡先（メール、電話番号）")]
        public string Contact { get; set; } = "";

        [MatchKey("Office hours")]
        [MatchKey("オフィスアワー")]
        public string OfficeHours { get; set; } = "";

        [MatchKey("Other")]
        [MatchKey("その他")]
        public string Other { get; set; } = "";
    }

    public class SummaryRecord
    {
        [MatchKey("Academic unit or major")]
        [MatchKey("開講元")]
        public string Unit { get; set; } = "";

        [MatchKey("Class Format")]
        [MatchKey("授業形態")]
        public string CourseType { get; set; } = "";

        [MatchKey("Media-enhanced courses")]
        [MatchKey("メディア利用科目")]
        public string MediaEnhanced { get; set; } = "";

        [MatchKey("Day/Period(Room No.)")]
        [MatchKey("曜日・時限(講義室)")]
        public string DayPeriod { get; set; } = "";

        [MatchKey("Group")]
        [MatchKey("クラス")]
        public string Class { get; set; } = "";

        [MatchKey("Course number")]
        [MatchKey("科目コード")]
        public string Code { get; set; } = "";

        [MatchKey("Credits")]
        [MatchKey("単位数")]
        public string Credit { get; set; } = "";

        [MatchKey("Academic year")]
        [MatchKey("開講年度")]
        public string Year { get; set; } = "";

        [MatchKey("Offered quarter")]
        [MatchKey("開講クォーター")]
        public string Quarter { get; set; } = "";

        [MatchKey("Syllabus updated")]
        [MatchKey("シラバス更新日")]
        public string SyllabusUpdate { get; set; } = "";

        [MatchKey("Lecture notes updated")]
        [MatchKey("講義資料更新日")]
        public string NotesUpdate { get; set; } = "";

        [MatchKey("Language used")]
        [MatchKey("使用言語")]
        public string Language { get; set; } = "";

        [MatchKey("Access Index")]
        [MatchKey("アクセスランキング")]
        public string AccessRanking { get; set; } = "";
    }

    public string YearStr { get; set; } = "";
    public string NameJa { get; set; } = "";
    public string NameEn { get; set; } = "";
    public SummaryRecord Summary { get; set; } = new();
    public DetailRecord Detail { get; set; } = new();
    public List<FacultyRecord> Faculties { get; set; } = new();
    public bool[] Skills { get; set; } = new bool[5];
    public List<(int, string, string)> Schedule { get; set; } = new();
    public List<(string, string)> Related { get; set; } = new();
    public List<NoteRecord> Notes { get; set; } = new();
}
