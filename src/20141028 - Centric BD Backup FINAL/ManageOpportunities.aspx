<%@ Page Language="VB" AutoEventWireup="true" CodeFile="ManageOpportunities.aspx.vb" Inherits="ManageOpportunities"%>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Opportunities</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/Safari3AjaxHack.js" />
                <asp:ScriptReference Path="~/Scripts/majax.js" />
                <asp:ScriptReference Path="~/Scripts/majax.maintainScrollPosition.js" />
            </Scripts>
        </asp:ScriptManager>
        <uc7:MyModal ID="myModalControl" runat="server" />
        <div id="header" >
            <span style="text-align:right;float:right"><asp:LoginName ID="LoginName1" runat="server" FormatString="Welcome, {0}" />&nbsp;<asp:LoginStatus ID="LoginStatus1" runat="server" LogoutPageUrl="login.aspx" LogoutAction="RedirectToLoginPage" /></span>
            <asp:Image ID="imgLogo" runat="server" skinid="mainlogo" ImageUrl="~/commonimages/logo1.jpg" /><br/>
        </div>
        <div id="leftcol" >
            <uc3:Navigation ID="Navigation1" runat="server" Location="Default"/>
        </div>
        <div id="content" >
            <div id="help" style="float:right;text-align:right">
                <asp:HyperLink runat="server" ID="HyperLink1" NavigateUrl="~/Help/NewOpportunity/NewOpportunity.html" Target="_blank">Help with entering a new Opportunity</asp:HyperLink><br />
                <asp:HyperLink runat="server" ID="HyperLink2" NavigateUrl="~/Help/UpdateOpportunity/UpdateOpportunity.html" Target="_blank">Help with Updating an Opportunity</asp:HyperLink><br />
                <asp:HyperLink runat="server" ID="helplink" NavigateUrl="~/Help/AdvancedForecasting/AdvancedForecasting.html" Target="_blank">Help with Advanced Forecasting</asp:HyperLink>
            </div>
            
            <asp:UpdatePanel runat="server" ID="pnlOpportunities" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <uc1:BU ID="BU1" runat="server"/>
                    <div style="float:left;">
                        <div style="width:65%;float:inherit;">
                            <asp:Label ID="lblKeepRed" runat="server" visible="false"/>
                            <asp:Label ID="lblHighlightYellow" runat="server" visible="false"/>
                            <asp:Label ID="lblLost" runat="server" visible="false"/>
                            <asp:Label ID="lblWon" runat="server" visible="false"/>
                            <ew:MultiTextListBox runat="server" ID="lstOpportunities" DataSourceid="ODS_Opportunities" DataValueField="PK_OpportunityID" DataTextFields="Client, OpportunityName,UpdateDate" DataTextFormatString="{0:g} {1:g}" SelectionMode="Single" Rows="20" AutoPostBack="true" Width="100%" class="maintain-scroll"/>
                        </div>
                        <div style="width:35%;float:inherit;">
                            <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="true">
                                <asp:ListItem Text="Working Pipeline" Value="Working Pipeline" Selected="True"/>
                                <asp:ListItem Text="Needing Updates" Value="Needing Updates" />
                                <asp:ListItem Text="Unqualified" Value="Un-qualified" />
                                <asp:ListItem Text="Include Win/Loss in Past 7 Days" Value="WinLoss" />
                                <asp:ListItem Text="All" Value="All" />
                            </asp:RadioButtonList>
                            <asp:CheckBox ID="ckExcludeExtensions" runat="server" Checked="false" AutoPostBack="true" Text="Filter out extensions"/>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="true" UpdateMode="Always">
                <ContentTemplate>
                <div style="clear:left">
                    <asp:FormView ID="FormView1" runat="server" DataSourceID="ODS_SingleOpportunity" Width="99%" >
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text="Opportunity Name"></asp:Label><br />
                            <asp:Textbox ID="txtName" runat="server" Text='<%# Bind("OpportunityName") %>' Width="90%" Enabled="false"/><br />
                            <div style="float:left;clear:right;width:90%">
                                <div>
                                    <div style="width:20%;float:left"><asp:Label ID="Label2" runat="server" Text="Hunted or Farmed"/></div>
                                    <div style="width:20%;float:left;clear:right"><asp:Label ID="Label3" runat="server" Text="Centric Fit"/></div>
                                    <div style="width:20%;float:left"><asp:Label ID="Label4" runat="server" Text="Opportunity Owner"/></div>
                                    <div style="width:20%;float:left">&nbsp;</div>
                                    <div style="width:20%;float:left"><asp:label ID="lblRolesNeeded" runat="server" Text="Roles Needed" /></div>
                                </div>
                                <div style="float:left;clear:both;width:90%">
                                    <div style="width:20%;float:left">
                                        <asp:DropDownList ID="cboSource" runat="server" DataValueField='<%# Bind("Source") %>' OnDataBound="SetUpInitialEditValue" Enabled="false">
                                            <asp:ListItem Text="" Value="" Selected="True"/>
                                            <asp:ListItem Text="Hunted" Value="Hunted"/>
                                            <asp:ListItem Text="Farmed" Value="Farmed"/>
                                        </asp:DropDownList>
                                    </div>
                                    <div style="width:20%;float:left">                                  
                                        <asp:DropDownList ID="cboFit" runat="server" DataValueField='<%# Bind("Fit") %>' OnDataBound="SetUpInitialEditValue" Enabled="false">
                                            <asp:ListItem Text="" Value="" Selected="True"/>
                                            <asp:ListItem Text="H" Value="H"/>
                                            <asp:ListItem Text="M" Value="M"/>
                                            <asp:ListItem Text="L" Value="L"/>
                                        </asp:DropDownList>
                                    </div>
                                    <div style="width:20%;float:left">
                                        <asp:TextBox runat="server" ID="Owner" Text='<%# Bind("OpportunityOwner") %>' Enabled="false"/>
                                    </div>
                                    <div style="width:20%;float:left">
                                        <asp:CheckBox runat="server" ID="ckExtension" Text="Extension?" Checked='<%# IIf(Eval("Extension") Is DBNull.Value, "False", Eval("Extension")) %>' Enabled="false"/><br />
                                    </div>                                        
                                    <div style="width:20%;float:left">
                                        <asp:textbox ID="txtRolesNeeded" runat="server" Text='<%# Bind("RolesNeeded") %>' enabled="false"/>
                                    </div>
                                </div>
                            </div>
                            <div style="float:left;clear:right;width:90%">
                                <div>
                                    <div style="width:20%;float:left"><asp:Label ID="Label8" runat="server" Text="RFP Required?"/></div>
                                    <div style="width:20%;float:left;clear:right"><asp:Label ID="Label9" runat="server" Text="RFP Lead"/></div>
                                    <div style="width:20%;float:left"><asp:Label ID="Label10" runat="server" Text="RFP Risk Assessment Location"/></div>
                                </div>
                            </div>
                            <div style="float:left;clear:right;width:90%">
                                <div>
                                    <div style="width:20%;float:left"><asp:Checkbox runat="server" ID="ckRFPRequired" Checked='<%# IIf(Eval("RFPRequired") Is DBNull.Value, "False", Eval("RFPRequired")) %>' Enabled="false"/></div>
                                    <div style="width:20%;float:left"><asp:TextBox runat="server" ID="txtRFPLead" Text='<%# Bind("RFPLead") %>' Enabled="false"/></div>
                                    <div style="width:20%;float:left"><asp:Hyperlink runat="server" ID="txtRFPRiskAssessment" NavigateUrl='<%# Bind("RFPRiskAssessment") %>' Text='<%# Bind("RFPRiskAssessment") %>' Enabled="true"/></div>
                                </div>
                            </div>
                            <div style="clear:both;width:99%">
                                <asp:Button ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit  Opportunity - Not the Updates"/>
                                <asp:Button ID="NewButton" runat="server" CausesValidation="False" CommandName="New" Text="New Opportunity" />
                            </div>
                            <hr />
                            <asp:Label ID="lblWarning" runat="server" Text="A New Close Date is Needed!" Font-Bold="true" ForeColor="Red" Visible="false"></asp:Label><br />
                            <asp:GridView ID="grdOpportunityUpdate" runat="server" AutoGenerateColumns="False" 
                            DataSourceID="ODS_OpportunityUpdate" ShowFooter="True" DataKeyNames="PK_OpportunityUpdateID" 
                            Width="95%" Visible="True" EmptyDataText="No Updates Yet" Caption="Opportunity Updates" OnRowCommand="grdOpportunityUpdate_RowCommand" OnDataBound="ShowFooterCode" OnRowDataBound="grdOpportunityUpdate_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="PK_OpportunityUpdateID" HeaderText="PK_OpportunityUpdateID ID" Visible="False"/>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" HeaderText="Update Date" SortExpression="UpdateDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUpdateDate" runat="server" Text='<%# Bind("UpdateDate","{0:d}") %>' CssClass="DateTextBox"  />
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <asp:Label runat="server" ID="lblUpdateDate" Text="Update Date" />
                                            <asp:Textbox ID="txtUpdateDate" runat="server" Text='<%# Now()%>' CssClass="DateTextBox" Visible="false"/>
                                            <cc1:CalendarExtender ID="ccUpdateDate" runat="server" TargetControlID="txtUpdateDate" />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" HeaderText="Close Date" SortExpression="OpportunityCloseDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblOpportunityCloseDate" runat="server" Text='<%# Eval("OpportunityCloseDate","{0:d}") %>' CssClass="DateTextBox"/>
                                        </ItemTemplate>
                                        <HeaderStyle Width="6em" />
                                        <ItemStyle Width="6em" />
                                        <HeaderTemplate>
                                            <asp:Label runat="server" ID="txtCloseDate" Text="Close Date" /><br />
                                            <asp:Textbox ID="txtOpportunityCloseDate" runat="server" Text='<%# Bind("OpportunityCloseDate") %>' CssClass="DateTextBox" Visible="true" skinid="Watermark" Width="95%"/>
                                            <cc1:CalendarExtender ID="ccCloseDate" runat="server" TargetControlID="txtOpportunityCloseDate" />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderText="Win %" SortExpression="WinPercentage" FooterStyle-HorizontalAlign="Right" >
                                        <HeaderStyle Width="6em" />
                                        <ItemStyle Width="6em" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblWinPercentage" runat="server" Text='<%# Bind("WinPercentage","{0:p0}") %>' />
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <asp:Label runat="server" ID="lblWinPercentage" Text="Win %" /><br />
                                            <asp:Textbox ID="txtWinPercentage" runat="server" Text='<%# Bind("WinPercentage") %>' CssClass="DateTextBox" skinid="Watermark" Width="95%"/>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderText="Est. Revenue" SortExpression="EstimatedRevenue" FooterStyle-HorizontalAlign="Right" >
                                        <HeaderStyle Width="10em" />
                                        <ItemStyle Width="10em" />
                                        <ItemTemplate>
                                            <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# Bind("EstimatedRevenue","{0:c0}") %>' CssClass="DateTextBox" />
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <asp:Label runat="server" ID="lblEstimatedRevenue" Text="Est. Revenue" /><br />
                                            <asp:Textbox ID="txtEstimatedRevenue" runat="server" Text='<%# Bind("EstimatedRevenue") %>' CssClass="DateTextBox" skinid="Watermark" Width="95%"/>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Next Steps" SortExpression="NextSteps">
                                        <ItemTemplate>
                                            <asp:Label ID="lblNextSteps" runat="server" Text='<%# Bind("NextSteps") %>' />
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <asp:Label runat="server" ID="lblNextSteps" Text="Next Steps" /><br />
                                            <asp:Textbox ID="txtNextSteps" runat="server" Text='<%# Bind("NextSteps") %>' />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Other Notes" SortExpression="UpdateNotes" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUpdateNotes" runat="server" Text='<%# Bind("UpdateNotes") %>' />
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <asp:Label runat="server" ID="lblUpdateNotes" Text="Other Notes" /><br />
                                            <asp:Textbox ID="txtUpdateNotes" runat="server" Text='<%# Bind("UpdateNotes") %>' />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Update Person" SortExpression="UpdatePerson">
                                        <ItemTemplate>
                                            <asp:Label ID="lblUpdatePerson" runat="server" Text='<%# Bind("UpdatePerson") %>' />
                                        </ItemTemplate>
                                        <HeaderTemplate>
                                            <asp:Label runat="server" ID="lblUpdatePerson" Text="Update Person" /><br />
                                            <asp:Textbox ID="txtUpdatePerson" runat="server" Text='<%# Bind("UpdatePerson") %>' />
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:ImageButton ID="lnkInsertUpdate" runat="server" ImageUrl="~/CommonImages/Insert.GIF" AlternateText="Insert New Update" CommandName="INSERT"/>
                                        </HeaderTemplate>
                                    </asp:TemplateField>
                               </Columns>
                            </asp:GridView>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <ew:MultiTextDropDownList runat="server" ID="cboClients" DataSourceID="ODS_Clients" DataValueField="ClientID" DataTextField="Client" OnDataBound="SetUpInitialEditValue"/><br />
                            <asp:Label ID="Label1" runat="server" Text="Opportunity Name"></asp:Label><br />
                            <asp:Textbox ID="txtPK_OpportunityID" runat="server" Text='<%# Bind("PK_OpportunityID") %>' Visible="false"/>
                            <asp:Textbox ID="txtName" runat="server" Text='<%# Bind("OpportunityName") %>' Width="90%" Enabled="true"/><br />
                            <div style="float:left;clear:right;width:90%">
                                <div>
                                    <div style="width:20%;float:left"><asp:Label ID="Label2" runat="server" Text="Hunted or Farmed"/></div>
                                    <div style="width:20%;float:left;clear:right"><asp:Label ID="Label3" runat="server" Text="Centric Fit"/></div>
                                    <div style="width:20%;float:left"><asp:Label ID="Label4" runat="server" Text="Opportunity Owner"/></div>
                                    <div style="width:20%;float:left">&nbsp;</div>
                                    <div style="width:20%;float:left"><asp:label ID="lblRolesNeeded" runat="server" Text="Roles Needed" /></div>
                                </div>
                                <div style="float:left;clear:both;width:90%">
                                    <div style="width:20%;float:left">
                                        <asp:DropDownList ID="cboSource" runat="server" DataValueField='<%# Bind("Source") %>' OnDataBound="SetUpInitialEditValue" Enabled="true">
                                            <asp:ListItem Text="" Value="" Selected="True"/>
                                            <asp:ListItem Text="Hunted" Value="Hunted"/>
                                            <asp:ListItem Text="Farmed" Value="Farmed"/>
                                        </asp:DropDownList>
                                    </div>
                                    <div style="width:20%;float:left">                                  
                                        <asp:DropDownList ID="cboFit" runat="server" DataValueField='<%# Bind("Fit") %>' OnDataBound="SetUpInitialEditValue" Enabled="true">
                                            <asp:ListItem Text="" Value="" Selected="True"/>
                                            <asp:ListItem Text="H" Value="H"/>
                                            <asp:ListItem Text="M" Value="M"/>
                                            <asp:ListItem Text="L" Value="L"/>
                                        </asp:DropDownList>
                                    </div>
                                    <div style="width:20%;float:left">
                                        <asp:TextBox runat="server" ID="Owner" Text='<%# Bind("OpportunityOwner") %>' />
                                    </div>
                                    <div style="width:20%;float:left">
                                        <asp:CheckBox runat="server" ID="ckExtension" Text="Extension?" Checked='<%# IIf(Eval("Extension") Is DBNull.Value, "False", Eval("Extension")) %>' Enabled="true"/><br />
                                    </div>                                        
                                    <div style="width:20%;float:left">
                                        <asp:textbox ID="txtRolesNeeded" runat="server" Text='<%# Bind("RolesNeeded") %>' enabled="true"/>
                                    </div>
                                </div>
                                <div style="float:left;clear:right;width:90%">
                                    <div>
                                        <div style="width:20%;float:left"><asp:Label ID="Label8" runat="server" Text="RFP Required?"/></div>
                                        <div style="width:20%;float:left;clear:right"><asp:Label ID="Label9" runat="server" Text="RFP Lead"/></div>
                                        <div style="width:20%;float:left"><asp:Label ID="Label10" runat="server" Text="RFP Risk Assessment Location"/></div>
                                    </div>
                                 </div>
                                 <div style="float:left;clear:right;width:90%">
                                    <div>
                                        <div style="width:20%;float:left"><asp:Checkbox runat="server" ID="ckRFPRequired" Checked='<%# IIf(Eval("RFPRequired") Is DBNull.Value, "False", Eval("RFPRequired")) %>' /></div>
                                        <div style="width:20%;float:left"><asp:TextBox runat="server" ID="txtRFPLead" Text='<%# Bind("RFPLead") %>' /></div>
                                        <div style="width:20%;float:left"><asp:textbox runat="server" ID="txtRFPRiskAssessment" Text='<%# Bind("RFPRiskAssessment") %>' /></div>
                                    </div>
                                </div>
                            </div>
                           
                            <div style="clear:both;width:99%">
                                <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="Update" Text="Save" />
                                <asp:Button ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"/>
                            </div>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <ew:MultiTextDropDownList runat="server" ID="cboClients" DataSourceID="ODS_Clients" DataValueField="ClientID" DataTextField="Client" OnDataBound="SetUpInitialEditValue"/><br />
                            <asp:Label ID="Label1" runat="server" Text="Opportunity Name"></asp:Label><br />
                            <asp:Textbox ID="txtName" runat="server" Text='<%# Bind("OpportunityName") %>' Width="90%" Enabled="true"/><br />
                            <div style="float:left;clear:right;width:90%">
                                <div style="float:left;clear:both;width:90%">
                                    <div style="width:20%;float:left"><asp:Label ID="Label4" runat="server" Text="Opportunity Owner"/></div>
                                    <div style="width:20%;float:left"><asp:Label ID="Label2" runat="server" Text="Hunted or Farmed"/></div>
                                    <div style="width:20%;float:left;clear:right"><asp:Label ID="Label3" runat="server" Text="Centric Fit"/></div>
                                    <div style="width:20%;float:left">&nbsp;</div>
                                    <div style="width:20%;float:left"><asp:label ID="lblRolesNeeded" runat="server" Text="Roles Needed" /></div>
                                </div>
                                <div style="float:left;clear:both;width:90%">
                                    <div style="width:20%;float:left">
                                        <asp:TextBox runat="server" ID="Owner" Text='<%# Bind("OpportunityOwner") %>' />
                                    </div>
                                    <div style="width:20%;float:left">
                                        <asp:DropDownList ID="cboSource" runat="server" DataValueField='<%# Bind("Source") %>' OnDataBound="SetUpInitialEditValue" Enabled="true">
                                            <asp:ListItem Text="" Value="" Selected="True"/>
                                            <asp:ListItem Text="Hunted" Value="Hunted"/>
                                            <asp:ListItem Text="Farmed" Value="Farmed"/>
                                        </asp:DropDownList>
                                    </div>
                                    <div style="width:20%;float:left">                                  
                                        <asp:DropDownList ID="cboFit" runat="server" DataValueField='<%# Bind("Fit") %>' OnDataBound="SetUpInitialEditValue" Enabled="true">
                                            <asp:ListItem Text="" Value="" Selected="True"/>
                                            <asp:ListItem Text="H" Value="H"/>
                                            <asp:ListItem Text="M" Value="M"/>
                                            <asp:ListItem Text="L" Value="L"/>
                                        </asp:DropDownList>
                                    </div>
                                    
                                    <div style="width:20%;float:left">
                                        <asp:CheckBox runat="server" ID="ckExtension" Text="Extension?" Checked='<%# IIf(Eval("Extension") Is DBNull.Value, "False", Eval("Extension")) %>' Enabled="true"/><br />
                                    </div>
                                    <div style="width:20%;float:left">
                                        <asp:textbox ID="txtRolesNeeded" runat="server" Text='<%# Bind("RolesNeeded") %>' enabled="true"/>
                                    </div> 
                                </div>
                            </div><br />
                            <div style="float:left;clear:both;width:90%">
                                <div style="width:20%;float:left"><asp:Label runat="server" ID="Label5" Text="Close Date" /></div>
                                <div style="width:20%;float:left"><asp:Label runat="server" ID="lbl1" Text="Win Percentage" /></div>
                                <div style="width:20%;float:left"><asp:Label ID="Label7" runat="server" Text="Estimated Revenue"/></div>
                                <div style="width:20%;float:left"><asp:Label runat="server" ID="Label6" Text="Updating Person" /></div>
                            </div><br />
                            <div style="float:left;clear:both;width:90%">
                                <span style="width:20%;float:left"><asp:TextBox runat="server" ID="txtCloseDate" CssClass="DateTextBox" Width="80%"/><cc1:CalendarExtender ID="ccUpdateDate_Insert" runat="server" TargetControlID="txtCloseDate" /></span>
                                <span style="width:20%;float:left;"><asp:TextBox runat="server" ID="txtWinPercentage" Width="80%" CssClass="DateTextBox"/>%</span>
                                <div style="width:20%;float:left"><asp:Textbox runat="server" ID="txtEstimatedRevenue" Text="0" Enabled="true" CssClass="DateTextBox"/></div> 
                                <span style="width:20%;float:left"><asp:TextBox runat="server" ID="txtUpdatePerson" Width="80%"/></span>
                            </div><br />
                            <div style="float:left;clear:both;width:90%">
                                <div style="width:40%;float:left"><asp:Label runat="server" ID="lbltxtNextSteps" Text="Next Steps" /></div>
                            </div><br />
                            <div style="float:left;clear:both;width:90%">
                                <asp:TextBox runat="server" ID="txtNextSteps" Text='<%# Bind("NextSteps") %>' Width="90%" />
                            </div><br />
                            <div style="float:left;clear:right;width:90%">
                                    <div>
                                        <div style="width:20%;float:left"><asp:Label ID="Label8" runat="server" Text="RFP Required?"/></div>
                                        <div style="width:20%;float:left;clear:right"><asp:Label ID="Label9" runat="server" Text="RFP Lead"/></div>
                                        <div style="width:20%;float:left"><asp:Label ID="Label10" runat="server" Text="RFP Risk Assessment Location"/></div>
                                    </div>
                                </div>
                                <div style="float:left;clear:right;width:90%">
                                    <div>
                                        <div style="width:20%;float:left"><asp:Checkbox runat="server" ID="ckRFPRequired" Checked='<%# IIf(Eval("RFPRequired") Is DBNull.Value, "False", Eval("RFPRequired")) %>' /></div>
                                        <div style="width:20%;float:left"><asp:TextBox runat="server" ID="txtRFPLead" Text='<%# Bind("RFPLead") %>' /></div>
                                        <div style="width:20%;float:left"><asp:textbox runat="server" ID="txtRFPRiskAssessment" Text='<%# Bind("RFPRiskAssessment") %>' /></div>
                                    </div>
                                </div>
                            <div style="clear:both;width:99%">
                                <asp:Button ID="InsertButton" runat="server" CausesValidation="True" CommandName="Insert" Text="Save" />
                                <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"/>
                            </div>
                        </InsertItemTemplate>
                    </asp:FormView>
                </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="lstOpportunities" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="BU1" EventName="BUSelectionChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_Opportunities" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetOpportunitiesByBU" TypeName="cOpportunity">
        <SelectParameters>
            <asp:ControlParameter ControlID="RadioButtonList1" DefaultValue="Working Pipeline" Name="strFilter" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="ckExcludeExtensions" DefaultValue="Unchecked" Name="bExcludeExclusions" PropertyName="Checked" Type="Boolean" />
            <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource> 
    <asp:ObjectDataSource ID="ODS_SingleOpportunity" runat="server" 
            SelectMethod="GetSingleOpportunities" TypeName="cOpportunity" UpdateMethod="UpdateSingleOpportunity" InsertMethod="InsertSingleOpportunity">
        <SelectParameters>
            <asp:ControlParameter ControlID="lstOpportunities" DefaultValue="-1"  Name="PK_OpportunityID" PropertyName="SelectedValue" Type="Double" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="PK_OpportunityID" Type="Double" DefaultValue="-1"/>
            <asp:Parameter Name="Original_PK_OpportunityID" Type="Double" />
            <asp:Parameter Name="OpportunityOwner" Type="String" />
            <asp:Parameter Name="ClientID" Type="Double" />
            <asp:Parameter Name="OpportunityName" Type="String" />
            <asp:Parameter Name="Extension" Type="Boolean" />
            <asp:Parameter Name="Fit" Type="String" />
            <asp:Parameter Name="Anchor" Type="Boolean" DefaultValue="False"/>
            <asp:Parameter Name="Source" Type="String" />
            <asp:Parameter Name="Skills" Type="String" DefaultValue="None"/>
            <asp:Parameter Name="LastUpdateBy" Type="String"/>
            <asp:Parameter Name="RolesNeeded" Type="String" DefaultValue="TBD"/>
            <asp:Parameter Name="RFPRequired" Type="String" DefaultValue="False"/>
            <asp:Parameter Name="RFPLead" Type="String" DefaultValue="N/A"/>
            <asp:Parameter Name="RFPRiskAssessment" Type="String" DefaultValue="N/A"/>
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="OpportunityOwner" Type="String" />
            <asp:Parameter Name="ClientID" Type="Double" />
            <asp:Parameter Name="OpportunityName" Type="String" />
            <asp:Parameter Name="Extension" Type="Boolean" />
            <asp:Parameter Name="Fit" Type="String" />
            <asp:Parameter Name="Anchor" Type="Boolean" DefaultValue="False"/>
            <asp:Parameter Name="Source" Type="String" />
            <asp:Parameter Name="DateEntered" Type="DateTime" />
            <asp:Parameter Name="OpportunityCloseDate" Type="DateTime" />
            <asp:Parameter Name="WinPercentage" Type="String"/>
            <asp:Parameter Name="UpdatePerson" Type="String"/>
            <asp:Parameter Name="RolesNeeded" Type="String" DefaultValue="TBD"/>
            <asp:Parameter Name="RFPRequired" Type="String" DefaultValue="False"/>
            <asp:Parameter Name="RFPLead" Type="String" DefaultValue="N/A"/>
            <asp:Parameter Name="RFPRiskAssessment" Type="String" DefaultValue="N/A"/>
            <asp:Parameter Name="NewItemID" Type="Double" />
        </InsertParameters>
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
        
    <asp:ObjectDataSource ID="ODS_Clients" runat="server" SelectMethod="GetAllClientsByBU" TypeName="cAccountClientIndustry" InsertMethod="InsertSingleClient"  UpdateMethod="UpdateSingleClient" DeleteMethod="DeleteSingleClient">
        <SelectParameters>
            <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </form>
</body>
</html>
