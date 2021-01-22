using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKDB_Explorer
{
    public partial class Form1 : Form
    {
        StringDatabase strDatabaseParser;
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileD = new OpenFileDialog(); //create object
            fileD.Filter = "FEAR String Database |*.Strdb00p"; //define filter
            fileD.ShowDialog(); //show dialog

            //Clear everything if we need to
            //if (strDatabaseParser != null)
            //    strDatabaseParser.

            //Parse this shit

            //Make a new object if we need to.
            if (strDatabaseParser == null)
                strDatabaseParser = new StringDatabase();

            List<string[]> strDb = new List<string[]>();

            strDb = strDatabaseParser.ReadStringDatabase(fileD.FileName);
            //MessageBox.Show("test");
            //string[][] item = strDb.ToArray();

            
            foreach(var item in strDb)
            {
                listView1.Items.Add(item[0]);
            }

            /*
            strDb = strDatabaseParser.ReadStringDatabaseData(fileD.FileName);

            int t = 0;
            foreach(ListViewItem item in listView1.Items)
            {
                item.SubItems.Add(strDb[t][0]);
                t++;
            }*/
            List<StringDatabase.Entry> strDbId = new List<StringDatabase.Entry>();
            strDbId = strDatabaseParser.ReadStringDatabaseTOC(fileD.FileName);


            List<string> stringData = new List<string>();
            List<StringDatabase.Voice> voiceData = new List<StringDatabase.Voice>();

            stringData = strDatabaseParser.ReadStrings(fileD.FileName);

            voiceData = strDatabaseParser.ReadVoices(fileD.FileName);

            
            int t = 0;
            foreach (ListViewItem item in listView1.Items)
            {
                string stringContent = "";
                string voice = "";

                bool showVoice = true;


                    if (strDbId[t].stringId == 0 && voiceData[strDbId[t].voiceId].blank == true)
                    {
                        showVoice = false;
                    }


                    stringContent = String.Format("{0}", stringData[strDbId[t].stringId]);


                    if (showVoice)
                        voice = String.Format("{0}", voiceData[strDbId[t].voiceId].voice);
                    //String.Format({ })

                    item.SubItems.Add(stringContent);
                    item.SubItems.Add(voice);
                t++;

            }
            

            //strDbId.Sort();

            //foreach(var entry in strDbId)
            // {
            //    list
            //}
            //ListViewItem tempItem = new ListViewItem
            //listView1.Items.Add()
        }
    }
}
