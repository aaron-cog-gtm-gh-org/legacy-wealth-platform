using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace PV.Core
{
    // RJM 2013-05, reworked KP 2016-02: the whole archive is small enough to
    // parse at startup. Holds statements by safe acct/date and confirmations
    // per day folder. Admin page can rebuild it without a recycle.
    public class ArchiveIndex
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ArchiveIndex));
        private static readonly object Gate = new object();
        private static ArchiveIndex _current;

        public string Root;
        public DateTime LoadedAt;
        public long LoadMs;
        public int Mt535Count;
        public int Mt54xCount;
        public int ParseErrors;

        // safe acct (unpadded) -> stmt date -> list of statements for that day
        public Dictionary<string, Dictionary<DateTime, List<Mt535Statement>>> Statements
            = new Dictionary<string, Dictionary<DateTime, List<Mt535Statement>>>();

        // day folder -> confirmations in file-name order
        public Dictionary<string, List<SettlementConfirm>> ConfirmFolders
            = new Dictionary<string, List<SettlementConfirm>>();

        // flat list for the message browser
        public List<ArchiveFileEntry> Files = new List<ArchiveFileEntry>();

        public class ArchiveFileEntry
        {
            public string FullPath;
            public string FileName;
            public string MsgType;   // MT535 / MT54x
            public DateTime Date;    // statement/confirm date from the name
            public string SafeCode;
            public long Size;
        }

        public static ArchiveIndex Current
        {
            get
            {
                lock (Gate) return _current;
            }
        }

        public static ArchiveIndex Build(string root)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var idx = new ArchiveIndex();
            idx.Root = root;

            string d535 = Path.Combine(root, "mt535");
            if (Directory.Exists(d535))
            {
                foreach (string f in Directory.GetFiles(d535, "*.txt"))
                {
                    idx.Mt535Count++;
                    idx.Files.Add(MakeEntry(f, "MT535"));
                    try
                    {
                        Mt535Statement st = SwiftParser.ParseStatement(File.ReadAllText(f), f);
                        string key = st.SafekeepingAcct != null ? st.SafekeepingAcct.TrimStart('0') : "";
                        Dictionary<DateTime, List<Mt535Statement>> byDate;
                        if (!idx.Statements.TryGetValue(key, out byDate))
                        {
                            byDate = new Dictionary<DateTime, List<Mt535Statement>>();
                            idx.Statements[key] = byDate;
                        }
                        List<Mt535Statement> day;
                        if (!byDate.TryGetValue(st.StatementDate, out day))
                        {
                            day = new List<Mt535Statement>();
                            byDate[st.StatementDate] = day;
                        }
                        day.Add(st);
                    }
                    catch (Exception ex)
                    {
                        idx.ParseErrors++;
                        Log.Warn("archive parse error " + Path.GetFileName(f) + ": " + ex.Message);
                    }
                }
            }

            string d54x = Path.Combine(root, "mt54x");
            if (Directory.Exists(d54x))
            {
                foreach (string dir in Directory.GetDirectories(d54x))
                {
                    string dayName = Path.GetFileName(dir);
                    var confs = new List<SettlementConfirm>();
                    foreach (string f in Directory.GetFiles(dir, "*.txt"))
                    {
                        idx.Mt54xCount++;
                        idx.Files.Add(MakeEntry(f, "MT54x"));
                        try
                        {
                            confs.Add(SwiftParser.ParseConfirm(File.ReadAllText(f), f));
                        }
                        catch (Exception ex)
                        {
                            idx.ParseErrors++;
                            Log.Warn("archive parse error " + Path.GetFileName(f) + ": " + ex.Message);
                        }
                    }
                    // Directory.GetFiles order is not guaranteed; keep name order
                    confs.Sort((a, b) => string.CompareOrdinal(
                        Path.GetFileName(a.FilePath), Path.GetFileName(b.FilePath)));
                    idx.ConfirmFolders[dayName] = confs;
                }
            }

            idx.Files.Sort((a, b) => string.CompareOrdinal(a.FullPath, b.FullPath));
            idx.LoadedAt = DateTime.Now;
            idx.LoadMs = sw.ElapsedMilliseconds;
            Log.Info(string.Format(
                "Archive loaded from {0}: {1} MT535, {2} MT54x, {3} stmt accounts, {4} confirm days, {5} parse errors in {6} ms",
                root, idx.Mt535Count, idx.Mt54xCount, idx.Statements.Count,
                idx.ConfirmFolders.Count, idx.ParseErrors, idx.LoadMs));
            return idx;
        }

        public static void Reload(string root)
        {
            ArchiveIndex idx = Build(root);
            lock (Gate) _current = idx;
        }

        private static ArchiveFileEntry MakeEntry(string path, string type)
        {
            var e = new ArchiveFileEntry();
            e.FullPath = path;
            e.FileName = Path.GetFileName(path);
            e.MsgType = type;
            e.Size = new FileInfo(path).Length;
            // MT535_<safe>_<yyyyMMdd>_NNN.txt / MT54n_<safe>_<yyyyMMdd>_NNN.txt
            string[] parts = Path.GetFileNameWithoutExtension(path).Split('_');
            if (parts.Length >= 3)
            {
                e.SafeCode = parts[1];
                DateTime d;
                if (DateTime.TryParseExact(parts[2], "yyyyMMdd",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None, out d))
                    e.Date = d;
            }
            return e;
        }
    }
}
