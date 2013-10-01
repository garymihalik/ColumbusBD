Type.registerNamespace('majax');

majax._Common = function() { }

majax._Common.prototype = {

    getElementsByClassName : function (className, tag, elm){
        var _getElementsByClassName = null;

	    if (document.getElementsByClassName) {
		    _getElementsByClassName = function (className, tag, elm) {
			    elm = elm || document;
			    var elements = elm.getElementsByClassName(className),
				    nodeName = (tag)? new RegExp("\\b" + tag + "\\b", "i") : null,
				    returnElements = [],
				    current;
			    for(var i=0, il=elements.length; i<il; i+=1){
				    current = elements[i];
				    if(!nodeName || nodeName.test(current.nodeName)) {
					    returnElements.push(current);
				    }
			    }
			    return returnElements;
		    };
	    }
	    else if (document.evaluate) {
		    _getElementsByClassName = function (className, tag, elm) {
			    tag = tag || "*";
			    elm = elm || document;
			    var classes = className.split(" "),
				    classesToCheck = "",
				    xhtmlNamespace = "http://www.w3.org/1999/xhtml",
				    namespaceResolver = (document.documentElement.namespaceURI === xhtmlNamespace)? xhtmlNamespace : null,
				    returnElements = [],
				    elements,
				    node;
			    for(var j=0, jl=classes.length; j<jl; j+=1){
				    classesToCheck += "[contains(concat(' ', @class, ' '), ' " + classes[j] + " ')]";
			    }
			    try	{
				    elements = document.evaluate(".//" + tag + classesToCheck, elm, namespaceResolver, 0, null);
			    }
			    catch (e) {
				    elements = document.evaluate(".//" + tag + classesToCheck, elm, null, 0, null);
			    }
			    while ((node = elements.iterateNext())) {
				    returnElements.push(node);
			    }
			    return returnElements;
		    };
	    }
	    else {
		    _getElementsByClassName = function (className, tag, elm) {
			    tag = tag || "*";
			    elm = elm || document;
			    var classes = className.split(" "),
				    classesToCheck = [],
				    elements = (tag === "*" && elm.all)? elm.all : elm.getElementsByTagName(tag),
				    current,
				    returnElements = [],
				    match;
			    for(var k=0, kl=classes.length; k<kl; k+=1){
				    classesToCheck.push(new RegExp("(^|\\s)" + classes[k] + "(\\s|$)"));
			    }
			    for(var l=0, ll=elements.length; l<ll; l+=1){
				    current = elements[l];
				    match = false;
				    for(var m=0, ml=classesToCheck.length; m<ml; m+=1){
					    match = classesToCheck[m].test(current.className);
					    if (!match) {
						    break;
					    }
				    }
				    if (match) {
					    returnElements.push(current);
				    }
			    }
			    return returnElements;
		    };
	    }
	    return _getElementsByClassName(className, tag, elm);
    }
    
}

// Create the singleton instance of the Common
var Common = majax.Common = new majax._Common();
var $majax = Common;