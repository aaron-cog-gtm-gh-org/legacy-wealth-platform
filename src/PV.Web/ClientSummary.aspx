<%@ Page Title="Client Summary" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClientSummary.aspx.cs" Inherits="PV.Web.ClientSummary" %>
<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Client Summary</h3>
    <table class="detail">
        <tr><th>Client Ref</th><td><asp:Label ID="lblRef" runat="server" /></td></tr>
        <tr><th>Client Name</th><td><asp:Label ID="lblName" runat="server" /></td></tr>
        <tr><th>Adviser</th><td><asp:Label ID="lblAdviser" runat="server" /></td></tr>
        <tr><th>Status</th><td><asp:Label ID="lblStatus" runat="server" /></td></tr>
        <tr><th>Client Since</th><td><asp:Label ID="lblSince" runat="server" /></td></tr>
    </table>
    <br />
    <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="false"
        AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="ACCT_NO" HeaderText="Acct No" />
            <asp:BoundField DataField="ACCT_TYPE" HeaderText="Acct Type" />
            <asp:BoundField DataField="BASE_CCY" HeaderText="Ccy" />
            <asp:BoundField DataField="SAFE_CODE" HeaderText="Safe Code" />
            <asp:BoundField DataField="STATUS_TXT" HeaderText="Status" />
            <asp:BoundField DataField="OPENED" HeaderText="Opened" />
            <asp:BoundField DataField="CLOSED" HeaderText="Closed" />
            <asp:BoundField DataField="STMT_DATE" HeaderText="Stmt Date" />
            <asp:BoundField DataField="MKTVAL" HeaderText="Mkt Val (GBP)" ItemStyle-CssClass="num" />
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:HyperLink runat="server" NavigateUrl='<%# "Holdings.aspx?acct=" + Eval("ACCT_ID") %>'
                        Visible='<%# Eval("STMT_RAW") != DBNull.Value %>'>Holdings</asp:HyperLink>
                    &nbsp;
                    <asp:HyperLink runat="server" NavigateUrl='<%# "Settlements.aspx?acct=" + Eval("ACCT_ID") %>'>Settlements</asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <div class="msg"><b>Grand Total (GBP):</b> <asp:Label ID="lblTotal" runat="server" /></div>
</asp:Content>
