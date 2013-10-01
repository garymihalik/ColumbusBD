<%@ Control Language="VB" AutoEventWireup="false" CodeFile="myModalLoading.ascx.vb" Inherits="myModalLoading" %>
<%@ Register Assembly="AjaxControlToolkit" TagPrefix="act" Namespace="AjaxControlToolkit" %>

<script type="text/javascript" language="javascript">
        
            //  register for our events
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequest);
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);    
            
            function beginRequest(sender, args){
                // show the popup
                //$find('mdlPopup').show();
                $find('<%= mdlPopup.ClientID %>').show();
            }

            function endRequest(sender, args) {
                //  hide the popup
                //$find('mdlPopup').hide();
                $find('<%= mdlPopup.ClientID %>').hide();
            }
        
        </script>  
              
<act:ModalPopupExtender ID="mdlPopup" runat="server" TargetControlID="pnlPopup" PopupControlID="pnlPopup" BackgroundCssClass="modalBackground"  />
<asp:Panel ID="pnlPopup" runat="server" CssClass="updateProgress" style="display:none">
    <div align="center" style="margin-top:13px;">
        <img src="commonimages/updateanimation.gif" alt="Loading..." />
        <span class="updateProgressMessage">Loading ...</span>
    </div>
</asp:Panel>

<style type="text/css">
.modalBackground 
        {
            background-color:Gray;
            filter:alpha(opacity=60);
            opacity:0.60;   
        }    
        .updateProgress
        {
            border-width:1px; 
            border-style:solid; 
            background-color:#FFFFFF; 
            position:absolute; 
            width:130px; 
            height:50px;    
        }
        .updateProgressMessage
        {
            margin:3px; 
            font-family:Trebuchet MS; 
            font-size:small; 
            vertical-align: middle;
        }

</style>