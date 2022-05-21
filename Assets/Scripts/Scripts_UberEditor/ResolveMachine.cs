using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ResolveMachine
{   
    // Main resolver class
    public class ResolveMaster
    {
        #pragma warning disable 0649
        public Func<string, string, StatsClass, bool> ResolveCondition;
        public Func<string, string, StatsClass, bool> ResolveAction;
        #pragma warning restore 0649

        public bool DebugInfo = false;
        public bool DetailLogs = false;
        public Dictionary<string, decimal> Counters = new Dictionary<string, decimal>();
        public Dictionary<string, string> Texts = new Dictionary<string, string>();
        public List<string> _gates = new List<string>();
        private Dictionary<string, ResolveData> _datas = new Dictionary<string, ResolveData>();

        // Get data
        internal ResolveData GetResolveData(string datname)
        {
            return (_datas.ContainsKey(datname)) ? _datas[datname] : null;
        }
        // Add new editor data to resolve
        public void AddDataNode(string dataname, Dictionary<string, StatsClass> data)
        {
            ResolveData rd = new ResolveData(this, dataname, data);
            _datas.Add(dataname, rd);
            // Log
            if (Application.isEditor && DetailLogs) Debug.Log(string.Format("MASTER [{0}]: Initialized {1} nodes with {2} behaviors!", dataname, rd.DataCount, rd.BehaviorCount));
        }
        internal void ModifyDataNode(string dataname, Dictionary<string, StatsClass> data)
        {
            if (data == null) return;
            if (_datas.ContainsKey(dataname))
            {
                _datas[dataname].ModifyData(data);
                // Log
                if (Application.isEditor && DetailLogs) Debug.Log(string.Format("MASTER [{0}]: Modified {1} nodes!", dataname, data.Count));
            }
        }
        internal void AllowLoops(string dataname)
        {
            if (_datas.ContainsKey(dataname)) _datas[dataname].AllowLoops = true;
        }
        public List<StatsClass> GetDataKeys(string dataname)
        {
            return (_datas.ContainsKey(dataname)) ? new List<StatsClass>(_datas[dataname].GetDataKeys().Values) : new List<StatsClass>();
        }
        // Add new data resolver to data node
        internal ResolveSlave AddDataSlave(string dataname, string keyname)
        {
            return (_datas.ContainsKey(dataname)) ? _datas[dataname].InitSlave(keyname) : null;
        }
        // Add new data resolver to data node
        internal void KillSlave(string dataname, ResolveSlave slave)
        {
            if (_datas.ContainsKey(dataname))
            {
                _datas[dataname].Fails.Remove(slave.GetHeader.Title);
                _datas[dataname].KillSlave(slave);
            }
        }
        // Add new data resolver to data node
        internal void KillAllSlaves(string dataname, string keyname)
        {
            if (_datas.ContainsKey(dataname)) _datas[dataname].KillSlaves(keyname);
        }
        internal string GetText(string key)
        {
            return (Texts.ContainsKey(key)) ? Texts[key] : key;
        }
        internal decimal GetCounter(string key)
        {
            return Counters.ContainsKey(key) ? Counters[key] : 0m;
        }
        // Export state data
        internal StatsClass ExportState()
        {
            StatsClass data = new StatsClass("RM");
            // Gates
            data.AddStat("Gates", _gates.Count.ToString());
            // Texts
            foreach (var s in Texts.Keys) data.AddStat(s, Texts[s]);
            // Counter
            foreach (var s in Counters.Keys) data.AddStat(s, Counters[s]);
            return data;
        }
        // Gate handling
        internal void AddGate(string key)
        {
            _gates.Add(key);
        }
        // Gate handling
        internal bool GetGate(string key)
        {
            return _gates.Contains(key);
        }
        // Reset all variables
        internal void Reset()
        {
            Counters = new Dictionary<string, decimal>();
            Texts = new Dictionary<string, string>();
            _gates = new List<string>();
        }
        // Clear all data
        internal void Clear()
        {
            Reset();
            foreach (var s in _datas.Values) s.Clear();
            _datas = new Dictionary<string, ResolveData>();
        }
        // Texts
        internal void AddText(string key, string val)
        {
            Texts.Remove(key);
            Texts.Add(key, val);
        }
        internal void AddCounter(string key, decimal val)
        {
            Counters.Remove(key);
            Counters.Add(key, val);
        }
        internal void PurgeCounters(string klic)
        {
            foreach (var kl in Counters.Where(x => x.Key.StartsWith(klic)).Select(x => x.Key).ToList()) Counters.Remove(kl);
        }
        internal void PurgeTexts(string klic)
        {
            foreach (var kl in Texts.Where(x => x.Key.StartsWith(klic)).Select(x => x.Key).ToList()) Texts.Remove(kl);
        }
        internal string LogTexts()
        {
            var ret = "";
            foreach (var tx in Texts) ret = $"{ret} {tx.Key}={tx.Value}";
            return ret;
        }
        internal string LogCounters()
        {
            var ret = "";
            foreach (var cn in Counters) ret = $"{ret} {cn.Key}={cn.Value:0.###}";
            return ret;
        }
    }
    // Class for resolving one data file
    internal class ResolveData
    {
        private ResolveMaster _rm;
        private string _title;
        private Dictionary<int, bool[]> _behav = new Dictionary<int, bool[]>();
        private Dictionary<string, StatsClass> _fulldata = new Dictionary<string, StatsClass>();
        private List<ResolveSlave> _slaves = new List<ResolveSlave>();
        public bool DetailLogs { get { return _rm != null && _rm.DetailLogs; } }
        public bool AllowLoops { get; set; }

        internal int DataCount { get { return _fulldata.Count; } }
        internal int BehaviorCount { get { return _behav.Count; } }

        internal Dictionary<string, int> Fails = new Dictionary<string, int>();
        internal static int? MaxChoices;

        // Constructor
        internal ResolveData(ResolveMaster rm, string title, Dictionary<string, StatsClass> data)
        {
            _rm = rm;
            _title = title;
            _fulldata = data;
            // Separate behavior table
            StatsClass bstat = data.Keys.Where(x => x.Contains("$B")).Select(x => data[x]).FirstOrDefault();
            if (bstat != null)
            {
                foreach (var s in bstat.GetKeys())
                {
                    bool[] bhs = new bool[10];
                    foreach (var b in bstat.GetStrStat(s).Split(',')) bhs[Convert.ToInt32(b.Substring(1))] = true;
                    _behav.Add(Convert.ToInt32(s), bhs);
                }
                _fulldata.Remove(bstat.Title);
            }
        }
        internal void ModifyData(Dictionary<string, StatsClass> data)
        {
            List<string> datas = data.Keys.Where(x => _fulldata.ContainsKey(x)).Select(x => x).ToList();
            foreach (string key in datas)
            {
                if (_fulldata.ContainsKey(key)) _fulldata[key].MergeStats(data[key]);
            }
        }
        internal Dictionary<string, StatsClass> GetDataKeys()
        {
            return _fulldata;
        }
        // Initialize new data slave
        internal ResolveSlave InitSlave(string keyname)
        {
            if (_fulldata.ContainsKey(keyname))
            {
                ResolveSlave newslave = new ResolveSlave(this, new StatsClass(_fulldata[keyname]));
                _slaves.Add(newslave);
                return newslave;
            }
            return null;
        }
        // Remove slaves with key name from memory
        internal void KillSlaves(string keyname)
        {
            List<ResolveSlave> list = _slaves.Where(x => x.Keyname == keyname).ToList();
            foreach (var s in list) KillSlave(s);
        }
        // Remove slave instance from memory
        internal void KillSlave(ResolveSlave rs)
        {
            rs.Clear();
            _slaves.Remove(rs);
        }
        // Check if element type index has behavior
        internal bool Is(int node, int what)
        {
            return _behav.ContainsKey(node) && _behav[node][what];
        }
        // Resolve condition on element
        internal bool GetCondition(StatsClass data, string id)
        {
            bool? ret = _rm.ResolveCondition?.Invoke(_title, id, data);
            return ret.HasValue && ret.Value;
        }
        // Resolve action on element
        internal bool GetAction(StatsClass data, string id)
        {
            bool? ret = _rm.ResolveAction?.Invoke(_title, id, data);
            return ret.HasValue && ret.Value;
        }
        // Gate handling
        internal void AddGate(string node)
        {
            _rm.AddGate(node);
        }
        // Does counter or text exists?
        internal bool HasKey(string key)
        {
            bool ret = false;
            ret = _rm.Counters.ContainsKey(key);
            if (!ret) ret = _rm.Texts.ContainsKey(key);
            return ret;
        }
        // Counters
        internal void AddCounter(string key, decimal val)
        {
            _rm.AddCounter(key, val);
        }
        internal decimal GetCounter(string key)
        {
            return _rm.GetCounter(key);
        }
        // Gate handling
        internal bool GetGate(string node)
        {
            return _rm.GetGate(node);
        }
        // Clear all data
        internal void Clear()
        {
            foreach (var s in _slaves) s.Clear();
            _slaves = new List<ResolveSlave>();
            _fulldata = new Dictionary<string, StatsClass>();
            _behav = new Dictionary<int, bool[]>();
        }
        internal static string CodeToSign(string code)
        {
            switch (code)
            {
                case "Oe": return "==";
                case "On": return "<>";
                case "Oeg": return ">=";
                case "Oel": return "<=";
                case "Og": return "==";
                case "Ol": return "==";
                case "Os": return "=";
                case "O+": return "+";
                case "O-": return "-";
                case "O*": return "*";
                case "O:": return "/";
                default: return "";
            }
        }
        internal bool ResolveGate(StatsClass data, string id, Dictionary<int, int> _gates = null)
        {
            MaxChoices = null;
            bool nxt = false;
            string dbginfo = "";
            int node = Convert.ToInt32(data.Title);
            string ityp = data.GetStrStat("GT");
            // Key Existence
            if (ityp == "G0")
            {
                var klic = data.GetStrStat("Var");
                var exi = data.GetBoolStat("Ex");
                nxt = HasKey(klic) == exi;

                // Debug
                dbginfo = $"[{(exi ? "má" : "nemá")} {klic}]";
            }
            // Counters
            else if (ityp == "G1")
            {
                string op = data.GetStrStat("O");
                // Value 1 - Counter
                var klic1 = data.GetStrStat("Cnt");
                decimal val1 = GetCounter(klic1);
                // Value 2 - Counter or constant value
                var klic2 = data.GetStrStat("Cnt2");
                decimal val2 = data.GetBoolStat("T2") ? data.GetDecStat("Val") : GetCounter(klic2);
                // Equal
                switch (op)
                {
                    case "Oe": nxt = val1 == val2; break;
                    // Not equal
                    case "On": nxt = val1 != val2; break;
                    // Not equal or greater than
                    case "Oeg": nxt = val1 >= val2; break;
                    // Not equal or less than
                    case "Oel": nxt = val1 <= val2; break;
                    // Greater than
                    case "Og": nxt = val1 > val2; break;
                    // Less than
                    case "Ol": nxt = val1 < val2; break;
                }
                // Debug
                dbginfo = $"[{klic1}({val1}) {CodeToSign(op)} {klic2}({val2})]";
            }
            // Only once
            else if (ityp == "G4")
            {
                var gt = $"{id}-{node}";
                nxt = !GetGate(gt);
                if (nxt) AddGate(gt);

                // Debug
                dbginfo = $"[pouze jednou]";
            }
            // Random
            else if (ityp == "G2")
            {
                var rndval = data.GetNumStat("Rng");
                float dice = UnityEngine.Random.value;
                nxt = dice < rndval * 0.01f;
                if (!nxt) ++Fails[id];

                // Debug
                dbginfo = $"[náhoda {rndval:0.00}% ({dice:0.00})]";
            }
            // Watcher
            else if (ityp == "G3" && _gates != null)
            {
                bool fails = data.GetBoolStat("PF");
                if (!fails)
                {
                    if (_gates.ContainsKey(node)) _gates[node]++; else _gates.Add(node, 1);
                }
                bool rest = data.GetBoolStat("FR");
                int p = data.GetIntStat("Con");
                int g = fails ? Fails[id] : _gates[node];
                nxt = g >= p;
                if (rest) Fails[id] = 0;

                // Debug
                dbginfo = $"[brána {g} >= {p} {(fails ? "(fails)" : "")}]";
            }
            // Random connection filter
            else if (ityp == "G5")
            {
                int p = data.GetIntStat("Con", 1);
                MaxChoices = p;
                nxt = true;

                // Debug
                dbginfo = $"[náhodný filtr {p}]";
            }
            // Debug
            if (_rm.DebugInfo) data.AddStat("$Dbg", dbginfo);
            return nxt;
        }
        internal void ResolveFunction(StatsClass data)
        {
            string dbginfo = "";
            string ityp = data.GetStrStat("GT");
            // Key Existence
            if (ityp == "G0")
            {
                var klic = data.GetStrStat("Var");
                var txt = data.GetStrStat("Txt");
                _rm.AddText(klic, txt);

                // Debug
                dbginfo = $"[{klic} = {txt}]";
            }
            // Counters
            else if (ityp == "G1")
            {
                string op = data.GetStrStat("O");
                // Value 1 - Counter
                string cntr = data.GetStrStat("Cnt");
                decimal val1 = GetCounter(cntr);
                // Value 2 - Counter or constant value
                var cntr2 = data.GetStrStat("Cnt2");
                decimal val2 = data.GetBoolStat("T2") ? data.GetDecStat("Val") : GetCounter(cntr2);
                // Set
                if (op == "Os") AddCounter(cntr, val2);
                // Add
                else if (op == "O+") AddCounter(cntr, val1 + val2);
                // Subtract
                else if (op == "O-") AddCounter(cntr, val1 - val2);
                // Multiply
                else if (op == "O*") AddCounter(cntr, val1 * val2);
                // Divide
                else if (op == "O:") AddCounter(cntr, val1 / val2);

                // Debug
                dbginfo = $"[{cntr}({val1}) {CodeToSign(op)} {cntr2}({val2})]";
            }
            // Debug
            if (_rm.DebugInfo) data.AddStat("$Dbg", dbginfo);
        }
    }
    // Class to resolve specific data diagrams inside 
    internal class ResolveSlave
    {
        private enum Behaviors { Exclusive, Terminator, Stoper, Condition, Silent, Master, Action, B7, Gate, Function }
        internal enum SlaveProgress { Ready, Working, Waiting }

        private ResolveData _rd;
        private StatsClass _header;
        private List<StatsClass> _elems = new List<StatsClass>();
        private Dictionary<int, List<int>> _conns = new Dictionary<int, List<int>>();
        private List<int> _trueConds = new List<int>();
        private List<StatsClass> _todo = new List<StatsClass>();
        private Dictionary<int, int> _gates = new Dictionary<int, int>();
        internal StatsClass GetHeader { get { return _header; } }
        internal bool NoInternalResolve { get; set; }
        internal bool DoNotAddTrueConditions { get; set; }

        // Data key name
        internal string Keyname { get { return _header == null ? "" : _header.Title; } }
        // Status
        internal SlaveProgress State { get; set; }

        // Init
        internal ResolveSlave(ResolveData rd, StatsClass data)
        {
            State = SlaveProgress.Ready;
            _rd = rd;
            _header = data;
            StatsClass edata = _header.GetData("$E");
            foreach (var s in edata.GetKeys()) _elems.Add(edata.GetData(s));
            _header.RemoveStat("$E");
            edata = _header.GetData("$C");
            foreach (var s in edata.GetKeys())
            {
                StatsClass cdata = edata.GetData(s);
                int grp = cdata.GetIntStat("$S", -1);
                if (!_conns.ContainsKey(grp)) _conns.Add(grp, new List<int>());
                _conns[grp].Add(cdata.GetIntStat("$E", -1));
            }
            // Reorder connection ends
            foreach (var g in new List<int>(_conns.Keys)) _conns[g] = _conns[g].OrderBy(x => x).ToList();
            _header.RemoveStat("$C");
            // Log
            if (Application.isEditor && _rd.DetailLogs) Debug.Log(string.Format("SLAVE [{0}]: Initialized {1} elements and {2} connections!", _header.Title, _elems.Count, _conns.Keys.Select(x => _conns[x].Count).Sum()));
        }
        // Is element added in TRUE conditions?
        internal bool IsTrue(int inx)
        {
            return _trueConds.Contains(inx);
        }
        // Add condition elemnt index that is TRUE or not
        internal void AddTrueCondition(int inx, bool how)
        {
            if (how && !_trueConds.Contains(inx)) _trueConds.Add(inx);
            else if (!how) _trueConds.Remove(inx);
        }
        // Add condition elemnt indexes that are TRUE
        internal void AddTrueConditions(List<int> list)
        {
            _trueConds = _trueConds.Union(list).ToList();
        }
        // Clear list of conditions that are TRUE
        internal void ResetTrueConditions()
        {
            _trueConds = new List<int>();
        }
        // Start resolve from element index
        internal void StartResolve(int start = -1)
        {
            // Init starting elements
            int smstr = -1;
            if (start == -1)
            {
                _rd.Fails.Remove(_header.Title);
                _rd.Fails.Add(_header.Title, 0);
            }
            if (start != -1 && Is(_elems[start].GetIntStat("$T"), Behaviors.Master)) smstr = start;
            if (_conns.ContainsKey(start)) _todo = _conns[start].Select(x => GetElem(start, start, smstr, _elems[x])).ToList();
        }
        // Resolve diagram step or all steps
        internal Dictionary<string, List<StatsClass>> Resolve(bool all = true)
        {
            // Init
            Dictionary<string, List<StatsClass>> ret = new Dictionary<string, List<StatsClass>>();
            List<StatsClass> rets = new List<StatsClass>();
            List<StatsClass> failed = new List<StatsClass>();
            List<StatsClass> exclud = new List<StatsClass>();
            // Iterate all elements in order
            bool allstep = true;
            while (allstep && _todo.Count > 0)
            {
                if (!all) allstep = false;
                // Remove from planned elements
                StatsClass data = _todo[0];
                _todo.RemoveAt(0);
                // Init parameters
                int node = Convert.ToInt32(data.Title);
                int myparent = data.GetIntStat("$P", -1);
                int mymaster = data.GetIntStat("$M", -1);
                int newparent = node;
                int mstr = mymaster;
                int typ = data.GetIntStat("$T");
                // Check existence in output
                bool exis = !_rd.AllowLoops && rets.Contains(data);
                bool add = !exis;
                bool nxt = !exis;
                bool frc = false;
                // Exclusive - Do not resolve other elements with the same parent and return them as excluded
                if (Is(typ, Behaviors.Exclusive))
                {
                    foreach (var dat in new List<StatsClass>(_todo.Where(x => x.GetIntStat("$P") == myparent).ToList()))
                    {
                        exclud.Add(dat);
                        _todo.Remove(dat);
                    }
                }
                // Terminator - Do not resolve any other elements and return them as excluded
                if (Is(typ, Behaviors.Terminator))
                {
                    foreach (var dat in _todo) exclud.Add(dat);
                    _todo = new List<StatsClass>();
                }
                // Stoper - Do not resolve child elements
                if (Is(typ, Behaviors.Stoper)) nxt = false;
                // Condition - Ask for permission to resolve child elements (add it to failed if fails)
                if (Is(typ, Behaviors.Condition))
                {
                    var truecond = _trueConds.Contains(node);
                    nxt = truecond || _rd.GetCondition(data, _header.Title);
                    if (!nxt) failed.Add(data);
                    add = nxt && (!DoNotAddTrueConditions || !truecond);
                }
                // Action - Raise action event and may act as a stoper
                if (Is(typ, Behaviors.Action))
                {
                    nxt = _rd.GetAction(data, _header.Title);
                    add = true;
                }
                // Internal gate condition (add it to failed if fails)
                else if (Is(typ, Behaviors.Gate))
                {
                    // Is true condition
                    if (NoInternalResolve || _trueConds.Contains(node)) nxt = true;
                    else nxt = _rd.ResolveGate(data, _header.Title, _gates);
                    if (!nxt) failed.Add(data);
                    add = nxt;
                }
                // Internal function
                else if (Is(typ, Behaviors.Function))
                {
                    if (!NoInternalResolve) _rd.ResolveFunction(data);
                }
                // Silent - Does not count as parent to child elements and forces this branch to continue first
                if (Is(typ, Behaviors.Silent))
                {
                    frc = true;
                    newparent = myparent;
                }
                // Master - Give reference to self to child elements
                if (Is(typ, Behaviors.Master)) mstr = node;
                // Add element to result and remove it from excluded
                if (add)
                {
                    exclud.Remove(data);
                    rets.Add(data);
                }
                // Add children for resolving
                if (nxt && _conns.ContainsKey(node))
                {
                    List<StatsClass> newtodo = _conns[node].Select(x => GetElem(node, newparent, mstr, _elems[x])).ToList();
                    if (ResolveData.MaxChoices.HasValue)
                    {
                        while (newtodo.Count > ResolveData.MaxChoices.Value)
                        {
                            int inx = UnityEngine.Random.Range(0, newtodo.Count);
                            exclud.Add(newtodo[inx]);
                            newtodo.RemoveAt(inx);
                        }
                        ResolveData.MaxChoices = null;
                    }
                    if (frc) _todo.InsertRange(0, newtodo); else _todo.AddRange(newtodo);
                }
            }
            // Return output
            failed = failed.Except(rets).ToList();
            exclud = exclud.Except(rets).ToList();
            ret.Add("Result", rets);
            ret.Add("Fails", failed);
            ret.Add("Excluded", exclud);
            // Change state
            State = (_todo.Count > 0) ? SlaveProgress.Working : SlaveProgress.Waiting; 
            return ret;
        }
        // Add parent and master to data
        private StatsClass GetElem(int node, int prnt, int mstr, StatsClass data)
        {
            if (mstr > -1) data.AddStat("$M", mstr);
            data.AddStat("$C", node);
            data.AddStat("$P", prnt);
            return data;
        }
        // Behavior query to data
        private bool Is(int node, Behaviors what)
        {
            return _rd.Is(node, (int)what);
        }
        // External query for stoper elements
        internal bool IsStoper(int node)
        {
            return _rd.Is(node, (int)Behaviors.Stoper);
        }
        // External query for stoper elements
        internal bool IsCondition(int node)
        {
            return _rd.Is(node, (int)Behaviors.Condition) || _rd.Is(node, (int)Behaviors.Gate);
        }
        // Get all nodes with condition tag
        internal Dictionary<string, bool> GetConditionNodes()
        {
            return _elems.Where(x => IsCondition(x.GetIntStat("$T"))).Select(x => x.Title).ToDictionary(x => x, x => _trueConds.Contains(Convert.ToInt32(x)));
        }
        // Reset machine
        internal void Reset()
        {
            State = SlaveProgress.Ready;
            _todo = new List<StatsClass>();
            _gates = new Dictionary<int, int>();
        }
        // Clear all data
        internal void Clear()
        {
            _header = null;
            _elems = new List<StatsClass>();
            _conns = new Dictionary<int, List<int>>();
            _trueConds = new List<int>();
        }
        // Get all eccessible elements from raw editor data
        public static List<string> GetAccessibleFromRawData(StatsClass data)
        {
            // Init
            List<string> rets = new List<string>();
            Dictionary<string, List<string>> conns = new Dictionary<string, List<string>>();
            // Init conections
            StatsClass edata = data.GetData("$Conns");
            foreach (var s in edata.GetKeys())
            {
                StatsClass cdata = edata.GetData(s);
                string grp = cdata.GetStrStat("$S");
                if (!conns.ContainsKey(grp)) conns.Add(grp, new List<string>());
                conns[grp].Add(cdata.GetStrStat("$E"));
            }
            // Return all accessible element ids
            List<string> todo = conns.ContainsKey("") ? conns[""] : new List<string>();
            while (todo.Count > 0)
            {
                string node = todo[0];
                todo.RemoveAt(0);
                if (!rets.Contains(node))
                {
                    rets.Add(node);
                    if (conns.ContainsKey(node)) todo.AddRange(conns[node]);
                }
            }
            return rets;
        }
    }
}
