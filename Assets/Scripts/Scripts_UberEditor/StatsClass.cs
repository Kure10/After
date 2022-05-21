using UnityEngine;
using System;
using System.Xml.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

// Special dynamic effect data class
public class EffectClass
{
    // Lists
    internal string Title { get; set; }
    private List<string> effs;
    private List<string> conds;

    // ***********************************************************************************************************************************************

    // Constructors
    public EffectClass()
    {
        Title = "";
        effs = new List<string>();
        conds = new List<string>();
    }
    public EffectClass(string title, params object[] pars)
    {
        Title = title;
        effs = (from li in pars select li.ToString()).ToList();
        conds = new List<string>();
    }
    public EffectClass(string title, List<string> listeff, List<string> listcond)
    {
        Title = title;
        effs = listeff;
        conds = listcond;
    }
    public EffectClass(XElement xml)
    {
        Title = "";
        effs = new List<string>();
        conds = new List<string>();
        try
        {
            foreach (XAttribute atr in xml.Attributes())
            {
                // Name
                if (atr.Name == "n") Title = atr.Value;
                // Effect
                else if (atr.Name.LocalName[0] == 'e') effs.Add(atr.Value);
                // Condition
                else if (atr.Name.LocalName[0] == 'c') conds.Add(atr.Value);
            }
        }
        catch
        {
            if (Application.isEditor) Debug.LogError("Cannot parse effect class from XML!");
        }
    }

    // ***********************************************************************************************************************************************

    // Debug to log
    public string ToLog()
    {
        return string.Format("{0}-{1}-{2}", Title, string.Join("|", effs.ToArray()), string.Join("|", conds.ToArray()));
    }
    // Create xml element
    public XElement ToXml()
    {
        XElement elm = new XElement("Eff");
        for (int i = conds.Count - 1; i >= 0; i--) elm.Add(new XAttribute(string.Format("c{0}", i), conds[i]));
        for (int i = effs.Count - 1; i >= 0; i--) elm.Add(new XAttribute(string.Format("e{0}", i), effs[i]));
        elm.Add(new XAttribute("n", Title));
        return elm;
    }

    // Add condition
    public void AddCond(string cnd)
    {
        if (!conds.Contains(cnd)) conds.Add(cnd);
    }
    // Remove condition
    public void RemoveCond(string cnd)
    {
        if (conds.Contains(cnd)) conds.Remove(cnd);
    }
    // Remove all conditions
    public void ClearConds()
    {
        conds = new List<string>();
    }
    // Has condition?
    public bool HasCond(string cnd)
    {
        return (conds.Contains(cnd));
    }

    // Add effect
    public void AddEff(string eff)
    {
        if (!effs.Contains(eff)) effs.Add(eff);
    }
    // Remove effect
    public void RemoveEff(string eff)
    {
        if (effs.Contains(eff)) effs.Remove(eff);
    }
    // Get string effect X
    public string GetStr(int inx)
    {
        return (effs.Count > inx) ? effs[inx] : "";
    }
    // Get bool effect X
    public bool GetBool(int inx)
    {
        return (effs.Count > inx) ? (effs[inx] == "1") : false;
    }
    // Get int effect X
    public int GetInt(int inx)
    {
        return (effs.Count > inx) ? Convert.ToInt32(effs[inx], CultureInfo.InvariantCulture) : 0;
    }
    // Get decimal effect X
    public decimal GetDec(int inx)
    {
        return (effs.Count > inx) ? Convert.ToDecimal(effs[inx], CultureInfo.InvariantCulture) : 0m;
    }
    // Get float effect X
    public float GetNum(int inx)
    {
        return (effs.Count > inx) ? Convert.ToSingle(effs[inx], CultureInfo.InvariantCulture) : 0f;
    }
    // Get effs
    public List<string> GetEffs()
    {
        return effs;
    }
}

// Main data class
public class StatsClass
{
    // Lists
    public string Title { get; set; }
    private Dictionary<string, object> stats;

    // ***********************************************************************************************************************************************

    // Constructors
    public StatsClass()
    {
        Title = "";
        stats = new Dictionary<string, object>();
    }
    public StatsClass(StatsClass old)
    {
        Title = old.Title;
        stats = new Dictionary<string, object>();
        foreach (string hlp in old.GetKeys())
        {
            object stat = old.GetStat(hlp);
            if (stat is StatsClass) AddStat(hlp, new StatsClass((StatsClass)stat));
            else AddStat(hlp, stat);
        }
    }
    public StatsClass(string title)
    {
        Title = title;
        stats = new Dictionary<string, object>();
    }
    public StatsClass(XElement xml, string grp)
    {
        // Title
        XAttribute tit = xml.Attribute("n");
        Title = string.Format("{0}{1}", grp, (tit != null) ? tit.Value : "");

        // Simple keys
        stats = new Dictionary<string, object>();
        foreach (XElement el in xml.Elements("Key"))
        {
            XAttribute jmn = el.Attribute("n");
            XAttribute typ = el.Attribute("t");
            XAttribute val = el.Attribute("v");
            if (jmn != null && typ != null && val != null) stats.Add(jmn.Value, XmlToVal(val.Value, typ.Value));
            else if (Application.isEditor) Debug.LogError("Cannot add data line!");
        }
        // Dynamic effects
        int i = 0;
        foreach (XElement el in xml.Elements("Eff"))
        {
            stats.Add(string.Format("Eff{0}", i), new EffectClass(el));
            ++i;
        }
        // Nested data
        foreach (XElement el in xml.Elements("Data"))
        {
            StatsClass data = new StatsClass(el, "");
            stats.Add(data.Title, data);
        }
    }

    // ***********************************************************************************************************************************************

    // Debug data to log
    public string ToLog(bool header = true)
    {
        string outdata = header ? Title : "";
        foreach (string key in GetKeys())
        {
            string odd = ", ";
            object obj = GetStat(key);
            string val = obj?.ToString() ?? "";
            if (obj is EffectClass) val = ((EffectClass)obj).ToLog();
            else if (obj is StatsClass)
            {
                val = ((StatsClass)obj).ToLog(false);
                odd = Environment.NewLine;
            }
            outdata = string.Format("{0}{3}{1}:{2}", outdata, key, val, outdata.Length > 0 ? odd : "");
        }
        return outdata;
    }
    // Type to string
    private object ValToXml(object obj)
    {
        // Parse color
        if (obj is Color)
        {
            Color32 clr = (Color)obj;
            obj = string.Format(CultureInfo.InvariantCulture, "{0:000},{1:000},{2:000},{3:000}", clr.r, clr.g, clr.b, clr.a);
        }
        // Parse rectangle
        else if (obj is Rect)
        {
            Rect rct = (Rect)obj;
            obj = string.Format(CultureInfo.InvariantCulture, "{0:0.###},{1:0.###},{2:0.###},{3:0.###}", rct.x, rct.y, rct.width, rct.height);
        }
        // Parse quaternion
        else if (obj is Quaternion)
        {
            Quaternion qua = (Quaternion)obj;
            obj = string.Format(CultureInfo.InvariantCulture, "{0:0.###},{1:0.###},{2:0.###},{3:0.###}", qua.x, qua.y, qua.z, qua.w);
        }
        // Parse vector3
        else if (obj is Vector3)
        {
            Vector3 vct = (Vector3)obj;
            obj = string.Format(CultureInfo.InvariantCulture, "{0:0.###},{1:0.###},{2:0.###}", vct.x, vct.y, vct.z);
        }
        // Parse vector2
        else if (obj is Vector2)
        {
            Vector2 vct = (Vector2)obj;
            obj = string.Format(CultureInfo.InvariantCulture, "{0:0.###},{1:0.###}", vct.x, vct.y);
        }
        // Correct float
        else if (obj is float)
        {
            obj = string.Format(CultureInfo.InvariantCulture, "{0:0.#####}", obj);
        }
        return obj;
    }
    // Type to string
    public static object XmlToVal(object val, string typ)
    {
        try
        {
            // String
            if (typ == "String") val = Convert.ToString(val);
            // Integer
            else if (typ == "Int32") val = Convert.ToInt32(val, CultureInfo.InvariantCulture);
            // Decimal
            else if (typ == "Decimal") val = Convert.ToDecimal(val, CultureInfo.InvariantCulture);
            // Float
            else if (typ == "Single") val = Convert.ToSingle(val, CultureInfo.InvariantCulture);
            // Parsing
            else
            {
                string[] prts = val.ToString().Split(',');
                // Color
                if (typ == "Color") val = (Color)new Color32(Convert.ToByte(prts[0]), Convert.ToByte(prts[1], CultureInfo.InvariantCulture), Convert.ToByte(prts[2], CultureInfo.InvariantCulture), Convert.ToByte(prts[3], CultureInfo.InvariantCulture));
                // Vector2
                else if (typ == "Vector2") val = new Vector2(Convert.ToSingle(prts[0], CultureInfo.InvariantCulture), Convert.ToSingle(prts[1], CultureInfo.InvariantCulture));
                // Vector3
                else if (typ == "Vector3") val = new Vector3(Convert.ToSingle(prts[0], CultureInfo.InvariantCulture), Convert.ToSingle(prts[1], CultureInfo.InvariantCulture), Convert.ToSingle(prts[2], CultureInfo.InvariantCulture));
                // Quaternion
                else if (typ == "Quaternion") val = new Quaternion(Convert.ToSingle(prts[0], CultureInfo.InvariantCulture), Convert.ToSingle(prts[1], CultureInfo.InvariantCulture), Convert.ToSingle(prts[2], CultureInfo.InvariantCulture), Convert.ToSingle(prts[3], CultureInfo.InvariantCulture));
                // Rectangle
                else if (typ == "Rect") val = new Rect(Convert.ToSingle(prts[0], CultureInfo.InvariantCulture), Convert.ToSingle(prts[1], CultureInfo.InvariantCulture), Convert.ToSingle(prts[2], CultureInfo.InvariantCulture), Convert.ToSingle(prts[3], CultureInfo.InvariantCulture));
            }
        }
        catch (Exception e)
        {
            if (Application.isEditor) Debug.LogError(string.Format("{0}={1}\r\n{2}", typ, val, e.Message));
        }

        return val;
    }
    // Create xml element
    public XElement ToXml(string root)
    {
        XElement elm = new XElement("Data", new XAttribute("n", (root.Length > 0 && Title.IndexOf(root) == 0) ? Title.Substring(root.Length) : Title));
        foreach (string key in GetKeys())
        {
            object obj = GetStat(key);
            // Nested data
            if (obj is StatsClass)
            {
                StatsClass stat = (StatsClass)obj;
                stat.Title = key;
                elm.Add(stat.ToXml(""));
            }
            // Dynamic effects
            else if (obj is EffectClass) elm.Add(((EffectClass)obj).ToXml());
            // Keys
            else
            {
                XElement kel = new XElement("Key");
                string typ = obj.GetType().ToString().Replace("System.", "").Replace("UnityEngine.", "");
                kel.Add(new XAttribute("v", ValToXml(obj)), new XAttribute("t", typ), new XAttribute("n", key));
                elm.Add(kel);
            }
        }
        return elm;
    }

    // Adds/Replaces new stat under specified key
    public StatsClass AddStat(string key, object val)
    {
        if (stats.ContainsKey(key)) stats[key] = val; else stats.Add(key, val);
        return this;
    }
    // Removes new stat under specified key
    public void RemoveStat(string key)
    {
        if (stats.ContainsKey(key)) stats.Remove(key);
    }
    // Merge range of stats with the same name
    public void MergeStats(StatsClass newstats)
    {
        foreach (string hlp in newstats.GetKeys())
        {
            if (newstats.GetStat(hlp) is StatsClass)
            {
                StatsClass data = GetDataRead(hlp);
                data.MergeStats(newstats.GetData(hlp));
                AddStat(hlp, data);
            }
            else AddStat(hlp, newstats.GetStat(hlp));
        }
    }
    // Adds range of stats
    public void AddStats(StatsClass newstats)
    {
        foreach (string hlp in newstats.GetKeys()) AddStat(hlp, newstats.GetStat(hlp));
    }
    // Gets all stat keys
    public List<string> GetKeys()
    {
        return new List<string>(stats.Keys);
    }
    public IEnumerable<KeyValuePair<string, object>> GetPairs()
    {
        return stats;
    }
    // Gets all stat keys
    public List<StatsClass> UnpackKeys()
    {
        List<StatsClass> rets = new List<StatsClass>();
        foreach (string s in stats.Keys) if (stats[s] is StatsClass) rets.Add((StatsClass)stats[s]);
        return rets;
    }
    // Gets all stat keys that contain string
    public List<string> GetKeysSub(string name)
    {
        return (from s in stats.Keys where s.Contains(name) select s).ToList();
    }
    // Checks for empty data
    public bool IsEmpty()
    {
        return (stats.Count == 0);
    }
    // Has stat?
    public bool HasStat(string key)
    {
        return (stats.ContainsKey(key));
    }
    // Gets generic stat from key
    public object GetStat(string key)
    {
        if (stats.ContainsKey(key)) return stats[key]; else return null;
    }
    // Gets stat from key as string
    public string GetStrStat(string key)
    {
        return GetStrStat(key, "");
    }
    // Gets stat from key as string, no value return default
    public string GetStrStat(string key, string deflt)
    {
        object obj = GetStat(key);
        return (obj != null) ? obj.ToString() : deflt;
    }
    // Gets stat from key as boolean
    public bool GetBoolStat(string key)
    {
        return GetBoolStat(key, false);
    }
    // Gets stat from key as boolean
    public bool GetBoolStat(string key, bool deflt)
    {
        object obj = GetStat(key);
        if (obj != null) return obj.ToString() == "1";
        return deflt;
    }
    // Gets stat from key as nullable boolean
    public bool? GetBool3Stat(string key)
    {
        object obj = GetStat(key);
        if (obj != null) return obj.ToString() == "1";
        return null;
    }
    // Gets stat from key as integer
    public int GetIntStat(string key)
    {
        return GetIntStat(key, 0);
    }
    // Gets stat from key as integer, no value returns default
    public int GetIntStat(string key, int deflt)
    {
        object obj = GetStat(key);
        if (obj != null)
        {
            if (obj is int) return (int)obj;
            else if (obj is float) return Mathf.FloorToInt((float)obj);
            else if (obj is decimal) return (int)Math.Floor((decimal)obj);
            else if (obj is string)
            {
                int val = deflt;
                if (!int.TryParse((string)obj, out val)) val = deflt;
                return val;
            }
        }
        return deflt;
    }
    // Gets stat from key as decimal
    public decimal GetDecStat(string key)
    {
        return GetDecStat(key, 0);
    }
    // Gets stat from key as decimal, no value return default
    public decimal GetDecStat(string key, decimal deflt)
    {
        object obj = GetStat(key);
        if (obj != null)
        {
            if (obj is decimal || obj is int) return Convert.ToDecimal(obj, CultureInfo.InvariantCulture);
            else if (obj is string)
            {
                decimal val = deflt;
                if (!decimal.TryParse((string)obj, NumberStyles.AllowDecimalPoint | NumberStyles.AllowTrailingSign, CultureInfo.InvariantCulture, out val)) val = deflt;
                return val;
            }
        }
        return deflt;
    }
    // Gets stat from key as float
    public float GetNumStat(string key)
    {
        return GetNumStat(key, 0f);
    }
    // Gets stat from key as float, no value return default
    public float GetNumStat(string key, float deflt)
    {
        object obj = GetStat(key);
        if (obj != null)
        {
            if (obj is float || obj is int || obj is decimal) return Convert.ToSingle(obj, CultureInfo.InvariantCulture);
            else if (obj is string)
            {
                double val = deflt;
                if (!double.TryParse((string)obj, NumberStyles.AllowDecimalPoint | NumberStyles.AllowTrailingSign, CultureInfo.InvariantCulture, out val)) val = deflt;
                return (float)val;
            }
        }
        return deflt;
    }
    // Gets stat from key as Vector3
    public Vector3 GetVectorStat(string key)
    {
        return GetVectorStat(key, Vector3.zero);
    }
    // Gets stat from key as Vector3, no value returns default
    public Vector3 GetVectorStat(string key, Vector3 deflt)
    {
        object obj = GetStat(key);
        if (obj is string)
        {
            string[] prts = obj.ToString().Split(',');
            obj = new Vector3(Convert.ToSingle(prts[0], CultureInfo.InvariantCulture), Convert.ToSingle(prts[1], CultureInfo.InvariantCulture), Convert.ToSingle(prts[2], CultureInfo.InvariantCulture));
            AddStat(key, obj);
        }
        if (obj != null && obj is Vector3) return (Vector3)obj;
        return deflt;
    }
    // Gets stat from key as Vector2
    public Vector2 GetVect2Stat(string key)
    {
        return GetVect2Stat(key, Vector2.zero);
    }
    // Gets stat from key as Vector2, no value returns default
    public Vector2 GetVect2Stat(string key, Vector2 deflt)
    {
        object obj = GetStat(key);
        if (obj is string)
        {
            string[] prts = obj.ToString().Split(',');
            obj = new Vector2(Convert.ToSingle(prts[0], CultureInfo.InvariantCulture), Convert.ToSingle(prts[1], CultureInfo.InvariantCulture));
            AddStat(key, obj);
        }
        if (obj != null && (obj is Vector2 || obj is Vector3)) return (Vector2)obj;
        return deflt;
    }
    // Gets stat from key as Quaternion
    public Quaternion GetQuatStat(string key)
    {
        return GetQuatStat(key, Quaternion.identity);
    }
    // Gets stat from key as Quaternion, no value returns default
    public Quaternion GetQuatStat(string key, Quaternion deflt)
    {
        object obj = GetStat(key);
        if (obj is string)
        {
            string[] prts = obj.ToString().Split(',');
            obj = new Quaternion(Convert.ToSingle(prts[0], CultureInfo.InvariantCulture), Convert.ToSingle(prts[1], CultureInfo.InvariantCulture), Convert.ToSingle(prts[2], CultureInfo.InvariantCulture), Convert.ToSingle(prts[3], CultureInfo.InvariantCulture));
            AddStat(key, obj);
        }
        if (obj != null && obj is Quaternion) return (Quaternion)obj;
        return deflt;
    }
    // Gets stat from key as color
    public Color GetColorStat(string key)
    {
        object obj = GetStat(key);
        if (obj is string)
        {
            string[] prts = obj.ToString().Split(',');
            obj = (Color)new Color32(Convert.ToByte(prts[0], CultureInfo.InvariantCulture), Convert.ToByte(prts[1], CultureInfo.InvariantCulture), Convert.ToByte(prts[2], CultureInfo.InvariantCulture), Convert.ToByte(prts[3], CultureInfo.InvariantCulture));
            AddStat(key, obj);
        }
        if (obj != null && obj is Color) return (Color)obj;
        return Color.clear;
    }
    // Gets stat from key as color
    public Rect GetRectStat(string key)
    {
        object obj = GetStat(key);
        if (obj is string)
        {
            string[] prts = obj.ToString().Split(',');
            obj = new Rect(Convert.ToSingle(prts[0], CultureInfo.InvariantCulture), Convert.ToSingle(prts[1], CultureInfo.InvariantCulture), Convert.ToSingle(prts[2], CultureInfo.InvariantCulture), Convert.ToSingle(prts[3], CultureInfo.InvariantCulture));
            AddStat(key, obj);
        }
        if (obj != null && obj is Rect) return (Rect)obj;
        return new Rect(0f, 0f, 0f, 0f);
    }
    // Gets stat from key as dynamic effect
    public EffectClass GetEff(string key)
    {
        object obj = GetStat(key);
        if (obj != null && obj is EffectClass) return (EffectClass)obj;
        return new EffectClass();
    }
    // Gets list of all dynamic effects
    public List<EffectClass> GetEffList()
    {
        return (from li in GetKeysSub("Eff") select (EffectClass)GetStat(li)).ToList();
    }
    // Gets data stat from data
    public StatsClass GetData(string key)
    {
        object obj = GetStat(key);
        if (obj != null && obj is StatsClass) return (StatsClass)obj;
        return new StatsClass();
    }
    public StatsClass GetDataFirst()
    {
        if (stats.Keys.Count > 0)
        {
            object obj = stats[stats.Keys.Select(x => x).First()];
            if (obj is StatsClass) return (StatsClass)obj;
        }
        return new StatsClass();
    }
    // Gets clone of data stat from data
    public StatsClass GetDataRead(string key)
    {
        object obj = GetStat(key);
        if (obj != null && obj is StatsClass) return new StatsClass((StatsClass)obj);
        return new StatsClass();
    }
    // Get list of all dynamic effects nested under key
    public List<EffectClass> GetEffects(string key)
    {
        return GetData(key).GetEffList();
    }

    // Save binary data file
    public static void SaveBinaryFile(string filepath, string filename, List<StatsClass> datas)
    {
        // Check path
        if (!Directory.Exists(filepath)) Directory.CreateDirectory(filepath);

        // Write file
        string finalpath = string.Format("{0}/{1}.bytes", filepath, filename);
        if (File.Exists(finalpath)) File.Delete(finalpath);

        using (FileStream stream = File.Open(finalpath, FileMode.Create))
        {
            using (BinaryWriter w = new BinaryWriter(stream, Encoding.UTF8))
            {
                foreach (var data in datas) WriteBytes(w, data);
                w.Write((byte)15);
            }
            if (Application.isEditor) Debug.LogWarning(string.Format("Created file [{0}]", finalpath));
        }
    }
    // Data to binary
    private static void WriteBytes(BinaryWriter w, StatsClass data)
    {
        w.Write((byte)13);
        w.Write(data.Title);
        foreach (var s in data.GetKeys())
        {
            object obj = data.GetStat(s);
            // String
            if (obj is string)
            {
                w.Write((byte)0);
                w.Write(s);
                w.Write((string)obj);
            }
            // Integer
            else if (obj is int)
            {
                w.Write((byte)1);
                w.Write(s);
                w.Write((int)obj);
            }
            // Float
            else if (obj is float)
            {
                w.Write((byte)2);
                w.Write(s);
                w.Write((float)obj);
            }
            // Decimal
            else if (obj is decimal)
            {
                w.Write((byte)3);
                w.Write(s);
                w.Write((decimal)obj);
            }
            // Parse color
            else if (obj is Color)
            {
                Color32 clr = (Color)obj;
                w.Write((byte)4);
                w.Write(s);
                w.Write(clr.r);
                w.Write(clr.g);
                w.Write(clr.b);
                w.Write(clr.a);
            }
            // Parse rectangle
            else if (obj is Rect)
            {
                Rect rct = (Rect)obj;
                w.Write((byte)5);
                w.Write(s);
                w.Write(rct.x);
                w.Write(rct.y);
                w.Write(rct.width);
                w.Write(rct.height);
            }
            // Parse quaternion
            else if (obj is Quaternion)
            {
                Quaternion qua = (Quaternion)obj;
                w.Write((byte)6);
                w.Write(s);
                w.Write(qua.x);
                w.Write(qua.y);
                w.Write(qua.z);
                w.Write(qua.w);
            }
            // Parse vector3
            else if (obj is Vector3)
            {
                Vector3 vct = (Vector3)obj;
                w.Write((byte)7);
                w.Write(s);
                w.Write(vct.x);
                w.Write(vct.y);
                w.Write(vct.z);
            }
            // Parse vector2
            else if (obj is Vector2)
            {
                Vector2 vct = (Vector2)obj;
                w.Write((byte)8);
                w.Write(s);
                w.Write(vct.x);
                w.Write(vct.y);
            }
            // Data
            else if (obj is StatsClass)
            {
                WriteBytes(w, (StatsClass)obj);
            }
            else if (Application.isEditor) Debug.LogError(obj.GetType());
        }
        w.Write((byte)14);
    }
    // Load binary data file
    public static Dictionary<string, StatsClass> LoadBinaryFile(string filepath, string filename, bool logs = true)
    {
        float cas = Time.realtimeSinceStartup;
        Dictionary<string, StatsClass> ret = null;
        // Check path
        if (Directory.Exists(filepath))
        {
            // Read file
            string finalpath = string.Format("{0}/{1}.bytes", filepath, filename);
            if (File.Exists(finalpath))
            {
                using (FileStream stream = File.Open(finalpath, FileMode.Open))
                {
                    using (BinaryReader r = new BinaryReader(stream, Encoding.UTF8))
                    {
                        try { ret = ReadBytes(r); }
                        catch (Exception e) { if (Application.isEditor) Debug.LogError(e.Message); }
                    }
                }
            }
        }
        // Log
        if (Application.isEditor && logs) Debug.Log(string.Format("Loaded binary file [{0}] in {1:0.0000}s", filename, Time.realtimeSinceStartup - cas));
        return ret;
    }
    // Load xml data file
    public static Dictionary<string, StatsClass> LoadXmlFile(string filepath, string filename, bool logs = true)
    {
        float cas = Time.realtimeSinceStartup;
        XDocument doc = null;
        Dictionary<string, StatsClass> rets = new Dictionary<string, StatsClass>();
        try
        {
            string finalpath = string.Format("{0}/{1}.xml", filepath, filename);
            if (File.Exists(finalpath))
            {
                // Read file
                using (FileStream stream = File.Open(finalpath, FileMode.Open))
                {
                    using (TextReader reader = new StreamReader(stream)) doc = XDocument.Load(reader);
                }
                // Parse xml document
                XElement elm2 = doc.Element("DataRoot");
                XAttribute tit = elm2.Attribute("n");
                string grp = (tit != null) ? tit.Value : "";
                foreach (XElement el in elm2.Elements())
                {
                    StatsClass dat = new StatsClass(el, grp);
                    rets.Add(dat.Title, dat);
                }
            }
        }
        catch (Exception e) { if (Application.isEditor) Debug.LogError(e.Message); }
        // Log
        if (Application.isEditor && logs) Debug.Log(string.Format("Loaded XML file [{0}] in {1:0.0000}s", filename, Time.realtimeSinceStartup - cas));
        return rets;
    }
    // Load binary data file
    public static Dictionary<string, StatsClass> LoadBinaryAsset(TextAsset txt, bool logs = true)
    {
        float cas = Time.realtimeSinceStartup;

        Dictionary<string, StatsClass> ret = null;
        // Read text
        using (BinaryReader r = new BinaryReader(new MemoryStream(txt.bytes), Encoding.UTF8))
        {
            try { ret = ReadBytes(r); }
            catch (Exception e) { if (Application.isEditor) Debug.LogError(e.Message); }
        }
        // Log
        if (Application.isEditor && logs) Debug.Log(string.Format("Loaded binary asset [{0}] in {1:0.0000}s", ret != null ? txt.name : "", Time.realtimeSinceStartup - cas));
        return ret;
    }
    // Data from binary
    private static Dictionary<string, StatsClass> ReadBytes(BinaryReader r)
    {
        Dictionary<string, StatsClass> rets = new Dictionary<string, StatsClass>();
        List<StatsClass> zasob = new List<StatsClass>();
        StatsClass data = null;
        byte wht = 0;
        string titl = "";
        while (wht < 15)
        {
            wht = r.ReadByte();
            if (wht < 13) titl = r.ReadString();
            switch (wht)
            {
                // String
                case 0: data.AddStat(titl, r.ReadString()); break;
                // Integer
                case 1: data.AddStat(titl, r.ReadInt32()); break;
                // Float
                case 2: data.AddStat(titl, r.ReadSingle()); break;
                // Decimal
                case 3: data.AddStat(titl, r.ReadDecimal()); break;
                // Color
                case 4: data.AddStat(titl, new Color32(r.ReadByte(), r.ReadByte(), r.ReadByte(), r.ReadByte())); break;
                // Rectangle
                case 5: data.AddStat(titl, new Rect(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle())); break;
                // Quaternion
                case 6: data.AddStat(titl, new Quaternion(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle())); break;
                // Vector3
                case 7: data.AddStat(titl, new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle())); break;
                // Vector2
                case 8: data.AddStat(titl, new Vector2(r.ReadSingle(), r.ReadSingle())); break;
                // New data
                case 13:
                    if (data != null) zasob.Insert(0, data);
                    data = new StatsClass(r.ReadString());
                    break;
                // Close data
                case 14:
                    if (zasob.Count > 0)
                    {
                        zasob[0].AddStat(data.Title, data);
                        data = zasob[0];
                        zasob.RemoveAt(0);
                    }
                    else
                    {
                        rets.Add(data.Title, data);
                        data = null;
                    }
                    break;
                default: break;
            }
        }
        return rets;
    }
}