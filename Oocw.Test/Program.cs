﻿// See https://aka.ms/new-console-template for more information

using Oocw.Base;
using Oocw.Crawler.Core;
using Oocw.Query;
using System.Linq.Expressions;

var res = Lexer.NaiveMatch("a \\in [ false , true, , 2, \"3\", 4] && b #< ['ffff', 'fffffff']");
foreach (var lex in res)
    Console.WriteLine(lex);

var driver = new DriverWrapper("G:/chromedriver.exe");
var crawler = new Crawler(driver);

Func<int, int> finc = x => x + 1;
Func<int, int> fadd3 = x => finc(finc(finc(x)));
Expression<Func<int, int>> eadd3 = x => fadd3(x);
Expression<Func<int, int>> eadd31 = x => finc(finc(finc(x)));
Expression<Func<int, int>> eadd32 = x => x + 3;
Expression<Func<int, int>> eadd6 = ExpressionUtils.Combine(eadd3, eadd32);


driver.Initialize();
crawler.Task2(Meta.savepath_course_list_raw);
crawler.Task3(Meta.savepath_course_list_raw, Meta.savepath_details_raw, 2020, 2022);