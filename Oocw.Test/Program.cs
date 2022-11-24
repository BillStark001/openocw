// See https://aka.ms/new-console-template for more information


using Oocw.Crawler.Core;
using Oocw.Query;

var res = Lexer.NaiveMatch("a \\in [ false , true, , 2, \"3\", 4] && b #< ['ffff', 'fffffff']");
foreach (var lex in res)
    Console.WriteLine(lex);

var driver = new DriverWrapper("G:/chromedriver.exe");
var crawler = new Crawler(driver);

driver.init_driver();
crawler.task2(Meta.savepath_course_list_raw);