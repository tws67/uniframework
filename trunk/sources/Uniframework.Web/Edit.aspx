<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="Edit.aspx.cs" Inherits="Edit" Title="Untitled Page" %>

<script runat="server">

    void ProcessCommand(object sender, FormViewCommandEventArgs args)
    {
        if (args.CommandName == "Cancel")
            Response.Redirect("userlist.aspx");
    }

    void ProcessUpdating(object sender, ObjectDataSourceMethodEventArgs args)
    {
        MembershipUser user = Membership.GetUser(args.InputParameters["userName"].ToString());

        user.Email = args.InputParameters["email"].ToString();
        args.InputParameters.Clear();
        args.InputParameters.Add("user", user);

        foreach (ListItem item in ListBox1.Items)
        {
            if (item.Selected & !Roles.IsUserInRole(Request["username"].ToString(), item.Value))
            {
                Roles.AddUserToRole(Request["username"].ToString(), item.Value);
            }
        }

    }

    void UpdateUser(object sender, ObjectDataSourceMethodEventArgs args)
    {
        MembershipUser user = Membership.GetUser(args.InputParameters["userName"]);
        if (user != null)
        {
            args.InputParameters.Add(typeof(MembershipUser), user);
        }
    }

    void ProcessUpdated(object sender, ObjectDataSourceStatusEventArgs args)
    {
        if (args.Exception == null)
            Response.Redirect("userlist.aspx");
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Membership.DeleteUser(this.Request["userName"].ToString(), true);
        Response.Redirect("userlist.aspx");
    }
</script>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h2>
        编辑成员资料</h2>
    <asp:FormView ID="FormView1" runat="server" DefaultMode="Edit" OnItemCommand="ProcessCommand"
        DataSourceID="source">
        <EditItemTemplate>
            <table width="100%">
                <tr>
                    <td style="width: 186px; height: 26px;" bgcolor="lightgrey">
                        名称:</td>
                    <td style="width: 65%; height: 26px;">
                        <asp:TextBox ID="username" runat="server" Width="388px" ReadOnly="True" Height="22px"
                            Text='<%# Bind("username") %>'></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td bgcolor="lightgrey" style="width: 186px">
                        Email:</td>
                    <td style="width: 65%">
                        <asp:TextBox ID="email" runat="server" Width="388px" Height="22px" Text='<%# Bind("Email") %>'></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left" colspan="2">
                        <asp:Button ID="Button2" runat="server" Text="确定" CommandName="Update" />
                        <asp:Button ID="Button1" runat="server" Text="取消" CommandName="Cancel" />&nbsp;&nbsp;
                    </td>
                </tr>
            </table>
        </EditItemTemplate>
    </asp:FormView>
    <asp:ObjectDataSource ID="source" runat="server" OnUpdating="ProcessUpdating" TypeName="System.Web.Security.Membership"
        SelectMethod="GetUser" OnUpdated="ProcessUpdated" OldValuesParameterFormatString="original_{0}"
        UpdateMethod="UpdateUser">
        <SelectParameters>
            <asp:QueryStringParameter Name="userName" QueryStringField="userName" Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
    &nbsp;
    <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">Delete this user</asp:LinkButton>
    <br />
    <br />
    选择组（角色）:<br />
    <asp:ListBox ID="ListBox1" runat="server" SelectionMode="Multiple"></asp:ListBox>
</asp:Content>
