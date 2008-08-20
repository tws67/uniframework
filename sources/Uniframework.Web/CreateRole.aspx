<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="CreateRole.aspx.cs" Inherits="CreateRole" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<h3>创建角色</h3>
<p>角色名称<br />
    &nbsp;<asp:TextBox ID="tbRoleName" runat="server" Width="318px"></asp:TextBox>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="tbRoleName"
        Display="Dynamic" ErrorMessage="Please provide role name"></asp:RequiredFieldValidator><br />
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="创建" /></p>
</asp:Content>

