<%@ Page Language="VB" AutoEventWireup="true" CodeFile="PipelineUpdates.aspx.vb" Inherits="_Default" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pipeline Updates</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"><Scripts><asp:ScriptReference Path="~/Scripts/Safari3AjaxHack.js" /></Scripts></asp:ScriptManager>
        <div id="header" >
            <span style="text-align:right;float:right"><asp:LoginName ID="LoginName1" runat="server" FormatString="Welcome, {0}" />&nbsp;<asp:LoginStatus ID="LoginStatus1" runat="server" LogoutPageUrl="login.aspx" LogoutAction="RedirectToLoginPage" /></span>
            <asp:Image ID="imgLogo" runat="server" skinid="mainlogo" ImageUrl="~/commonimages/logo.gif" /><br/>
        </div>
        <div id="leftcol" >
            <uc3:Navigation ID="Navigation1" runat="server" Location="Default"/>
        </div>
        <div id="content" >
            <asp:UpdatePanel runat="server" ID="pnlOpportunities" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <ew:MultiTextListBox runat="server" ID="lstOpportunities" DataSourceid="ODS_Opportunities" DataValueField="PK_OpportunityID" DataTextFields="Client, OpportunityName" DataTextFormatString="{0:g}          {1:g}" SelectionMode="Single" Rows="30" AutoPostBack="true"/><br />
                    <asp:CheckBox runat="server" Checked="true" ID="ckOnlyWorking" Text="Show only the working pipeline opportunities" AutoPostBack="true"/><br />
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <asp:FormView ID="FormView1" runat="server" 
                        DataSourceID="ODS_SingleOpportunity" DefaultMode="Edit" Width="99%">
                        <EditItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text="Opportunity Name"></asp:Label><br />
                            <asp:Textbox ID="txtName" runat="server" Text='<%# Bind("OpportunityName") %>' Width="99%" Enabled="false"/><br />
                            <asp:Label ID="Label2" runat="server" Text="Hunted or Farmed"></asp:Label><br />
                            <asp:DropDownList ID="cboSource" runat="server" DataValueField='<%# Bind("Source") %>' OnDataBound="SetUpInitialEditValue" Enabled="false">
                                <asp:ListItem Text="" Value="" Selected="True"/>
                                <asp:ListItem Text="Hunted" Value="Hunted"/>
                                <asp:ListItem Text="Farmed" Value="Farmed"/>
                            </asp:DropDownList><br />
                            <asp:Label ID="Label3" runat="server" Text="Centric Fit"></asp:Label><br />
                            <asp:DropDownList ID="cboFit" runat="server" DataValueField='<%# Bind("Fit") %>' OnDataBound="SetUpInitialEditValue" Enabled="false">
                                <asp:ListItem Text="" Value="" Selected="True"/>
                                <asp:ListItem Text="H" Value="H"/>
                                <asp:ListItem Text="M" Value="M"/>
                                <asp:ListItem Text="L" Value="L"/>
                            </asp:DropDownList><br />
                            <asp:CheckBox runat="server" ID="ckExtension" Text="Extension?" Checked='<%# IIf(Eval("Extension") Is DBNull.Value, "False", Eval("Extension")) %>' Enabled="false"/><br />
                            <asp:Label ID="Label4" runat="server" Text="Updates on this Opportunity"></asp:Label><br />
                            <asp:Label ID="lblWarning" runat="server" Text="A New Close Date is Needed!" Font-Bold="true" ForeColor="Red" Visible="false"></asp:Label><br />
                            <asp:GridView ID="grdOpportunityUpdate" runat="server" AutoGenerateColumns="False" 
                            DataSourceID="ODS_OpportunityUpdate" ShowFooter="True" DataKeyNames="PK_OpportunityUpdateID" 
                            Width="98%" Visible="True" EmptyDataText="No Updates Yet" Caption="Opportunity Updates" OnRowCommand="grdOpportunityUpdate_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="PK_OpportunityUpdateID" HeaderText="PK_OpportunityUpdateID ID" Visible="False"/>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" HeaderText="Update Date" SortExpression="UpdateDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUpdateDate" runat="server" Text='<%# Bind("UpdateDate","{0:d}") %>' CssClass="DateTextBox"  />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Textbox ID="txtUpdateDate" runat="server" Text='<%# Now()%>' CssClass="DateTextBox" Visible="false"/>
                                            <cc1:CalendarExtender ID="ccUpdateDate" runat="server" TargetControlID="txtUpdateDate" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" HeaderText="Close Date" SortExpression="OpportunityCloseDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOpportunityCloseDate" runat="server" Text='<%# Eval("OpportunityCloseDate","{0:d}") %>' CssClass="DateTextBox"/>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Textbox ID="txtOpportunityCloseDate" runat="server" Text='<%# Bind("OpportunityCloseDate") %>' CssClass="DateTextBox"/>
                                            <cc1:CalendarExtender ID="ccCloseDate" runat="server" TargetControlID="txtOpportunityCloseDate" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderText="Win %" SortExpression="WinPercentage" FooterStyle-HorizontalAlign="Right" >
                                        <ItemTemplate>
                                            <asp:Label ID="lblWinPercentage" runat="server" Text='<%# Bind("WinPercentage","{0:p0}") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Textbox ID="txtWinPercentage" runat="server" Text='<%# Bind("WinPercentage") %>' CssClass="DateTextBox" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderText="Est. Revenue" SortExpression="EstimatedRevenue" FooterStyle-HorizontalAlign="Right" >
                                        <ItemTemplate>
                                            <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# Bind("EstimatedRevenue","{0:c0}") %>' CssClass="DateTextBox" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Textbox ID="txtEstimatedRevenue" runat="server" Text='<%# Bind("EstimatedRevenue") %>' CssClass="DateTextBox" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Next Steps" SortExpression="NextSteps">
                                        <ItemTemplate>
                                            <asp:Label ID="lblNextSteps" runat="server" Text='<%# Bind("NextSteps") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Textbox ID="txtNextSteps" runat="server" Text='<%# Bind("NextSteps") %>' />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Other Notes" SortExpression="UpdateNotes">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUpdateNotes" runat="server" Text='<%# Bind("UpdateNotes") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Textbox ID="txtUpdateNotes" runat="server" Text='<%# Bind("UpdateNotes") %>' />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Update Person" SortExpression="UpdatePerson">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUpdatePerson" runat="server" Text='<%# Bind("UpdatePerson") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Textbox ID="txtUpdatePerson" runat="server" Text='<%# Bind("UpdatePerson") %>' />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <FooterTemplate>
                                            <asp:ImageButton ID="lnkInsertUpdate" runat="server" ImageUrl="~/CommonImages/Insert.GIF" AlternateText="Insert New Update" CommandName="INSERT"/>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                               </Columns>
                            </asp:GridView>
                        </EditItemTemplate>                        
                    </asp:FormView>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="lstOpportunities" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_Opportunities" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetOpportunities" TypeName="cOpportunity">
        <SelectParameters>
            <asp:ControlParameter ControlID="ckOnlyWorking" DefaultValue="True" Name="bWorkingOnly" PropertyName="Checked" Type="Boolean" />
        </SelectParameters>
    </asp:ObjectDataSource> 
    <asp:ObjectDataSource ID="ODS_SingleOpportunity" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetSingleOpportunities" TypeName="cOpportunity">
        <SelectParameters>
            <asp:ControlParameter ControlID="lstOpportunities" DefaultValue="-1" 
                Name="PK_OpportunityID" PropertyName="SelectedValue" Type="Double" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="ODS_OpportunityUpdate" runat="server" 
            OldValuesParameterFormatString="original_{0}" 
            SelectMethod="GetOpportunityUpdates" TypeName="cOpportunity" 
            InsertMethod="InsertSingleOpportunityUpdate">
        <SelectParameters>
            <asp:ControlParameter ControlID="lstOpportunities" Name="FK_OpportunityID" PropertyName="SelectedValue" Type="Double" />
        </SelectParameters>
        <InsertParameters>
            <asp:Parameter Name="UpdateDate" Type="DateTime" />
            <asp:Parameter Name="OpportunityCloseDate" Type="DateTime" />
            <asp:Parameter Name="WinPercentage" Type="Double" />
            <asp:Parameter Name="NextSteps" Type="String" />
            <asp:Parameter Name="EstimatedRevenue" Type="Double" />
            <asp:Parameter Name="UpdateNotes" Type="String" />
            <asp:Parameter Name="UpdatePerson" Type="String" />
            <asp:Parameter Name="FK_OpportunityID" Type="Double" />
            <asp:Parameter Name="NewItemID" Type="Double" />
        </InsertParameters>
        </asp:ObjectDataSource>
    </form>
</body>
</html>
