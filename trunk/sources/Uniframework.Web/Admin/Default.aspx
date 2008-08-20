<%@ Page Language="VB" MasterPageFile="~/MasterPage.master" title="Admin Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<h1>管理员!</h1>
<p>呵呵, 
    <asp:LoginName ID="LoginName1" runat="server" />. 你必须以管理员身份登录才可看见此页！
</p>
</asp:Content>

