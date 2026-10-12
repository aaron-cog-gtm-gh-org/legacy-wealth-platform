<%@ Page Title="Clients" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClientSearch.aspx.cs" Inherits="PV.Web.ClientSearch" %>
<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Clients</h3>
    Name or Client Ref&nbsp;<asp:TextBox ID="txtSearch" runat="server" Width="200" />
    &nbsp;<asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
    &nbsp;<asp:Button ID="btnAll" runat="server" Text="Show All" OnClick="btnAll_Click" />
    <div class="msg err"><asp:Label ID="lblErr" runat="server" /></div>
    <div class="msg"><asp:Label ID="lblMsg" runat="server" /></div>
    <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="false"
        AllowPaging="true" PageSize="25" OnPageIndexChanging="gv_PageIndexChanging"
        RowStyle-CssClass="" AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="CLIENT_REF" HeaderText="Client Ref" />
            <asp:HyperLinkField HeaderText="Client Name" DataTextField="CLIENT_NAME"
                DataNavigateUrlFields="CLIENT_ID" DataNavigateUrlFormatString="ClientSummary.aspx?cid={0}" />
            <asp:BoundField DataField="ADVISER_NAME" HeaderText="Adviser" />
            <asp:BoundField DataField="STATUS_TXT" HeaderText="Status" />
            <asp:BoundField DataField="ACCT_COUNT" HeaderText="No. Accts" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="STMT_DATE" HeaderText="Stmt Date" />
            <asp:BoundField DataField="MKTVAL" HeaderText="Mkt Val (GBP)" ItemStyle-CssClass="num" />
        </Columns>
    </asp:GridView>
    <div class="msg"><b>Total (GBP):</b> <asp:Label ID="lblTotal" runat="server" />
        &nbsp;&nbsp;<asp:Label ID="lblCount" runat="server" /></div>
</asp:Content>
