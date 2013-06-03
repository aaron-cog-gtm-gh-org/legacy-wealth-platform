<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="PV.Web.Login" %>
<!DOCTYPE html>
<html>
<head>
    <title>Portfolio Viewer - Log In</title>
    <link href="Styles/pv.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="pagewrap">
        <div class="hdr"><span class="hdrtitle">Portfolio Viewer</span></div>
        <div class="content">
            <table>
                <tr>
                    <td>User ID</td>
                    <td><asp:TextBox ID="txtUser" runat="server" Width="140" /></td>
                </tr>
                <tr>
                    <td>Password</td>
                    <td><asp:TextBox ID="txtPwd" runat="server" TextMode="Password" Width="140" /></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:Button ID="btnLogin" runat="server" Text="Log In" OnClick="btnLogin_Click" /></td>
                </tr>
            </table>
            <div class="msg err"><asp:Label ID="lblErr" runat="server" /></div>
        </div>
        <div class="ftr">Portfolio Viewer v3.2.1 (build 2018.11) &nbsp;|&nbsp; Best viewed at 1024x768</div>
    </div>
    </form>
</body>
</html>
