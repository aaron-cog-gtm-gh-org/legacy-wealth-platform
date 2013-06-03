<%@ Page Title="Holdings" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Holdings.aspx.cs" Inherits="PV.Web.Holdings" %>
<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Holdings</h3>
    Acct No <b><asp:Label ID="lblAcct" runat="server" /></b>
    &nbsp; Client <b><asp:Label ID="lblClient" runat="server" /></b>
    &nbsp; Safe Code <b><asp:Label ID="lblSafe" runat="server" /></b>
    <br /><br />
    Stmt Date&nbsp;<asp:DropDownList ID="ddlDate" runat="server" AutoPostBack="true"
        OnSelectedIndexChanged="ddlDate_Changed" />
    <div class="msg"><asp:Label ID="lblMsg" runat="server" /></div>
    <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="false"
        AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="Isin" HeaderText="ISIN" />
            <asp:BoundField DataField="Instrument" HeaderText="Instrument" />
            <asp:BoundField DataField="QtyTxt" HeaderText="Qty" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="PxTxt" HeaderText="Px" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="Ccy" HeaderText="Ccy" />
            <asp:BoundField DataField="MvTxt" HeaderText="Mkt Val" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="MvGbpTxt" HeaderText="Mkt Val (GBP)" ItemStyle-CssClass="num" />
        </Columns>
    </asp:GridView>
    <div class="msg"><b>Total Mkt Val (GBP):</b> <asp:Label ID="lblTotal" runat="server" /></div>
</asp:Content>
