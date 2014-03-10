<%@ Page Title="Settlements" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Settlements.aspx.cs" Inherits="PV.Web.Settlements" %>
<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Settlements</h3>
    Acct No <b><asp:Label ID="lblAcct" runat="server" /></b>
    &nbsp; Client <b><asp:Label ID="lblClient" runat="server" /></b>
    &nbsp; Safe Code <b><asp:Label ID="lblSafe" runat="server" /></b>
    <br /><br />
    From&nbsp;<asp:TextBox ID="txtFrom" runat="server" Width="80" />
    &nbsp;To&nbsp;<asp:TextBox ID="txtTo" runat="server" Width="80" />
    &nbsp;<asp:Button ID="btnShow" runat="server" Text="Show" OnClick="btnShow_Click" />
    &nbsp;(dd/MM/yyyy)
    <div class="msg"><asp:Label ID="lblMsg" runat="server" /></div>
    <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="false"
        AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="TradeTxt" HeaderText="Trade Date" />
            <asp:BoundField DataField="SettleTxt" HeaderText="Settle Date" />
            <asp:BoundField DataField="Msg" HeaderText="Msg" />
            <asp:BoundField DataField="Dir" HeaderText="Dir" />
            <asp:BoundField DataField="Isin" HeaderText="ISIN" />
            <asp:BoundField DataField="Instrument" HeaderText="Instrument" />
            <asp:BoundField DataField="QtyTxt" HeaderText="Qty" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="AmtTxt" HeaderText="Amount" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="Ccy" HeaderText="Ccy" />
            <asp:BoundField DataField="Seme" HeaderText="Ref" />
        </Columns>
    </asp:GridView>
</asp:Content>
