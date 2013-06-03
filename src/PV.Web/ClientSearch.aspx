<%@ Page Title="Client Search" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClientSearch.aspx.cs" Inherits="PV.Web.ClientSearch" %>
<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Client Search</h3>
    Name or Client Ref&nbsp;<asp:TextBox ID="txtSearch" runat="server" Width="200" />
    &nbsp;<asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
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
        </Columns>
    </asp:GridView>
</asp:Content>
