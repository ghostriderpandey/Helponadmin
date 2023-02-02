﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;

namespace HelponAdminNew.Merchant
{
    public partial class Login : System.Web.UI.Page
    {
        Cls_Connection cls = new Cls_Connection();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["logout"] != null)
            {
                Session["MerchantSession"] = null;
                Response.Redirect("Login.aspx");

            
            }
           
        }

        protected void btnlogin_Click(object sender, EventArgs e)
        {
            cls.loginname = txtUserName.Text.Trim();
            cls.password = txtPassword.Text.Trim();
            cls.action = "MerchantPanel";
            cls.IpAddress = Request.ServerVariables["REMOTE_ADDR"].ToString();
            DataTable dt = cls.AdminLoginAuthentication();
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["Status"].ToString() == "1")
                {
                    Session.Add("MerchantSession", dt);
                    Response.Redirect("Dashboard.aspx");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "swal('Alert !','" + dt.Rows[0]["Message"] + "','error')", true);
                }
            }
        }
    }
}