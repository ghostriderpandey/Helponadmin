﻿using HelponAdminNew.GlobalHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace HelponAdminNew.AP
{
    public partial class ManageCouponImg : System.Web.UI.Page
    {
        Cls_Connection cls = new Cls_Connection();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminSession"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                //HtmlContainerControl obj;
                //HtmlContainerControl obj1;
                //obj = (HtmlContainerControl)this.Master.FindControl("pagename");
                //obj1 = (HtmlContainerControl)this.Master.FindControl("pagename1");
                //string pagename = Path.GetFileName(Request.Url.AbsolutePath);
                //obj.InnerText = cls.ExecuteStringScalar("EXEC ProcGet_AdminMenuName '" + pagename + "'");
                //obj1.InnerText = obj.InnerText;
                FillData();
            }
        }
        private void FillData()
        {
            DataTable dtData = cls.selectDataTable("ProcMaster_AdminCoupon 'GetAll',0,'" + Request.QueryString["Type"] + "'");
            GvData.DataSource = dtData;
            GvData.DataBind();
        }
        private void GetData(int id)
        {
            DataTable dtresult = cls.selectDataTable("ProcMaster_AdminCoupon 'GetAll','" + id + "'");
            if (dtresult.Rows.Count > 0)
            {
                txtName.Text = dtresult.Rows[0]["Name"].ToString();
                ViewState["ID"] = id;
                btnSubmit.Text = "Update";
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int id = 0;
            if (ViewState["ID"] != null)
            {
                id = Convert.ToInt32(ViewState["ID"]);
            }
            ImageUploadStatus imageUpload = new ImageUploadStatus();

            DataTable dt = cls.selectDataTable("Exec ProcMaster_AdminCoupon @Action='insert',@ID='" + id + "',@Name='" + txtName.Text.Replace("'", "").Trim() + "'");
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["Status"].ToString() == "1")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + dt.Rows[0]["Message"] + "');location.replace('ManageCouponImg.aspx')", true);

                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + dt.Rows[0]["Message"] + "');", true);
                }
            }
        }

        protected void GvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "IsDelete")
            {
                cls.ExecuteQuery("Exec ProcMaster_AdminCoupon 'IsDelete','" + e.CommandArgument + "'");
                FillData();
            }
            else if (e.CommandName == "IsChange")
            {
                GetData(Convert.ToInt32(e.CommandArgument));
            }
            else
            {
                Response.Redirect("CouponImgUpload.aspx?ID='" + e.CommandArgument + "'");
            }
        }

    }
}