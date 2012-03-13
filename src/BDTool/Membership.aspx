<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Membership.aspx.vb" Inherits="_Default" Title="Membership Editor For IIS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table border="0">
                <tr>
                    <td>
                        <asp:GridView ID="GridViewMemberUser" runat="server" OnSelectedIndexChanged="GridViewMembershipUser_SelectedIndexChanged"
                            OnRowDeleted="GridViewMembership_RowDeleted" AllowPaging="True" AutoGenerateColumns="False"
                            DataKeyNames="UserName" DataSourceID="ObjectDataSourceMembershipUser" AllowSorting="True">
                            <Columns>
                                <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" ShowSelectButton="True" />
                                <asp:BoundField DataField="UserName" HeaderText="UserName" ReadOnly="True" SortExpression="UserName" />
                                <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
                                
                                <asp:BoundField DataField="PasswordQuestion" HeaderText="PasswordQuestion" ReadOnly="True"
                                    SortExpression="PasswordQuestion" />
  
                                <asp:BoundField DataField="Comment" HeaderText="Comment" SortExpression="Comment" />
                                
                                <asp:BoundField DataField="CreationDate" HeaderText="CreationDate" ReadOnly="True"
                                    SortExpression="CreationDate" />
                                <asp:CheckBoxField DataField="IsApproved" HeaderText="IsApproved" SortExpression="IsApproved" />
                                <asp:BoundField DataField="LastLockoutDate" Visible="false" HeaderText="LastLockoutDate" ReadOnly="True"
                                    SortExpression="LastLockoutDate" />
                                <asp:BoundField DataField="LastLoginDate" HeaderText="LastLoginDate" SortExpression="LastLoginDate" />
                                <asp:CheckBoxField DataField="IsOnline" Visible="false" HeaderText="IsOnline" ReadOnly="True" SortExpression="IsOnline" />
                                <asp:CheckBoxField DataField="IsLockedOut" HeaderText="IsLockedOut" ReadOnly="True"
                                    SortExpression="IsLockedOut" Visible="false" />
                                <asp:BoundField DataField="LastActivityDate" HeaderText="LastActivityDate" SortExpression="LastActivityDate" Visible="false" />
                                <asp:BoundField DataField="LastPasswordChangedDate" HeaderText="LastPasswordChangedDate" Visible="false"
                                    ReadOnly="True" SortExpression="LastPasswordChangedDate" />
                                
                                <asp:BoundField DataField="ProviderName" HeaderText="ProviderName" ReadOnly="True" Visible="false"
                                    SortExpression="ProviderName" />
                            </Columns>
                        </asp:GridView>
                        <asp:ObjectDataSource ID="ObjectDataSourceMembershipUser" runat="server" DeleteMethod="Delete"
                            InsertMethod="Insert"  SelectMethod="GetMembers"
                            TypeName="MembershipUtilities.MembershipUserODS" UpdateMethod="Update"
                            SortParameterName="SortData" OnInserted="ObjectDataSourceMembershipUser_Inserted" >
                            <DeleteParameters>
                                <asp:Parameter Name="UserName" Type="String" />
                            </DeleteParameters>
                            <UpdateParameters>
                                <asp:Parameter Name="email" Type="String" />
                                <asp:Parameter Name="isApproved" Type="Boolean" />
                                <asp:Parameter Name="comment" Type="String" />
                                <asp:Parameter Name="lastActivityDate" Type="DateTime" />
                                <asp:Parameter Name="lastLoginDate" Type="DateTime" />
                            </UpdateParameters>
                            <SelectParameters>
                                <asp:Parameter Name="sortData" Type="String" />
                            </SelectParameters>
                            <InsertParameters>
                                <asp:Parameter Name="userName" Type="String" />
                                <asp:Parameter Name="isApproved" Type="Boolean" />
                                <asp:Parameter Name="comment" Type="String" />
                                <asp:Parameter Name="lastLockoutDate" Type="DateTime" />
                                <asp:Parameter Name="creationDate" Type="DateTime" />
                                <asp:Parameter Name="email" Type="String" />
                                <asp:Parameter Name="lastActivityDate" Type="DateTime" />
                                <asp:Parameter Name="providerName" Type="String" />
                                <asp:Parameter Name="isLockedOut" Type="Boolean" />
                                <asp:Parameter Name="lastLoginDate" Type="DateTime" />
                                <asp:Parameter Name="isOnline" Type="Boolean" />
                                <asp:Parameter Name="passwordQuestion" Type="String" />
                                <asp:Parameter Name="lastPasswordChangedDate" Type="DateTime" />
                                <asp:Parameter Name="password" Type="String" />
                                <asp:Parameter Name="passwordAnswer" Type="String" />
                            </InsertParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <table>
                <tr>
                    <td>
                        <asp:GridView ID="GridViewRole" runat="server" AutoGenerateColumns="False" DataSourceID="ObjectDataSourceRoleObject"
                            DataKeyNames="RoleName" CellPadding="3" CellSpacing="3" AllowPaging="True">
                            <Columns>
                                <asp:CommandField ShowDeleteButton="True" DeleteText="Delete Role" />
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="Button1" runat="server" CausesValidation="false" Width="500px" OnClick="ToggleInRole_Click"
                                            Text='<%# ShowInRoleStatus(Eval("UserName"),Eval("RoleName")) %>' />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="NumberOfUsersInRole" HeaderText="Number Of Users In Role"
                                    SortExpression="NumberOfUsersInRole" />
                                
                                <asp:BoundField DataField="RoleName" ReadOnly="True" Visible="False" HeaderText="RoleName"
                                    SortExpression="RoleName" />
                                <asp:CheckBoxField DataField="UserInRole" HeaderText="UserInRole" Visible="False"
                                    SortExpression="UserInRole" />
                            </Columns>
                        </asp:GridView>
                    </td>
                    <td>
                        <asp:CheckBox ID="CheckBoxShowRolesAssigned" runat="server" AutoPostBack="True" Text="Show Roles Assigned Only" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="ButtonCreateNewRole" runat="server" OnClick="ButtonCreateNewRole_Click"
                            Text="Create New Role" />
                        <asp:TextBox ID="TextBoxCreateNewRole" runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <br />
            <asp:Panel ID="PanelCreateUser" runat="server" Height="50px" Width="125px" BorderColor="Black"
                BorderWidth="1px">
                <table cellpadding="3" cellspacing="3">
                    <tr>
                        <td style="height: 32px">
                            <asp:Label ID="Label3" Text="UserName" runat="server"></asp:Label>
                        </td>
                        <td style="height: 32px">
                            <asp:TextBox ID="TextBoxUserName" runat="server"></asp:TextBox>
                        </td>
                        <td style="height: 32px">
                            <asp:Label ID="Label4" Text="Password" runat="server"></asp:Label>
                        </td>
                        <td style="height: 32px">
                            <asp:TextBox ID="TextBoxPassword" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label5" Text="PasswordQuestion" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBoxPasswordQuestion" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label6" Text="PasswordAnswer" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBoxPasswordAnswer" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" Text="Email" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="TextBoxEmail" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="Label9" Text="Approved" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ID="CheckboxApproval" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="ButtonNewUser" runat="server" Text="Create New User" OnClick="ButtonNewUser_Click" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="LabelInsertMessage" runat="server"></asp:Label></asp:Panel>
            
            <asp:ObjectDataSource ID="ObjectDataSourceRoleObject" runat="server" SelectMethod="GetRoles"
                TypeName="MembershipUtilities.RoleDataObject" InsertMethod="Insert" DeleteMethod="Delete"  >
                <SelectParameters>
                    <asp:ControlParameter ControlID="GridViewMemberUser" Name="UserName" PropertyName="SelectedValue"
                        Type="String" />
                    <asp:ControlParameter ControlID="CheckBoxShowRolesAssigned" Name="ShowOnlyAssignedRolls"
                        PropertyName="Checked" Type="Boolean" />
                </SelectParameters>
                <InsertParameters>
                    <asp:Parameter Name="RoleName" Type="String" />
                </InsertParameters>
                <DeleteParameters>
                    <asp:Parameter Name="RoleName" Type="String" />
                </DeleteParameters>
            </asp:ObjectDataSource>
        </div>
    </form>
</body>
</html>
