using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Oocw.Database.Models;
using MongoDB.Driver;
using Oocw.Database;
using System.Linq.Expressions;
using Yaap;
using System.Threading;

namespace Oocw.Cli.Utils;

public static class TitechUtils
{
    public const string ORG_MAPPING = 
@"{
  ""理学院"": ""org.sos"",
  ""工学院"": ""org.soe"",
  ""物質理工学院"": ""org.somct"",
  ""情報理工学院"": ""org.soc"",
  ""生命理工学院"": ""org.solst"",
  ""環境・社会理工学院"": ""org.soes"",
  ""教養科目"": ""crs.la"",
  ""専門科目"": ""crs.prof"",
  ""文系教養科目"": ""crs.la.la"",
  ""英語科目"": ""crs.la.en"",
  ""第二外国語科目"": ""crs.la.sfl"",
  ""日本語・日本文化科目"": ""crs.la.jp"",
  ""教職科目"": ""crs.la.edu"",
  ""キャリア科目"": ""crs.la.car"",
  ""広域教養科目"": ""crs.la.brd"",
  ""理工系教養科目"": ""crs.la.se"",
  ""共通専門科目"": ""crs.prof.common"",
  ""数学系"": ""org.m.u.mth"",
  ""物理学系"": ""org.m.u.phy"",
  ""化学系"": ""org.m.u.chm"",
  ""地球惑星科学系"": ""org.m.u.eps"",
  ""数学コース"": ""org.m.g.mth"",
  ""物理学コース"": ""org.m.g.phy"",
  ""化学コース"": ""org.m.g.chm"",
  ""地球惑星科学コース"": ""org.m.g.eps"",
  ""機械系"": ""org.m.u.mec"",
  ""システム制御系"": ""org.m.u.sce"",
  ""電気電子系"": ""org.m.u.eee"",
  ""情報通信系"": ""org.m.u.ict"",
  ""経営工学系"": ""org.m.u.iee"",
  ""機械コース"": ""org.m.g.mec"",
  ""システム制御コース"": ""org.m.g.sce"",
  ""電気電子コース"": ""org.m.g.eee"",
  ""情報通信コース"": ""org.m.g.ict"",
  ""経営工学コース"": ""org.m.g.iee"",
  ""材料系"": ""org.m.u.mat"",
  ""応用化学系"": ""org.m.u.cap"",
  ""材料コース"": ""org.m.g.mat"",
  ""応用化学コース"": ""org.m.g.cap"",
  ""数理・計算科学系"": ""org.m.u.mcs"",
  ""情報工学系"": ""org.m.u.cse"",
  ""数理・計算科学コース"": ""org.m.g.mcs"",
  ""情報工学コース"": ""org.m.g.cse"",
  ""生命理工学系"": ""org.m.u.lst"",
  ""生命理工学コース"": ""org.m.g.lst"",
  ""建築学 系"": ""org.m.u.arc"",
  ""土木・環境工学系"": ""org.m.u.cve"",
  ""融合理工学系"": ""org.m.u.tse"",
  ""社会・人間科学系"": ""org.m.u.shs"",
  ""建築学コース"": ""org.m.g.arc"",
  ""土木工学コース"": ""org.m.g.cve"",
  ""地球環境共創コース"": ""org.m.g.tse"",
  ""社会・人間科学コース"": ""org.m.g.shs"",
  ""イノベー ション科学コース"": ""org.m.g.isc"",
  ""技術経営専門職学位課程"": ""org.m.g.tim"",
  ""エネルギーコース"": ""org.m.g.enr"",
  ""原子核工学コース"": ""org.m.g.ncl"",
  ""ライフエンジニアリングコース"": ""org.m.g.hcb"",
  ""エンジニアリングデザインコース"": ""org.m.g.esd"",
  ""知能情報コース"": ""org.m.g.art"",
  ""都市・環境学コース"": ""org.m.g.ude"",
  ""地球生命コース"": ""org.m.g.els"",
  ""リーダーシップ教育課程"": ""org.ac.ldsp"",
  ""グローバルリーダー教育課程"": ""org.ac.glb_ldsp"",
  ""環境エネルギー協創教育課程"": ""org.ac.env_enr_coc"",
  ""情報生命博士教育課程"": ""org.ac.cmp_ls"",
  ""物質・情報卓越教育課程"": ""org.ac.conv_mat_info"",
  ""超スマート社会卓越教育課程"": ""org.ac.smart_soc"",
  ""エネルギー・情報卓越教育課程"": ""org.ac.enr_info"",
  ""領域横断的な大学院コース"": ""org.ext.trsdis"",
  ""学位プログラムとして特別に設けた教育課程"": ""org.ext.ac""
}";

    public const string KEY_NULL = "null";
    public const string KEY_UNCAT = "uncat";

    private static IReadOnlyDictionary<string, string> Orgs;

    static TitechUtils()
    {
        Orgs = JsonSerializer.Deserialize<Dictionary<string, string>>(ORG_MAPPING)!;
    }

    public static async Task RefreshOrganizations(this DBWrapper db, bool handleUncategorized = false, CancellationToken token = default)
    {
        Expression<Func<Course, bool>> filter = 
            handleUncategorized ? 
            x => x.Unit.Key == null || x.Unit.Key == KEY_NULL || x.Unit.Key == KEY_UNCAT : 
            x => x.Unit.Key == null || x.Unit.Key == KEY_NULL;

        var totalCount = (await db.Courses.CountDocumentsAsync(filter, cancellationToken: token));

        var cursor = 
            db is DBSessionWrapper dbSess ? 
            dbSess.Courses.FindAsync(dbSess.Session, filter, cancellationToken: token) : 
            db.Courses.FindAsync(filter, cancellationToken: token);

        long cnt = 0;
        foreach (var res in (await cursor).ToEnumerable())
        {
            var replStr = res.Unit.Ja != null && Orgs.ContainsKey(res.Unit.Ja) ? Orgs[res.Unit.Ja] : KEY_UNCAT;
            var setDef = Builders<Course>.Update.Set(x => x.Unit.Key, replStr);
            if (db is DBSessionWrapper dbSess2)
                await dbSess2.Courses.UpdateOneAsync(dbSess2.Session, x => x.IdRaw == res.IdRaw, setDef, cancellationToken: token);
            else
                await db.Courses.UpdateOneAsync(x => x.IdRaw == res.IdRaw, setDef, cancellationToken: token);
            
            ++cnt;
            if (cnt % 100 == 0)
                Console.WriteLine($"{cnt}/{totalCount}");
        }
        Console.WriteLine($"{cnt} uncategorized documents handled");
    }

}