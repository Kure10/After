using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ResolveMachine
{   
    // Main resolver class
    internal class ResolveMaster
    {
        #pragma warning disable 0649
        public Func<string, StatsClass, bool> ResolveCondition;
        public Func<string, StatsClass, bool> ResolveAction;
        #pragma warning restore 0649

        public Dictionary<string, decimal> Counters = new Dictionary<string, decimal>();
        public Dictionary<string, string> Texts = new Dictionary<string, string>();
        public List<string> _gates = new List<string>();
        private Dictionary<string, ResolveData> _datas = new Dictionary<string, ResolveData>();

        // Add new editor data to resolve
        internal void AddDataNode(string dataname, Dictionary<string, StatsClass> data)
        {
            ResolveData rd = new ResolveData(this, dataname, data);
            _datas.Add(dataname, rd);
            // Log
            if (Application.isEditor) Debug.Log(string.Format("MASTER [{0}]: Initialized {1} nodes with {2} behaviors!", dataname, rd.DataCount, rd.BehaviorCount));
        }
        internal void ModifyDataNode(string dataname, Dictionary<string, StatsClass> data)
        {
            if (_datas.ContainsKey(dataname))
            {
                _datas[dataname].ModifyData(data);
                // Log
                if (Application.isEditor) Debug.Log(string.Format("MASTER [{0}]: Modified {1} nodes!", dataname, data.Count));
            }
        }
        internal List<StatsClass> GetDataKeys(string dataname)
        {
            return (_datas.ContainsKey(dataname)) ? new List<StatsClass>(_datas[dataname].GetDataKeys().Values) : new List<StatsClass>();
        }
        // Add new data resolver to data node
        internal ResolveSlave AddDataSlave(string dataname, string keyname)
        {
            return (_datas.ContainsKey(dataname)) ? _datas[dataname].InitSlave(keyname) : null;
        }
        internal bool GetCondition(StatsClass data, string title)
        {
            bool? ret = ResolveCondition?.Invoke(title, data);
            return ret.HasValue && ret.Value;
        }
        // Export state data
        internal StatsClass ExportState()
        {
            StatsClass data = new StatsClass("RM");
            // Counters
            StatsClass cnts = new StatsClass("C");
            foreach (var s in Counters.Keys) cnts.AddStat(s, Counters[s]);
            data.AddStat(cnts.Title, cnts);
            // Texts
            StatsClass txts = new StatsClass("T");
            foreach (var s in Texts.Keys) cnts.AddStat(s, Texts[s]);
            data.AddStat(txts.Title, txts);
            // Gates
            StatsClass gts = new StatsClass("G");
            data.AddStat(gts.Title, gts);
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
    }
    // Class for resolving one data file
    internal class ResolveData
    {
        private ResolveMaster _rm;
        private string _title;
        private Dictionary<int, bool[]> _behav = new Dictionary<int, bool[]>();
        private Dictionary<string, StatsClass> _fulldata = new Dictionary<string, StatsClass>();
        private List<ResolveSlave> _slaves = new List<ResolveSlave>();

        internal int DataCount { get { return _fulldata.Count; } }
        internal int BehaviorCount { get { return _behav.Count; } }

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
                ResolveSlave newslave = new ResolveSlave(this, _fulldata[keyname]);
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
        internal bool GetCondition(StatsClass data)
        {
            bool? ret = _rm.ResolveCondition?.Invoke(_title, data);
            return ret.HasValue && ret.Value;
        }
        // Resolve action on element
        internal bool GetAction(StatsClass data)
        {
            bool? ret = _rm.ResolveAction?.Invoke(_title, data);
            return ret.HasValue && ret.Value;
        }
        // Gate handling
        internal void AddGate(int node)
        {
            _rm.AddGate($"{_title}{node}");
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
            _rm.Counters.Remove(key);
            _rm.Counters.Add(key, val);
        }
        internal decimal GetCounter(string key)
        {
            return _rm.Counters.ContainsKey(key) ? _rm.Counters[key] : 0m; 
        }
        // Texts
        internal void AddText(string key, string val)
        {
            _rm.Texts.Remove(key);
            _rm.Texts.Add(key, val);
        }
        internal string GetText(string key)
        {
            return _rm.Texts.ContainsKey(key) ? _rm.Texts[key] : "";
        }
        // Gate handling
        internal bool GetGate(int node)
        {
            return _rm.GetGate($"{_title}{node}");
        }
        // Clear all data
        internal void Clear()
        {
            foreach (var s in _slaves) s.Clear();
            _slaves = new List<ResolveSlave>();
            _fulldata = new Dictionary<string, StatsClass>();
            _behav = new Dictionary<int, bool[]>();
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
            if (Application.isEditor) Debug.Log(string.Format("SLAVE [{0}]: Initialized {1} elements and {2} connections!", _header.Title, _elems.Count, _conns.Keys.Select(x => _conns[x].Count).Sum()));
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
                bool exis = rets.Contains(data);
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
                    nxt = _trueConds.Contains(node) || _rd.GetCondition(data);
                    if (!nxt) failed.Add(data);
                    add = nxt;
                }
                // Action - Raise action event and may act as a stoper
                if (Is(typ, Behaviors.Action))
                {
                    nxt = !_rd.GetAction(data);
                    add = true;
                }
                // Internal gate condition (add it to failed if fails)
                else if (Is(typ, Behaviors.Gate))
                {
                    bool isfail = true;
                    string ityp = data.GetStrStat("GT");

                    // Is true condition
                    if (_trueConds.Contains(node)) nxt = true;
                    // Key Existence
                    else if (ityp == "G0")
                    {
                        nxt = _rd.HasKey(data.GetStrStat("Var")) == data.GetBoolStat("Ex");
                    }
                    // Counters
                    else if (ityp == "G1")
                    {
                        string op = data.GetStrStat("O");
                        // Value 1 - Counter
                        decimal val1 = _rd.GetCounter(data.GetStrStat("Cnt"));
                        // Value 2 - Counter or constant value
                        decimal val2 = data.GetBoolStat("T2") ? data.GetDecStat("Val") : _rd.GetCounter(data.GetStrStat("Cnt2"));
                        // Equal
                        if (op == "Oe") nxt = val1 == val2;
                        // Not equal
                        else if (op == "On") nxt = val1 != val2;
                        // Not equal or greater than
                        else if (op == "Oeg") nxt = val1 >= val2;
                        // Not equal or less than
                        else if (op == "Oel") nxt = val1 <= val2;
                        // Greater than
                        else if (op == "Og") nxt = val1 > val2;
                        // Less than
                        else if (op == "Ol") nxt = val1 < val2;
                    }
                    // Only once
                    else if (ityp == "G4")
                    {
                        nxt = !_rd.GetGate(node);
                        if (nxt) _rd.AddGate(node);
                    }
                    // Random
                    else if (ityp == "G2")
                    {
                        float dice = UnityEngine.Random.value;
                        nxt = dice < data.GetNumStat("Rng") * 0.01f;
                    }
                    // Watcher
                    else if (ityp == "G3")
                    {
                        if (_gates.ContainsKey(node)) _gates[node]++; else _gates.Add(node, 1);
                        int p = data.GetIntStat("Con");
                        int g = _gates[node];
                        nxt = g == p;
                        if (g > p) isfail = false;
                    }
                    if (!nxt && isfail) failed.Add(data);
                    add = nxt;
                }
                // Internal function
                else if (Is(typ, Behaviors.Function))
                {
                    string ityp = data.GetStrStat("GT");
                    // Key Existence
                    if (ityp == "G0")
                    {
                        _rd.AddText(data.GetStrStat("Var"), data.GetStrStat("Txt"));
                    }
                    // Counters
                    else if (ityp == "G1")
                    {
                        string op = data.GetStrStat("O");
                        // Value 1 - Counter
                        string cntr = data.GetStrStat("Cnt");
                        decimal val1 = _rd.GetCounter(cntr);
                        // Value 2 - Counter or constant value
                        decimal val2 = data.GetBoolStat("T2") ? data.GetDecStat("Val") : _rd.GetCounter(data.GetStrStat("Cnt2"));
                        // Set
                        if (op == "Os") _rd.AddCounter(cntr, val2);
                        // Add
                        else if (op == "O+") _rd.AddCounter(cntr, val1 + val2);
                        // Subtract
                        else if (op == "O-") _rd.AddCounter(cntr, val1 - val2);
                        // Multiply
                        else if (op == "O*") _rd.AddCounter(cntr, val1 * val2);
                        // Divide
                        else if (op == "O:") _rd.AddCounter(cntr, val1 / val2);
                    }
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
            List<string> todo = conns[""];
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
