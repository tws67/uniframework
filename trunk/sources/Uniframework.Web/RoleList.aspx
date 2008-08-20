<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RoleList.aspx.cs" Inherits="RoleList" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<h2>角色列表</h2>
    <p>
        <strong>
        </strong><strong>角色<asp:DropDownList ID="rolesDDL" runat="server">
            </asp:DropDownList>
            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">删除</asp:LinkButton>
            &nbsp;
            <asp:LinkButton ID="LinkButton2" runat="server" OnClick="LinkButton2_Click">删除角色成员</asp:LinkButton></strong></p>
    <p>
        <strong>角色及其成员列表:</strong><br />
            <asp:GridView ID="gvRolesAndUsers" runat="server" AutoGenerateColumns="False" CellPadding="4" OnRowDataBound="gvRolesAndUsers_RowDataBound" ForeColor="#333333" GridLines="None">
                <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                <Columns>
                    <asp:TemplateField HeaderText="角色">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="roleName"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="成员列表">
                        <ItemTemplate>
                            <asp:BulletedList runat="server" ID="userList"></asp:BulletedList>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <PagerStyle ForeColor="White" HorizontalAlign="Center" BackColor="#284775" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#999999" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            </asp:GridView>
        
    </p>
</asp:Content>

