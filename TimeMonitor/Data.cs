﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimeMonitor
{
    class Data
    {
        public class Actions
        {
            public string DateTimeString;
            public int Type;
            public string Action;
            public string Title;
            public Icon Icon;
        }
        public static bool AddData(Actions A)
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

                    MemoryStream ms = new MemoryStream();
                    if (A.Icon != null) A.Icon.Save(ms);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();
                    cmd.Parameters.AddWithValue("@Icon", Convert.ToBase64String(arr));

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
        }

        public static List<Actions> GetData(string StartDateTime, string EndDateTime)
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
                        Actions A = new Actions();
                        DateTime D = new DateTime((long)r["TimeStamp"]);
                        A.DateTimeString = D.ToString("yyyy-MM-dd HH:mm:ss");
                        //D.Year + "-" + D.Month + "-" + D.Day + " " + D.Hour + ":" + D.Minute + ":" + D.Second;
                        A.Type = (int)r["Type"];
                        A.Action = r["Action"].ToString();
                        A.Title = r["Title"].ToString();
                        res.Add(A);
                    }
                }
            }
            return res;
        }
    }
}
