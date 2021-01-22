using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SKDB_Explorer
{
    class StringDatabase
    {
        public string fileOpened;
        public Int32 header, version, entryCount, stringCount, voiceCount, tocOffset;
        public int lastPosition;

        public struct Entry
        {
            public Entry(int id, int vid)
            {
                stringId = id;
                voiceId = vid;
            }
            public Int32 stringId;
            public Int32 voiceId;
        }

        public struct Voice
        {
            public Voice(string voiceString, bool flag)
            {
                voice = voiceString;
                blank = flag;
            }
            public string voice;
            public bool blank;
        }

        public StringDatabase()
        {
            lastPosition = 0;
        }

        public List<string[]> ReadStringDatabase(string filePath)
        {
            FileStream stringDatabaseReader;
            fileOpened = filePath;

            lastPosition = 0;

            List<string[]> returnData = new List<string[]>();

            stringDatabaseReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] temp = new byte[4];
            string tempString = "";

            stringDatabaseReader.Read(temp, 0, 4);
            header = BitConverter.ToInt32(temp, 0);

            stringDatabaseReader.Read(temp, 0, 4);
            version = BitConverter.ToInt32(temp, 0);

            stringDatabaseReader.Read(temp, 0, 4);
            entryCount = BitConverter.ToInt32(temp, 0);

            stringDatabaseReader.Read(temp, 0, 4);
            stringCount = BitConverter.ToInt32(temp, 0);

            stringDatabaseReader.Read(temp, 0, 4);
            voiceCount = BitConverter.ToInt32(temp, 0);

            stringDatabaseReader.Read(temp, 0, 4);
            tocOffset = BitConverter.ToInt32(temp, 0);

            for (int i = 0; i < entryCount; i++)
            {
                string[] item = null;
                while (stringDatabaseReader.Read(temp, 0, 4) > 0)
                {

                    tempString += System.Text.Encoding.ASCII.GetString(temp);

                    if (temp[3] == 0x0)
                    {
                        tempString = Regex.Replace(tempString, "\0", String.Empty);
                        item = tempString.Split('\\');
                        tempString = "";
                        break;
                    }
                }
                returnData.Add(item);
            }

            //Move position to the start of the DATA
            stringDatabaseReader.Position += 4;
            lastPosition = (int)stringDatabaseReader.Position;
            return returnData;
        }

        public List<string[]> ReadStringDatabaseData(string filePath)
        {
            FileStream stringDatabaseReader;
            fileOpened = filePath;

            List<string[]> returnData = new List<string[]>();

            stringDatabaseReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            //Move to DATA offset
            stringDatabaseReader.Position = lastPosition;

            byte[] temp = new byte[4];
            byte[] temp2 = new byte[4];
            byte[] pass1 = new byte[4];
            byte[] pass2 = new byte[4];
            string tempString = "";
            bool evenOddPass = true;
            bool startOfData = true;
            for (int i = 0; i < entryCount; i++)
            {
                string[] item = null;
                bool[] tempb = new bool[4];
                bool[] tempb2 = new bool[4];
                bool firstRun = true;

                while (stringDatabaseReader.Read(temp, 0, 4) > 0)
                {

                    /*
                    if(evenOddPass && startOfData)
                    {
                        pass1 = temp;
                        //Regex.Replace(tempString, "\0", String.Empty);
                        tempString += Regex.Replace(System.Text.Encoding.ASCII.GetString(pass1), "\0", String.Empty);
                        evenOddPass = false;
                    }
                    else if(!evenOddPass && !startOfData)
                    {
                        pass2 = temp;
                        tempString += Regex.Replace(System.Text.Encoding.ASCII.GetString(pass2), "\0", String.Empty);
                        evenOddPass = true;
                    }
                    */

                    pass1 = temp;
                    tempString += Regex.Replace(System.Text.Encoding.ASCII.GetString(pass1), "\0", String.Empty);
                    //Check last bit of data
                    if (pass1[3] == 0 && pass1[2] == 0 && !firstRun)
                    {
                        //if(pass1[0] != 0)
                        //{
                        //start of new data
                        //stringDatabaseReader.Position -= 4;
                        returnData.Add(tempString.Split('^'));
                        tempString = "";
                        break;
                        //}
                    }
                    firstRun = false;

                    //tempString += Regex.Replace(System.Text.Encoding.ASCII.GetString(pass2), "\0", String.Empty);


                    /*
                    for(int t = 0; t < pass1.Length; t++)
                    {
                        if (pass1[t] != 0)
                            tempb[t] = true;
                        else
                            tempb[t] = false;
                    }

                    for (int t = 0; t < pass2.Length; t++)
                    {
                        if (pass2[t] != 0)
                            tempb2[t] = true;
                        else
                            tempb2[t] = false;
                    }

                */
                    //tempString += System.Text.Encoding.ASCII.GetString(pass1);
                    /*
                    if (temp[3] == 0x0)
                    {
                        tempString = Regex.Replace(tempString, "\0", String.Empty);
                        item = tempString.Split('\\');
                        tempString = "";
                        break;
                    }
                    */

                }

            }

            return returnData;
        }

        public List<Entry> ReadStringDatabaseTOC(string filePath)
        {
            FileStream stringDatabaseReader;
            fileOpened = filePath;
            //lastPosition = 0;

            List<Entry> returnData = new List<Entry>();

            stringDatabaseReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] byte1 = new byte[4];
            byte[] byte2 = new byte[4];

            //move to TOC offset
            stringDatabaseReader.Position = 24 + tocOffset;

            for (int i = 0; i < entryCount; i++)
            {

                stringDatabaseReader.Read(byte1, 0, 4);
                stringDatabaseReader.Read(byte2, 0, 4);
                returnData.Add(new Entry(BitConverter.ToInt32(byte1, 0), BitConverter.ToInt32(byte2, 0)));
            }

            stringDatabaseReader.Close();

            return returnData;
        }

        public List<string> ReadStrings(string filePath)
        {
            FileStream stringDatabaseReader;
            fileOpened = filePath;

            byte[] temp = new byte[4];

            List<string> returnData = new List<string>();



            string tempString = "";
            stringDatabaseReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            //Move to Strings
            stringDatabaseReader.Position = lastPosition;

            //Strings start from 1 add null
            returnData.Add("\0");


            for (int i = 0; i < (stringCount - entryCount) -1; i++)
            {
                while (stringDatabaseReader.Read(temp, 0, 4) > 0)
                {
                    tempString += Regex.Replace(System.Text.Encoding.ASCII.GetString(temp), "\0", String.Empty);
                    if (temp[3] == 0x0 && temp[2] == 0x0)
                    {
                        //End of Data hit
                        returnData.Add(tempString);
                        tempString = "";
                        break;
                    }
                }
            }
            lastPosition = (int)stringDatabaseReader.Position;
            stringDatabaseReader.Close();
            return returnData;
        }

        public List<Voice> ReadVoices(string filePath)
        {
            FileStream stringDatabaseReader;
            fileOpened = filePath;

            byte[] temp = new byte[4];

            List<Voice> returnData = new List<Voice>();
            string tempString = "";
            stringDatabaseReader = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            stringDatabaseReader.Position = lastPosition;

            int lengthOfVoice = (tocOffset + 24) - lastPosition;
            int currentLength = 0;



            returnData.Add(new Voice("\0", false));
            while (stringDatabaseReader.Read(temp, 0, 4) > 0 && currentLength != lengthOfVoice)
            {

                tempString += System.Text.Encoding.ASCII.GetString(temp);
                currentLength += 4;
            }

            List<string> tempReturnData = new List<string>();
            tempReturnData = tempString.Split(new string[] { ".wav" }, StringSplitOptions.None).ToList();

            int y = 0;
            foreach(string str in tempReturnData)
            {
                int flagged = str.IndexOf("\0\0\0\0");
                if(flagged == 0)
                {
                    //We have found a null string
                    //Edit old index
                    returnData.Add(new Voice(Regex.Replace(str, "\0", String.Empty), false));
                    returnData[y] = new Voice(Regex.Replace(returnData[y].voice, "\0", String.Empty), true);

                }
                else
                {
                    returnData.Add(new Voice(Regex.Replace(str, "\0", String.Empty), false));
                }
                y++;
            }
            
            return returnData;

        }
    }
}
