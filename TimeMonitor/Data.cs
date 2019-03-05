using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TimeMonitor
{
    public class Data
    {
        public class Actions
        {
            public string DateTimeString { get { return DateTime.ToString(Consts.DateTimeFormatString); } }
            public DateTime DateTime;
            public int Type;
            public string Action;
            public string Title;
            public Icon Icon;
        }
        public class ShowData
        {
            public Icon I;
            public DateTime Start, End;
            public string Action, Title;
            public SolidColorBrush Color;
            public bool IsCatchFish;
            public long Ticks
            {
                get
                {
                    return (End - Start).Ticks;
                }
                set
                {
                    End = Start.AddTicks(value);
                }
            }
            public string ActionTitle { get { return Action + "|" + Title; } }
            public ShowData() { }
            public ShowData(ShowData old)
            {
                I = old.I;
                Start = old.Start;
                End = old.End;
                Action = old.Action;
                Title = old.Title;
                //ID = old.ID;
                Color = old.Color;
                IsCatchFish = old.IsCatchFish;
            }
        }

        public static bool AddActions(Actions A)
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Actions.db;Version=3;"))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Actions (TimeStamp, Type, Action, Title, Icon) VALUES (@TS, @Type, @Action, @Title, @Icon)", con))
                {
                    cmd.Parameters.AddWithValue("@TS", DateTime.Now.Ticks);
                    if (A.Type != 0 && A.Type != 1)
                        return false;
                    cmd.Parameters.AddWithValue("@Type", A.Type);
                    cmd.Parameters.AddWithValue("@Action", A.Action);
                    cmd.Parameters.AddWithValue("@Title", A.Title);
                    cmd.Parameters.AddWithValue("@Icon", Consts.Icon2Base64String(A.Icon));

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public static List<Actions> GetActions(string StartDateTime, string EndDateTime)
        {
            DateTime SDT, EDT;
            try
            {
                SDT = Convert.ToDateTime(StartDateTime);
                EDT = Convert.ToDateTime(EndDateTime);
            }
            catch
            {
                return null;
            }
            List<Actions> res = new List<Actions>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Actions.db;Version=3;"))
            {
                con.Open();
                using (SQLiteDataAdapter ada = new SQLiteDataAdapter("SELECT * FROM Actions WHERE TimeStamp >= " + SDT.Ticks + " and TimeStamp <= " + EDT.Ticks + " ORDER BY TimeStamp", con))
                {
                    DataTable DT = new DataTable();
                    ada.Fill(DT);
                    foreach (DataRow r in DT.Rows)
                    {
                        if (res.Count > 999)
                        {
                            MessageBox.Show("条目过多，仅显示1000条。");
                            break;
                        }
                        Actions A = new Actions();
                        A.DateTime = new DateTime((long)r["TimeStamp"]);
                        //D.Year + "-" + D.Month + "-" + D.Day + " " + D.Hour + ":" + D.Minute + ":" + D.Second;
                        A.Type = (int)(long)r["Type"];
                        A.Action = r["Action"].ToString();
                        A.Title = r["Title"].ToString();
                        if (r["Icon"] != null && r["Icon"].ToString() != "")
                            A.Icon = Consts.Base64String2Icon(r["Icon"].ToString());
                        res.Add(A);
                    }
                }
            }
            return res;
        }

        public static List<string> GetFishs()
        {
            List<string> res = new List<string>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Actions.db;Version=3;"))
            {
                con.Open();
                using (SQLiteDataAdapter DA = new SQLiteDataAdapter("SELECT * FROM Relax", con))
                {
                    DataTable DT = new DataTable();
                    DA.Fill(DT);
                    foreach (DataRow DR in DT.Rows)
                        res.Add(DR["Name"] as string);
                }
            }
            return res;
        }

        public static bool AddFish(string name)
        {
            List<string> res = new List<string>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Actions.db;Version=3;"))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Relax (Name) VALUES (@Name)", con))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public static bool RemoveFish(string name)
        {
            List<string> res = new List<string>();
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Actions.db;Version=3;"))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Relax WHERE Name = @name", con))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }
    }
}
