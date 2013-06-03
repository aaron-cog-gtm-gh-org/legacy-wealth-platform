using System.Collections.Generic;
using System.Data;

namespace PV.Data
{
    // RJM 2013-06: reference table is small, cache the whole isin->name map.
    // TODO RJM 2015-02: this never refreshes, bounce the app to pick up new ISINs
    public static class InstrumentData
    {
        private static Dictionary<string, string> _names;

        public static Dictionary<string, string> GetNameMap()
        {
            if (_names == null)
            {
                var map = new Dictionary<string, string>();
                DataTable dt = SqlHelper.ExecuteDataTable("SELECT ISIN, INSTR_NAME FROM INSTRUMENT");
                foreach (DataRow r in dt.Rows)
                    map[r["ISIN"].ToString().Trim()] = r["INSTR_NAME"].ToString();
                _names = map;
            }
            return _names;
        }

        public static string GetName(string isin)
        {
            string name;
            return GetNameMap().TryGetValue(isin, out name) ? name : null;
        }

        public static void ResetCache()
        {
            _names = null;
        }
    }
}
