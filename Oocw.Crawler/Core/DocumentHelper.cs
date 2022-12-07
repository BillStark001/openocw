using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Oocw.Base;
using Oocw.Crawler.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oocw.Crawler.Core;

public static class DocumentHelper
{

    static DocumentHelper()
    {


    }


    public delegate bool NodeSelector(INode node);
    public delegate T NodeParser<T>(int i, INode node, IEnumerable<INode> nodes);

    public static NodeSelector ByName(params string[] names) { return x => names.Contains(x.NodeName); }

    public static IEnumerable<(int, INode, IEnumerable<INode>)> SelectAsList(this IElement content, NodeSelector keySelector)
    {
        var items = content.ChildNodes;

        var dss = new List<(int, INode, IEnumerable<INode>)>();
        var hs = Enumerable.Range(0, items.Length).Where(i => keySelector(items[i])).ToList();
        for (int j = 0; j < hs.Count; ++j)
        {
            var i = hs[j];
            var valStart = i + 1;
            var valEnd = j == hs.Count - 1 ? items.Count() : hs[j + 1];
            var keyNode = items[i];
            var valNodes = items.Skip(valStart).Take(valEnd - valStart);
            dss.Add((i, keyNode, valNodes));
        }
        return dss;
    }

    public static Dictionary<string, List<string>> ParseAsVerticalIndexedTable(this IHtmlTableElement table)
    {

        var dss = new Dictionary<string, List<string>>();
        int i = 0;
        foreach (var item in table.Rows)
        {
            var nodes = item.QuerySelectorAll("th,td");
            var keyNode = nodes.First();

            var key = keyNode.TextContent.ToHalfWidth().Cleanout().Replace(" ", "");
            var val = new StringBuilder();

            foreach (var valNode in nodes.Skip(1))
            {
                val.AppendLine(valNode.TextContent);
            }

            dss[key] = val.ToString().ToHalfWidth()
                .Replace("\r\n", "\n").Split('\n')
                .Select(x => x.Cleanout()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            ++i;
        }
        return dss;
    }

    public static IEnumerable<(string, string)> GetAllAnchors(this IElement element, bool recursive = false)
    {
        IEnumerable<IHtmlAnchorElement?> elems;
        if (recursive)
            elems = element.QuerySelectorAll<IHtmlAnchorElement>("a");
        else
            elems = element.GetElementsByTagName("a").Select(x => x as IHtmlAnchorElement);
        return elems.Where(x => x != null).Select(x => (x!.InnerHtml, x.Href));
    }

    public static (IReadOnlyList<string>, IEnumerable<IHtmlTableRowElement>) GetRowSelector(this IHtmlTableElement table)
    {

        IReadOnlyList<string> headers = new List<string>();
        var headCells = table.Rows.First().Cells;
        foreach (var cell in headCells)
            ((List<string>)headers).Add(cell.TextContent.ToHalfWidth().Cleanout().Replace(" ", ""));
        HashSet<string> x = new();
        for (int i = 0; i < headers.Count; ++i)
        {
            if (x.Contains(headers[i]))
                ((List<string>)headers)[i] = headers[i] + $"_{i}";
            x.Add(headers[i]);
            x.Add(headers[i] + "_url");
        }
        headers = ((List<string>)headers).AsReadOnly();

        return (headers, table.Rows.Skip(1));
    }

    public static (IReadOnlyList<string>, List<Dictionary<string, List<string>>>) ParseAsHorizontalIndexedTable(this IHtmlTableElement table)
    {

        var (headers, rows) = table.GetRowSelector();

        List<Dictionary<string, List<string>>> res = new();

        foreach (var row in table.Rows.Skip(1))
        {
            Dictionary<string, List<string>> _res = new();

            var cells = row.Cells;

            for (int i = 0; i < headers.Count; ++i)
            {
                if (cells.Length <= i)
                    continue;
                var cell = cells[i];
                var key = headers[i];
                var links = cell.QuerySelectorAll<IHtmlAnchorElement>("a").Select(x => (x.Href, "_url"))
                    .Concat(cell.QuerySelectorAll<IHtmlImageElement>("img").Select(x => (x.Source ?? "", "_img")));
                foreach (var (link, suffix) in links)
                {
                    if (!string.IsNullOrWhiteSpace(link))
                    {
                        var urlKey = key + suffix;
                        if (!_res.ContainsKey(urlKey))
                            _res[urlKey] = new();
                        _res[urlKey].Add(link);
                    }
                    // cell = link;
                }
                _res[key] = cell.TextContent.ToHalfWidth()
                    .Replace("\r\n", "\n").Split('\n')
                    .Select(x => x.Cleanout()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            }
            res.Add(_res);
        }

        return (headers, res);
    }

}