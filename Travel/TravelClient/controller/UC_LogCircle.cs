﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using TravelClient.utils;
using System.Xml.Serialization;
using System.Net.Http;
using TravelClient.Models;

namespace TravelClient.controller
{
    public partial class UC_LogCircle : UserControl
    {
        private string Uid;
        public UC_LogCircle(string uid)
        {
            InitializeComponent();
            this.Uid = id;
            SetFont();
            InitInfo();
        }

        private void SetFont()
        {
            string AppPath = Application.StartupPath;
            try
            {
                PrivateFontCollection font = new PrivateFontCollection();
                font.AddFontFile(AppPath + @"\font\造字工房映力黑规体.otf");
                font.AddFontFile(AppPath + @"\font\SF-Pro-Text-Medium.otf");

                Font titleFont25 = new Font(font.Families[1], 25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                Font SFProFont9 = new Font(font.Families[0], 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                Font SFProFont15 = new Font(font.Families[0], 15F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));

                //设置窗体控件字体，哪些控件要更改都写到下面
                label1.Font = titleFont25;
                label2.Font = SFProFont9;
                btnLogCircle.Font = SFProFont15;
                btnMyShare.Font = SFProFont15;
            }
            catch
            {
                MessageBox.Show("字体不存在或加载失败\n程序将以默认字体显示", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void BtnMyShare_Click(object sender, EventArgs e)
        {
            string id = this.Uid;
            string url = "Https://localhost:5001/api/blogger/get/user?uid="+ id;
            ShowLogList(url);
        }

        private void BtnLogCircle_Click(object sender, EventArgs e)
        {
            InitInfo();
        }

        private void InitInfo()
        {
            string url = "Https://localhost:5001/api/blogger/get";
            ShowLogList(url);
        }

        private async void ShowLogList(string url)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Diary>));
            Client client = new Client();
            try
            {
                HttpResponseMessage result = await client.Get(url);
                if (result.IsSuccessStatusCode)
                {
                    List<Diary> diaries = (List<Diary>)xmlSerializer.Deserialize(await result.Content.ReadAsStreamAsync());
                    panelLogList.Controls.Clear();

                    foreach (Diary diary in diaries)
                    {
                        UC_LogCell cell = new UC_LogCell();
                        cell.lblTitle.Text = diary.Title;
                        cell.lblTime.Text = diary.Time.ToString();

                        //拉取用户昵称
                        result = await client.Get("Https://localhost:5001/api/user/get?uid=" + diary.Uid);
                        if (result.IsSuccessStatusCode)
                        {
                            XmlSerializer xs = new XmlSerializer(typeof(User));
                            User user = (User)xs.Deserialize(await result.Content.ReadAsStreamAsync());
                            cell.lblUserName.Text = user.Uname;
                        }
                        //添加到panel中
                        panelLogList.Controls.Add(cell);
                    }
                    //添加底部标志
                    Label lblBottom = new Label();
                    lblBottom.Text = "到底了哦~";
                    lblBottom.Font = label2.Font;
                    lblBottom.Anchor = AnchorStyles.None;
                    panelLogList.Controls.Add(lblBottom);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
