<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TouchbaseGroup.aspx.cs" Inherits="TouchbaseInviteMember.TouchbaseGroup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <div class="container-fluid margin-tp-15 margin-10-lf margin-rt-10">
    <div>   
      <asp:Label ID="lblGroupName" runat="server" Font-Bold="True" ></asp:Label>
    </div>
      </br>
        <div class="left margin-25-rt">
            <span class="label">Member Name</span><asp:RequiredFieldValidator ID="RequiredFieldValidator3"
                runat="server" ControlToValidate="txtMemberName" Display="Dynamic" ValidationGroup="valAddMem"
                ForeColor="#dd4b39" CssClass="redText">mandatory</asp:RequiredFieldValidator><br />
            <asp:TextBox ID="txtMemberName" runat="server" Width="210"></asp:TextBox>
        </div>
        <div class="left margin-25-rt">
            <span class="label">Country</span>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="ddlCountry"
                Display="Dynamic" ValidationGroup="valAddMem" ForeColor="#dd4b39" CssClass="redText"
                InitialValue="0">mandatory</asp:RequiredFieldValidator><br />
            <asp:DropDownList ID="ddlCountry" runat="server" Width="210" AutoPostBack="True"
                OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged">
            </asp:DropDownList>
        </div>
        <div class="left margin-25-rt">
            <span class="label">Mobile No.</span>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtMobileNo"
                ValidationGroup="valAddMem" ForeColor="#dd4b39" CssClass="redText" Display="Dynamic">mandatory</asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator11" runat="server"
                ControlToValidate="txtMobileNo" ErrorMessage="Enter valid Mobile No." ValidationExpression="<%$ AppSettings:OnlyNumeric %>"
                ValidationGroup="valAddMem" ForeColor="#dd4b39" Display="Dynamic" CssClass="redText">
            </asp:RegularExpressionValidator><br />
            <asp:TextBox ID="txtcode" Enabled="False" Width="30" runat="server"></asp:TextBox>
            <asp:TextBox ID="txtMobileNo" runat="server" Width="210"></asp:TextBox>
            <div class="clear">
            </div>
        </div>
        <div class="left">
            <span class="label">Email Id.</span>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtEmail"
                ValidationExpression="<%$ AppSettings:EmailId %>" ValidationGroup="valAddMem"
                ForeColor="#dd4b39" CssClass="redText" Display="Dynamic">Enter valid email Id
            </asp:RegularExpressionValidator>
            <br />
            <asp:TextBox ID="txtEmail" runat="server" Width="210"></asp:TextBox>
            <div class="clear">
            </div>
        </div>
        <div class="clear">
            <asp:Label ID="lblmsg" runat="server" ></asp:Label>
        </div>
        <asp:Button ID="btnAdd" runat="server" Text="Add to Group" CssClass="add-btn" OnClick="btnAdd_Click"
            ValidationGroup="valAddMem" />
        <br />
        <br />
        </div>
    </div>
    </form>
</body>
</html>
