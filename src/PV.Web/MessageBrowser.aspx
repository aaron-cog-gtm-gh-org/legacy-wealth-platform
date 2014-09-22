<%@ Page Title="Message Browser" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MessageBrowser.aspx.cs" Inherits="PV.Web.MessageBrowser" %>
<asp:Content ID="c1" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Message Browser</h3>
    Type&nbsp;<asp:DropDownList ID="ddlType" runat="server">
        <asp:ListItem Value="">All</asp:ListItem>
        <asp:ListItem Value="MT535">MT535</asp:ListItem>
        <asp:ListItem Value="MT54x">MT54x</asp:ListItem>
    </asp:DropDownList>
    &nbsp; Date&nbsp;<asp:TextBox ID="txtDate" runat="server" Width="80" />
    &nbsp; Safe Code&nbsp;<asp:TextBox ID="txtSafe" runat="server" Width="80" />
    &nbsp;<asp:Button ID="btnList" runat="server" Text="List" OnClick="btnList_Click" />
    <div class="msg"><asp:Label ID="lblMsg" runat="server" /></div>
    <asp:GridView ID="gv" runat="server" CssClass="grid" AutoGenerateColumns="false"
        AlternatingRowStyle-CssClass="alt" AllowPaging="true" PageSize="50"
        OnPageIndexChanging="gv_PageIndexChanging">
        <Columns>
            <asp:HyperLinkField HeaderText="File Name" DataTextField="FileName"
                DataNavigateUrlFields="FileName"
                DataNavigateUrlFormatString="MessageBrowser.aspx?file={0}" />
            <asp:BoundField DataField="MsgType" HeaderText="Msg Type" />
            <asp:BoundField DataField="DateTxt" HeaderText="Date" />
            <asp:BoundField DataField="Size" HeaderText="Size" ItemStyle-CssClass="num" />
        </Columns>
    </asp:GridView>
    <br />
    <asp:Label ID="lblPath" runat="server" />
    <pre class="rawmsg"><asp:Literal ID="litRaw" runat="server" /></pre>
</asp:Content>
