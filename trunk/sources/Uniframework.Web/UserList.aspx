<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="UserList.aspx.cs" Inherits="UserList" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:GridView ID="GridView1" GridLines="None" runat="server" CellPadding="4" Width="100%"
        DataKeyNames="userName" DataSourceID="ObjectDataSource1" AutoGenerateColumns="False"
        ForeColor="#333333" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" OnRowDeleting="GridView1_RowDeleting"
        OnRowDataBound="GridView1_RowDataBound">
        <FooterStyle ForeColor="White" BackColor="#5D7B9D" Font-Bold="True"></FooterStyle>
        <PagerStyle ForeColor="White" HorizontalAlign="Center" BackColor="#284775"></PagerStyle>
        <HeaderStyle ForeColor="White" Font-Bold="True" BackColor="#5D7B9D"></HeaderStyle>
        <AlternatingRowStyle BackColor="White" ForeColor="#284775"></AlternatingRowStyle>
        <Columns>
            <asp:BoundField DataField="UserName" HeaderText="帐户名称" ReadOnly="True" SortExpression="UserName" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="LastPasswordChangedDate" DataFormatString="{0:d}" HeaderText="上次修改密码时间"
                ReadOnly="True" SortExpression="LastPasswordChangedDate" />
            <asp:BoundField DataField="CreationDate" DataFormatString="{0:d}" HeaderText="创建时间"
                ReadOnly="True" SortExpression="CreationDate" />
            <asp:BoundField DataField="LastLoginDate" DataFormatString="{0:d}" HeaderText="上次登录时间"
                SortExpression="LastLoginDate" />
            <asp:TemplateField HeaderText="所属角色">
                <ItemTemplate>
                    <asp:BulletedList ID="roleList" runat="server">
                    </asp:BulletedList>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowSelectButton="True" SelectText="Edit/Delete" />
        </Columns>
        <SelectedRowStyle ForeColor="#333333" Font-Bold="True" BackColor="#E2DED6"></SelectedRowStyle>
        <RowStyle ForeColor="#333333" BackColor="#F7F6F3"></RowStyle>
        <EditRowStyle BackColor="#999999" />
    </asp:GridView>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" TypeName="System.Web.Security.Membership"
        SelectMethod="GetAllUsers" OldValuesParameterFormatString="original_{0}" OnUpdating="ObjectDataSource1_Updating">
        <DeleteParameters>
            <asp:Parameter Name="username" Type="String" />
        </DeleteParameters>
    </asp:ObjectDataSource>
</asp:Content>

