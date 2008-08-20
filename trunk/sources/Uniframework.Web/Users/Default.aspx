<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" title="Users" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<h1>只有经过验证的用户组成员才能看见此页!</h1>
<p>你已经登录本系统，你的用户名：
    <asp:LoginName ID="LoginName1" runat="server" />
</p>
</asp:Content>

