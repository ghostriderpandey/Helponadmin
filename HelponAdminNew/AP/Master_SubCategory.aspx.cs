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
    public partial class Master_SubCategory : System.Web.UI.Page
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
                HtmlContainerControl obj;
                HtmlContainerControl obj1;
                obj = (HtmlContainerControl)this.Master.FindControl("pagename");
                obj1 = (HtmlContainerControl)this.Master.FindControl("pagename1");
                string pagename = Path.GetFileName(Request.Url.AbsolutePath);
                obj.InnerText = cls.ExecuteStringScalar("EXEC ProcGet_AdminMenuName '" + pagename + "'");
                obj1.InnerText = obj.InnerText;
                cls.BindDropDownList(ddlCategory, "ProcMaster_Category 'GetforDDL',0,'" + Request.QueryString["Type"] + "'", "Name", "ID");
                FillData();
            }
        }
        private void FillData()
        {
            DataTable dtData = cls.selectDataTable("ProcMaster_SubCategory 'GetAll',0,0,'"+ Request.QueryString["Type"] + "'");
            GvData.DataSource = dtData;
            GvData.DataBind();
        }
        private void GetData(int id)
        {
            DataTable dtresult = cls.selectDataTable("ProcMaster_SubCategory 'GetAll','" + id + "'");
            if (dtresult.Rows.Count > 0)
            {
                ddlCategory.SelectedValue = dtresult.Rows[0]["CID"].ToString();
                txtName.Text = dtresult.Rows[0]["SubcategoryName"].ToString();
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
            int max = 0;
            string SubcatIcon = "";
            if (ViewState["ID"] != null)
            {
                max = Convert.ToInt32(ViewState["ID"]);
            }
            else
            {
                max = cls.ExecuteIntScalar("select Isnull(Max(ID),0)+1 from tblMaster_Subcategory");
            }
            ImageUploadStatus imageUpload = new ImageUploadStatus();
            if (filecategory.HasFile)
            {

                
                imageUpload = UploadImage(filecategory, max.ToString() + "_SubCAT");
                if (imageUpload.Status == false)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + imageUpload.ImgName + "')", true);
                    return;
                }
            }
            if (fileIconcategory.HasFile)
            {
                imageUpload = UploadImage(fileIconcategory, max.ToString() + "_SubcatIcon");
                if (imageUpload.Status == false)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + imageUpload.ImgName + "')", true);
                    return;
                }
                SubcatIcon = imageUpload.ImgName;
            }
            DataTable dt = cls.selectDataTable("Exec ProcMaster_SubCategory 'insert','" + id + "','"+ddlCategory.SelectedValue+"','" + txtName.Text.Replace("'", "").Trim() + "','" + imageUpload.ImgName + "','"+ Request.QueryString["Type"] + "'");
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["Status"].ToString() == "1")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + dt.Rows[0]["Message"] + "');location.replace('Master_subCategory.aspx?Type="+ Request.QueryString["Type"] + "')", true);

                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + dt.Rows[0]["Message"] + "');", true);
                }
            }
        }
       
        private ImageUploadStatus UploadImage(FileUpload file, string Number)
        {
            ImageUploadStatus uploadStatus = new ImageUploadStatus();
            string ext = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower();
            string FileName = Number + ext;
            if (file.HasFile == true)
            {
                if (file.PostedFile.FileName != "")
                {
                    string Extension = ext;
                    if (Extension == ".jpg" || Extension == ".jpeg" || Extension == ".png" || Extension == ".gif")
                    {
                        string opath = Server.MapPath("../Upload/Category/" + FileName);
                        file.SaveAs(opath);
                        // Stream strm = file.PostedFile.InputStream;
                        // objImgae.GenerateThumbnails(1, strm, opath);
                        uploadStatus.Status = true;
                        uploadStatus.ImgName = FileName;
                    }
                    else
                    {
                        uploadStatus.Status = false;
                        uploadStatus.ImgName = "Invalid Image";
                    }
                }
            }
            else
            {
                uploadStatus.Status = false;
                uploadStatus.ImgName = "Please Select image";
            }
            return uploadStatus;
        }

        protected void GvData_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "IsDelete")
            {
                cls.ExecuteQuery("Exec ProcMaster_SubCategory 'IsDelete','" + e.CommandArgument + "'");
                FillData();
            }
            else if (e.CommandName == "IsChange")
            {
                GetData(Convert.ToInt32(e.CommandArgument));
            }
        }

        protected void GvData_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void GvData_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }
    }
}