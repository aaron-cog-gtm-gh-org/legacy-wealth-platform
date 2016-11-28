<%@ Page Title="Position Check" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PositionCheck.aspx.cs" Inherits="PV.Web.PositionCheck" %>
<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Position Check</h3>
    Account&nbsp;<asp:DropDownList ID="ddlAcct" runat="server" AutoPostBack="true"
        OnSelectedIndexChanged="ddlAcct_Changed" />
    &nbsp; Stmt Date&nbsp;<asp:DropDownList ID="ddlDate" runat="server" />
    &nbsp;<asp:Button ID="btnRun" runat="server" Text="Run Check" OnClick="btnRun_Click" />
    <br /><br />
    Check All Accounts&nbsp;for&nbsp;<asp:DropDownList ID="ddlAllDate" runat="server" />
    &nbsp;<asp:Button ID="btnAll" runat="server" Text="Check All" OnClick="btnAll_Click" />
    <div class="msg"><asp:Label ID="lblMsg" runat="server" /></div>

    <asp:GridView ID="gvAll" runat="server" CssClass="grid" AutoGenerateColumns="false"
        AlternatingRowStyle-CssClass="alt">
        <Columns>
            <asp:BoundField DataField="AcctNo" HeaderText="Acct No" />
            <asp:BoundField DataField="Client" HeaderText="Client" />
            <asp:BoundField DataField="Diffs" HeaderText="Differences" ItemStyle-CssClass="num" />
            <asp:HyperLinkField Text="View" DataNavigateUrlFields="LinkAcct,LinkDate"
                DataNavigateUrlFormatString="PositionCheck.aspx?acct={0}&stmt={1}" />
        </Columns>
    </asp:GridView>

    <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="false"
        AlternatingRowStyle-CssClass="alt" DataKeyNames="Isin"
        OnRowEditing="gv_RowEditing" OnRowUpdating="gv_RowUpdating" OnRowCancelingEdit="gv_RowCancel">
        <Columns>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                    <asp:Image ID="imgWarn" runat="server" ImageUrl="~/Images/warn.gif" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Isin" HeaderText="ISIN" ReadOnly="true" />
            <asp:BoundField DataField="Instrument" HeaderText="Instrument" ReadOnly="true" />
            <asp:BoundField DataField="PrevQtyTxt" HeaderText="Prev Qty" ReadOnly="true" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="NetTxt" HeaderText="Net Settled" ReadOnly="true" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="ExpTxt" HeaderText="Expected" ReadOnly="true" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="StmtTxt" HeaderText="Stmt Qty" ReadOnly="true" ItemStyle-CssClass="num" />
            <asp:BoundField DataField="DiffTxt" HeaderText="Diff" ReadOnly="true" ItemStyle-CssClass="num" />
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate><%# Eval("Status") %></ItemTemplate>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlStatus" runat="server">
                        <asp:ListItem>OPEN</asp:ListItem>
                        <asp:ListItem>CHASED</asp:ListItem>
                        <asp:ListItem>RESOLVED</asp:ListItem>
                        <asp:ListItem>WRITEOFF</asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Note">
                <ItemTemplate><%# Eval("Note") %></ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtNote" runat="server" TextMode="MultiLine" Rows="2" Width="180"
                        Text='<%# Eval("Note") %>' />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="UpdatedBy" HeaderText="Updated By" ReadOnly="true" />
            <asp:BoundField DataField="UpdatedTxt" HeaderText="Updated" ReadOnly="true" />
            <asp:CommandField ShowEditButton="true" ButtonType="Button" />
        </Columns>
    </asp:GridView>
</asp:Content>
