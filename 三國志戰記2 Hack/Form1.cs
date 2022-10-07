using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace 三國志戰記2_Hack
{
    public partial class Form1 : Form
    {
        Process proc = null;
        IntPtr processHandle;
        const int PROCESS_WM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        private void modifyMemory(int address, int value, int valueSize = 4)
        {
            int bytesRead = 0;
            int bytesWritten = 0;
            byte[] writeBuffer;

            if (valueSize == 1)
            {
                writeBuffer = new byte[1];
                writeBuffer = BitConverter.GetBytes(Convert.ToByte(value));
            }
            else if (valueSize == 2)
            {
                writeBuffer = new byte[2];
                writeBuffer = BitConverter.GetBytes(Convert.ToInt16(value));
            }
            else
            {
                writeBuffer = new byte[4];
            }
            WriteProcessMemory((int)processHandle, address, writeBuffer, valueSize, ref bytesWritten);
        }

        private int readMemory(int address, int valueSize = 4)
        {
            int bytesRead = 0;
            byte[] readBuffer;

            if (valueSize == 1)
            {
                readBuffer = new byte[1];
            }
            else if (valueSize == 2)
            {
                readBuffer = new byte[2];
            }
            else
            {
                readBuffer = new byte[4];
            }

            ReadProcessMemory((int)processHandle, address, readBuffer, valueSize, ref bytesRead);

            if (valueSize == 1)
                return Convert.ToInt32(readBuffer[0]);
            else if (valueSize == 2)
                return Convert.ToInt32(BitConverter.ToInt16(readBuffer, 0));
            else
                return BitConverter.ToInt32(readBuffer, 0);
        }
      
        private int findStringIdx(string[] strArr, string tar)
        {
            for (int i = 0; i < strArr.Length; ++i)
                if (strArr[i] == tar)
                    return i;
            return -1;
        }

        private int findStringIdx(ListBox.ObjectCollection strArr, string tar)
        {
            for (int i = 0; i < strArr.Count; ++i)
                if ((string)strArr[i] == tar)
                    return i;
            return -1;
        }

        private void writeAllData()
        {
            int ID = listBox1.SelectedIndex;
            WriteData writeData = new WriteData(ID);

            writeData.ArmySize = Convert.ToInt16(textBox4.Text);
            writeData.ArmyType = (byte)findStringIdx(GameData.ArmyType, textBox15.Text);
            writeData.Feature = (byte)findStringIdx(GameData.Feature, textBox5.Text);
            writeData.Friend[0] = (byte)findStringIdx(listBox1.Items, textBox6.Text);
            writeData.Friend[1] = (byte)findStringIdx(listBox1.Items, textBox7.Text);
            writeData.Friend[2] = (byte)findStringIdx(listBox1.Items, textBox8.Text);
            writeData.Honor = Convert.ToInt16(textBox3.Text);
            writeData.IQ = Convert.ToByte(textBox21.Text);
            writeData.Item = (byte)findStringIdx(GameData.Item, textBox17.Text);
            writeData.Leadership = Convert.ToByte(textBox20.Text);
            writeData.Morale = Convert.ToByte(textBox19.Text);
            writeData.Power = Convert.ToByte(textBox22.Text);
            writeData.Skill[0] = (byte)findStringIdx(GameData.Skill, textBox9.Text);
            writeData.Skill[1] = (byte)findStringIdx(GameData.Skill, textBox10.Text);
            writeData.Skill[2] = (byte)findStringIdx(GameData.Skill, textBox11.Text);
            writeData.Skill[3] = (byte)findStringIdx(GameData.Skill, textBox12.Text);
            writeData.Skill[4] = (byte)findStringIdx(GameData.Skill, textBox13.Text);
            writeData.Skill[5] = (byte)findStringIdx(GameData.Skill, textBox14.Text);
            writeData.Title = (byte)findStringIdx(GameData.Title, textBox16.Text);

            modifyMemory(writeData.RealAddress + 0x02, writeData.ArmySize, 2);
            modifyMemory(writeData.RealAddress + 0x33, writeData.ArmyType, 1);
            modifyMemory(writeData.RealAddress + 0x16, writeData.Feature, 1);
            modifyMemory(writeData.RealAddress + 0x17, writeData.Friend[0], 1);
            modifyMemory(writeData.RealAddress + 0x18, writeData.Friend[1], 1);
            modifyMemory(writeData.RealAddress + 0x19, writeData.Friend[2], 1);
            modifyMemory(writeData.RealAddress + 0x00, writeData.Honor, 2);
            modifyMemory(writeData.RealAddress + 0x1C, writeData.IQ, 1);
            modifyMemory(writeData.RealAddress + 0x39, writeData.Item, 1);
            modifyMemory(writeData.RealAddress + 0x1D, writeData.Leadership, 1);
            modifyMemory(writeData.RealAddress + 0x3C, writeData.Morale, 1);
            modifyMemory(writeData.RealAddress + 0x1B, writeData.Power, 1);
            modifyMemory(writeData.RealAddress + 0x2D, writeData.Skill[0], 1);
            modifyMemory(writeData.RealAddress + 0x2E, writeData.Skill[1], 1);
            modifyMemory(writeData.RealAddress + 0x2F, writeData.Skill[2], 1);
            modifyMemory(writeData.RealAddress + 0x30, writeData.Skill[3], 1);
            modifyMemory(writeData.RealAddress + 0x31, writeData.Skill[4], 1);
            modifyMemory(writeData.RealAddress + 0x32, writeData.Skill[5], 1);
            modifyMemory(writeData.RealAddress + 0x35, writeData.Title, 1);
        }

        private void fillTextBox()
        {
            int ID = listBox1.SelectedIndex;
            WriteData writeData = new WriteData(ID);

            textBox4.Text = readMemory(writeData.RealAddress + 0x02, 2).ToString();
            textBox15.Text = GameData.ArmyType[readMemory(writeData.RealAddress + 0x33, 1)];
            textBox5.Text = GameData.Feature[readMemory(writeData.RealAddress + 0x16, 1)];
            textBox6.Text = listBox1.Items[readMemory(writeData.RealAddress + 0x17, 1)].ToString();
            textBox7.Text = listBox1.Items[readMemory(writeData.RealAddress + 0x18, 1)].ToString();
            textBox8.Text = listBox1.Items[readMemory(writeData.RealAddress + 0x19, 1)].ToString();
            textBox3.Text = readMemory(writeData.RealAddress + 0x00, 2).ToString();
            textBox21.Text = readMemory(writeData.RealAddress + 0x1C, 1).ToString();
            textBox17.Text = GameData.Item[readMemory(writeData.RealAddress + 0x39, 1)];
            textBox20.Text = readMemory(writeData.RealAddress + 0x1D, 1).ToString();
            textBox19.Text = readMemory(writeData.RealAddress + 0x3C, 1).ToString();
            textBox22.Text = readMemory(writeData.RealAddress + 0x1B, 1).ToString();

            try {textBox9.Text = GameData.Skill[readMemory(writeData.RealAddress + 0x2D, 1)]; } catch (Exception) { textBox9.Text = "無"; }
            try {textBox10.Text = GameData.Skill[readMemory(writeData.RealAddress + 0x2E, 1)]; } catch (Exception) { textBox10.Text = "無"; }
            try {textBox11.Text = GameData.Skill[readMemory(writeData.RealAddress + 0x2F, 1)]; } catch (Exception) { textBox11.Text = "無"; }
            try {textBox12.Text = GameData.Skill[readMemory(writeData.RealAddress + 0x30, 1)]; } catch (Exception) { textBox12.Text = "無"; }
            try {textBox13.Text = GameData.Skill[readMemory(writeData.RealAddress + 0x31, 1)]; } catch (Exception) { textBox13.Text = "無"; }
            try {textBox14.Text = GameData.Skill[readMemory(writeData.RealAddress + 0x32, 1)];}catch (Exception){textBox14.Text = "無";}
            textBox16.Text = GameData.Title[readMemory(writeData.RealAddress + 0x35, 1)];
        }

        public Form1()
        {
            InitializeComponent();
            listBox2.SelectionMode = SelectionMode.One;
            initializeData();

            while (proc == null || Process.GetProcessesByName("pcsx2").Length == 0)
            {
                //MessageBox.Show("DEBUG MODE");// DEBUG
                try
                {
                    proc = Process.GetProcessesByName("pcsx2")[0];
                    processHandle = proc.Handle;
                    //MessageBox.Show("process attached");
                }
                catch
                {
                    //break; // DEBUG
                    MessageBox.Show("process attach failed");
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        TextBox focusTextBox;

        private void initializeData()
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for(int i=0;i<listBox1.Items.Count;++i)
            {
                if( listBox1.Items[i].ToString() == textBox1.Text )
                {
                    listBox1.SelectedIndex = i;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox2.Items.Count; ++i)
            {
                if (listBox2.Items[i].ToString() == textBox2.Text)
                {
                    listBox2.SelectedIndex = i;
                }
            }
        }

        private void textBox9_Enter(object sender, EventArgs e)
        {
            focusTextBox = (TextBox)sender;
            listBox2.Items.Clear();
            for(int i=0;i<82;++i)
                listBox2.Items.Add( GameData.Skill[i] );
            listBox2.Items.Add("無");
        }

        private void textBox9_Leave(object sender, EventArgs e)
        {
            if (listBox2.Focused == true || textBox2.Focused == true)
                return;
            listBox2.Items.Clear();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            focusTextBox.Text = (string)listBox2.SelectedItem;
        }

        private void textBox17_Enter(object sender, EventArgs e)
        {
            focusTextBox = (TextBox)sender;
            listBox2.Items.Clear();
            foreach (string s in GameData.Item)
                listBox2.Items.Add(s);
        }

        private void textBox17_Leave(object sender, EventArgs e)
        {
            if (listBox2.Focused == true || textBox2.Focused == true)
                return;
            listBox2.Items.Clear();
        }

        private void textBox15_Enter(object sender, EventArgs e)
        {
            focusTextBox = (TextBox)sender;
            listBox2.Items.Clear();
            foreach (string s in GameData.ArmyType)
                listBox2.Items.Add(s);
        }

        private void textBox15_Leave(object sender, EventArgs e)
        {
            if (listBox2.Focused == true || textBox2.Focused == true)
                return;
            listBox2.Items.Clear();
        }

        private void listBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Focused == true)
                return;
            listBox2.Items.Clear();
        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            focusTextBox = (TextBox)sender;
            listBox2.Items.Clear();
            foreach (string s in listBox1.Items)
                listBox2.Items.Add(s);
        }

        private void textBox6_Leave(object sender, EventArgs e)
        {
            if (listBox2.Focused == true || textBox2.Focused == true)
                return;
            listBox2.Items.Clear();
        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            focusTextBox = (TextBox)sender;
            listBox2.Items.Clear();
            foreach (string s in GameData.Feature)
                listBox2.Items.Add(s);
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
            if (listBox2.Focused == true || textBox2.Focused == true)
                return;
            listBox2.Items.Clear();
        }

        private void textBox16_Enter(object sender, EventArgs e)
        {
            focusTextBox = (TextBox)sender;
            listBox2.Items.Clear();
            foreach (string s in GameData.Title)
                listBox2.Items.Add(s);
        }

        private void textBox16_Leave(object sender, EventArgs e)
        {
            if (listBox2.Focused == true || textBox2.Focused == true)
                return;
            listBox2.Items.Clear();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == 248)
            {
                return;
            }
            fillTextBox();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            writeAllData();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}

class GameData
{
    static public string[] ArmyType = new string[13] {"輕步兵", "步兵", "槍兵", "重裝槍兵", "弓兵", "長弓兵", "弩兵", "強弩兵", "輕騎兵", "重裝騎兵",
                                        "鐵甲騎兵", "弓騎兵", "建設兵" };
    static public string[] Item = new string[59] { "古錠刀", "青釭劍", "倚天劍", "七星寶刀", "山海經", "尉繚子", "太平要術書", "遁甲天書", "治論", "論語集解",
                                            "春秋左氏傳", "史記", "六韜", "方天戟", "老子", "孟德新書", "蛇矛", "雙鐵戟", "偃月刀", "刺客列傳",
                                            "墨子", "吳子", "汗血馬", "漢書李廣傳", "孫子", "大斧", "便宜十六冊", "韓非子", "刎頸爵 ", "玉璽",
                                            "三尖刀", "列仙傳", "雌雄一對劍", "荀子", "伍子胥列傳", "黃石公三略", "五把寶刀", "司馬法", "四神鏡", "漢書衛青傳",
                                            "孫子用間篇", "戰國策", "真孫子", "山岳兵術書", "河川兵術書", "易經", "書經", "詩經", "青囊書", "玉龍紋璧",
                                            "神獸硯", "和氏璧", "長信宮燈", "銅雀", "羽扇", "金象嵌壺", "龍方壺", "漆塗鼎", "無" };
    static public string[] Skill = new string[90] { "突擊", "強襲突擊", "陷陣突擊", "強行突破", "側面攻擊", "側面強襲", "追擊", "猛追", "迫擊掃討", "單騎驅", "單騎破陣", "單騎突破", "鬼神急驅", "奮鬥", "轟雷奮鬥", "鬼神奮鬥", "李代桃僵", "速射", "亂射", "神仙亂射", "威壓", "威壓(變)", "大喝", "督戰", "大督戰", "火矢速射", "火矢亂射", "神仙火矢", "伏兵", "伏兵襲擊", "偃旗息鼓", "同士討", "同士討(變)", "二虎競食", "偽退誘敵", "偽退滅殺", "虛誘掩殺", "虛報前進", "虛報進軍", "圍魏救趙", "挑撥(變)", "挑撥", "大酒宴", "火計", "業火計", "爆殺火計", "水計", "瀑布計", "落石", "崩落計", "陷阱", "金輪奈落", "反客為主", "聲東擊西", "聲東擊西", "虛報集結", "離間計", "真孫子", "一齊攻擊", "集中攻擊", "一齊突擊", "十面突擊", "一齊破陣", "夾擊", "混亂夾擊", "打草驚蛇", "援護", "援護射擊", "後詰", "激勵", "大激勵", "波狀攻擊", "犄角", "犄角強襲", "犄角夾擊", "後方奇襲", "拋磚引玉", "包圍", "集合", "散開", "一齊砲擊", "一齊投射", "", "", "", "", "", "", "", "無"};
    static public string[] Title = new string[78] { "大將軍", "驃騎將軍", "車騎將軍", "衛將軍", "征東將軍", "征南將軍", "征西將軍", "征北將軍", "鎮東將軍", "鎮南將軍", "鎮西將軍", "鎮北將軍", "安東將軍", "安南將軍", "安西將軍", "安北將軍", "平東將軍", "平南將軍", "平西將軍", "平北將軍", "左將軍", "右將軍", "前將軍", "後將軍", "領軍將軍", "龍驤將軍", "征虜將軍", "鎮軍將軍", "撫軍將軍", "安遠將軍", "輔國將軍", "武衛將軍", "中衛將軍", "中堅將軍", "中壘將軍", "驍騎將軍", "游騎將軍", "寧朔將軍", "建德將軍", "建威將軍", "建武將軍", "振威將軍", "振武將軍", "奮威將軍", "奮武將軍", "揚威將軍", "揚武將軍", "廣威將軍", "廣武將軍", "積車將軍", "積弩將軍", "強弩將軍", "盪寇將軍", "昭武將軍", "昭文將軍", "討逆將軍", "破虜將軍", "橫江將軍", "安國將軍", "威東將軍", "威南將軍", "威西將軍", "威北將軍", "軍師將軍", "鷹揚將軍", "折衝將軍", "輕車將軍", "虎烈將軍", "宣威將軍", "威遠將軍", "寧遠將軍", "伏波將軍", "虎威將軍", "凌江將軍", "牙門將軍", "偏將軍", "裨將軍", "無" };
    static public string[] Feature = new string[30] { "策士", "萬人敵", "英雄", "奸雄", "霸王", "武神", "一騎當千", "不屈", "鐵壁", "支援", "神速", "神弓", "鬼謀", "暴虐", "冷靜沉著", "舌鋒", "心交", "德望", "暴威", "先見", "神技", "功名", "激情", "奸才", "勇猛", "名將", "反攻", "殊勳", "工作", "外交" };
}


class WriteData
{
    public WriteData(int ID)
    {
        RealAddress = BaseAddress + 0x5C * ID;
    }

    public static int BaseAddress = 0x20AA5102;
    public int RealAddress;

    public short Honor;
    public short ArmySize;
    public byte Feature;
    public byte[] Friend = new byte[3];
    public byte[] Skill = new byte[6];
    public byte ArmyType;
    public byte Title;
    public byte Item;
    public byte Morale;

    public byte Power;
    public byte IQ;
    public byte Leadership;
}