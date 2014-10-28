Type.registerNamespace("majax");

majax.TableHover = function() {
    majax.TableHover.initializeBase(this);
    this._behaviors = null;
    this._pageLoadedHandler = null;
    this._appLoadHandler = null;
}

majax.TableHover.prototype = {

    initialize : function() {
        majax.TableHover.callBaseMethod(this, 'initialize');

        this._behaviors = new Array();

        // create the handlers
        this._pageLoadedHandler = Function.createDelegate(this, this.onPageLoaded);
        this._appLoadHandler = Function.createDelegate(this, this.onLoad);

        // wire up event handlers 
        var pageRequestMgr = Sys.WebForms.PageRequestManager.getInstance();
        pageRequestMgr.add_pageLoaded(this._pageLoadedHandler);   
        Sys.Application.add_load(this._appLoadHandler);
    },


    dispose : function() {
        // tear down event handlers 
        var pageRequestMgr = Sys.WebForms.PageRequestManager.getInstance();
        pageRequestMgr.remove_pageLoaded(this._pageLoadedHandler);
        Sys.Application.remove_load(this._appLoadHandler);

        majax.TableHover.callBaseMethod(this, 'dispose');
    },

    onLoad : function(sender, args) {     
        if(!args.get_isPartialLoad()){
            //  find all of the TABLE elements with the
            //  table-hover css class
            Array.forEach($majax.getElementsByClassName('tablehover', 'TABLE'), function(table){
                //  create a behavior for the table and add it to the collection
                Array.add(this._behaviors, $create(majax.TableHoverBehavior, null, null, null, table));
            }, this);             
        }                      
    },

    onPageLoaded : function(sender, args) {                       
        
        //  if there are items within an update panel,
        //  go ahead and apply the column hover to them
        var updatedPanels = args.get_panelsUpdated();
        if(updatedPanels && updatedPanels.length > 0){
            // find all elements with the 
            // and remember the scroll position 
            for(var i = 0; i < updatedPanels.length; i++) {
                Array.forEach($majax.getElementsByClassName('tablehover', 'TABLE', updatedPanels[i]), function(e){
                    //  create a behavior for the table and add it to the collection
                    Array.add(this._behaviors, $create(majax.TableHover, null, null, null, table));
                }, this);
            }
        } 
    }
}

majax.TableHoverBehavior = function(element) {
    majax.TableHoverBehavior.initializeBase(this, [element]);

    //  Properties
    this._rowHoverCssClass = null;
    this._rowSelectCssClass = null;
    this._columnHoverCssClass = null;
    this._columnSelectCssClass = null;    
    this._cellHoverCssClass = null;    
    this._cellSelectCssClass = null;       
    this._headerCellHoverCssClass = null;    
    this._headerCellSelectCssClass = null;
    
    //  Class names for the datarows
    this._dataRowCssClass = null;
    this._alternateDataRowCssClass = null;
    this._headerRowCssClass = null;

    //  Variables
    this._rows = null;
}

majax.TableHoverBehavior.prototype = {

    initialize : function() {
        majax.TableHoverBehavior.callBaseMethod(this, 'initialize');
        
        //  initialize the css class names
        this._rowHoverCssClass = 'row-over';
        this._rowSelectCssClass = 'row-select';
        this._columnHoverCssClass = 'column-over';
        this._columnSelectCssClass = 'column-select';    
        this._cellHoverCssClass = 'cell-over';    
        this._cellSelectCssClass = 'cell-select';       
        this._headerCellHoverCssClass = 'header-over';    
        this._headerCellSelectCssClass = 'header-select';
        this._dataRowCssClass = 'data-row';
        this._alternateDataRowCssClass = 'alt-data-row';
        this._headerRowCssClass = 'header-row';
        
        // get the elements
        this._rows = this.get_element().getElementsByTagName("tr");
        
        if(this._rows) {
            for(var i = 0; i < this._rows.length; i++) {
                //  get the row
                var row = this._rows[i];
                
                for(var j = 0; j < row.cells.length; j++) {
                    var args = {rowIndex: i, cellIndex: j, behavior: this};
                    var cell = row.cells[j]
                    //  attach to the data cell events
                    if(this._isDataRow(row)) {
                        $addHandler(cell, 'mouseover', Function.createCallback(this._onDataCellOver, args));
                        $addHandler(cell, 'mouseout', Function.createCallback(this._onDataCellOut, args));                    
                        $addHandler(cell, 'click', Function.createCallback(this._onDataCellClick, args));                                        
                    }
                    else if(this._isHeaderRow(row)) {
                        $addHandler(cell, 'mouseover', Function.createCallback(this._onHeaderCellOver, args));
                        $addHandler(cell, 'mouseout', Function.createCallback(this._onHeaderCellOut, args));                   
                    }
                }            
            }
        }
    },

    dispose : function() {
        if(this._rows) {
            // remove our event handlers from all data rows
            for(var i = 0; i < this._rows.length; i++) {
                //  get the row
                var row = this._rows[i];
                for(var j = 0; j < row.cells.length; j++) {
                    //  remove our handler
                    $clearHandlers(row.cells[j]);                
                }            
            }
        }
        
        majax.TableHoverBehavior.callBaseMethod(this, 'dispose');
    },

    _isHeaderRow : function(tr) {
        var headerRowClass = this._headerRowCssClass;
        return (headerRowClass && Sys.UI.DomElement.containsCssClass(tr, headerRowClass));
    },

    _isDataRow : function(tr) {
        var dataRowClass = this._dataRowCssClass;
        var altDataRowClass = this._alternateDataRowCssClass;
        return (dataRowClass && Sys.UI.DomElement.containsCssClass(tr, dataRowClass)) || (altDataRowClass && Sys.UI.DomElement.containsCssClass(tr, altDataRowClass));
    },
    
    _onDataCellOver : function(e, args) {
        //  add the css class to the row
        var headerCellHoverCssClass = args.behavior._headerCellHoverCssClass;        
        var rowHoverCssClass = args.behavior._rowHoverCssClass;
        var columnHoverCssClass = args.behavior._columnHoverCssClass;
        var cellHoverCssClass = args.behavior._cellHoverCssClass;
        var rows = args.behavior._rows;
        
        //  apply the class to all cells in this row
        if(rowHoverCssClass) {
            for(var i = 0; i < rows[args.rowIndex].cells.length; i++) {
                Sys.UI.DomElement.addCssClass(rows[args.rowIndex].cells[i], rowHoverCssClass);
            }
        }

        //  apply the class to all cells in this column (including the header rows cell)
        if(columnHoverCssClass || headerCellHoverCssClass) {
            for(var i = 0; i < rows.length; i++) {
                if(columnHoverCssClass && args.behavior._isDataRow(rows[i])) {
                    Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], columnHoverCssClass); 
                }
                else if(headerCellHoverCssClass && args.behavior._isHeaderRow(rows[i])) {
                    Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], headerCellHoverCssClass); 
                }
            }
        }

        //  apply the class to the cell that raised this event
        if(cellHoverCssClass) {
            Sys.UI.DomElement.addCssClass(rows[args.rowIndex].cells[args.cellIndex], cellHoverCssClass);
        }
    },
    
    _onDataCellOut : function(e, args) {
        //  remove the css class to the row
        var headerCellHoverCssClass = args.behavior._headerCellHoverCssClass;                
        var rowHoverCssClass = args.behavior._rowHoverCssClass;
        var columnHoverCssClass = args.behavior._columnHoverCssClass;
        var cellHoverCssClass = args.behavior._cellHoverCssClass;
        var rows = args.behavior._rows;
        
        //  remove the class to all cells in this row
        if(rowHoverCssClass) {
            for(var i = 0; i < rows[args.rowIndex].cells.length; i++) {
                Sys.UI.DomElement.removeCssClass(rows[args.rowIndex].cells[i], rowHoverCssClass);
            }
        }

        //  remove the class to all cells in this column (including the header rows cell)
        if(columnHoverCssClass || headerCellHoverCssClass) {
            for(var i = 0; i < rows.length; i++) {
                if(columnHoverCssClass && args.behavior._isDataRow(rows[i])) {
                    Sys.UI.DomElement.removeCssClass(rows[i].cells[args.cellIndex], columnHoverCssClass); 
                }
                else if(headerCellHoverCssClass && args.behavior._isHeaderRow(rows[i])) {
                    Sys.UI.DomElement.removeCssClass(rows[i].cells[args.cellIndex], headerCellHoverCssClass); 
                }
            }
        }

        //  remove the class to the cell that raised this event
        if(cellHoverCssClass) {
            Sys.UI.DomElement.removeCssClass(rows[args.rowIndex].cells[args.cellIndex], cellHoverCssClass);
        }
    }, 
    
    _onDataCellClick : function(e, args) {
        //  remove the classes
        var rowSelectCssClass = args.behavior._rowSelectCssClass;
        var columnSelectCssClass = args.behavior._columnSelectCssClass;
        var cellSelectCssClass = args.behavior._cellSelectCssClass;
        var headerCellSelectCssClass = args.behavior._headerCellSelectCssClass;
        var rows = args.behavior._rows;
        
        for(var i = 0; i < args.behavior._rows.length; i++) {
            var row = args.behavior._rows[i];        
            if(args.behavior._isDataRow(row) || args.behavior._isHeaderRow(row)) {
                for(var j = 0; j < row.cells.length; j++) {
                    if(headerCellSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], headerCellSelectCssClass);
                    }
                    if(rowSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], rowSelectCssClass);
                    }
                    if(cellSelectCssClass) {
                        Sys.UI.DomElement.removeCssClass(row.cells[j], cellSelectCssClass);
                    }
                    if(columnSelectCssClass) {                    
                        Sys.UI.DomElement.removeCssClass(row.cells[j], columnSelectCssClass);                    
                    }
                }
            }
        }            
        
        if(rowSelectCssClass) {
            for(var i = 0; i < rows[args.rowIndex].cells.length; i++) {
                Sys.UI.DomElement.addCssClass(rows[args.rowIndex].cells[i], rowSelectCssClass);
            }
        }

        if(columnSelectCssClass || headerCellSelectCssClass) {
            for(var i = 0; i < rows.length; i++) {
                if(columnSelectCssClass && args.behavior._isDataRow(rows[i])) {
                    Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], columnSelectCssClass);
                }
                if(headerCellSelectCssClass && args.behavior._isHeaderRow(rows[i])) {
                    Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], headerCellSelectCssClass);
                }                
            }
        }
        
        if(cellSelectCssClass) {
            Sys.UI.DomElement.addCssClass(rows[args.rowIndex].cells[args.cellIndex], cellSelectCssClass);
        }        
    }, 
    
    _onHeaderCellOver : function(e, args) {
        //  add the css class to the row
        var headerCellHoverCssClass = args.behavior._headerCellHoverCssClass;        
        var columnHoverCssClass = args.behavior._columnHoverCssClass;
        var rows = args.behavior._rows;
        
        //  apply the class to all cells in this column (including the header rows cell)
        if(columnHoverCssClass || headerCellHoverCssClass) {
            for(var i = 0; i < rows.length; i++) {
                if(columnHoverCssClass && args.behavior._isDataRow(rows[i])) {
                    Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], columnHoverCssClass); 
                }
                else if(headerCellHoverCssClass && args.behavior._isHeaderRow(rows[i])) {
                    Sys.UI.DomElement.addCssClass(rows[i].cells[args.cellIndex], headerCellHoverCssClass); 
                }
            }
        }
    },
    
    _onHeaderCellOut : function(e, args) {
        //  remove the css class to the row
        var headerCellHoverCssClass = args.behavior._headerCellHoverCssClass;        
        var columnHoverCssClass = args.behavior._columnHoverCssClass;
        var rows = args.behavior._rows;
        
        //  apply the class to all cells in this column (including the header rows cell)
        if(columnHoverCssClass || headerCellHoverCssClass) {
            for(var i = 0; i < rows.length; i++) {
                if(columnHoverCssClass && args.behavior._isDataRow(rows[i])) {
                    Sys.UI.DomElement.removeCssClass(rows[i].cells[args.cellIndex], columnHoverCssClass); 
                }
                else if(headerCellHoverCssClass && args.behavior._isHeaderRow(rows[i])) {
                    Sys.UI.DomElement.removeCssClass(rows[i].cells[args.cellIndex], headerCellHoverCssClass); 
                }
            }
        }
    }
}

majax.TableHoverBehavior.registerClass('majax.TableHoverBehavior', Sys.UI.Behavior);
majax.TableHover.registerClass('majax.TableHover', Sys.Component);

//  create the singleton
$create(majax.TableHover, null, null, null);

// Since this script is not loaded by System.Web.Handlers.ScriptResourceHandler
// invoke Sys.Application.notifyScriptLoaded to notify ScriptManager 
// that this is the end of the script.
if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
