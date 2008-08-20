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

public partial class UserList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    
    protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        string url = "edit.aspx?userName=" + GridView1.DataKeys[GridView1.SelectedIndex].Value;
        Response.Redirect(url);
    }
    
    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string username = GridView1.DataKeys[e.Row.RowIndex].Value.ToString();
            string[] userRoles = Roles.GetRolesForUser(username);
            BulletedList bl = (BulletedList)e.Row.FindControl("roleList");
            bl.DataSource = userRoles;
            bl.DataBind();
        }
    }
    
    protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Membership.DeleteUser(GridView1.DataKeys[GridView1.SelectedIndex].Value.ToString());
        GridView1.DataBind();
    }
    
    protected void ObjectDataSource1_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        MembershipUser user = Membership.GetUser(e.InputParameters["username"].ToString());

        user.Email = e.InputParameters["email"].ToString();
        e.InputParameters.Clear();
        e.InputParameters.Add("user", user);
    }

    private void UpdateUser(object sender, ObjectDataSourceMethodEventArgs args)
    {
        MembershipUser user = Membership.GetUser(args.InputParameters["UserName"].ToString());
        if (user != null)
        {
            args.InputParameters.Add(typeof(MembershipUser), user);
        }
    }
}
