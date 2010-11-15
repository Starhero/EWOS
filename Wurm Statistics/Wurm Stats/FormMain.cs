﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WParse;



namespace Wurm_Stats
{
    public partial class FormMain : Form
    {
        private string Path
        {
            get;
            set;
        }

        public FormMain()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //    Cursor.Current = Cursors.WaitCursor;
            //    log.Parse();
            //    long tmp = log.GetCount("Start Repairing");
            //    treeView1.BeginUpdate();
            //    treeView1.Nodes.Clear();
            //    foreach (KeyValuePair<string, long> kv in log.GetAllCounts())
            //    {
            //        if (kv.Key == "Fasting")
            //        {
            //            TreeNode node = new TreeNode("Fasting");
            //            node.Nodes.Add(new TreeNode("You fasted " + kv.Value + "times."));
            //            treeView1.Nodes.Add(node);
            //        }
            //        else
            //        {
            //            TreeNode node = new TreeNode(kv.Key);
            //            node.Nodes.Add(new TreeNode("Count: " + kv.Value));
            //            treeView1.Nodes.Add(node);
            //        }
            //    }

            //    treeView1.EndUpdate();
            //    Cursor.Current = Cursors.Default;
            //    }
            //    else
            //    {
            //    }
            //}
        }

        private void FormMainLoad(object sender, EventArgs e)
        {
            string dir = Environment.GetEnvironmentVariable("USERPROFILE");

            if (File.Exists("config.dat"))
            {
                this.LoadConfig();
                if (Directory.Exists(@"parsers\"))
                {
                    if (Directory.GetFiles(@"Parsers\", "*.cs").Length != 0 || Directory.GetFiles(@"parsers\", "*.wsp").Length != 0)
                    {
                        //TODO: Need to find a way to get WSP files and CS files. Simple way would be and array added to the IEnumerable.
                        IEnumerable<string> parsers = Directory.GetFiles(@"Parsers\", "*.cs");

                        if (Directory.GetFiles(Path, "_Event*.txt").Length != 0)
                        {
                            IEnumerable<string> logs = Directory.GetFiles(Path, "_Event*.txt");
                            AggregateParser parser = null;
                            try
                            {
                                parser = new AggregateParser(logs, parsers);
                            }
                            catch (Exception ex)
                            {


                            }

                            parser.Parse();

                            Dictionary<string, Queue<DateTime>> entries = parser.Proxy.GetEntries();
                            foreach (KeyValuePair<string, List<string>> pair in parser.Proxy.GetLinks())
                            {
                                TreeNode Parent = new TreeNode(pair.Key);

                                foreach (string key in pair.Value)
                                {
                                    TreeNode child = new TreeNode(key);

                                    long count;
                                    if (!parser.Proxy.TryGetCount(key, out count))
                                    {
                                        continue;
                                    }

                                    Parent.Nodes.Add(String.Format("{0}:{1}", child.Text, count));

                                }
                                treeView1.Nodes.Add(Parent);
                            }

                        }

                        else
                        {
                            MessageBox.Show(this, @"ERROR: No logs are found to parse. Reconfig isn't implemented, please delete your SETTINGS.DAT file in the programs directory before relaunching.", "What the hell!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    }

                }
                else
                {
                    MessageBox.Show(this, "ERROR: I got no parsers to use! I can't run!/r/n/r/nPlease reinstall the application!", "SHEEEET", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            else
            {

                if (Directory.Exists(dir + @"\wurm\players\"))
                {
                    this.Path = dir + @"\wurm\players\";
                    MessageBox.Show("Wurm folder found!\r\n\r\nWelcome to Wurm Statistics!\r\n\r\n Since this is the first time you have ran the program, we need to know what logs you want to read from.\r\n\r\n In the next dialog, please specify your Wurm charactors event log folder.");

                    FrmSettings initset = new FrmSettings(this.Path);
                    initset.ShowDialog();

                }
                else
                {
                    this.Path = @"C:\";
                    MessageBox.Show("Wurm folder not found!\r\n\r\nWelcome to Wurm Statistics!\r\n\r\n Since this is the first time you have ran the program, we need to know what logs you want to read from.\r\n\r\n In the next dialog, please specify your Wurm charactors event log folder.");

                    FrmSettings initset = new FrmSettings(this.Path);
                    initset.ShowDialog();
                }
            }
        }

        private void LoadConfig()
        {
            TextReader load = new StreamReader("config.dat");

            this.Path = load.ReadLine();
            load.Close();
        }

        private static void BtnSaveClick(object sender, EventArgs e)
        {
            MessageBox.Show(@"Not Implemented yet sorry!");
        }

        private void BtnSettingsClick(object sender, EventArgs e)
        {
            FrmSettings set = new FrmSettings(this.Path);
            set.ShowDialog();
        }
    }
}