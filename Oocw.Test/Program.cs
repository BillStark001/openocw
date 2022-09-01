// See https://aka.ms/new-console-template for more information


using Oocw.Query;

var res = Lexer.NaiveMatch("a \\in [ false , true, , 2, \"3\", 4] && b #< ['ffff', 'fffffff']");
foreach (var lex in res)
    Console.WriteLine(lex);