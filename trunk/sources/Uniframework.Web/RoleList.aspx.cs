using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class RoleList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Bind();
    }

    private void Bind()
    {
        rolesDDL.DataSource = Roles.GetAllRoles();
        rolesDDL.DataBind();
        gvRolesAndUsers.DataSource = Roles.GetAllRoles();
        gvRolesAndUsers.DataBind();
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Roles.DeleteRole(rolesDDL.SelectedValue);
        Bind();
    }
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        Roles.RemoveUsersFromRole(Roles.GetUsersInRole(rolesDDL.SelectedValue), rolesDDL.SelectedValue);
        Bind();
    }
    protected void gvRolesAndUsers_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string role = (string)e.Row.DataItem;

            Label roleName = (Label)e.Row.FindControl("roleName");
            roleName.Text = role;

            BulletedList userList = (BulletedList)e.Row.FindControl("userList");
            userList.DataSource = Roles.GetUsersInRole(role);
            userList.DataBind();
        }
    }
}
