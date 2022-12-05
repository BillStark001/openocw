using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Crawler.Models;

public class SyllabusRecord
{
    public class Note
    {
        public string Title { get; set; } = "";
        public string LectureType { get; set; } = "";
        public string LectureDate { get; set; } = "";
        public string[] Links { get; set; } = new string[] { };
    }
    public string YearStr { get; set; } = "";
    public string NameJa { get; set; } = "";
    public string NameEn { get; set; } = "";
    public Dictionary<string, string> Summary { get; set; } = new();
    public Dictionary<string, string> Detail { get; set; } = new();
    public List<FacultyRecord> Faculties { get; set; } = new();
    public bool[] Skills { get; set; } = new bool[5];
    public List<(int, string, string)> Syllabus { get; set; } = new();
    public List<(string, string)> Related { get; set; } = new();
    public List<Note> Notes { get; set; } = new();
}
