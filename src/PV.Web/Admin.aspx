<%@ Page Title="Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="PV.Web.Admin" %>
<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Admin</h3>
    <h4>Archive</h4>
    <table class="detail">
        <tr><th>Archive Root</th><td><asp:Label ID="lblRoot" runat="server" /></td></tr>
        <tr><th>MT535 files</th><td><asp:Label ID="lbl535" runat="server" /></td></tr>
        <tr><th>MT54x files</th><td><asp:Label ID="lbl54x" runat="server" /></td></tr>
        <tr><th>Statements</th><td><asp:Label ID="lblStmts" runat="server" /></td></tr>
        <tr><th>Confirmations</th><td><asp:Label ID="lblConfs" runat="server" /></td></tr>
        <tr><th>Parse Errors</th><td><asp:Label ID="lblErrs" runat="server" /></td></tr>
        <tr><th>Last Loaded</th><td><asp:Label ID="lblLoaded" runat="server" /></td></tr>
        <tr><th>Load Time</th><td><asp:Label ID="lblMs" runat="server" /></td></tr>
    </table>
    <br />
    <asp:Button ID="btnReload" runat="server" Text="Reload archive" OnClick="btnReload_Click"
        OnClientClick="return confirm('Reload the archive index?');" />
    <div class="msg"><asp:Label ID="lblMsg" runat="server" /></div>
    <h4>Users</h4>
    <asp:GridView ID="gvUsers" runat="server" CssClass="grid" AutoGenerateColumns="false"
        AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="USERNAME" HeaderText="User ID" />
            <asp:BoundField DataField="FULL_NAME" HeaderText="Name" />
            <asp:BoundField DataField="ROLE_CD" HeaderText="Role" />
            <asp:BoundField DataField="ACTIVE_FLG" HeaderText="Active" />
            <asp:BoundField DataField="LAST_LOGIN" HeaderText="Last Login" />
        </Columns>
    </asp:GridView>
</asp:Content>
