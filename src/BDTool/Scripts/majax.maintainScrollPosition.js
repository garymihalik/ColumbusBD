Type.registerNamespace("majax");

majax.MaintainScrollPosition = function() {
    majax.MaintainScrollPosition.initializeBase(this);
    this._elements = null;
    this._pageLoadingHandler = null;
    this._pageLoadedHandler = null;
}

majax.MaintainScrollPosition.prototype = {

    initialize : function() {
        majax.MaintainScrollPosition.callBaseMethod(this, 'initialize');

        this._elements = new Array();

        // create the handlers
        this._pageLoadingHandler = Function.createDelegate(this, this.onPageLoading);
        this._pageLoadedHandler = Function.createDelegate(this, this.onPageLoaded);

        // wire up event handlers 
        var pageRequestMgr = Sys.WebForms.PageRequestManager.getInstance();
        pageRequestMgr.add_pageLoading(this._pageLoadingHandler);
        pageRequestMgr.add_pageLoaded(this._pageLoadedHandler);
    },


    dispose : function() {
        // tear down event handlers 
        var pageRequestMgr = Sys.WebForms.PageRequestManager.getInstance();
        pageRequestMgr.remove_pageLoading(this._pageLoadingHandler);
        pageRequestMgr.remove_pageLoaded(this._pageLoadedHandler);

        majax.MaintainScrollPosition.callBaseMethod(this, 'dispose');
    },

    onPageLoading : function(sender, args) {
        // get a list of the panels that are going to
        // be updated
        var updatedPanels = args.get_panelsUpdating();
        if(updatedPanels && updatedPanels.length > 0){

            // clear the array
            Array.clear(this._elements); 

            // find all elements with the 
            // and remember the scroll position 
            for(var i = 0; i < updatedPanels.length; i++) {
                Array.forEach($majax.getElementsByClassName('maintain-scroll', null, updatedPanels[i]),
                function(e){
                    if(e.id) {
                        Array.add(this._elements, { "id":e.id, "x": e.scrollLeft, "y":e.scrollTop });
                    }
                }, this); 
            }
        }
    },

    onPageLoaded : function(sender, args) {

        var updatedPanels = args.get_panelsUpdated();
        if(updatedPanels && updatedPanels.length > 0){
            // find all elements with the 
            // and remember the scroll position 
            for(var i = 0; i < updatedPanels.length; i++) {
                Array.forEach(this._elements, function(e){
                    var element = $get(e.id, updatedPanels[i]);
                    if(element) {
                        element.scrollLeft = e.x;
                        element.scrollTop = e.y;
                    }
                }, this);
            }
        } 
    }
}

majax.MaintainScrollPosition.registerClass('majax.MaintainScrollPosition', Sys.Component);

//  create the singleton
$create(majax.MaintainScrollPosition, null, null, null);

// Since this script is not loaded by System.Web.Handlers.ScriptResourceHandler
// invoke Sys.Application.notifyScriptLoaded to notify ScriptManager 
// that this is the end of the script.
if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
