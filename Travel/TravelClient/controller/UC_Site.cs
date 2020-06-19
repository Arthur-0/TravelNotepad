﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravelClient.form;
using System.Xml.Serialization;
using TravelClient.utils;
using System.Net.Http;
using TravelClient.Models;

namespace TravelClient.controller
{
    public partial class UC_Site : UserControl
    {
        ChangePanel changePanel;
        public long routeID;
        public string siteId;
        string siteName = "";

        public UC_Site()
        {
            InitializeComponent();
        }

        public UC_Site(ChangePanel changePanel,long routeID,string siteid)
        {
            InitializeComponent();
            this.routeID = routeID;
            this.changePanel = changePanel;
            this.siteId = siteid;
            GetSiteName();
            
        }

        private void Btn_ToSiteInfo_Click(object sender, EventArgs e)
        {
            UC_SiteInfo uc_siteInfo = new UC_SiteInfo(changePanel,routeID);
            changePanel(uc_siteInfo);
        }

        private async void GetSiteName()
        {
            string url = "https://localhost:5001/api/site/get?siteId=" + siteId;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Site));
            Client client = new Client();
            try
            {
                HttpResponseMessage result = await client.Get(url);
                if (result.IsSuccessStatusCode)
                {
                    Site site = (Site)xmlSerializer.Deserialize(await result.Content.ReadAsStreamAsync());
                    siteName = site.SiteName;
                    this.Btn_ToSiteInfo.Text = siteName;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void SetFont()
        {
            string AppPath = Application.StartupPath;
            try
            {
                PrivateFontCollection font = new PrivateFontCollection();
                font.AddFontFile(AppPath + @"\font\造字工房映力黑规体.otf");

                Font titleFont12 = new Font(font.Families[0], 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));

                //设置窗体控件字体，哪些控件要更改都写到下面
                Btn_ToSiteInfo.Font = titleFont12;
                
            }
            catch
            {
                MessageBox.Show("字体不存在或加载失败\n程序将以默认字体显示", "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
