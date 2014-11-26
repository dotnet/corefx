/*
* This file has been commented to support Visual Studio Intellisense.
* You should not use this file at runtime inside the browser--it is only
* intended to be used only for design-time IntelliSense.  Please use the
* standard jQuery library for all production use.
*
* Comment version: 1.6.2
*/

/*!
* Note: While Microsoft is not the author of this file, Microsoft is
* offering you a license subject to the terms of the Microsoft Software
* License Terms for Microsoft ASP.NET Model View Controller 4 Developer Preview.
* Microsoft reserves all other rights. The notices below are provided
* for informational purposes only and are not the license terms under
* which Microsoft distributed this file.
*
* jQuery JavaScript Library v1.6.2
* http://jquery.com/
*
* Copyright 2010, John Resig
*
* Includes Sizzle.js
* http://sizzlejs.com/
* Copyright 2010, The Dojo Foundation
*
*/
(function (window, undefined) {

    // Use the correct document accordingly with window argument (sandbox)
    var document = window.document;
    var jQuery = (function () {

        // Define a local copy of jQuery
        var jQuery = function (selector, context) {
            ///	<summary>
            ///     1: $(expression, context) - This function accepts a string containing a CSS selector which is then used to match a set of elements.
            ///     &#10;2: $(html) - Create DOM elements on-the-fly from the provided String of raw HTML.
            ///     &#10;3: $(elements) - Wrap jQuery functionality around a single or multiple DOM Element(s).
            ///     &#10;4: $(callback) - A shorthand for $(document).ready().
            ///     &#10;5: $() - As of jQuery 1.4, if you pass no arguments in to the jQuery() method, an empty jQuery set will be returned.
            ///	</summary>
            ///	<param name="selector" type="String">
            ///     1: expression - An expression to search with.
            ///     &#10;2: html - A string of HTML to create on the fly.
            ///     &#10;3: elements - DOM element(s) to be encapsulated by a jQuery object.
            ///     &#10;4: callback - The function to execute when the DOM is ready.
            ///	</param>
            ///	<param name="context" type="jQuery">
            ///     1: context - A DOM Element, Document or jQuery to use as context.
            ///	</param>
            ///	<returns type="jQuery" />

            // The jQuery object is actually just the init constructor 'enhanced'
            return new jQuery.fn.init(selector, context);
        },

        // Map over jQuery in case of overwrite
	_jQuery = window.jQuery,

        // Map over the $ in case of overwrite
	_$ = window.$,

        // A central reference to the root jQuery(document)
	rootjQuery,

        // A simple way to check for HTML strings or ID strings
        // (both of which we optimize for)
	quickExpr = /^(?:[^<]*(<[\w\W]+>)[^>]*$|#([\w\-]+)$)/,

        // Is it a simple selector
	isSimple = /^.[^:#\[\.,]*$/,

        // Check if a string has a non-whitespace character in it
	rnotwhite = /\S/,
	rwhite = /\s/,

        // Used for trimming whitespace
	trimLeft = /^\s+/,
	trimRight = /\s+$/,

        // Check for non-word characters
	rnonword = /\W/,

        // Check for digits
	rdigit = /\d/,

        // Match a standalone tag
	rsingleTag = /^<(\w+)\s*\/?>(?:<\/\1>)?$/,

        // JSON RegExp
	rvalidchars = /^[\],:{}\s]*$/,
	rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g,
	rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g,
	rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g,

        // Useragent RegExp
	rwebkit = /(webkit)[ \/]([\w.]+)/,
	ropera = /(opera)(?:.*version)?[ \/]([\w.]+)/,
	rmsie = /(msie) ([\w.]+)/,
	rmozilla = /(mozilla)(?:.*? rv:([\w.]+))?/,

        // Keep a UserAgent string for use with jQuery.browser
	userAgent = navigator.userAgent,

        // For matching the engine and version of the browser
	browserMatch,

        // Has the ready events already been bound?
	readyBound = false,

        // The functions to execute on DOM ready
	readyList = [],

        // The ready event handler
	DOMContentLoaded,

        // Save a reference to some core methods
	toString = Object.prototype.toString,
	hasOwn = Object.prototype.hasOwnProperty,
	push = Array.prototype.push,
	slice = Array.prototype.slice,
	trim = String.prototype.trim,
	indexOf = Array.prototype.indexOf,

        // [[Class]] -> type pairs
	class2type = {};

        jQuery.fn = jQuery.prototype = {
            init: function (selector, context) {
                var match, elem, ret, doc;

                // Handle $(""), $(null), or $(undefined)
                if (!selector) {
                    return this;
                }

                // Handle $(DOMElement)
                if (selector.nodeType) {
                    this.context = this[0] = selector;
                    this.length = 1;
                    return this;
                }

                // The body element only exists once, optimize finding it
                if (selector === "body" && !context && document.body) {
                    this.context = document;
                    this[0] = document.body;
                    this.selector = "body";
                    this.length = 1;
                    return this;
                }

                // Handle HTML strings
                if (typeof selector === "string") {
                    // Are we dealing with HTML string or an ID?
                    match = quickExpr.exec(selector);

                    // Verify a match, and that no context was specified for #id
                    if (match && (match[1] || !context)) {

                        // HANDLE: $(html) -> $(array)
                        if (match[1]) {
                            doc = (context ? context.ownerDocument || context : document);

                            // If a single string is passed in and it's a single tag
                            // just do a createElement and skip the rest
                            ret = rsingleTag.exec(selector);

                            if (ret) {
                                if (jQuery.isPlainObject(context)) {
                                    selector = [document.createElement(ret[1])];
                                    jQuery.fn.attr.call(selector, context, true);

                                } else {
                                    selector = [doc.createElement(ret[1])];
                                }

                            } else {
                                ret = jQuery.buildFragment([match[1]], [doc]);
                                selector = (ret.cacheable ? ret.fragment.cloneNode(true) : ret.fragment).childNodes;
                            }

                            return jQuery.merge(this, selector);

                            // HANDLE: $("#id")
                        } else {
                            elem = document.getElementById(match[2]);

                            // Check parentNode to catch when Blackberry 4.6 returns
                            // nodes that are no longer in the document #6963
                            if (elem && elem.parentNode) {
                                // Handle the case where IE and Opera return items
                                // by name instead of ID
                                if (elem.id !== match[2]) {
                                    return rootjQuery.find(selector);
                                }

                                // Otherwise, we inject the element directly into the jQuery object
                                this.length = 1;
                                this[0] = elem;
                            }

                            this.context = document;
                            this.selector = selector;
                            return this;
                        }

                        // HANDLE: $("TAG")
                    } else if (!context && !rnonword.test(selector)) {
                        this.selector = selector;
                        this.context = document;
                        selector = document.getElementsByTagName(selector);
                        return jQuery.merge(this, selector);

                        // HANDLE: $(expr, $(...))
                    } else if (!context || context.jquery) {
                        return (context || rootjQuery).find(selector);

                        // HANDLE: $(expr, context)
                        // (which is just equivalent to: $(context).find(expr)
                    } else {
                        return jQuery(context).find(selector);
                    }

                    // HANDLE: $(function)
                    // Shortcut for document ready
                } else if (jQuery.isFunction(selector)) {
                    return rootjQuery.ready(selector);
                }

                if (selector.selector !== undefined) {
                    this.selector = selector.selector;
                    this.context = selector.context;
                }

                return jQuery.makeArray(selector, this);
            },

            // Start with an empty selector
            selector: "",

            // The current version of jQuery being used
            jquery: "1.4.4",

            // The default length of a jQuery object is 0
            length: 0,

            // The number of elements contained in the matched element set
            size: function () {
                ///	<summary>
                ///     &#10;The number of elements currently matched.
                ///     &#10;Part of Core
                ///	</summary>
                ///	<returns type="Number" />

                return this.length;
            },

            toArray: function () {
                ///	<summary>
                ///     &#10;Retrieve all the DOM elements contained in the jQuery set, as an array.
                ///	</summary>
                ///	<returns type="Array" />
                return slice.call(this, 0);
            },

            // Get the Nth element in the matched element set OR
            // Get the whole matched element set as a clean array
            get: function (num) {
                ///	<summary>
                ///     &#10;Access a single matched element. num is used to access the
                ///     &#10;Nth element matched.
                ///     &#10;Part of Core
                ///	</summary>
                ///	<returns type="Element" />
                ///	<param name="num" type="Number">
                ///     &#10;Access the element in the Nth position.
                ///	</param>

                return num == null ?

                // Return a 'clean' array
			this.toArray() :

                // Return just the object
			(num < 0 ? this.slice(num)[0] : this[num]);
            },

            // Take an array of elements and push it onto the stack
            // (returning the new matched element set)
            pushStack: function (elems, name, selector) {
                ///	<summary>
                ///     &#10;Set the jQuery object to an array of elements, while maintaining
                ///     &#10;the stack.
                ///     &#10;Part of Core
                ///	</summary>
                ///	<returns type="jQuery" />
                ///	<param name="elems" type="Elements">
                ///     &#10;An array of elements
                ///	</param>

                // Build a new jQuery matched element set
                var ret = jQuery();

                if (jQuery.isArray(elems)) {
                    push.apply(ret, elems);

                } else {
                    jQuery.merge(ret, elems);
                }

                // Add the old object onto the stack (as a reference)
                ret.prevObject = this;

                ret.context = this.context;

                if (name === "find") {
                    ret.selector = this.selector + (this.selector ? " " : "") + selector;
                } else if (name) {
                    ret.selector = this.selector + "." + name + "(" + selector + ")";
                }

                // Return the newly-formed element set
                return ret;
            },

            // Execute a callback for every element in the matched set.
            // (You can seed the arguments with an array of args, but this is
            // only used internally.)
            each: function (callback, args) {
                ///	<summary>
                ///     &#10;Execute a function within the context of every matched element.
                ///     &#10;This means that every time the passed-in function is executed
                ///     &#10;(which is once for every element matched) the 'this' keyword
                ///     &#10;points to the specific element.
                ///     &#10;Additionally, the function, when executed, is passed a single
                ///     &#10;argument representing the position of the element in the matched
                ///     &#10;set.
                ///     &#10;Part of Core
                ///	</summary>
                ///	<returns type="jQuery" />
                ///	<param name="callback" type="Function">
                ///     &#10;A function to execute
                ///	</param>

                return jQuery.each(this, callback, args);
            },

            ready: function (fn) {
                ///	<summary>
                ///     &#10;Binds a function to be executed whenever the DOM is ready to be traversed and manipulated.
                ///	</summary>
                ///	<param name="fn" type="Function">The function to be executed when the DOM is ready.</param>

                // Attach the listeners
                jQuery.bindReady();

                // If the DOM is already ready
                if (jQuery.isReady) {
                    // Execute the function immediately
                    fn.call(document, jQuery);

                    // Otherwise, remember the function for later
                } else if (readyList) {
                    // Add the function to the wait list
                    readyList.push(fn);
                }

                return this;
            },

            eq: function (i) {
                ///	<summary>
                ///     &#10;Reduce the set of matched elements to a single element.
                ///     &#10;The position of the element in the set of matched elements
                ///     &#10;starts at 0 and goes to length - 1.
                ///     &#10;Part of Core
                ///	</summary>
                ///	<returns type="jQuery" />
                ///	<param name="num" type="Number">
                ///     &#10;pos The index of the element that you wish to limit to.
                ///	</param>

                return i === -1 ?
			this.slice(i) :
			this.slice(i, +i + 1);
            },

            first: function () {
                ///	<summary>
                ///     &#10;Reduce the set of matched elements to the first in the set.
                ///	</summary>
                ///	<returns type="jQuery" />

                return this.eq(0);
            },

            last: function () {
                ///	<summary>
                ///     &#10;Reduce the set of matched elements to the final one in the set.
                ///	</summary>
                ///	<returns type="jQuery" />

                return this.eq(-1);
            },

            slice: function () {
                ///	<summary>
                ///     &#10;Selects a subset of the matched elements.  Behaves exactly like the built-in Array slice method.
                ///	</summary>
                ///	<param name="start" type="Number" integer="true">Where to start the subset (0-based).</param>
                ///	<param name="end" optional="true" type="Number" integer="true">Where to end the subset (not including the end element itself).
                ///     &#10;If omitted, ends at the end of the selection</param>
                ///	<returns type="jQuery">The sliced elements</returns>

                return this.pushStack(slice.apply(this, arguments),
			"slice", slice.call(arguments).join(","));
            },

            map: function (callback) {
                ///	<summary>
                ///     &#10;This member is internal.
                ///	</summary>
                ///	<private />
                ///	<returns type="jQuery" />

                return this.pushStack(jQuery.map(this, function (elem, i) {
                    return callback.call(elem, i, elem);
                }));
            },

            end: function () {
                ///	<summary>
                ///     &#10;End the most recent 'destructive' operation, reverting the list of matched elements
                ///     &#10;back to its previous state. After an end operation, the list of matched elements will
                ///     &#10;revert to the last state of matched elements.
                ///     &#10;If there was no destructive operation before, an empty set is returned.
                ///     &#10;Part of DOM/Traversing
                ///	</summary>
                ///	<returns type="jQuery" />

                return this.prevObject || jQuery(null);
            },

            // For internal use only.
            // Behaves like an Array's method, not like a jQuery method.
            push: push,
            sort: [].sort,
            splice: [].splice
        };

        // Give the init function the jQuery prototype for later instantiation
        jQuery.fn.init.prototype = jQuery.fn;

        jQuery.extend = jQuery.fn.extend = function () {
            ///	<summary>
            ///     &#10;Extend one object with one or more others, returning the original,
            ///     &#10;modified, object. This is a great utility for simple inheritance.
            ///     &#10;jQuery.extend(settings, options);
            ///     &#10;var settings = jQuery.extend({}, defaults, options);
            ///     &#10;Part of JavaScript
            ///	</summary>
            ///	<param name="target" type="Object">
            ///     &#10; The object to extend
            ///	</param>
            ///	<param name="prop1" type="Object">
            ///     &#10; The object that will be merged into the first.
            ///	</param>
            ///	<param name="propN" type="Object" optional="true" parameterArray="true">
            ///     &#10; (optional) More objects to merge into the first
            ///	</param>
            ///	<returns type="Object" />

            var options, name, src, copy, copyIsArray, clone,
		target = arguments[0] || {},
		i = 1,
		length = arguments.length,
		deep = false;

            // Handle a deep copy situation
            if (typeof target === "boolean") {
                deep = target;
                target = arguments[1] || {};
                // skip the boolean and the target
                i = 2;
            }

            // Handle case when target is a string or something (possible in deep copy)
            if (typeof target !== "object" && !jQuery.isFunction(target)) {
                target = {};
            }

            // extend jQuery itself if only one argument is passed
            if (length === i) {
                target = this;
                --i;
            }

            for (; i < length; i++) {
                // Only deal with non-null/undefined values
                if ((options = arguments[i]) != null) {
                    // Extend the base object
                    for (name in options) {
                        src = target[name];
                        copy = options[name];

                        // Prevent never-ending loop
                        if (target === copy) {
                            continue;
                        }

                        // Recurse if we're merging plain objects or arrays
                        if (deep && copy && (jQuery.isPlainObject(copy) || (copyIsArray = jQuery.isArray(copy)))) {
                            if (copyIsArray) {
                                copyIsArray = false;
                                clone = src && jQuery.isArray(src) ? src : [];

                            } else {
                                clone = src && jQuery.isPlainObject(src) ? src : {};
                            }

                            // Never move original objects, clone them
                            target[name] = jQuery.extend(deep, clone, copy);

                            // Don't bring in undefined values
                        } else if (copy !== undefined) {
                            target[name] = copy;
                        }
                    }
                }
            }

            // Return the modified object
            return target;
        };

        jQuery.extend({
            noConflict: function (deep) {
                ///	<summary>
                ///     &#10;Run this function to give control of the $ variable back
                ///     &#10;to whichever library first implemented it. This helps to make 
                ///     &#10;sure that jQuery doesn't conflict with the $ object
                ///     &#10;of other libraries.
                ///     &#10;By using this function, you will only be able to access jQuery
                ///     &#10;using the 'jQuery' variable. For example, where you used to do
                ///     &#10;$(&quot;div p&quot;), you now must do jQuery(&quot;div p&quot;).
                ///     &#10;Part of Core 
                ///	</summary>
                ///	<returns type="undefined" />

                window.$ = _$;

                if (deep) {
                    window.jQuery = _jQuery;
                }

                return jQuery;
            },

            // Is the DOM ready to be used? Set to true once it occurs.
            isReady: false,

            // A counter to track how many items to wait for before
            // the ready event fires. See #6781
            readyWait: 1,

            // Handle when the DOM is ready
            ready: function (wait) {
                ///	<summary>
                ///     &#10;This method is internal.
                ///	</summary>
                ///	<private />

                // A third-party is pushing the ready event forwards
                if (wait === true) {
                    jQuery.readyWait--;
                }

                // Make sure that the DOM is not already loaded
                if (!jQuery.readyWait || (wait !== true && !jQuery.isReady)) {
                    // Make sure body exists, at least, in case IE gets a little overzealous (ticket #5443).
                    if (!document.body) {
                        return setTimeout(jQuery.ready, 1);
                    }

                    // Remember that the DOM is ready
                    jQuery.isReady = true;

                    // If a normal DOM Ready event fired, decrement, and wait if need be
                    if (wait !== true && --jQuery.readyWait > 0) {
                        return;
                    }

                    // If there are functions bound, to execute
                    if (readyList) {
                        // Execute all of them
                        var fn,
					i = 0,
					ready = readyList;

                        // Reset the list of functions
                        readyList = null;

                        while ((fn = ready[i++])) {
                            fn.call(document, jQuery);
                        }

                        // Trigger any bound ready events
                        if (jQuery.fn.trigger) {
                            jQuery(document).trigger("ready").unbind("ready");
                        }
                    }
                }
            },

            bindReady: function () {
                if (readyBound) {
                    return;
                }

                readyBound = true;

                // Catch cases where $(document).ready() is called after the
                // browser event has already occurred.
                if (document.readyState === "complete") {
                    // Handle it asynchronously to allow scripts the opportunity to delay ready
                    return setTimeout(jQuery.ready, 1);
                }

                // Mozilla, Opera and webkit nightlies currently support this event
                if (document.addEventListener) {
                    // Use the handy event callback
                    document.addEventListener("DOMContentLoaded", DOMContentLoaded, false);

                    // A fallback to window.onload, that will always work
                    window.addEventListener("load", jQuery.ready, false);

                    // If IE event model is used
                } else if (document.attachEvent) {
                    // ensure firing before onload,
                    // maybe late but safe also for iframes
                    document.attachEvent("onreadystatechange", DOMContentLoaded);

                    // A fallback to window.onload, that will always work
                    window.attachEvent("onload", jQuery.ready);

                    // If IE and not a frame
                    // continually check to see if the document is ready
                    var toplevel = false;

                    try {
                        toplevel = window.frameElement == null;
                    } catch (e) { }

                    if (document.documentElement.doScroll && toplevel) {
                        doScrollCheck();
                    }
                }
            },

            // See test/unit/core.js for details concerning isFunction.
            // Since version 1.3, DOM methods and functions like alert
            // aren't supported. They return false on IE (#2968).
            isFunction: function (obj) {
                ///	<summary>
                ///     &#10;Determines if the parameter passed is a function.
                ///	</summary>
                ///	<param name="obj" type="Object">The object to check</param>
                ///	<returns type="Boolean">True if the parameter is a function; otherwise false.</returns>

                return jQuery.type(obj) === "function";
            },

            isArray: Array.isArray || function (obj) {
                ///	<summary>
                ///     &#10;Determine if the parameter passed is an array.
                ///	</summary>
                ///	<param name="obj" type="Object">Object to test whether or not it is an array.</param>
                ///	<returns type="Boolean">True if the parameter is a function; otherwise false.</returns>

                return jQuery.type(obj) === "array";
            },

            // A crude way of determining if an object is a window
            isWindow: function (obj) {
                return obj && typeof obj === "object" && "setInterval" in obj;
            },

            isNaN: function (obj) {
                return obj == null || !rdigit.test(obj) || isNaN(obj);
            },

            type: function (obj) {
                return obj == null ?
			String(obj) :
			class2type[toString.call(obj)] || "object";
            },

            isPlainObject: function (obj) {
                ///	<summary>
                ///     &#10;Check to see if an object is a plain object (created using "{}" or "new Object").
                ///	</summary>
                ///	<param name="obj" type="Object">
                ///     &#10;The object that will be checked to see if it's a plain object.
                ///	</param>
                ///	<returns type="Boolean" />

                // Must be an Object.
                // Because of IE, we also have to check the presence of the constructor property.
                // Make sure that DOM nodes and window objects don't pass through, as well
                if (!obj || jQuery.type(obj) !== "object" || obj.nodeType || jQuery.isWindow(obj)) {
                    return false;
                }

                // Not own constructor property must be Object
                if (obj.constructor &&
			!hasOwn.call(obj, "constructor") &&
			!hasOwn.call(obj.constructor.prototype, "isPrototypeOf")) {
                    return false;
                }

                // Own properties are enumerated firstly, so to speed up,
                // if last one is own, then all properties are own.

                var key;
                for (key in obj) { }

                return key === undefined || hasOwn.call(obj, key);
            },

            isEmptyObject: function (obj) {
                ///	<summary>
                ///     &#10;Check to see if an object is empty (contains no properties).
                ///	</summary>
                ///	<param name="obj" type="Object">
                ///     &#10;The object that will be checked to see if it's empty.
                ///	</param>
                ///	<returns type="Boolean" />

                for (var name in obj) {
                    return false;
                }
                return true;
            },

            error: function (msg) {
                throw msg;
            },

            parseJSON: function (data) {
                if (typeof data !== "string" || !data) {
                    return null;
                }

                // Make sure leading/trailing whitespace is removed (IE can't handle it)
                data = jQuery.trim(data);

                // Make sure the incoming data is actual JSON
                // Logic borrowed from http://json.org/json2.js
                if (rvalidchars.test(data.replace(rvalidescape, "@")
			.replace(rvalidtokens, "]")
			.replace(rvalidbraces, ""))) {

                    // Try to use the native JSON parser first
                    return window.JSON && window.JSON.parse ?
				window.JSON.parse(data) :
				(new Function("return " + data))();

                } else {
                    jQuery.error("Invalid JSON: " + data);
                }
            },

            noop: function () {
                ///	<summary>
                ///     &#10;An empty function.
                ///	</summary>
                ///	<returns type="Function" />
            },

            // Evalulates a script in a global context
            globalEval: function (data) {
                ///	<summary>
                ///     &#10;Internally evaluates a script in a global context.
                ///	</summary>
                ///	<private />

                if (data && rnotwhite.test(data)) {
                    // Inspired by code by Andrea Giammarchi
                    // http://webreflection.blogspot.com/2007/08/global-scope-evaluation-and-dom.html
                    var head = document.getElementsByTagName("head")[0] || document.documentElement,
				script = document.createElement("script");

                    script.type = "text/javascript";

                    if (jQuery.support.scriptEval) {
                        script.appendChild(document.createTextNode(data));
                    } else {
                        script.text = data;
                    }

                    // Use insertBefore instead of appendChild to circumvent an IE6 bug.
                    // This arises when a base node is used (#2709).
                    head.insertBefore(script, head.firstChild);
                    head.removeChild(script);
                }
            },

            nodeName: function (elem, name) {
                ///	<summary>
                ///     &#10;Checks whether the specified element has the specified DOM node name.
                ///	</summary>
                ///	<param name="elem" type="Element">The element to examine</param>
                ///	<param name="name" type="String">The node name to check</param>
                ///	<returns type="Boolean">True if the specified node name matches the node's DOM node name; otherwise false</returns>

                return elem.nodeName && elem.nodeName.toUpperCase() === name.toUpperCase();
            },

            // args is for internal usage only
            each: function (object, callback, args) {
                ///	<summary>
                ///     &#10;A generic iterator function, which can be used to seemlessly
                ///     &#10;iterate over both objects and arrays. This function is not the same
                ///     &#10;as $().each() - which is used to iterate, exclusively, over a jQuery
                ///     &#10;object. This function can be used to iterate over anything.
                ///     &#10;The callback has two arguments:the key (objects) or index (arrays) as first
                ///     &#10;the first, and the value as the second.
                ///     &#10;Part of JavaScript
                ///	</summary>
                ///	<param name="obj" type="Object">
                ///     &#10; The object, or array, to iterate over.
                ///	</param>
                ///	<param name="fn" type="Function">
                ///     &#10; The function that will be executed on every object.
                ///	</param>
                ///	<returns type="Object" />

                var name, i = 0,
			length = object.length,
			isObj = length === undefined || jQuery.isFunction(object);

                if (args) {
                    if (isObj) {
                        for (name in object) {
                            if (callback.apply(object[name], args) === false) {
                                break;
                            }
                        }
                    } else {
                        for (; i < length; ) {
                            if (callback.apply(object[i++], args) === false) {
                                break;
                            }
                        }
                    }

                    // A special, fast, case for the most common use of each
                } else {
                    if (isObj) {
                        for (name in object) {
                            if (callback.call(object[name], name, object[name]) === false) {
                                break;
                            }
                        }
                    } else {
                        for (var value = object[0];
					i < length && callback.call(value, i, value) !== false; value = object[++i]) { }
                    }
                }

                return object;
            },

            // Use native String.trim function wherever possible
            trim: trim ?
		function (text) {
		    return text == null ?
				"" :
				trim.call(text);
		} :

            // Otherwise use our own trimming functionality
		function (text) {
		    return text == null ?
				"" :
				text.toString().replace(trimLeft, "").replace(trimRight, "");
		},

            // results is for internal usage only
            makeArray: function (array, results) {
                ///	<summary>
                ///     &#10;Turns anything into a true array.  This is an internal method.
                ///	</summary>
                ///	<param name="array" type="Object">Anything to turn into an actual Array</param>
                ///	<returns type="Array" />
                ///	<private />

                var ret = results || [];

                if (array != null) {
                    // The window, strings (and functions) also have 'length'
                    // The extra typeof function check is to prevent crashes
                    // in Safari 2 (See: #3039)
                    // Tweaked logic slightly to handle Blackberry 4.7 RegExp issues #6930
                    var type = jQuery.type(array);

                    if (array.length == null || type === "string" || type === "function" || type === "regexp" || jQuery.isWindow(array)) {
                        push.call(ret, array);
                    } else {
                        jQuery.merge(ret, array);
                    }
                }

                return ret;
            },

            inArray: function (elem, array) {
                if (array.indexOf) {
                    return array.indexOf(elem);
                }

                for (var i = 0, length = array.length; i < length; i++) {
                    if (array[i] === elem) {
                        return i;
                    }
                }

                return -1;
            },

            merge: function (first, second) {
                ///	<summary>
                ///     &#10;Merge two arrays together, removing all duplicates.
                ///     &#10;The new array is: All the results from the first array, followed
                ///     &#10;by the unique results from the second array.
                ///     &#10;Part of JavaScript
                ///	</summary>
                ///	<returns type="Array" />
                ///	<param name="first" type="Array">
                ///     &#10; The first array to merge.
                ///	</param>
                ///	<param name="second" type="Array">
                ///     &#10; The second array to merge.
                ///	</param>

                var i = first.length,
			j = 0;

                if (typeof second.length === "number") {
                    for (var l = second.length; j < l; j++) {
                        first[i++] = second[j];
                    }

                } else {
                    while (second[j] !== undefined) {
                        first[i++] = second[j++];
                    }
                }

                first.length = i;

                return first;
            },

            grep: function (elems, callback, inv) {
                ///	<summary>
                ///     &#10;Filter items out of an array, by using a filter function.
                ///     &#10;The specified function will be passed two arguments: The
                ///     &#10;current array item and the index of the item in the array. The
                ///     &#10;function must return 'true' to keep the item in the array, 
                ///     &#10;false to remove it.
                ///     &#10;});
                ///     &#10;Part of JavaScript
                ///	</summary>
                ///	<returns type="Array" />
                ///	<param name="elems" type="Array">
                ///     &#10;array The Array to find items in.
                ///	</param>
                ///	<param name="fn" type="Function">
                ///     &#10; The function to process each item against.
                ///	</param>
                ///	<param name="inv" type="Boolean">
                ///     &#10; Invert the selection - select the opposite of the function.
                ///	</param>

                var ret = [], retVal;
                inv = !!inv;

                // Go through the array, only saving the items
                // that pass the validator function
                for (var i = 0, length = elems.length; i < length; i++) {
                    retVal = !!callback(elems[i], i);
                    if (inv !== retVal) {
                        ret.push(elems[i]);
                    }
                }

                return ret;
            },

            // arg is for internal usage only
            map: function (elems, callback, arg) {
                ///	<summary>
                ///     &#10;Translate all items in an array to another array of items.
                ///     &#10;The translation function that is provided to this method is 
                ///     &#10;called for each item in the array and is passed one argument: 
                ///     &#10;The item to be translated.
                ///     &#10;The function can then return the translated value, 'null'
                ///     &#10;(to remove the item), or  an array of values - which will
                ///     &#10;be flattened into the full array.
                ///     &#10;Part of JavaScript
                ///	</summary>
                ///	<returns type="Array" />
                ///	<param name="elems" type="Array">
                ///     &#10;array The Array to translate.
                ///	</param>
                ///	<param name="fn" type="Function">
                ///     &#10; The function to process each item against.
                ///	</param>

                var ret = [], value;

                // Go through the array, translating each of the items to their
                // new value (or values).
                for (var i = 0, length = elems.length; i < length; i++) {
                    value = callback(elems[i], i, arg);

                    if (value != null) {
                        ret[ret.length] = value;
                    }
                }

                return ret.concat.apply([], ret);
            },

            // A global GUID counter for objects
            guid: 1,

            proxy: function (fn, proxy, thisObject) {
                ///	<summary>
                ///     &#10;Takes a function and returns a new one that will always have a particular scope.
                ///	</summary>
                ///	<param name="fn" type="Function">
                ///     &#10;The function whose scope will be changed.
                ///	</param>
                ///	<param name="proxy" type="Object">
                ///     &#10;The object to which the scope of the function should be set.
                ///	</param>
                ///	<returns type="Function" />

                if (arguments.length === 2) {
                    if (typeof proxy === "string") {
                        thisObject = fn;
                        fn = thisObject[proxy];
                        proxy = undefined;

                    } else if (proxy && !jQuery.isFunction(proxy)) {
                        thisObject = proxy;
                        proxy = undefined;
                    }
                }

                if (!proxy && fn) {
                    proxy = function () {
                        return fn.apply(thisObject || this, arguments);
                    };
                }

                // Set the guid of unique handler to the same of original handler, so it can be removed
                if (fn) {
                    proxy.guid = fn.guid = fn.guid || proxy.guid || jQuery.guid++;
                }

                // So proxy can be declared as an argument
                return proxy;
            },

            // Mutifunctional method to get and set values to a collection
            // The value/s can be optionally by executed if its a function
            access: function (elems, key, value, exec, fn, pass) {
                var length = elems.length;

                // Setting many attributes
                if (typeof key === "object") {
                    for (var k in key) {
                        jQuery.access(elems, k, key[k], exec, fn, value);
                    }
                    return elems;
                }

                // Setting one attribute
                if (value !== undefined) {
                    // Optionally, function values get executed if exec is true
                    exec = !pass && exec && jQuery.isFunction(value);

                    for (var i = 0; i < length; i++) {
                        fn(elems[i], key, exec ? value.call(elems[i], i, fn(elems[i], key)) : value, pass);
                    }

                    return elems;
                }

                // Getting an attribute
                return length ? fn(elems[0], key) : undefined;
            },

            now: function () {
                return (new Date()).getTime();
            },

            // Use of jQuery.browser is frowned upon.
            // More details: http://docs.jquery.com/Utilities/jQuery.browser
            uaMatch: function (ua) {
                ua = ua.toLowerCase();

                var match = rwebkit.exec(ua) ||
			ropera.exec(ua) ||
			rmsie.exec(ua) ||
			ua.indexOf("compatible") < 0 && rmozilla.exec(ua) ||
			[];

                return { browser: match[1] || "", version: match[2] || "0" };
            },

            browser: {}
        });

        // Populate the class2type map
        jQuery.each("Boolean Number String Function Array Date RegExp Object".split(" "), function (i, name) {
            class2type["[object " + name + "]"] = name.toLowerCase();
        });

        browserMatch = jQuery.uaMatch(userAgent);
        if (browserMatch.browser) {
            jQuery.browser[browserMatch.browser] = true;
            jQuery.browser.version = browserMatch.version;
        }

        // Deprecated, use jQuery.browser.webkit instead
        if (jQuery.browser.webkit) {
            jQuery.browser.safari = true;
        }

        if (indexOf) {
            jQuery.inArray = function (elem, array) {
                ///	<summary>
                ///     &#10;Determines the index of the first parameter in the array.
                ///	</summary>
                ///	<param name="elem">The value to see if it exists in the array.</param>
                ///	<param name="array" type="Array">The array to look through for the value</param>
                ///	<returns type="Number" integer="true">The 0-based index of the item if it was found, otherwise -1.</returns>

                return indexOf.call(array, elem);
            };
        }

        // Verify that \s matches non-breaking spaces
        // (IE fails on this test)
        if (!rwhite.test("\xA0")) {
            trimLeft = /^[\s\xA0]+/;
            trimRight = /[\s\xA0]+$/;
        }

        // All jQuery objects should point back to these
        rootjQuery = jQuery(document);

        // Cleanup functions for the document ready method
        if (document.addEventListener) {
            DOMContentLoaded = function () {
                document.removeEventListener("DOMContentLoaded", DOMContentLoaded, false);
                jQuery.ready();
            };

        } else if (document.attachEvent) {
            DOMContentLoaded = function () {
                // Make sure body exists, at least, in case IE gets a little overzealous (ticket #5443).
                if (document.readyState === "complete") {
                    document.detachEvent("onreadystatechange", DOMContentLoaded);
                    jQuery.ready();
                }
            };
        }

        // The DOM ready check for Internet Explorer
        function doScrollCheck() {
            if (jQuery.isReady) {
                return;
            }

            try {
                // If IE is used, use the trick by Diego Perini
                // http://javascript.nwbox.com/IEContentLoaded/
                document.documentElement.doScroll("left");
            } catch (e) {
                setTimeout(doScrollCheck, 1);
                return;
            }

            // and execute any waiting functions
            jQuery.ready();
        }

        // Expose jQuery to the global object
        return (window.jQuery = window.$ = jQuery);

    })();



    // [vsdoc] The following function has been modified for IntelliSense.
    // [vsdoc] Stubbing support properties to "false" for IntelliSense compat.
    (function () {

        jQuery.support = {};

        //	var root = document.documentElement,
        //		script = document.createElement("script"),
        //		div = document.createElement("div"),
        //		id = "script" + jQuery.now();

        //	div.style.display = "none";
        //	div.innerHTML = "   <link/><table></table><a href='/a' style='color:red;float:left;opacity:.55;'>a</a><input type='checkbox'/>";

        //	var all = div.getElementsByTagName("*"),
        //		a = div.getElementsByTagName("a")[0],
        //		select = document.createElement("select"),
        //		opt = select.appendChild( document.createElement("option") );

        //	// Can't get basic test support
        //	if ( !all || !all.length || !a ) {
        //		return;
        //	}

        jQuery.support = {
            // IE strips leading whitespace when .innerHTML is used
            leadingWhitespace: false,

            // Make sure that tbody elements aren't automatically inserted
            // IE will insert them into empty tables
            tbody: false,

            // Make sure that link elements get serialized correctly by innerHTML
            // This requires a wrapper element in IE
            htmlSerialize: false,

            // Get the style information from getAttribute
            // (IE uses .cssText insted)
            style: false,

            // Make sure that URLs aren't manipulated
            // (IE normalizes it by default)
            hrefNormalized: false,

            // Make sure that element opacity exists
            // (IE uses filter instead)
            // Use a regex to work around a WebKit issue. See #5145
            opacity: false,

            // Verify style float existence
            // (IE uses styleFloat instead of cssFloat)
            cssFloat: false,

            // Make sure that if no value is specified for a checkbox
            // that it defaults to "on".
            // (WebKit defaults to "" instead)
            checkOn: false,

            // Make sure that a selected-by-default option has a working selected property.
            // (WebKit defaults to false instead of true, IE too, if it's in an optgroup)
            optSelected: false,

            // Will be defined later
            deleteExpando: false,
            optDisabled: false,
            checkClone: false,
            scriptEval: false,
            noCloneEvent: false,
            boxModel: false,
            inlineBlockNeedsLayout: false,
            shrinkWrapBlocks: false,
            reliableHiddenOffsets: true
        };

        //	// Make sure that the options inside disabled selects aren't marked as disabled
        //	// (WebKit marks them as diabled)
        //	select.disabled = true;
        //	jQuery.support.optDisabled = !opt.disabled;

        //	script.type = "text/javascript";
        //	try {
        //		script.appendChild( document.createTextNode( "window." + id + "=1;" ) );
        //	} catch(e) {}

        //	root.insertBefore( script, root.firstChild );

        //	// Make sure that the execution of code works by injecting a script
        //	// tag with appendChild/createTextNode
        //	// (IE doesn't support this, fails, and uses .text instead)
        //	if ( window[ id ] ) {
        //		jQuery.support.scriptEval = true;
        //		delete window[ id ];
        //	}

        //	// Test to see if it's possible to delete an expando from an element
        //	// Fails in Internet Explorer
        //	try {
        //		delete script.test;

        //	} catch(e) {
        //		jQuery.support.deleteExpando = false;
        //	}

        //	root.removeChild( script );

        //	if ( div.attachEvent && div.fireEvent ) {
        //		div.attachEvent("onclick", function click() {
        //			// Cloning a node shouldn't copy over any
        //			// bound event handlers (IE does this)
        //			jQuery.support.noCloneEvent = false;
        //			div.detachEvent("onclick", click);
        //		});
        //		div.cloneNode(true).fireEvent("onclick");
        //	}

        //	div = document.createElement("div");
        //	div.innerHTML = "<input type='radio' name='radiotest' checked='checked'/>";

        //	var fragment = document.createDocumentFragment();
        //	fragment.appendChild( div.firstChild );

        //	// WebKit doesn't clone checked state correctly in fragments
        //	jQuery.support.checkClone = fragment.cloneNode(true).cloneNode(true).lastChild.checked;

        //	// Figure out if the W3C box model works as expected
        //	// document.body must exist before we can do this
        //	jQuery(function() {
        //		var div = document.createElement("div");
        //		div.style.width = div.style.paddingLeft = "1px";

        //		document.body.appendChild( div );
        //		jQuery.boxModel = jQuery.support.boxModel = div.offsetWidth === 2;

        //		if ( "zoom" in div.style ) {
        //			// Check if natively block-level elements act like inline-block
        //			// elements when setting their display to 'inline' and giving
        //			// them layout
        //			// (IE < 8 does this)
        //			div.style.display = "inline";
        //			div.style.zoom = 1;
        //			jQuery.support.inlineBlockNeedsLayout = div.offsetWidth === 2;

        //			// Check if elements with layout shrink-wrap their children
        //			// (IE 6 does this)
        //			div.style.display = "";
        //			div.innerHTML = "<div style='width:4px;'></div>";
        //			jQuery.support.shrinkWrapBlocks = div.offsetWidth !== 2;
        //		}

        //		div.innerHTML = "<table><tr><td style='padding:0;display:none'></td><td>t</td></tr></table>";
        //		var tds = div.getElementsByTagName("td");

        //		// Check if table cells still have offsetWidth/Height when they are set
        //		// to display:none and there are still other visible table cells in a
        //		// table row; if so, offsetWidth/Height are not reliable for use when
        //		// determining if an element has been hidden directly using
        //		// display:none (it is still safe to use offsets if a parent element is
        //		// hidden; don safety goggles and see bug #4512 for more information).
        //		// (only IE 8 fails this test)
        //		jQuery.support.reliableHiddenOffsets = tds[0].offsetHeight === 0;

        //		tds[0].style.display = "";
        //		tds[1].style.display = "none";

        //		// Check if empty table cells still have offsetWidth/Height
        //		// (IE < 8 fail this test)
        //		jQuery.support.reliableHiddenOffsets = jQuery.support.reliableHiddenOffsets && tds[0].offsetHeight === 0;
        //		div.innerHTML = "";

        //		document.body.removeChild( div ).style.display = "none";
        //		div = tds = null;
        //	});

        //	// Technique from Juriy Zaytsev
        //	// http://thinkweb2.com/projects/prototype/detecting-event-support-without-browser-sniffing/
        //	var eventSupported = function( eventName ) {
        //		var el = document.createElement("div");
        //		eventName = "on" + eventName;

        //		var isSupported = (eventName in el);
        //		if ( !isSupported ) {
        //			el.setAttribute(eventName, "return;");
        //			isSupported = typeof el[eventName] === "function";
        //		}
        //		el = null;

        //		return isSupported;
        //	};

        jQuery.support.submitBubbles = false;
        jQuery.support.changeBubbles = false;

        //	// release memory in IE
        //	root = script = div = all = a = null;
    })();



    var windowData = {},
	rbrace = /^(?:\{.*\}|\[.*\])$/;

    jQuery.extend({
        cache: {},

        // Please use with caution
        uuid: 0,

        // Unique for each copy of jQuery on the page	
        expando: "jQuery" + jQuery.now(),

        // The following elements throw uncatchable exceptions if you
        // attempt to add expando properties to them.
        noData: {
            "embed": true,
            // Ban all objects except for Flash (which handle expandos)
            "object": "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000",
            "applet": true
        },

        data: function (elem, name, data) {
            ///	<summary>
            ///     &#10;Store arbitrary data associated with the specified element.
            ///	</summary>
            ///	<param name="elem" type="Element">
            ///     &#10;The DOM element to associate with the data.
            ///	</param>
            ///	<param name="name" type="String">
            ///     &#10;A string naming the piece of data to set.
            ///	</param>
            ///	<param name="value" type="Object">
            ///     &#10;The new data value.
            ///	</param>
            ///	<returns type="jQuery" />

            if (!jQuery.acceptData(elem)) {
                return;
            }

            elem = elem == window ?
			windowData :
			elem;

            var isNode = elem.nodeType,
			id = isNode ? elem[jQuery.expando] : null,
			cache = jQuery.cache, thisCache;

            if (isNode && !id && typeof name === "string" && data === undefined) {
                return;
            }

            // Get the data from the object directly
            if (!isNode) {
                cache = elem;

                // Compute a unique ID for the element
            } else if (!id) {
                elem[jQuery.expando] = id = ++jQuery.uuid;
            }

            // Avoid generating a new cache unless none exists and we
            // want to manipulate it.
            if (typeof name === "object") {
                if (isNode) {
                    cache[id] = jQuery.extend(cache[id], name);

                } else {
                    jQuery.extend(cache, name);
                }

            } else if (isNode && !cache[id]) {
                cache[id] = {};
            }

            thisCache = isNode ? cache[id] : cache;

            // Prevent overriding the named cache with undefined values
            if (data !== undefined) {
                thisCache[name] = data;
            }

            return typeof name === "string" ? thisCache[name] : thisCache;
        },

        removeData: function (elem, name) {
            if (!jQuery.acceptData(elem)) {
                return;
            }

            elem = elem == window ?
			windowData :
			elem;

            var isNode = elem.nodeType,
			id = isNode ? elem[jQuery.expando] : elem,
			cache = jQuery.cache,
			thisCache = isNode ? cache[id] : id;

            // If we want to remove a specific section of the element's data
            if (name) {
                if (thisCache) {
                    // Remove the section of cache data
                    delete thisCache[name];

                    // If we've removed all the data, remove the element's cache
                    if (isNode && jQuery.isEmptyObject(thisCache)) {
                        jQuery.removeData(elem);
                    }
                }

                // Otherwise, we want to remove all of the element's data
            } else {
                if (isNode && jQuery.support.deleteExpando) {
                    delete elem[jQuery.expando];

                } else if (elem.removeAttribute) {
                    elem.removeAttribute(jQuery.expando);

                    // Completely remove the data cache
                } else if (isNode) {
                    delete cache[id];

                    // Remove all fields from the object
                } else {
                    for (var n in elem) {
                        delete elem[n];
                    }
                }
            }
        },

        // A method for determining if a DOM node can handle the data expando
        acceptData: function (elem) {
            if (elem.nodeName) {
                var match = jQuery.noData[elem.nodeName.toLowerCase()];

                if (match) {
                    return !(match === true || elem.getAttribute("classid") !== match);
                }
            }

            return true;
        }
    });

    jQuery.fn.extend({
        data: function (key, value) {
            ///	<summary>
            ///     &#10;Store arbitrary data associated with the matched elements.
            ///	</summary>
            ///	<param name="key" type="String">
            ///     &#10;A string naming the piece of data to set.
            ///	</param>
            ///	<param name="value" type="Object">
            ///     &#10;The new data value.
            ///	</param>
            ///	<returns type="jQuery" />

            var data = null;

            if (typeof key === "undefined") {
                if (this.length) {
                    var attr = this[0].attributes, name;
                    data = jQuery.data(this[0]);

                    for (var i = 0, l = attr.length; i < l; i++) {
                        name = attr[i].name;

                        if (name.indexOf("data-") === 0) {
                            name = name.substr(5);
                            dataAttr(this[0], name, data[name]);
                        }
                    }
                }

                return data;

            } else if (typeof key === "object") {
                return this.each(function () {
                    jQuery.data(this, key);
                });
            }

            var parts = key.split(".");
            parts[1] = parts[1] ? "." + parts[1] : "";

            if (value === undefined) {
                data = this.triggerHandler("getData" + parts[1] + "!", [parts[0]]);

                // Try to fetch any internally stored data first
                if (data === undefined && this.length) {
                    data = jQuery.data(this[0], key);
                    data = dataAttr(this[0], key, data);
                }

                return data === undefined && parts[1] ?
				this.data(parts[0]) :
				data;

            } else {
                return this.each(function () {
                    var $this = jQuery(this),
					args = [parts[0], value];

                    $this.triggerHandler("setData" + parts[1] + "!", args);
                    jQuery.data(this, key, value);
                    $this.triggerHandler("changeData" + parts[1] + "!", args);
                });
            }
        },

        removeData: function (key) {
            return this.each(function () {
                jQuery.removeData(this, key);
            });
        }
    });

    function dataAttr(elem, key, data) {
        // If nothing was found internally, try to fetch any
        // data from the HTML5 data-* attribute
        if (data === undefined && elem.nodeType === 1) {
            data = elem.getAttribute("data-" + key);

            if (typeof data === "string") {
                try {
                    data = data === "true" ? true :
				data === "false" ? false :
				data === "null" ? null :
				!jQuery.isNaN(data) ? parseFloat(data) :
					rbrace.test(data) ? jQuery.parseJSON(data) :
					data;
                } catch (e) { }

                // Make sure we set the data so it isn't changed later
                jQuery.data(elem, key, data);

            } else {
                data = undefined;
            }
        }

        return data;
    }




    jQuery.extend({
        queue: function (elem, type, data) {
            if (!elem) {
                return;
            }

            type = (type || "fx") + "queue";
            var q = jQuery.data(elem, type);

            // Speed up dequeue by getting out quickly if this is just a lookup
            if (!data) {
                return q || [];
            }

            if (!q || jQuery.isArray(data)) {
                q = jQuery.data(elem, type, jQuery.makeArray(data));

            } else {
                q.push(data);
            }

            return q;
        },

        dequeue: function (elem, type) {
            type = type || "fx";

            var queue = jQuery.queue(elem, type),
			fn = queue.shift();

            // If the fx queue is dequeued, always remove the progress sentinel
            if (fn === "inprogress") {
                fn = queue.shift();
            }

            if (fn) {
                // Add a progress sentinel to prevent the fx queue from being
                // automatically dequeued
                if (type === "fx") {
                    queue.unshift("inprogress");
                }

                fn.call(elem, function () {
                    jQuery.dequeue(elem, type);
                });
            }
        }
    });

    jQuery.fn.extend({
        queue: function (type, data) {
            ///	<summary>
            ///     &#10;1: queue() - Returns a reference to the first element's queue (which is an array of functions).
            ///     &#10;2: queue(callback) - Adds a new function, to be executed, onto the end of the queue of all matched elements.
            ///     &#10;3: queue(queue) - Replaces the queue of all matched element with this new queue (the array of functions).
            ///	</summary>
            ///	<param name="type" type="Function">The function to add to the queue.</param>
            ///	<returns type="jQuery" />

            if (typeof type !== "string") {
                data = type;
                type = "fx";
            }

            if (data === undefined) {
                return jQuery.queue(this[0], type);
            }
            return this.each(function (i) {
                var queue = jQuery.queue(this, type, data);

                if (type === "fx" && queue[0] !== "inprogress") {
                    jQuery.dequeue(this, type);
                }
            });
        },
        dequeue: function (type) {
            ///	<summary>
            ///     &#10;Removes a queued function from the front of the queue and executes it.
            ///	</summary>
            ///	<param name="type" type="String" optional="true">The type of queue to access.</param>
            ///	<returns type="jQuery" />

            return this.each(function () {
                jQuery.dequeue(this, type);
            });
        },

        // Based off of the plugin by Clint Helfers, with permission.
        // http://blindsignals.com/index.php/2009/07/jquery-delay/
        delay: function (time, type) {
            ///	<summary>
            ///     &#10;Set a timer to delay execution of subsequent items in the queue.
            ///	</summary>
            ///	<param name="time" type="Number">
            ///     &#10;An integer indicating the number of milliseconds to delay execution of the next item in the queue.
            ///	</param>
            ///	<param name="type" type="String">
            ///     &#10;A string containing the name of the queue. Defaults to fx, the standard effects queue.
            ///	</param>
            ///	<returns type="jQuery" />

            time = jQuery.fx ? jQuery.fx.speeds[time] || time : time;
            type = type || "fx";

            return this.queue(type, function () {
                var elem = this;
                setTimeout(function () {
                    jQuery.dequeue(elem, type);
                }, time);
            });
        },

        clearQueue: function (type) {
            ///	<summary>
            ///     &#10;Remove from the queue all items that have not yet been run.
            ///	</summary>
            ///	<param name="type" type="String" optional="true">
            ///     &#10;A string containing the name of the queue. Defaults to fx, the standard effects queue.
            ///	</param>
            ///	<returns type="jQuery" />

            return this.queue(type || "fx", []);
        }
    });




    var rclass = /[\n\t]/g,
	rspaces = /\s+/,
	rreturn = /\r/g,
	rspecialurl = /^(?:href|src|style)$/,
	rtype = /^(?:button|input)$/i,
	rfocusable = /^(?:button|input|object|select|textarea)$/i,
	rclickable = /^a(?:rea)?$/i,
	rradiocheck = /^(?:radio|checkbox)$/i;

    jQuery.props = {
        "for": "htmlFor",
        "class": "className",
        readonly: "readOnly",
        maxlength: "maxLength",
        cellspacing: "cellSpacing",
        rowspan: "rowSpan",
        colspan: "colSpan",
        tabindex: "tabIndex",
        usemap: "useMap",
        frameborder: "frameBorder"
    };

    jQuery.fn.extend({
        attr: function (name, value) {
            ///	<summary>
            ///     &#10;Set a single property to a computed value, on all matched elements.
            ///     &#10;Instead of a value, a function is provided, that computes the value.
            ///     &#10;Part of DOM/Attributes
            ///	</summary>
            ///	<returns type="jQuery" />
            ///	<param name="name" type="String">
            ///     &#10;The name of the property to set.
            ///	</param>
            ///	<param name="value" type="Function">
            ///     &#10;A function returning the value to set.
            ///	</param>

            return jQuery.access(this, name, value, true, jQuery.attr);
        },

        removeAttr: function (name, fn) {
            ///	<summary>
            ///     &#10;Remove an attribute from each of the matched elements.
            ///     &#10;Part of DOM/Attributes
            ///	</summary>
            ///	<param name="name" type="String">
            ///     &#10;An attribute to remove.
            ///	</param>
            ///	<returns type="jQuery" />

            return this.each(function () {
                jQuery.attr(this, name, "");
                if (this.nodeType === 1) {
                    this.removeAttribute(name);
                }
            });
        },

        addClass: function (value) {
            ///	<summary>
            ///     &#10;Adds the specified class(es) to each of the set of matched elements.
            ///     &#10;Part of DOM/Attributes
            ///	</summary>
            ///	<param name="value" type="String">
            ///     &#10;One or more class names to be added to the class attribute of each matched element.
            ///	</param>
            ///	<returns type="jQuery" />

            if (jQuery.isFunction(value)) {
                return this.each(function (i) {
                    var self = jQuery(this);
                    self.addClass(value.call(this, i, self.attr("class")));
                });
            }

            if (value && typeof value === "string") {
                var classNames = (value || "").split(rspaces);

                for (var i = 0, l = this.length; i < l; i++) {
                    var elem = this[i];

                    if (elem.nodeType === 1) {
                        if (!elem.className) {
                            elem.className = value;

                        } else {
                            var className = " " + elem.className + " ",
							setClass = elem.className;

                            for (var c = 0, cl = classNames.length; c < cl; c++) {
                                if (className.indexOf(" " + classNames[c] + " ") < 0) {
                                    setClass += " " + classNames[c];
                                }
                            }
                            elem.className = jQuery.trim(setClass);
                        }
                    }
                }
            }

            return this;
        },

        removeClass: function (value) {
            ///	<summary>
            ///     &#10;Removes all or the specified class(es) from the set of matched elements.
            ///     &#10;Part of DOM/Attributes
            ///	</summary>
            ///	<param name="value" type="String" optional="true">
            ///     &#10;(Optional) A class name to be removed from the class attribute of each matched element.
            ///	</param>
            ///	<returns type="jQuery" />

            if (jQuery.isFunction(value)) {
                return this.each(function (i) {
                    var self = jQuery(this);
                    self.removeClass(value.call(this, i, self.attr("class")));
                });
            }

            if ((value && typeof value === "string") || value === undefined) {
                var classNames = (value || "").split(rspaces);

                for (var i = 0, l = this.length; i < l; i++) {
                    var elem = this[i];

                    if (elem.nodeType === 1 && elem.className) {
                        if (value) {
                            var className = (" " + elem.className + " ").replace(rclass, " ");
                            for (var c = 0, cl = classNames.length; c < cl; c++) {
                                className = className.replace(" " + classNames[c] + " ", " ");
                            }
                            elem.className = jQuery.trim(className);

                        } else {
                            elem.className = "";
                        }
                    }
                }
            }

            return this;
        },

        toggleClass: function (value, stateVal) {
            ///	<summary>
            ///     &#10;Add or remove a class from each element in the set of matched elements, depending
            ///     &#10;on either the class's presence or the value of the switch argument.
            ///	</summary>
            ///	<param name="value" type="Object">
            ///     &#10;A class name to be toggled for each element in the matched set.
            ///	</param>
            ///	<param name="stateVal" type="Object">
            ///     &#10;A boolean value to determine whether the class should be added or removed.
            ///	</param>
            ///	<returns type="jQuery" />

            var type = typeof value,
			isBool = typeof stateVal === "boolean";

            if (jQuery.isFunction(value)) {
                return this.each(function (i) {
                    var self = jQuery(this);
                    self.toggleClass(value.call(this, i, self.attr("class"), stateVal), stateVal);
                });
            }

            return this.each(function () {
                if (type === "string") {
                    // toggle individual class names
                    var className,
					i = 0,
					self = jQuery(this),
					state = stateVal,
					classNames = value.split(rspaces);

                    while ((className = classNames[i++])) {
                        // check each className given, space seperated list
                        state = isBool ? state : !self.hasClass(className);
                        self[state ? "addClass" : "removeClass"](className);
                    }

                } else if (type === "undefined" || type === "boolean") {
                    if (this.className) {
                        // store className if set
                        jQuery.data(this, "__className__", this.className);
                    }

                    // toggle whole className
                    this.className = this.className || value === false ? "" : jQuery.data(this, "__className__") || "";
                }
            });
        },

        hasClass: function (selector) {
            ///	<summary>
            ///     &#10;Checks the current selection against a class and returns whether at least one selection has a given class.
            ///	</summary>
            ///	<param name="selector" type="String">The class to check against</param>
            ///	<returns type="Boolean">True if at least one element in the selection has the class, otherwise false.</returns>

            var className = " " + selector + " ";
            for (var i = 0, l = this.length; i < l; i++) {
                if ((" " + this[i].className + " ").replace(rclass, " ").indexOf(className) > -1) {
                    return true;
                }
            }

            return false;
        },

        val: function (value) {
            ///	<summary>
            ///     &#10;Set the value of every matched element.
            ///     &#10;Part of DOM/Attributes
            ///	</summary>
            ///	<returns type="jQuery" />
            ///	<param name="value" type="String">
            ///     &#10;A string of text or an array of strings to set as the value property of each
            ///     &#10;matched element.
            ///	</param>

            if (!arguments.length) {
                var elem = this[0];

                if (elem) {
                    if (jQuery.nodeName(elem, "option")) {
                        // attributes.value is undefined in Blackberry 4.7 but
                        // uses .value. See #6932
                        var val = elem.attributes.value;
                        return !val || val.specified ? elem.value : elem.text;
                    }

                    // We need to handle select boxes special
                    if (jQuery.nodeName(elem, "select")) {
                        var index = elem.selectedIndex,
						values = [],
						options = elem.options,
						one = elem.type === "select-one";

                        // Nothing was selected
                        if (index < 0) {
                            return null;
                        }

                        // Loop through all the selected options
                        for (var i = one ? index : 0, max = one ? index + 1 : options.length; i < max; i++) {
                            var option = options[i];

                            // Don't return options that are disabled or in a disabled optgroup
                            if (option.selected && (jQuery.support.optDisabled ? !option.disabled : option.getAttribute("disabled") === null) &&
								(!option.parentNode.disabled || !jQuery.nodeName(option.parentNode, "optgroup"))) {

                                // Get the specific value for the option
                                value = jQuery(option).val();

                                // We don't need an array for one selects
                                if (one) {
                                    return value;
                                }

                                // Multi-Selects return an array
                                values.push(value);
                            }
                        }

                        return values;
                    }

                    // Handle the case where in Webkit "" is returned instead of "on" if a value isn't specified
                    if (rradiocheck.test(elem.type) && !jQuery.support.checkOn) {
                        return elem.getAttribute("value") === null ? "on" : elem.value;
                    }


                    // Everything else, we just grab the value
                    return (elem.value || "").replace(rreturn, "");

                }

                return undefined;
            }

            var isFunction = jQuery.isFunction(value);

            return this.each(function (i) {
                var self = jQuery(this), val = value;

                if (this.nodeType !== 1) {
                    return;
                }

                if (isFunction) {
                    val = value.call(this, i, self.val());
                }

                // Treat null/undefined as ""; convert numbers to string
                if (val == null) {
                    val = "";
                } else if (typeof val === "number") {
                    val += "";
                } else if (jQuery.isArray(val)) {
                    val = jQuery.map(val, function (value) {
                        return value == null ? "" : value + "";
                    });
                }

                if (jQuery.isArray(val) && rradiocheck.test(this.type)) {
                    this.checked = jQuery.inArray(self.val(), val) >= 0;

                } else if (jQuery.nodeName(this, "select")) {
                    var values = jQuery.makeArray(val);

                    jQuery("option", this).each(function () {
                        this.selected = jQuery.inArray(jQuery(this).val(), values) >= 0;
                    });

                    if (!values.length) {
                        this.selectedIndex = -1;
                    }

                } else {
                    this.value = val;
                }
            });
        }
    });

    jQuery.extend({
        attrFn: {
            val: true,
            css: true,
            html: true,
            text: true,
            data: true,
            width: true,
            height: true,
            offset: true
        },

        attr: function (elem, name, value, pass) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            // don't set attributes on text and comment nodes
            if (!elem || elem.nodeType === 3 || elem.nodeType === 8) {
                return undefined;
            }

            if (pass && name in jQuery.attrFn) {
                return jQuery(elem)[name](value);
            }

            var notxml = elem.nodeType !== 1 || !jQuery.isXMLDoc(elem),
            // Whether we are setting (or getting)
			set = value !== undefined;

            // Try to normalize/fix the name
            name = notxml && jQuery.props[name] || name;

            // These attributes require special treatment
            var special = rspecialurl.test(name);

            // Safari mis-reports the default selected property of an option
            // Accessing the parent's selectedIndex property fixes it
            if (name === "selected" && !jQuery.support.optSelected) {
                var parent = elem.parentNode;
                if (parent) {
                    parent.selectedIndex;

                    // Make sure that it also works with optgroups, see #5701
                    if (parent.parentNode) {
                        parent.parentNode.selectedIndex;
                    }
                }
            }

            // If applicable, access the attribute via the DOM 0 way
            // 'in' checks fail in Blackberry 4.7 #6931
            if ((name in elem || elem[name] !== undefined) && notxml && !special) {
                if (set) {
                    // We can't allow the type property to be changed (since it causes problems in IE)
                    if (name === "type" && rtype.test(elem.nodeName) && elem.parentNode) {
                        jQuery.error("type property can't be changed");
                    }

                    if (value === null) {
                        if (elem.nodeType === 1) {
                            elem.removeAttribute(name);
                        }

                    } else {
                        elem[name] = value;
                    }
                }

                // browsers index elements by id/name on forms, give priority to attributes.
                if (jQuery.nodeName(elem, "form") && elem.getAttributeNode(name)) {
                    return elem.getAttributeNode(name).nodeValue;
                }

                // elem.tabIndex doesn't always return the correct value when it hasn't been explicitly set
                // http://fluidproject.org/blog/2008/01/09/getting-setting-and-removing-tabindex-values-with-javascript/
                if (name === "tabIndex") {
                    var attributeNode = elem.getAttributeNode("tabIndex");

                    return attributeNode && attributeNode.specified ?
					attributeNode.value :
					rfocusable.test(elem.nodeName) || rclickable.test(elem.nodeName) && elem.href ?
						0 :
						undefined;
                }

                return elem[name];
            }

            if (!jQuery.support.style && notxml && name === "style") {
                if (set) {
                    elem.style.cssText = "" + value;
                }

                return elem.style.cssText;
            }

            if (set) {
                // convert the value to a string (all browsers do this but IE) see #1070
                elem.setAttribute(name, "" + value);
            }

            // Ensure that missing attributes return undefined
            // Blackberry 4.7 returns "" from getAttribute #6938
            if (!elem.attributes[name] && (elem.hasAttribute && !elem.hasAttribute(name))) {
                return undefined;
            }

            var attr = !jQuery.support.hrefNormalized && notxml && special ?
            // Some attributes require a special call on IE
				elem.getAttribute(name, 2) :
				elem.getAttribute(name);

            // Non-existent attributes return null, we normalize to undefined
            return attr === null ? undefined : attr;
        }
    });




    var rnamespaces = /\.(.*)$/,
	rformElems = /^(?:textarea|input|select)$/i,
	rperiod = /\./g,
	rspace = / /g,
	rescape = /[^\w\s.|`]/g,
	fcleanup = function (nm) {
	    return nm.replace(rescape, "\\$&");
	},
	focusCounts = { focusin: 0, focusout: 0 };

    /*
    * A number of helper functions used for managing events.
    * Many of the ideas behind this code originated from
    * Dean Edwards' addEvent library.
    */
    jQuery.event = {

        // Bind an event to an element
        // Original by Dean Edwards
        add: function (elem, types, handler, data) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            if (elem.nodeType === 3 || elem.nodeType === 8) {
                return;
            }

            // For whatever reason, IE has trouble passing the window object
            // around, causing it to be cloned in the process
            if (jQuery.isWindow(elem) && (elem !== window && !elem.frameElement)) {
                elem = window;
            }

            if (handler === false) {
                handler = returnFalse;
            } else if (!handler) {
                // Fixes bug #7229. Fix recommended by jdalton
                return;
            }

            var handleObjIn, handleObj;

            if (handler.handler) {
                handleObjIn = handler;
                handler = handleObjIn.handler;
            }

            // Make sure that the function being executed has a unique ID
            if (!handler.guid) {
                handler.guid = jQuery.guid++;
            }

            // Init the element's event structure
            var elemData = jQuery.data(elem);

            // If no elemData is found then we must be trying to bind to one of the
            // banned noData elements
            if (!elemData) {
                return;
            }

            // Use a key less likely to result in collisions for plain JS objects.
            // Fixes bug #7150.
            var eventKey = elem.nodeType ? "events" : "__events__",
			events = elemData[eventKey],
			eventHandle = elemData.handle;

            if (typeof events === "function") {
                // On plain objects events is a fn that holds the the data
                // which prevents this data from being JSON serialized
                // the function does not need to be called, it just contains the data
                eventHandle = events.handle;
                events = events.events;

            } else if (!events) {
                if (!elem.nodeType) {
                    // On plain objects, create a fn that acts as the holder
                    // of the values to avoid JSON serialization of event data
                    elemData[eventKey] = elemData = function () { };
                }

                elemData.events = events = {};
            }

            if (!eventHandle) {
                elemData.handle = eventHandle = function () {
                    // Handle the second event of a trigger and when
                    // an event is called after a page has unloaded
                    return typeof jQuery !== "undefined" && !jQuery.event.triggered ?
					jQuery.event.handle.apply(eventHandle.elem, arguments) :
					undefined;
                };
            }

            // Add elem as a property of the handle function
            // This is to prevent a memory leak with non-native events in IE.
            eventHandle.elem = elem;

            // Handle multiple events separated by a space
            // jQuery(...).bind("mouseover mouseout", fn);
            types = types.split(" ");

            var type, i = 0, namespaces;

            while ((type = types[i++])) {
                handleObj = handleObjIn ?
				jQuery.extend({}, handleObjIn) :
				{ handler: handler, data: data };

                // Namespaced event handlers
                if (type.indexOf(".") > -1) {
                    namespaces = type.split(".");
                    type = namespaces.shift();
                    handleObj.namespace = namespaces.slice(0).sort().join(".");

                } else {
                    namespaces = [];
                    handleObj.namespace = "";
                }

                handleObj.type = type;
                if (!handleObj.guid) {
                    handleObj.guid = handler.guid;
                }

                // Get the current list of functions bound to this event
                var handlers = events[type],
				special = jQuery.event.special[type] || {};

                // Init the event handler queue
                if (!handlers) {
                    handlers = events[type] = [];

                    // Check for a special event handler
                    // Only use addEventListener/attachEvent if the special
                    // events handler returns false
                    if (!special.setup || special.setup.call(elem, data, namespaces, eventHandle) === false) {
                        // Bind the global event handler to the element
                        if (elem.addEventListener) {
                            elem.addEventListener(type, eventHandle, false);

                        } else if (elem.attachEvent) {
                            elem.attachEvent("on" + type, eventHandle);
                        }
                    }
                }

                if (special.add) {
                    special.add.call(elem, handleObj);

                    if (!handleObj.handler.guid) {
                        handleObj.handler.guid = handler.guid;
                    }
                }

                // Add the function to the element's handler list
                handlers.push(handleObj);

                // Keep track of which events have been used, for global triggering
                jQuery.event.global[type] = true;
            }

            // Nullify elem to prevent memory leaks in IE
            elem = null;
        },

        global: {},

        // Detach an event or set of events from an element
        remove: function (elem, types, handler) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            // don't do events on text and comment nodes
            if (elem.nodeType === 3 || elem.nodeType === 8) {
                return;
            }

            if (handler === false) {
                handler = returnFalse;
            }

            var ret, type, fn, j, i = 0, all, namespaces, namespace, special, eventType, handleObj, origType,
			eventKey = elem.nodeType ? "events" : "__events__",
			elemData = jQuery.data(elem),
			events = elemData && elemData[eventKey];

            if (!elemData || !events) {
                return;
            }

            if (typeof events === "function") {
                elemData = events;
                events = events.events;
            }

            // types is actually an event object here
            if (types && types.type) {
                handler = types.handler;
                types = types.type;
            }

            // Unbind all events for the element
            if (!types || typeof types === "string" && types.charAt(0) === ".") {
                types = types || "";

                for (type in events) {
                    jQuery.event.remove(elem, type + types);
                }

                return;
            }

            // Handle multiple events separated by a space
            // jQuery(...).unbind("mouseover mouseout", fn);
            types = types.split(" ");

            while ((type = types[i++])) {
                origType = type;
                handleObj = null;
                all = type.indexOf(".") < 0;
                namespaces = [];

                if (!all) {
                    // Namespaced event handlers
                    namespaces = type.split(".");
                    type = namespaces.shift();

                    namespace = new RegExp("(^|\\.)" +
					jQuery.map(namespaces.slice(0).sort(), fcleanup).join("\\.(?:.*\\.)?") + "(\\.|$)");
                }

                eventType = events[type];

                if (!eventType) {
                    continue;
                }

                if (!handler) {
                    for (j = 0; j < eventType.length; j++) {
                        handleObj = eventType[j];

                        if (all || namespace.test(handleObj.namespace)) {
                            jQuery.event.remove(elem, origType, handleObj.handler, j);
                            eventType.splice(j--, 1);
                        }
                    }

                    continue;
                }

                special = jQuery.event.special[type] || {};

                for (j = pos || 0; j < eventType.length; j++) {
                    handleObj = eventType[j];

                    if (handler.guid === handleObj.guid) {
                        // remove the given handler for the given type
                        if (all || namespace.test(handleObj.namespace)) {
                            if (pos == null) {
                                eventType.splice(j--, 1);
                            }

                            if (special.remove) {
                                special.remove.call(elem, handleObj);
                            }
                        }

                        if (pos != null) {
                            break;
                        }
                    }
                }

                // remove generic event handler if no more handlers exist
                if (eventType.length === 0 || pos != null && eventType.length === 1) {
                    if (!special.teardown || special.teardown.call(elem, namespaces) === false) {
                        jQuery.removeEvent(elem, type, elemData.handle);
                    }

                    ret = null;
                    delete events[type];
                }
            }

            // Remove the expando if it's no longer used
            if (jQuery.isEmptyObject(events)) {
                var handle = elemData.handle;
                if (handle) {
                    handle.elem = null;
                }

                delete elemData.events;
                delete elemData.handle;

                if (typeof elemData === "function") {
                    jQuery.removeData(elem, eventKey);

                } else if (jQuery.isEmptyObject(elemData)) {
                    jQuery.removeData(elem);
                }
            }
        },

        // bubbling is internal
        trigger: function (event, data, elem /*, bubbling */) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            // Event object or event type
            var type = event.type || event,
			bubbling = arguments[3];

            if (!bubbling) {
                event = typeof event === "object" ?
                // jQuery.Event object
				event[jQuery.expando] ? event :
                // Object literal
				jQuery.extend(jQuery.Event(type), event) :
                // Just the event type (string)
				jQuery.Event(type);

                if (type.indexOf("!") >= 0) {
                    event.type = type = type.slice(0, -1);
                    event.exclusive = true;
                }

                // Handle a global trigger
                if (!elem) {
                    // Don't bubble custom events when global (to avoid too much overhead)
                    event.stopPropagation();

                    // Only trigger if we've ever bound an event for it
                    if (jQuery.event.global[type]) {
                        jQuery.each(jQuery.cache, function () {
                            if (this.events && this.events[type]) {
                                jQuery.event.trigger(event, data, this.handle.elem);
                            }
                        });
                    }
                }

                // Handle triggering a single element

                // don't do events on text and comment nodes
                if (!elem || elem.nodeType === 3 || elem.nodeType === 8) {
                    return undefined;
                }

                // Clean up in case it is reused
                event.result = undefined;
                event.target = elem;

                // Clone the incoming data, if any
                data = jQuery.makeArray(data);
                data.unshift(event);
            }

            event.currentTarget = elem;

            // Trigger the event, it is assumed that "handle" is a function
            var handle = elem.nodeType ?
			jQuery.data(elem, "handle") :
			(jQuery.data(elem, "__events__") || {}).handle;

            if (handle) {
                handle.apply(elem, data);
            }

            var parent = elem.parentNode || elem.ownerDocument;

            // Trigger an inline bound script
            try {
                if (!(elem && elem.nodeName && jQuery.noData[elem.nodeName.toLowerCase()])) {
                    if (elem["on" + type] && elem["on" + type].apply(elem, data) === false) {
                        event.result = false;
                        event.preventDefault();
                    }
                }

                // prevent IE from throwing an error for some elements with some event types, see #3533
            } catch (inlineError) { }

            if (!event.isPropagationStopped() && parent) {
                jQuery.event.trigger(event, data, parent, true);

            } else if (!event.isDefaultPrevented()) {
                var old,
				target = event.target,
				targetType = type.replace(rnamespaces, ""),
				isClick = jQuery.nodeName(target, "a") && targetType === "click",
				special = jQuery.event.special[targetType] || {};

                if ((!special._default || special._default.call(elem, event) === false) &&
				!isClick && !(target && target.nodeName && jQuery.noData[target.nodeName.toLowerCase()])) {

                    try {
                        if (target[targetType]) {
                            // Make sure that we don't accidentally re-trigger the onFOO events
                            old = target["on" + targetType];

                            if (old) {
                                target["on" + targetType] = null;
                            }

                            jQuery.event.triggered = true;
                            target[targetType]();
                        }

                        // prevent IE from throwing an error for some elements with some event types, see #3533
                    } catch (triggerError) { }

                    if (old) {
                        target["on" + targetType] = old;
                    }

                    jQuery.event.triggered = false;
                }
            }
        },

        handle: function (event) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            var all, handlers, namespaces, namespace_re, events,
			namespace_sort = [],
			args = jQuery.makeArray(arguments);

            event = args[0] = jQuery.event.fix(event || window.event);
            event.currentTarget = this;

            // Namespaced event handlers
            all = event.type.indexOf(".") < 0 && !event.exclusive;

            if (!all) {
                namespaces = event.type.split(".");
                event.type = namespaces.shift();
                namespace_sort = namespaces.slice(0).sort();
                namespace_re = new RegExp("(^|\\.)" + namespace_sort.join("\\.(?:.*\\.)?") + "(\\.|$)");
            }

            event.namespace = event.namespace || namespace_sort.join(".");

            events = jQuery.data(this, this.nodeType ? "events" : "__events__");

            if (typeof events === "function") {
                events = events.events;
            }

            handlers = (events || {})[event.type];

            if (events && handlers) {
                // Clone the handlers to prevent manipulation
                handlers = handlers.slice(0);

                for (var j = 0, l = handlers.length; j < l; j++) {
                    var handleObj = handlers[j];

                    // Filter the functions by class
                    if (all || namespace_re.test(handleObj.namespace)) {
                        // Pass in a reference to the handler function itself
                        // So that we can later remove it
                        event.handler = handleObj.handler;
                        event.data = handleObj.data;
                        event.handleObj = handleObj;

                        var ret = handleObj.handler.apply(this, args);

                        if (ret !== undefined) {
                            event.result = ret;
                            if (ret === false) {
                                event.preventDefault();
                                event.stopPropagation();
                            }
                        }

                        if (event.isImmediatePropagationStopped()) {
                            break;
                        }
                    }
                }
            }

            return event.result;
        },

        props: "altKey attrChange attrName bubbles button cancelable charCode clientX clientY ctrlKey currentTarget data detail eventPhase fromElement handler keyCode layerX layerY metaKey newValue offsetX offsetY pageX pageY prevValue relatedNode relatedTarget screenX screenY shiftKey srcElement target toElement view wheelDelta which".split(" "),

        fix: function (event) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            if (event[jQuery.expando]) {
                return event;
            }

            // store a copy of the original event object
            // and "clone" to set read-only properties
            var originalEvent = event;
            event = jQuery.Event(originalEvent);

            for (var i = this.props.length, prop; i; ) {
                prop = this.props[--i];
                event[prop] = originalEvent[prop];
            }

            // Fix target property, if necessary
            if (!event.target) {
                // Fixes #1925 where srcElement might not be defined either
                event.target = event.srcElement || document;
            }

            // check if target is a textnode (safari)
            if (event.target.nodeType === 3) {
                event.target = event.target.parentNode;
            }

            // Add relatedTarget, if necessary
            if (!event.relatedTarget && event.fromElement) {
                event.relatedTarget = event.fromElement === event.target ? event.toElement : event.fromElement;
            }

            // Calculate pageX/Y if missing and clientX/Y available
            if (event.pageX == null && event.clientX != null) {
                var doc = document.documentElement,
				body = document.body;

                event.pageX = event.clientX + (doc && doc.scrollLeft || body && body.scrollLeft || 0) - (doc && doc.clientLeft || body && body.clientLeft || 0);
                event.pageY = event.clientY + (doc && doc.scrollTop || body && body.scrollTop || 0) - (doc && doc.clientTop || body && body.clientTop || 0);
            }

            // Add which for key events
            if (event.which == null && (event.charCode != null || event.keyCode != null)) {
                event.which = event.charCode != null ? event.charCode : event.keyCode;
            }

            // Add metaKey to non-Mac browsers (use ctrl for PC's and Meta for Macs)
            if (!event.metaKey && event.ctrlKey) {
                event.metaKey = event.ctrlKey;
            }

            // Add which for click: 1 === left; 2 === middle; 3 === right
            // Note: button is not normalized, so don't use it
            if (!event.which && event.button !== undefined) {
                event.which = (event.button & 1 ? 1 : (event.button & 2 ? 3 : (event.button & 4 ? 2 : 0)));
            }

            return event;
        },

        // Deprecated, use jQuery.guid instead
        guid: 1E8,

        // Deprecated, use jQuery.proxy instead
        proxy: jQuery.proxy,

        special: {
            ready: {
                // Make sure the ready event is setup
                setup: jQuery.bindReady,
                teardown: jQuery.noop
            },

            live: {
                add: function (handleObj) {
                    jQuery.event.add(this,
					liveConvert(handleObj.origType, handleObj.selector),
					jQuery.extend({}, handleObj, { handler: liveHandler, guid: handleObj.handler.guid }));
                },

                remove: function (handleObj) {
                    jQuery.event.remove(this, liveConvert(handleObj.origType, handleObj.selector), handleObj);
                }
            },

            beforeunload: {
                setup: function (data, namespaces, eventHandle) {
                    // We only want to do this special case on windows
                    if (jQuery.isWindow(this)) {
                        this.onbeforeunload = eventHandle;
                    }
                },

                teardown: function (namespaces, eventHandle) {
                    if (this.onbeforeunload === eventHandle) {
                        this.onbeforeunload = null;
                    }
                }
            }
        }
    };

    jQuery.removeEvent = document.removeEventListener ?
	function (elem, type, handle) {
	    if (elem.removeEventListener) {
	        elem.removeEventListener(type, handle, false);
	    }
	} :
	function (elem, type, handle) {
	    if (elem.detachEvent) {
	        elem.detachEvent("on" + type, handle);
	    }
	};

    jQuery.Event = function (src) {
        // Allow instantiation without the 'new' keyword
        if (!this.preventDefault) {
            return new jQuery.Event(src);
        }

        // Event object
        if (src && src.type) {
            this.originalEvent = src;
            this.type = src.type;
            // Event type
        } else {
            this.type = src;
        }

        // timeStamp is buggy for some events on Firefox(#3843)
        // So we won't rely on the native value
        this.timeStamp = jQuery.now();

        // Mark it as fixed
        this[jQuery.expando] = true;
    };

    function returnFalse() {
        return false;
    }
    function returnTrue() {
        return true;
    }

    // jQuery.Event is based on DOM3 Events as specified by the ECMAScript Language Binding
    // http://www.w3.org/TR/2003/WD-DOM-Level-3-Events-20030331/ecma-script-binding.html
    jQuery.Event.prototype = {
        preventDefault: function () {
            this.isDefaultPrevented = returnTrue;

            var e = this.originalEvent;
            if (!e) {
                return;
            }

            // if preventDefault exists run it on the original event
            if (e.preventDefault) {
                e.preventDefault();

                // otherwise set the returnValue property of the original event to false (IE)
            } else {
                e.returnValue = false;
            }
        },
        stopPropagation: function () {
            this.isPropagationStopped = returnTrue;

            var e = this.originalEvent;
            if (!e) {
                return;
            }
            // if stopPropagation exists run it on the original event
            if (e.stopPropagation) {
                e.stopPropagation();
            }
            // otherwise set the cancelBubble property of the original event to true (IE)
            e.cancelBubble = true;
        },
        stopImmediatePropagation: function () {
            this.isImmediatePropagationStopped = returnTrue;
            this.stopPropagation();
        },
        isDefaultPrevented: returnFalse,
        isPropagationStopped: returnFalse,
        isImmediatePropagationStopped: returnFalse
    };

    // Checks if an event happened on an element within another element
    // Used in jQuery.event.special.mouseenter and mouseleave handlers
    var withinElement = function (event) {
        // Check if mouse(over|out) are still within the same parent element
        var parent = event.relatedTarget;

        // Firefox sometimes assigns relatedTarget a XUL element
        // which we cannot access the parentNode property of
        try {
            // Traverse up the tree
            while (parent && parent !== this) {
                parent = parent.parentNode;
            }

            if (parent !== this) {
                // set the correct event type
                event.type = event.data;

                // handle event if we actually just moused on to a non sub-element
                jQuery.event.handle.apply(this, arguments);
            }

            // assuming we've left the element since we most likely mousedover a xul element
        } catch (e) { }
    },

    // In case of event delegation, we only need to rename the event.type,
    // liveHandler will take care of the rest.
delegate = function (event) {
    event.type = event.data;
    jQuery.event.handle.apply(this, arguments);
};

    // Create mouseenter and mouseleave events
    jQuery.each({
        mouseenter: "mouseover",
        mouseleave: "mouseout"
    }, function (orig, fix) {
        jQuery.event.special[orig] = {
            setup: function (data) {
                jQuery.event.add(this, fix, data && data.selector ? delegate : withinElement, orig);
            },
            teardown: function (data) {
                jQuery.event.remove(this, fix, data && data.selector ? delegate : withinElement);
            }
        };
    });

    // submit delegation
    if (!jQuery.support.submitBubbles) {

        jQuery.event.special.submit = {
            setup: function (data, namespaces) {
                if (this.nodeName.toLowerCase() !== "form") {
                    jQuery.event.add(this, "click.specialSubmit", function (e) {
                        var elem = e.target,
						type = elem.type;

                        if ((type === "submit" || type === "image") && jQuery(elem).closest("form").length) {
                            e.liveFired = undefined;
                            return trigger("submit", this, arguments);
                        }
                    });

                    jQuery.event.add(this, "keypress.specialSubmit", function (e) {
                        var elem = e.target,
						type = elem.type;

                        if ((type === "text" || type === "password") && jQuery(elem).closest("form").length && e.keyCode === 13) {
                            e.liveFired = undefined;
                            return trigger("submit", this, arguments);
                        }
                    });

                } else {
                    return false;
                }
            },

            teardown: function (namespaces) {
                jQuery.event.remove(this, ".specialSubmit");
            }
        };

    }

    // change delegation, happens here so we have bind.
    if (!jQuery.support.changeBubbles) {

        var changeFilters,

	getVal = function (elem) {
	    var type = elem.type, val = elem.value;

	    if (type === "radio" || type === "checkbox") {
	        val = elem.checked;

	    } else if (type === "select-multiple") {
	        val = elem.selectedIndex > -1 ?
				jQuery.map(elem.options, function (elem) {
				    return elem.selected;
				}).join("-") :
				"";

	    } else if (elem.nodeName.toLowerCase() === "select") {
	        val = elem.selectedIndex;
	    }

	    return val;
	},

	testChange = function testChange(e) {
	    var elem = e.target, data, val;

	    if (!rformElems.test(elem.nodeName) || elem.readOnly) {
	        return;
	    }

	    data = jQuery.data(elem, "_change_data");
	    val = getVal(elem);

	    // the current data will be also retrieved by beforeactivate
	    if (e.type !== "focusout" || elem.type !== "radio") {
	        jQuery.data(elem, "_change_data", val);
	    }

	    if (data === undefined || val === data) {
	        return;
	    }

	    if (data != null || val) {
	        e.type = "change";
	        e.liveFired = undefined;
	        return jQuery.event.trigger(e, arguments[1], elem);
	    }
	};

        jQuery.event.special.change = {
            filters: {
                focusout: testChange,

                beforedeactivate: testChange,

                click: function (e) {
                    var elem = e.target, type = elem.type;

                    if (type === "radio" || type === "checkbox" || elem.nodeName.toLowerCase() === "select") {
                        return testChange.call(this, e);
                    }
                },

                // Change has to be called before submit
                // Keydown will be called before keypress, which is used in submit-event delegation
                keydown: function (e) {
                    var elem = e.target, type = elem.type;

                    if ((e.keyCode === 13 && elem.nodeName.toLowerCase() !== "textarea") ||
					(e.keyCode === 32 && (type === "checkbox" || type === "radio")) ||
					type === "select-multiple") {
                        return testChange.call(this, e);
                    }
                },

                // Beforeactivate happens also before the previous element is blurred
                // with this event you can't trigger a change event, but you can store
                // information
                beforeactivate: function (e) {
                    var elem = e.target;
                    jQuery.data(elem, "_change_data", getVal(elem));
                }
            },

            setup: function (data, namespaces) {
                if (this.type === "file") {
                    return false;
                }

                for (var type in changeFilters) {
                    jQuery.event.add(this, type + ".specialChange", changeFilters[type]);
                }

                return rformElems.test(this.nodeName);
            },

            teardown: function (namespaces) {
                jQuery.event.remove(this, ".specialChange");

                return rformElems.test(this.nodeName);
            }
        };

        changeFilters = jQuery.event.special.change.filters;

        // Handle when the input is .focus()'d
        changeFilters.focus = changeFilters.beforeactivate;
    }

    function trigger(type, elem, args) {
        args[0].type = type;
        return jQuery.event.handle.apply(elem, args);
    }

    // Create "bubbling" focus and blur events
    if (document.addEventListener) {
        jQuery.each({ focus: "focusin", blur: "focusout" }, function (orig, fix) {
            jQuery.event.special[fix] = {
                setup: function () {
                    ///	<summary>
                    ///     &#10;This method is internal.
                    ///	</summary>
                    ///	<private />

                    if (focusCounts[fix]++ === 0) {
                        document.addEventListener(orig, handler, true);
                    }
                },
                teardown: function () {
                    ///	<summary>
                    ///     &#10;This method is internal.
                    ///	</summary>
                    ///	<private />

                    if (--focusCounts[fix] === 0) {
                        document.removeEventListener(orig, handler, true);
                    }
                }
            };

            function handler(e) {
                e = jQuery.event.fix(e);
                e.type = fix;
                return jQuery.event.trigger(e, null, e.target);
            }
        });
    }

    //	jQuery.each(["bind", "one"], function( i, name ) {
    //		jQuery.fn[ name ] = function( type, data, fn ) {
    //			// Handle object literals
    //			if ( typeof type === "object" ) {
    //				for ( var key in type ) {
    //					this[ name ](key, data, type[key], fn);
    //				}
    //				return this;
    //			}

    //			if ( jQuery.isFunction( data ) || data === false ) {
    //				fn = data;
    //				data = undefined;
    //			}

    //			var handler = name === "one" ? jQuery.proxy( fn, function( event ) {
    //				jQuery( this ).unbind( event, handler );
    //				return fn.apply( this, arguments );
    //			}) : fn;

    //			if ( type === "unload" && name !== "one" ) {
    //				this.one( type, data, fn );

    //			} else {
    //				for ( var i = 0, l = this.length; i < l; i++ ) {
    //					jQuery.event.add( this[i], type, handler, data );
    //				}
    //			}

    //			return this;
    //		};
    //	});

    jQuery.fn["bind"] = function (type, data, fn) {
        ///	<summary>
        ///     &#10;Binds a handler to one or more events for each matched element.  Can also bind custom events.
        ///	</summary>
        ///	<param name="type" type="String">One or more event types separated by a space.  Built-in event type values are: blur, focus, load, resize, scroll, unload, click, dblclick, mousedown, mouseup, mousemove, mouseover, mouseout, mouseenter, mouseleave, change, select, submit, keydown, keypress, keyup, error .</param>
        ///	<param name="data" optional="true" type="Object">Additional data passed to the event handler as event.data</param>
        ///	<param name="fn" type="Function">A function to bind to the event on each of the set of matched elements.  function callback(eventObject) such that this corresponds to the dom element.</param>

        // Handle object literals
        if (typeof type === "object") {
            for (var key in type) {
                this["bind"](key, data, type[key], fn);
            }
            return this;
        }

        if (jQuery.isFunction(data)) {
            fn = data;
            data = undefined;
        }

        var handler = "bind" === "one" ? jQuery.proxy(fn, function (event) {
            jQuery(this).unbind(event, handler);
            return fn.apply(this, arguments);
        }) : fn;

        return type === "unload" && "bind" !== "one" ?
		this.one(type, data, fn) :
		this.each(function () {
		    jQuery.event.add(this, type, handler, data);
		});
    };

    jQuery.fn["one"] = function (type, data, fn) {
        ///	<summary>
        ///     &#10;Binds a handler to one or more events to be executed exactly once for each matched element.
        ///	</summary>
        ///	<param name="type" type="String">One or more event types separated by a space.  Built-in event type values are: blur, focus, load, resize, scroll, unload, click, dblclick, mousedown, mouseup, mousemove, mouseover, mouseout, mouseenter, mouseleave, change, select, submit, keydown, keypress, keyup, error .</param>
        ///	<param name="data" optional="true" type="Object">Additional data passed to the event handler as event.data</param>
        ///	<param name="fn" type="Function">A function to bind to the event on each of the set of matched elements.  function callback(eventObject) such that this corresponds to the dom element.</param>

        // Handle object literals
        if (typeof type === "object") {
            for (var key in type) {
                this["one"](key, data, type[key], fn);
            }
            return this;
        }

        if (jQuery.isFunction(data)) {
            fn = data;
            data = undefined;
        }

        var handler = "one" === "one" ? jQuery.proxy(fn, function (event) {
            jQuery(this).unbind(event, handler);
            return fn.apply(this, arguments);
        }) : fn;

        return type === "unload" && "one" !== "one" ?
		this.one(type, data, fn) :
		this.each(function () {
		    jQuery.event.add(this, type, handler, data);
		});
    };

    jQuery.fn.extend({
        unbind: function (type, fn) {
            ///	<summary>
            ///     &#10;Unbinds a handler from one or more events for each matched element.
            ///	</summary>
            ///	<param name="type" type="String">One or more event types separated by a space.  Built-in event type values are: blur, focus, load, resize, scroll, unload, click, dblclick, mousedown, mouseup, mousemove, mouseover, mouseout, mouseenter, mouseleave, change, select, submit, keydown, keypress, keyup, error .</param>
            ///	<param name="fn" type="Function">A function to bind to the event on each of the set of matched elements.  function callback(eventObject) such that this corresponds to the dom element.</param>

            // Handle object literals
            if (typeof type === "object" && !type.preventDefault) {
                for (var key in type) {
                    this.unbind(key, type[key]);
                }

            } else {
                for (var i = 0, l = this.length; i < l; i++) {
                    jQuery.event.remove(this[i], type, fn);
                }
            }

            return this;
        },

        delegate: function (selector, types, data, fn) {
            return this.live(types, data, fn, selector);
        },

        undelegate: function (selector, types, fn) {
            if (arguments.length === 0) {
                return this.unbind("live");

            } else {
                return this.die(types, null, fn, selector);
            }
        },

        trigger: function (type, data) {
            ///	<summary>
            ///     &#10;Triggers a type of event on every matched element.
            ///	</summary>
            ///	<param name="type" type="String">One or more event types separated by a space.  Built-in event type values are: blur, focus, load, resize, scroll, unload, click, dblclick, mousedown, mouseup, mousemove, mouseover, mouseout, mouseenter, mouseleave, change, select, submit, keydown, keypress, keyup, error .</param>
            ///	<param name="data" optional="true" type="Array">Additional data passed to the event handler as additional arguments.</param>
            ///	<param name="fn" type="Function">This parameter is undocumented.</param>

            return this.each(function () {
                jQuery.event.trigger(type, data, this);
            });
        },

        triggerHandler: function (type, data) {
            ///	<summary>
            ///     &#10;Triggers all bound event handlers on an element for a specific event type without executing the browser's default actions.
            ///	</summary>
            ///	<param name="type" type="String">One or more event types separated by a space.  Built-in event type values are: blur, focus, load, resize, scroll, unload, click, dblclick, mousedown, mouseup, mousemove, mouseover, mouseout, mouseenter, mouseleave, change, select, submit, keydown, keypress, keyup, error .</param>
            ///	<param name="data" optional="true" type="Array">Additional data passed to the event handler as additional arguments.</param>
            ///	<param name="fn" type="Function">This parameter is undocumented.</param>

            if (this[0]) {
                var event = jQuery.Event(type);
                event.preventDefault();
                event.stopPropagation();
                jQuery.event.trigger(event, data, this[0]);
                return event.result;
            }
        },

        toggle: function (fn) {
            ///	<summary>
            ///     &#10;Toggles among two or more function calls every other click.
            ///	</summary>
            ///	<param name="fn" type="Function">The functions among which to toggle execution</param>

            // Save reference to arguments for access in closure
            var args = arguments,
			i = 1;

            // link all the functions, so any of them can unbind this click handler
            while (i < args.length) {
                jQuery.proxy(fn, args[i++]);
            }

            return this.click(jQuery.proxy(fn, function (event) {
                // Figure out which function to execute
                var lastToggle = (jQuery.data(this, "lastToggle" + fn.guid) || 0) % i;
                jQuery.data(this, "lastToggle" + fn.guid, lastToggle + 1);

                // Make sure that clicks stop
                event.preventDefault();

                // and execute the function
                return args[lastToggle].apply(this, arguments) || false;
            }));
        },

        hover: function (fnOver, fnOut) {
            ///	<summary>
            ///     &#10;Simulates hovering (moving the mouse on or off of an object).
            ///	</summary>
            ///	<param name="fnOver" type="Function">The function to fire when the mouse is moved over a matched element.</param>
            ///	<param name="fnOut" type="Function">The function to fire when the mouse is moved off of a matched element.</param>

            return this.mouseenter(fnOver).mouseleave(fnOut || fnOver);
        }
    });

    var liveMap = {
        focus: "focusin",
        blur: "focusout",
        mouseenter: "mouseover",
        mouseleave: "mouseout"
    };

    //	jQuery.each(["live", "die"], function( i, name ) {
    //		jQuery.fn[ name ] = function( types, data, fn, origSelector /* Internal Use Only */ ) {
    //			var type, i = 0, match, namespaces, preType,
    //				selector = origSelector || this.selector,
    //				context = origSelector ? this : jQuery( this.context );

    //			if ( typeof types === "object" && !types.preventDefault ) {
    //				for ( var key in types ) {
    //					context[ name ]( key, data, types[key], selector );
    //				}

    //				return this;
    //			}

    //			if ( jQuery.isFunction( data ) ) {
    //				fn = data;
    //				data = undefined;
    //			}

    //			types = (types || "").split(" ");

    //			while ( (type = types[ i++ ]) != null ) {
    //				match = rnamespaces.exec( type );
    //				namespaces = "";

    //				if ( match )  {
    //					namespaces = match[0];
    //					type = type.replace( rnamespaces, "" );
    //				}

    //				if ( type === "hover" ) {
    //					types.push( "mouseenter" + namespaces, "mouseleave" + namespaces );
    //					continue;
    //				}

    //				preType = type;

    //				if ( type === "focus" || type === "blur" ) {
    //					types.push( liveMap[ type ] + namespaces );
    //					type = type + namespaces;

    //				} else {
    //					type = (liveMap[ type ] || type) + namespaces;
    //				}

    //				if ( name === "live" ) {
    //					// bind live handler
    //					for ( var j = 0, l = context.length; j < l; j++ ) {
    //						jQuery.event.add( context[j], "live." + liveConvert( type, selector ),
    //							{ data: data, selector: selector, handler: fn, origType: type, origHandler: fn, preType: preType } );
    //					}

    //				} else {
    //					// unbind live handler
    //					context.unbind( "live." + liveConvert( type, selector ), fn );
    //				}
    //			}

    //			return this;
    //		};
    //	});

    jQuery.fn["live"] = function (types, data, fn) {
        ///	<summary>
        ///     &#10;Attach a handler to the event for all elements which match the current selector, now or
        ///     &#10;in the future.
        ///	</summary>
        ///	<param name="types" type="String">
        ///     &#10;A string containing a JavaScript event type, such as "click" or "keydown".
        ///	</param>
        ///	<param name="data" type="Object">
        ///     &#10;A map of data that will be passed to the event handler.
        ///	</param>
        ///	<param name="fn" type="Function">
        ///     &#10;A function to execute at the time the event is triggered.
        ///	</param>
        ///	<returns type="jQuery" />

        var type, i = 0;

        if (jQuery.isFunction(data)) {
            fn = data;
            data = undefined;
        }

        types = (types || "").split(/\s+/);

        while ((type = types[i++]) != null) {
            type = type === "focus" ? "focusin" : // focus --> focusin
				type === "blur" ? "focusout" : // blur --> focusout
				type === "hover" ? types.push("mouseleave") && "mouseenter" : // hover support
				type;

            if ("live" === "live") {
                // bind live handler
                jQuery(this.context).bind(liveConvert(type, this.selector), {
                    data: data, selector: this.selector, live: type
                }, fn);

            } else {
                // unbind live handler
                jQuery(this.context).unbind(liveConvert(type, this.selector), fn ? { guid: fn.guid + this.selector + type} : null);
            }
        }

        return this;
    }

    jQuery.fn["die"] = function (types, data, fn) {
        ///	<summary>
        ///     &#10;Remove all event handlers previously attached using .live() from the elements.
        ///	</summary>
        ///	<param name="types" type="String">
        ///     &#10;A string containing a JavaScript event type, such as click or keydown.
        ///	</param>
        ///	<param name="data" type="Object">
        ///     &#10;The function that is to be no longer executed.
        ///	</param>
        ///	<returns type="jQuery" />

        var type, i = 0;

        if (jQuery.isFunction(data)) {
            fn = data;
            data = undefined;
        }

        types = (types || "").split(/\s+/);

        while ((type = types[i++]) != null) {
            type = type === "focus" ? "focusin" : // focus --> focusin
				type === "blur" ? "focusout" : // blur --> focusout
				type === "hover" ? types.push("mouseleave") && "mouseenter" : // hover support
				type;

            if ("die" === "live") {
                // bind live handler
                jQuery(this.context).bind(liveConvert(type, this.selector), {
                    data: data, selector: this.selector, live: type
                }, fn);

            } else {
                // unbind live handler
                jQuery(this.context).unbind(liveConvert(type, this.selector), fn ? { guid: fn.guid + this.selector + type} : null);
            }
        }

        return this;
    }

    function liveHandler(event) {
        var stop, maxLevel, related, match, handleObj, elem, j, i, l, data, close, namespace, ret,
		elems = [],
		selectors = [],
		events = jQuery.data(this, this.nodeType ? "events" : "__events__");

        if (typeof events === "function") {
            events = events.events;
        }

        // Make sure we avoid non-left-click bubbling in Firefox (#3861)
        if (event.liveFired === this || !events || !events.live || event.button && event.type === "click") {
            return;
        }

        if (event.namespace) {
            namespace = new RegExp("(^|\\.)" + event.namespace.split(".").join("\\.(?:.*\\.)?") + "(\\.|$)");
        }

        event.liveFired = this;

        var live = events.live.slice(0);

        for (j = 0; j < live.length; j++) {
            handleObj = live[j];

            if (handleObj.origType.replace(rnamespaces, "") === event.type) {
                selectors.push(handleObj.selector);

            } else {
                live.splice(j--, 1);
            }
        }

        match = jQuery(event.target).closest(selectors, event.currentTarget);

        for (i = 0, l = match.length; i < l; i++) {
            close = match[i];

            for (j = 0; j < live.length; j++) {
                handleObj = live[j];

                if (close.selector === handleObj.selector && (!namespace || namespace.test(handleObj.namespace))) {
                    elem = close.elem;
                    related = null;

                    // Those two events require additional checking
                    if (handleObj.preType === "mouseenter" || handleObj.preType === "mouseleave") {
                        event.type = handleObj.preType;
                        related = jQuery(event.relatedTarget).closest(handleObj.selector)[0];
                    }

                    if (!related || related !== elem) {
                        elems.push({ elem: elem, handleObj: handleObj, level: close.level });
                    }
                }
            }
        }

        for (i = 0, l = elems.length; i < l; i++) {
            match = elems[i];

            if (maxLevel && match.level > maxLevel) {
                break;
            }

            event.currentTarget = match.elem;
            event.data = match.handleObj.data;
            event.handleObj = match.handleObj;

            ret = match.handleObj.origHandler.apply(match.elem, arguments);

            if (ret === false || event.isPropagationStopped()) {
                maxLevel = match.level;

                if (ret === false) {
                    stop = false;
                }
                if (event.isImmediatePropagationStopped()) {
                    break;
                }
            }
        }

        return stop;
    }

    function liveConvert(type, selector) {
        return (type && type !== "*" ? type + "." : "") + selector.replace(rperiod, "`").replace(rspace, "&");
    }

    //	jQuery.each( ("blur focus focusin focusout load resize scroll unload click dblclick " +
    //		"mousedown mouseup mousemove mouseover mouseout mouseenter mouseleave " +
    //		"change select submit keydown keypress keyup error").split(" "), function( i, name ) {

    //		// Handle event binding
    //		jQuery.fn[ name ] = function( data, fn ) {
    //			if ( fn == null ) {
    //				fn = data;
    //				data = null;
    //			}

    //			return arguments.length > 0 ?
    //				this.bind( name, data, fn ) :
    //				this.trigger( name );
    //		};

    //		if ( jQuery.attrFn ) {
    //			jQuery.attrFn[ name ] = true;
    //		}
    //	});

    jQuery.fn["blur"] = function (fn) {
        ///	<summary>
        ///     &#10;1: blur() - Triggers the blur event of each matched element.
        ///     &#10;2: blur(fn) - Binds a function to the blur event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("blur", fn) : this.trigger("blur");
    };

    jQuery.fn["focus"] = function (fn) {
        ///	<summary>
        ///     &#10;1: focus() - Triggers the focus event of each matched element.
        ///     &#10;2: focus(fn) - Binds a function to the focus event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("focus", fn) : this.trigger("focus");
    };

    jQuery.fn["focusin"] = function (fn) {
        ///	<summary>
        ///     &#10;Bind an event handler to the "focusin" JavaScript event.
        ///	</summary>
        ///	<param name="fn" type="Function">
        ///     &#10;A function to execute each time the event is triggered.
        ///	</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("focusin", fn) : this.trigger("focusin");
    };

    jQuery.fn["focusout"] = function (fn) {
        ///	<summary>
        ///     &#10;Bind an event handler to the "focusout" JavaScript event.
        ///	</summary>
        ///	<param name="fn" type="Function">
        ///     &#10;A function to execute each time the event is triggered.
        ///	</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("focusout", fn) : this.trigger("focusout");
    };

    jQuery.fn["load"] = function (fn) {
        ///	<summary>
        ///     &#10;1: load() - Triggers the load event of each matched element.
        ///     &#10;2: load(fn) - Binds a function to the load event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("load", fn) : this.trigger("load");
    };

    jQuery.fn["resize"] = function (fn) {
        ///	<summary>
        ///     &#10;1: resize() - Triggers the resize event of each matched element.
        ///     &#10;2: resize(fn) - Binds a function to the resize event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("resize", fn) : this.trigger("resize");
    };

    jQuery.fn["scroll"] = function (fn) {
        ///	<summary>
        ///     &#10;1: scroll() - Triggers the scroll event of each matched element.
        ///     &#10;2: scroll(fn) - Binds a function to the scroll event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("scroll", fn) : this.trigger("scroll");
    };

    jQuery.fn["unload"] = function (fn) {
        ///	<summary>
        ///     &#10;1: unload() - Triggers the unload event of each matched element.
        ///     &#10;2: unload(fn) - Binds a function to the unload event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("unload", fn) : this.trigger("unload");
    };

    jQuery.fn["click"] = function (fn) {
        ///	<summary>
        ///     &#10;1: click() - Triggers the click event of each matched element.
        ///     &#10;2: click(fn) - Binds a function to the click event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("click", fn) : this.trigger("click");
    };

    jQuery.fn["dblclick"] = function (fn) {
        ///	<summary>
        ///     &#10;1: dblclick() - Triggers the dblclick event of each matched element.
        ///     &#10;2: dblclick(fn) - Binds a function to the dblclick event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("dblclick", fn) : this.trigger("dblclick");
    };

    jQuery.fn["mousedown"] = function (fn) {
        ///	<summary>
        ///     &#10;Binds a function to the mousedown event of each matched element. 
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("mousedown", fn) : this.trigger("mousedown");
    };

    jQuery.fn["mouseup"] = function (fn) {
        ///	<summary>
        ///     &#10;Bind a function to the mouseup event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("mouseup", fn) : this.trigger("mouseup");
    };

    jQuery.fn["mousemove"] = function (fn) {
        ///	<summary>
        ///     &#10;Bind a function to the mousemove event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("mousemove", fn) : this.trigger("mousemove");
    };

    jQuery.fn["mouseover"] = function (fn) {
        ///	<summary>
        ///     &#10;Bind a function to the mouseover event of each matched element. 
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("mouseover", fn) : this.trigger("mouseover");
    };

    jQuery.fn["mouseout"] = function (fn) {
        ///	<summary>
        ///     &#10;Bind a function to the mouseout event of each matched element. 
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("mouseout", fn) : this.trigger("mouseout");
    };

    jQuery.fn["mouseenter"] = function (fn) {
        ///	<summary>
        ///     &#10;Bind a function to the mouseenter event of each matched element. 
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("mouseenter", fn) : this.trigger("mouseenter");
    };

    jQuery.fn["mouseleave"] = function (fn) {
        ///	<summary>
        ///     &#10;Bind a function to the mouseleave event of each matched element. 
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("mouseleave", fn) : this.trigger("mouseleave");
    };

    jQuery.fn["change"] = function (fn) {
        ///	<summary>
        ///     &#10;1: change() - Triggers the change event of each matched element.
        ///     &#10;2: change(fn) - Binds a function to the change event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("change", fn) : this.trigger("change");
    };

    jQuery.fn["select"] = function (fn) {
        ///	<summary>
        ///     &#10;1: select() - Triggers the select event of each matched element.
        ///     &#10;2: select(fn) - Binds a function to the select event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("select", fn) : this.trigger("select");
    };

    jQuery.fn["submit"] = function (fn) {
        ///	<summary>
        ///     &#10;1: submit() - Triggers the submit event of each matched element.
        ///     &#10;2: submit(fn) - Binds a function to the submit event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("submit", fn) : this.trigger("submit");
    };

    jQuery.fn["keydown"] = function (fn) {
        ///	<summary>
        ///     &#10;1: keydown() - Triggers the keydown event of each matched element.
        ///     &#10;2: keydown(fn) - Binds a function to the keydown event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("keydown", fn) : this.trigger("keydown");
    };

    jQuery.fn["keypress"] = function (fn) {
        ///	<summary>
        ///     &#10;1: keypress() - Triggers the keypress event of each matched element.
        ///     &#10;2: keypress(fn) - Binds a function to the keypress event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("keypress", fn) : this.trigger("keypress");
    };

    jQuery.fn["keyup"] = function (fn) {
        ///	<summary>
        ///     &#10;1: keyup() - Triggers the keyup event of each matched element.
        ///     &#10;2: keyup(fn) - Binds a function to the keyup event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("keyup", fn) : this.trigger("keyup");
    };

    jQuery.fn["error"] = function (fn) {
        ///	<summary>
        ///     &#10;1: error() - Triggers the error event of each matched element.
        ///     &#10;2: error(fn) - Binds a function to the error event of each matched element.
        ///	</summary>
        ///	<param name="fn" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return fn ? this.bind("error", fn) : this.trigger("error");
    };

    // Prevent memory leaks in IE
    // Window isn't included so as not to unbind existing unload events
    // More info:
    //  - http://isaacschlueter.com/2006/10/msie-memory-leaks/
    if (window.attachEvent && !window.addEventListener) {
        jQuery(window).bind("unload", function () {
            for (var id in jQuery.cache) {
                if (jQuery.cache[id].handle) {
                    // Try/Catch is to handle iframes being unloaded, see #4280
                    try {
                        jQuery.event.remove(jQuery.cache[id].handle.elem);
                    } catch (e) { }
                }
            }
        });
    }


    (function () {

        var chunker = /((?:\((?:\([^()]+\)|[^()]+)+\)|\[(?:\[[^\[\]]*\]|['"][^'"]*['"]|[^\[\]'"]+)+\]|\\.|[^ >+~,(\[\\]+)+|[>+~])(\s*,\s*)?((?:.|\r|\n)*)/g,
	done = 0,
	toString = Object.prototype.toString,
	hasDuplicate = false,
	baseHasDuplicate = true;

        // Here we check if the JavaScript engine is using some sort of
        // optimization where it does not always call our comparision
        // function. If that is the case, discard the hasDuplicate value.
        //   Thus far that includes Google Chrome.
        [0, 0].sort(function () {
            baseHasDuplicate = false;
            return 0;
        });

        var Sizzle = function (selector, context, results, seed) {
            results = results || [];
            context = context || document;

            var origContext = context;

            if (context.nodeType !== 1 && context.nodeType !== 9) {
                return [];
            }

            if (!selector || typeof selector !== "string") {
                return results;
            }

            var m, set, checkSet, extra, ret, cur, pop, i,
		prune = true,
		contextXML = Sizzle.isXML(context),
		parts = [],
		soFar = selector;

            // Reset the position of the chunker regexp (start from head)
            do {
                chunker.exec("");
                m = chunker.exec(soFar);

                if (m) {
                    soFar = m[3];

                    parts.push(m[1]);

                    if (m[2]) {
                        extra = m[3];
                        break;
                    }
                }
            } while (m);

            if (parts.length > 1 && origPOS.exec(selector)) {

                if (parts.length === 2 && Expr.relative[parts[0]]) {
                    set = posProcess(parts[0] + parts[1], context);

                } else {
                    set = Expr.relative[parts[0]] ?
				[context] :
				Sizzle(parts.shift(), context);

                    while (parts.length) {
                        selector = parts.shift();

                        if (Expr.relative[selector]) {
                            selector += parts.shift();
                        }

                        set = posProcess(selector, set);
                    }
                }

            } else {
                // Take a shortcut and set the context if the root selector is an ID
                // (but not if it'll be faster if the inner selector is an ID)
                if (!seed && parts.length > 1 && context.nodeType === 9 && !contextXML &&
				Expr.match.ID.test(parts[0]) && !Expr.match.ID.test(parts[parts.length - 1])) {

                    ret = Sizzle.find(parts.shift(), context, contextXML);
                    context = ret.expr ?
				Sizzle.filter(ret.expr, ret.set)[0] :
				ret.set[0];
                }

                if (context) {
                    ret = seed ?
				{ expr: parts.pop(), set: makeArray(seed)} :
				Sizzle.find(parts.pop(), parts.length === 1 && (parts[0] === "~" || parts[0] === "+") && context.parentNode ? context.parentNode : context, contextXML);

                    set = ret.expr ?
				Sizzle.filter(ret.expr, ret.set) :
				ret.set;

                    if (parts.length > 0) {
                        checkSet = makeArray(set);

                    } else {
                        prune = false;
                    }

                    while (parts.length) {
                        cur = parts.pop();
                        pop = cur;

                        if (!Expr.relative[cur]) {
                            cur = "";
                        } else {
                            pop = parts.pop();
                        }

                        if (pop == null) {
                            pop = context;
                        }

                        Expr.relative[cur](checkSet, pop, contextXML);
                    }

                } else {
                    checkSet = parts = [];
                }
            }

            if (!checkSet) {
                checkSet = set;
            }

            if (!checkSet) {
                Sizzle.error(cur || selector);
            }

            if (toString.call(checkSet) === "[object Array]") {
                if (!prune) {
                    results.push.apply(results, checkSet);

                } else if (context && context.nodeType === 1) {
                    for (i = 0; checkSet[i] != null; i++) {
                        if (checkSet[i] && (checkSet[i] === true || checkSet[i].nodeType === 1 && Sizzle.contains(context, checkSet[i]))) {
                            results.push(set[i]);
                        }
                    }

                } else {
                    for (i = 0; checkSet[i] != null; i++) {
                        if (checkSet[i] && checkSet[i].nodeType === 1) {
                            results.push(set[i]);
                        }
                    }
                }

            } else {
                makeArray(checkSet, results);
            }

            if (extra) {
                Sizzle(extra, origContext, results, seed);
                Sizzle.uniqueSort(results);
            }

            return results;
        };

        Sizzle.uniqueSort = function (results) {
            ///	<summary>
            ///     &#10;Removes all duplicate elements from an array of elements.
            ///	</summary>
            ///	<param name="array" type="Array&lt;Element&gt;">The array to translate</param>
            ///	<returns type="Array&lt;Element&gt;">The array after translation.</returns>

            if (sortOrder) {
                hasDuplicate = baseHasDuplicate;
                results.sort(sortOrder);

                if (hasDuplicate) {
                    for (var i = 1; i < results.length; i++) {
                        if (results[i] === results[i - 1]) {
                            results.splice(i--, 1);
                        }
                    }
                }
            }

            return results;
        };

        Sizzle.matches = function (expr, set) {
            return Sizzle(expr, null, null, set);
        };

        Sizzle.matchesSelector = function (node, expr) {
            return Sizzle(expr, null, null, [node]).length > 0;
        };

        Sizzle.find = function (expr, context, isXML) {
            var set;

            if (!expr) {
                return [];
            }

            for (var i = 0, l = Expr.order.length; i < l; i++) {
                var match,
			type = Expr.order[i];

                if ((match = Expr.leftMatch[type].exec(expr))) {
                    var left = match[1];
                    match.splice(1, 1);

                    if (left.substr(left.length - 1) !== "\\") {
                        match[1] = (match[1] || "").replace(/\\/g, "");
                        set = Expr.find[type](match, context, isXML);

                        if (set != null) {
                            expr = expr.replace(Expr.match[type], "");
                            break;
                        }
                    }
                }
            }

            if (!set) {
                set = context.getElementsByTagName("*");
            }

            return { set: set, expr: expr };
        };

        Sizzle.filter = function (expr, set, inplace, not) {
            var match, anyFound,
		old = expr,
		result = [],
		curLoop = set,
		isXMLFilter = set && set[0] && Sizzle.isXML(set[0]);

            while (expr && set.length) {
                for (var type in Expr.filter) {
                    if ((match = Expr.leftMatch[type].exec(expr)) != null && match[2]) {
                        var found, item,
					filter = Expr.filter[type],
					left = match[1];

                        anyFound = false;

                        match.splice(1, 1);

                        if (left.substr(left.length - 1) === "\\") {
                            continue;
                        }

                        if (curLoop === result) {
                            result = [];
                        }

                        if (Expr.preFilter[type]) {
                            match = Expr.preFilter[type](match, curLoop, inplace, result, not, isXMLFilter);

                            if (!match) {
                                anyFound = found = true;

                            } else if (match === true) {
                                continue;
                            }
                        }

                        if (match) {
                            for (var i = 0; (item = curLoop[i]) != null; i++) {
                                if (item) {
                                    found = filter(item, match, i, curLoop);
                                    var pass = not ^ !!found;

                                    if (inplace && found != null) {
                                        if (pass) {
                                            anyFound = true;

                                        } else {
                                            curLoop[i] = false;
                                        }

                                    } else if (pass) {
                                        result.push(item);
                                        anyFound = true;
                                    }
                                }
                            }
                        }

                        if (found !== undefined) {
                            if (!inplace) {
                                curLoop = result;
                            }

                            expr = expr.replace(Expr.match[type], "");

                            if (!anyFound) {
                                return [];
                            }

                            break;
                        }
                    }
                }

                // Improper expression
                if (expr === old) {
                    if (anyFound == null) {
                        Sizzle.error(expr);

                    } else {
                        break;
                    }
                }

                old = expr;
            }

            return curLoop;
        };

        Sizzle.error = function (msg) {
            throw "Syntax error, unrecognized expression: " + msg;
        };

        var Expr = Sizzle.selectors = {
            order: ["ID", "NAME", "TAG"],

            match: {
                ID: /#((?:[\w\u00c0-\uFFFF\-]|\\.)+)/,
                CLASS: /\.((?:[\w\u00c0-\uFFFF\-]|\\.)+)/,
                NAME: /\[name=['"]*((?:[\w\u00c0-\uFFFF\-]|\\.)+)['"]*\]/,
                ATTR: /\[\s*((?:[\w\u00c0-\uFFFF\-]|\\.)+)\s*(?:(\S?=)\s*(['"]*)(.*?)\3|)\s*\]/,
                TAG: /^((?:[\w\u00c0-\uFFFF\*\-]|\\.)+)/,
                CHILD: /:(only|nth|last|first)-child(?:\((even|odd|[\dn+\-]*)\))?/,
                POS: /:(nth|eq|gt|lt|first|last|even|odd)(?:\((\d*)\))?(?=[^\-]|$)/,
                PSEUDO: /:((?:[\w\u00c0-\uFFFF\-]|\\.)+)(?:\((['"]?)((?:\([^\)]+\)|[^\(\)]*)+)\2\))?/
            },

            leftMatch: {},

            attrMap: {
                "class": "className",
                "for": "htmlFor"
            },

            attrHandle: {
                href: function (elem) {
                    return elem.getAttribute("href");
                }
            },

            relative: {
                "+": function (checkSet, part) {
                    var isPartStr = typeof part === "string",
				isTag = isPartStr && !/\W/.test(part),
				isPartStrNotTag = isPartStr && !isTag;

                    if (isTag) {
                        part = part.toLowerCase();
                    }

                    for (var i = 0, l = checkSet.length, elem; i < l; i++) {
                        if ((elem = checkSet[i])) {
                            while ((elem = elem.previousSibling) && elem.nodeType !== 1) { }

                            checkSet[i] = isPartStrNotTag || elem && elem.nodeName.toLowerCase() === part ?
						elem || false :
						elem === part;
                        }
                    }

                    if (isPartStrNotTag) {
                        Sizzle.filter(part, checkSet, true);
                    }
                },

                ">": function (checkSet, part) {
                    var elem,
				isPartStr = typeof part === "string",
				i = 0,
				l = checkSet.length;

                    if (isPartStr && !/\W/.test(part)) {
                        part = part.toLowerCase();

                        for (; i < l; i++) {
                            elem = checkSet[i];

                            if (elem) {
                                var parent = elem.parentNode;
                                checkSet[i] = parent.nodeName.toLowerCase() === part ? parent : false;
                            }
                        }

                    } else {
                        for (; i < l; i++) {
                            elem = checkSet[i];

                            if (elem) {
                                checkSet[i] = isPartStr ?
							elem.parentNode :
							elem.parentNode === part;
                            }
                        }

                        if (isPartStr) {
                            Sizzle.filter(part, checkSet, true);
                        }
                    }
                },

                "": function (checkSet, part, isXML) {
                    var nodeCheck,
				doneName = done++,
				checkFn = dirCheck;

                    if (typeof part === "string" && !/\W/.test(part)) {
                        part = part.toLowerCase();
                        nodeCheck = part;
                        checkFn = dirNodeCheck;
                    }

                    checkFn("parentNode", part, doneName, checkSet, nodeCheck, isXML);
                },

                "~": function (checkSet, part, isXML) {
                    var nodeCheck,
				doneName = done++,
				checkFn = dirCheck;

                    if (typeof part === "string" && !/\W/.test(part)) {
                        part = part.toLowerCase();
                        nodeCheck = part;
                        checkFn = dirNodeCheck;
                    }

                    checkFn("previousSibling", part, doneName, checkSet, nodeCheck, isXML);
                }
            },

            find: {
                ID: function (match, context, isXML) {
                    if (typeof context.getElementById !== "undefined" && !isXML) {
                        var m = context.getElementById(match[1]);
                        // Check parentNode to catch when Blackberry 4.6 returns
                        // nodes that are no longer in the document #6963
                        return m && m.parentNode ? [m] : [];
                    }
                },

                NAME: function (match, context) {
                    if (typeof context.getElementsByName !== "undefined") {
                        var ret = [],
					results = context.getElementsByName(match[1]);

                        for (var i = 0, l = results.length; i < l; i++) {
                            if (results[i].getAttribute("name") === match[1]) {
                                ret.push(results[i]);
                            }
                        }

                        return ret.length === 0 ? null : ret;
                    }
                },

                TAG: function (match, context) {
                    return context.getElementsByTagName(match[1]);
                }
            },
            preFilter: {
                CLASS: function (match, curLoop, inplace, result, not, isXML) {
                    match = " " + match[1].replace(/\\/g, "") + " ";

                    if (isXML) {
                        return match;
                    }

                    for (var i = 0, elem; (elem = curLoop[i]) != null; i++) {
                        if (elem) {
                            if (not ^ (elem.className && (" " + elem.className + " ").replace(/[\t\n]/g, " ").indexOf(match) >= 0)) {
                                if (!inplace) {
                                    result.push(elem);
                                }

                            } else if (inplace) {
                                curLoop[i] = false;
                            }
                        }
                    }

                    return false;
                },

                ID: function (match) {
                    return match[1].replace(/\\/g, "");
                },

                TAG: function (match, curLoop) {
                    return match[1].toLowerCase();
                },

                CHILD: function (match) {
                    if (match[1] === "nth") {
                        // parse equations like 'even', 'odd', '5', '2n', '3n+2', '4n-1', '-n+6'
                        var test = /(-?)(\d*)n((?:\+|-)?\d*)/.exec(
					match[2] === "even" && "2n" || match[2] === "odd" && "2n+1" ||
					!/\D/.test(match[2]) && "0n+" + match[2] || match[2]);

                        // calculate the numbers (first)n+(last) including if they are negative
                        match[2] = (test[1] + (test[2] || 1)) - 0;
                        match[3] = test[3] - 0;
                    }

                    // TODO: Move to normal caching system
                    match[0] = done++;

                    return match;
                },

                ATTR: function (match, curLoop, inplace, result, not, isXML) {
                    var name = match[1].replace(/\\/g, "");

                    if (!isXML && Expr.attrMap[name]) {
                        match[1] = Expr.attrMap[name];
                    }

                    if (match[2] === "~=") {
                        match[4] = " " + match[4] + " ";
                    }

                    return match;
                },

                PSEUDO: function (match, curLoop, inplace, result, not) {
                    if (match[1] === "not") {
                        // If we're dealing with a complex expression, or a simple one
                        if ((chunker.exec(match[3]) || "").length > 1 || /^\w/.test(match[3])) {
                            match[3] = Sizzle(match[3], null, null, curLoop);

                        } else {
                            var ret = Sizzle.filter(match[3], curLoop, inplace, true ^ not);

                            if (!inplace) {
                                result.push.apply(result, ret);
                            }

                            return false;
                        }

                    } else if (Expr.match.POS.test(match[0]) || Expr.match.CHILD.test(match[0])) {
                        return true;
                    }

                    return match;
                },

                POS: function (match) {
                    match.unshift(true);

                    return match;
                }
            },

            filters: {
                enabled: function (elem) {
                    return elem.disabled === false && elem.type !== "hidden";
                },

                disabled: function (elem) {
                    return elem.disabled === true;
                },

                checked: function (elem) {
                    return elem.checked === true;
                },

                selected: function (elem) {
                    // Accessing this property makes selected-by-default
                    // options in Safari work properly
                    elem.parentNode.selectedIndex;

                    return elem.selected === true;
                },

                parent: function (elem) {
                    return !!elem.firstChild;
                },

                empty: function (elem) {
                    return !elem.firstChild;
                },

                has: function (elem, i, match) {
                    ///	<summary>
                    ///     &#10;Internal use only; use hasClass('class')
                    ///	</summary>
                    ///	<private />

                    return !!Sizzle(match[3], elem).length;
                },

                header: function (elem) {
                    return (/h\d/i).test(elem.nodeName);
                },

                text: function (elem) {
                    return "text" === elem.type;
                },
                radio: function (elem) {
                    return "radio" === elem.type;
                },

                checkbox: function (elem) {
                    return "checkbox" === elem.type;
                },

                file: function (elem) {
                    return "file" === elem.type;
                },
                password: function (elem) {
                    return "password" === elem.type;
                },

                submit: function (elem) {
                    return "submit" === elem.type;
                },

                image: function (elem) {
                    return "image" === elem.type;
                },

                reset: function (elem) {
                    return "reset" === elem.type;
                },

                button: function (elem) {
                    return "button" === elem.type || elem.nodeName.toLowerCase() === "button";
                },

                input: function (elem) {
                    return (/input|select|textarea|button/i).test(elem.nodeName);
                }
            },
            setFilters: {
                first: function (elem, i) {
                    return i === 0;
                },

                last: function (elem, i, match, array) {
                    return i === array.length - 1;
                },

                even: function (elem, i) {
                    return i % 2 === 0;
                },

                odd: function (elem, i) {
                    return i % 2 === 1;
                },

                lt: function (elem, i, match) {
                    return i < match[3] - 0;
                },

                gt: function (elem, i, match) {
                    return i > match[3] - 0;
                },

                nth: function (elem, i, match) {
                    return match[3] - 0 === i;
                },

                eq: function (elem, i, match) {
                    return match[3] - 0 === i;
                }
            },
            filter: {
                PSEUDO: function (elem, match, i, array) {
                    var name = match[1],
				filter = Expr.filters[name];

                    if (filter) {
                        return filter(elem, i, match, array);

                    } else if (name === "contains") {
                        return (elem.textContent || elem.innerText || Sizzle.getText([elem]) || "").indexOf(match[3]) >= 0;

                    } else if (name === "not") {
                        var not = match[3];

                        for (var j = 0, l = not.length; j < l; j++) {
                            if (not[j] === elem) {
                                return false;
                            }
                        }

                        return true;

                    } else {
                        Sizzle.error("Syntax error, unrecognized expression: " + name);
                    }
                },

                CHILD: function (elem, match) {
                    var type = match[1],
				node = elem;

                    switch (type) {
                        case "only":
                        case "first":
                            while ((node = node.previousSibling)) {
                                if (node.nodeType === 1) {
                                    return false;
                                }
                            }

                            if (type === "first") {
                                return true;
                            }

                            node = elem;

                        case "last":
                            while ((node = node.nextSibling)) {
                                if (node.nodeType === 1) {
                                    return false;
                                }
                            }

                            return true;

                        case "nth":
                            var first = match[2],
						last = match[3];

                            if (first === 1 && last === 0) {
                                return true;
                            }

                            var doneName = match[0],
						parent = elem.parentNode;

                            if (parent && (parent.sizcache !== doneName || !elem.nodeIndex)) {
                                var count = 0;

                                for (node = parent.firstChild; node; node = node.nextSibling) {
                                    if (node.nodeType === 1) {
                                        node.nodeIndex = ++count;
                                    }
                                }

                                parent.sizcache = doneName;
                            }

                            var diff = elem.nodeIndex - last;

                            if (first === 0) {
                                return diff === 0;

                            } else {
                                return (diff % first === 0 && diff / first >= 0);
                            }
                    }
                },

                ID: function (elem, match) {
                    return elem.nodeType === 1 && elem.getAttribute("id") === match;
                },

                TAG: function (elem, match) {
                    return (match === "*" && elem.nodeType === 1) || elem.nodeName.toLowerCase() === match;
                },

                CLASS: function (elem, match) {
                    return (" " + (elem.className || elem.getAttribute("class")) + " ")
				.indexOf(match) > -1;
                },

                ATTR: function (elem, match) {
                    var name = match[1],
				result = Expr.attrHandle[name] ?
					Expr.attrHandle[name](elem) :
					elem[name] != null ?
						elem[name] :
						elem.getAttribute(name),
				value = result + "",
				type = match[2],
				check = match[4];

                    return result == null ?
				type === "!=" :
				type === "=" ?
				value === check :
				type === "*=" ?
				value.indexOf(check) >= 0 :
				type === "~=" ?
				(" " + value + " ").indexOf(check) >= 0 :
				!check ?
				value && result !== false :
				type === "!=" ?
				value !== check :
				type === "^=" ?
				value.indexOf(check) === 0 :
				type === "$=" ?
				value.substr(value.length - check.length) === check :
				type === "|=" ?
				value === check || value.substr(0, check.length + 1) === check + "-" :
				false;
                },

                POS: function (elem, match, i, array) {
                    var name = match[2],
				filter = Expr.setFilters[name];

                    if (filter) {
                        return filter(elem, i, match, array);
                    }
                }
            }
        };

        var origPOS = Expr.match.POS,
	fescape = function (all, num) {
	    return "\\" + (num - 0 + 1);
	};

        for (var type in Expr.match) {
            Expr.match[type] = new RegExp(Expr.match[type].source + (/(?![^\[]*\])(?![^\(]*\))/.source));
            Expr.leftMatch[type] = new RegExp(/(^(?:.|\r|\n)*?)/.source + Expr.match[type].source.replace(/\\(\d+)/g, fescape));
        }

        var makeArray = function (array, results) {
            array = Array.prototype.slice.call(array, 0);

            if (results) {
                results.push.apply(results, array);
                return results;
            }

            return array;
        };

        // Perform a simple check to determine if the browser is capable of
        // converting a NodeList to an array using builtin methods.
        // Also verifies that the returned array holds DOM nodes
        // (which is not the case in the Blackberry browser)
        try {
            Array.prototype.slice.call(document.documentElement.childNodes, 0)[0].nodeType;

            // Provide a fallback method if it does not work
        } catch (e) {
            makeArray = function (array, results) {
                var i = 0,
			ret = results || [];

                if (toString.call(array) === "[object Array]") {
                    Array.prototype.push.apply(ret, array);

                } else {
                    if (typeof array.length === "number") {
                        for (var l = array.length; i < l; i++) {
                            ret.push(array[i]);
                        }

                    } else {
                        for (; array[i]; i++) {
                            ret.push(array[i]);
                        }
                    }
                }

                return ret;
            };
        }

        var sortOrder, siblingCheck;

        if (document.documentElement.compareDocumentPosition) {
            sortOrder = function (a, b) {
                if (a === b) {
                    hasDuplicate = true;
                    return 0;
                }

                if (!a.compareDocumentPosition || !b.compareDocumentPosition) {
                    return a.compareDocumentPosition ? -1 : 1;
                }

                return a.compareDocumentPosition(b) & 4 ? -1 : 1;
            };

        } else {
            sortOrder = function (a, b) {
                var al, bl,
			ap = [],
			bp = [],
			aup = a.parentNode,
			bup = b.parentNode,
			cur = aup;

                // The nodes are identical, we can exit early
                if (a === b) {
                    hasDuplicate = true;
                    return 0;

                    // If the nodes are siblings (or identical) we can do a quick check
                } else if (aup === bup) {
                    return siblingCheck(a, b);

                    // If no parents were found then the nodes are disconnected
                } else if (!aup) {
                    return -1;

                } else if (!bup) {
                    return 1;
                }

                // Otherwise they're somewhere else in the tree so we need
                // to build up a full list of the parentNodes for comparison
                while (cur) {
                    ap.unshift(cur);
                    cur = cur.parentNode;
                }

                cur = bup;

                while (cur) {
                    bp.unshift(cur);
                    cur = cur.parentNode;
                }

                al = ap.length;
                bl = bp.length;

                // Start walking down the tree looking for a discrepancy
                for (var i = 0; i < al && i < bl; i++) {
                    if (ap[i] !== bp[i]) {
                        return siblingCheck(ap[i], bp[i]);
                    }
                }

                // We ended someplace up the tree so do a sibling check
                return i === al ?
			siblingCheck(a, bp[i], -1) :
			siblingCheck(ap[i], b, 1);
            };

            siblingCheck = function (a, b, ret) {
                if (a === b) {
                    return ret;
                }

                var cur = a.nextSibling;

                while (cur) {
                    if (cur === b) {
                        return -1;
                    }

                    cur = cur.nextSibling;
                }

                return 1;
            };
        }

        // Utility function for retreiving the text value of an array of DOM nodes
        Sizzle.getText = function (elems) {
            var ret = "", elem;

            for (var i = 0; elems[i]; i++) {
                elem = elems[i];

                // Get the text from text nodes and CDATA nodes
                if (elem.nodeType === 3 || elem.nodeType === 4) {
                    ret += elem.nodeValue;

                    // Traverse everything else, except comment nodes
                } else if (elem.nodeType !== 8) {
                    ret += Sizzle.getText(elem.childNodes);
                }
            }

            return ret;
        };

        // [vsdoc] The following function has been modified for IntelliSense.
        // Check to see if the browser returns elements by name when
        // querying by getElementById (and provide a workaround)
        (function () {
            // We're going to inject a fake input element with a specified name
            //	var form = document.createElement("div"),
            //		id = "script" + (new Date()).getTime(),
            //		root = document.documentElement;

            //	form.innerHTML = "<a name='" + id + "'/>";

            //	// Inject it into the root element, check its status, and remove it quickly
            //	root.insertBefore( form, root.firstChild );

            //	// The workaround has to do additional checks after a getElementById
            //	// Which slows things down for other browsers (hence the branching)
            //	if ( document.getElementById( id ) ) {
            Expr.find.ID = function (match, context, isXML) {
                if (typeof context.getElementById !== "undefined" && !isXML) {
                    var m = context.getElementById(match[1]);

                    return m ?
					m.id === match[1] || typeof m.getAttributeNode !== "undefined" && m.getAttributeNode("id").nodeValue === match[1] ?
						[m] :
						undefined :
					[];
                }
            };

            Expr.filter.ID = function (elem, match) {
                var node = typeof elem.getAttributeNode !== "undefined" && elem.getAttributeNode("id");

                return elem.nodeType === 1 && node && node.nodeValue === match;
            };
            //	}

            //	root.removeChild( form );

            // release memory in IE
            root = form = null;
        })();

        // [vsdoc] The following function has been modified for IntelliSense.
        (function () {
            // Check to see if the browser returns only elements
            // when doing getElementsByTagName("*")

            // Create a fake element
            //	var div = document.createElement("div");
            //	div.appendChild( document.createComment("") );

            // Make sure no comments are found
            //	if ( div.getElementsByTagName("*").length > 0 ) {
            Expr.find.TAG = function (match, context) {
                var results = context.getElementsByTagName(match[1]);

                // Filter out possible comments
                if (match[1] === "*") {
                    var tmp = [];

                    for (var i = 0; results[i]; i++) {
                        if (results[i].nodeType === 1) {
                            tmp.push(results[i]);
                        }
                    }

                    results = tmp;
                }

                return results;
            };
            //	}

            // Check to see if an attribute returns normalized href attributes
            //	div.innerHTML = "<a href='#'></a>";

            //	if ( div.firstChild && typeof div.firstChild.getAttribute !== "undefined" &&
            //			div.firstChild.getAttribute("href") !== "#" ) {

            //		Expr.attrHandle.href = function( elem ) {
            //			return elem.getAttribute( "href", 2 );
            //		};
            //	}

            // release memory in IE
            div = null;
        })();

        if (document.querySelectorAll) {
            (function () {
                var oldSizzle = Sizzle,
			div = document.createElement("div"),
			id = "__sizzle__";

                div.innerHTML = "<p class='TEST'></p>";

                // Safari can't handle uppercase or unicode characters when
                // in quirks mode.
                if (div.querySelectorAll && div.querySelectorAll(".TEST").length === 0) {
                    return;
                }

                Sizzle = function (query, context, extra, seed) {
                    context = context || document;

                    // Make sure that attribute selectors are quoted
                    query = query.replace(/\=\s*([^'"\]]*)\s*\]/g, "='$1']");

                    // Only use querySelectorAll on non-XML documents
                    // (ID selectors don't work in non-HTML documents)
                    if (!seed && !Sizzle.isXML(context)) {
                        if (context.nodeType === 9) {
                            try {
                                return makeArray(context.querySelectorAll(query), extra);
                            } catch (qsaError) { }

                            // qSA works strangely on Element-rooted queries
                            // We can work around this by specifying an extra ID on the root
                            // and working up from there (Thanks to Andrew Dupont for the technique)
                            // IE 8 doesn't work on object elements
                        } else if (context.nodeType === 1 && context.nodeName.toLowerCase() !== "object") {
                            var old = context.getAttribute("id"),
						nid = old || id;

                            if (!old) {
                                context.setAttribute("id", nid);
                            }

                            try {
                                return makeArray(context.querySelectorAll("#" + nid + " " + query), extra);

                            } catch (pseudoError) {
                            } finally {
                                if (!old) {
                                    context.removeAttribute("id");
                                }
                            }
                        }
                    }

                    return oldSizzle(query, context, extra, seed);
                };

                for (var prop in oldSizzle) {
                    Sizzle[prop] = oldSizzle[prop];
                }

                // release memory in IE
                div = null;
            })();
        }

        (function () {
            var html = document.documentElement,
		matches = html.matchesSelector || html.mozMatchesSelector || html.webkitMatchesSelector || html.msMatchesSelector,
		pseudoWorks = false;

            try {
                // This should fail with an exception
                // Gecko does not error, returns false instead
                matches.call(document.documentElement, "[test!='']:sizzle");

            } catch (pseudoError) {
                pseudoWorks = true;
            }

            if (matches) {
                Sizzle.matchesSelector = function (node, expr) {
                    // Make sure that attribute selectors are quoted
                    expr = expr.replace(/\=\s*([^'"\]]*)\s*\]/g, "='$1']");

                    if (!Sizzle.isXML(node)) {
                        try {
                            if (pseudoWorks || !Expr.match.PSEUDO.test(expr) && !/!=/.test(expr)) {
                                return matches.call(node, expr);
                            }
                        } catch (e) { }
                    }

                    return Sizzle(expr, null, null, [node]).length > 0;
                };
            }
        })();

        (function () {
            var div = document.createElement("div");

            div.innerHTML = "<div class='test e'></div><div class='test'></div>";

            // Opera can't find a second classname (in 9.6)
            // Also, make sure that getElementsByClassName actually exists
            if (!div.getElementsByClassName || div.getElementsByClassName("e").length === 0) {
                return;
            }

            // Safari caches class attributes, doesn't catch changes (in 3.2)
            div.lastChild.className = "e";

            if (div.getElementsByClassName("e").length === 1) {
                return;
            }

            Expr.order.splice(1, 0, "CLASS");
            Expr.find.CLASS = function (match, context, isXML) {
                if (typeof context.getElementsByClassName !== "undefined" && !isXML) {
                    return context.getElementsByClassName(match[1]);
                }
            };

            // release memory in IE
            div = null;
        })();

        function dirNodeCheck(dir, cur, doneName, checkSet, nodeCheck, isXML) {
            for (var i = 0, l = checkSet.length; i < l; i++) {
                var elem = checkSet[i];

                if (elem) {
                    var match = false;

                    elem = elem[dir];

                    while (elem) {
                        if (elem.sizcache === doneName) {
                            match = checkSet[elem.sizset];
                            break;
                        }

                        if (elem.nodeType === 1 && !isXML) {
                            elem.sizcache = doneName;
                            elem.sizset = i;
                        }

                        if (elem.nodeName.toLowerCase() === cur) {
                            match = elem;
                            break;
                        }

                        elem = elem[dir];
                    }

                    checkSet[i] = match;
                }
            }
        }

        function dirCheck(dir, cur, doneName, checkSet, nodeCheck, isXML) {
            for (var i = 0, l = checkSet.length; i < l; i++) {
                var elem = checkSet[i];

                if (elem) {
                    var match = false;

                    elem = elem[dir];

                    while (elem) {
                        if (elem.sizcache === doneName) {
                            match = checkSet[elem.sizset];
                            break;
                        }

                        if (elem.nodeType === 1) {
                            if (!isXML) {
                                elem.sizcache = doneName;
                                elem.sizset = i;
                            }

                            if (typeof cur !== "string") {
                                if (elem === cur) {
                                    match = true;
                                    break;
                                }

                            } else if (Sizzle.filter(cur, [elem]).length > 0) {
                                match = elem;
                                break;
                            }
                        }

                        elem = elem[dir];
                    }

                    checkSet[i] = match;
                }
            }
        }

        if (document.documentElement.contains) {
            Sizzle.contains = function (a, b) {
                ///	<summary>
                ///     &#10;Check to see if a DOM node is within another DOM node.
                ///	</summary>
                ///	<param name="a" type="Object">
                ///     &#10;The DOM element that may contain the other element.
                ///	</param>
                ///	<param name="b" type="Object">
                ///     &#10;The DOM node that may be contained by the other element.
                ///	</param>
                ///	<returns type="Boolean" />

                return a !== b && (a.contains ? a.contains(b) : true);
            };

        } else if (document.documentElement.compareDocumentPosition) {
            Sizzle.contains = function (a, b) {
                ///	<summary>
                ///     &#10;Check to see if a DOM node is within another DOM node.
                ///	</summary>
                ///	<param name="a" type="Object">
                ///     &#10;The DOM element that may contain the other element.
                ///	</param>
                ///	<param name="b" type="Object">
                ///     &#10;The DOM node that may be contained by the other element.
                ///	</param>
                ///	<returns type="Boolean" />

                return !!(a.compareDocumentPosition(b) & 16);
            };

        } else {
            Sizzle.contains = function () {
                return false;
            };
        }

        Sizzle.isXML = function (elem) {
            ///	<summary>
            ///     &#10;Determines if the parameter passed is an XML document.
            ///	</summary>
            ///	<param name="elem" type="Object">The object to test</param>
            ///	<returns type="Boolean">True if the parameter is an XML document; otherwise false.</returns>

            // documentElement is verified for cases where it doesn't yet exist
            // (such as loading iframes in IE - #4833) 
            var documentElement = (elem ? elem.ownerDocument || elem : 0).documentElement;

            return documentElement ? documentElement.nodeName !== "HTML" : false;
        };

        var posProcess = function (selector, context) {
            var match,
		tmpSet = [],
		later = "",
		root = context.nodeType ? [context] : context;

            // Position selectors must be done after the filter
            // And so must :not(positional) so we move all PSEUDOs to the end
            while ((match = Expr.match.PSEUDO.exec(selector))) {
                later += match[0];
                selector = selector.replace(Expr.match.PSEUDO, "");
            }

            selector = Expr.relative[selector] ? selector + "*" : selector;

            for (var i = 0, l = root.length; i < l; i++) {
                Sizzle(selector, root[i], tmpSet);
            }

            return Sizzle.filter(later, tmpSet);
        };

        // EXPOSE
        jQuery.find = Sizzle;
        jQuery.expr = Sizzle.selectors;
        jQuery.expr[":"] = jQuery.expr.filters;
        jQuery.unique = Sizzle.uniqueSort;
        jQuery.text = Sizzle.getText;
        jQuery.isXMLDoc = Sizzle.isXML;
        jQuery.contains = Sizzle.contains;


    })();


    var runtil = /Until$/,
	rparentsprev = /^(?:parents|prevUntil|prevAll)/,
    // Note: This RegExp should be improved, or likely pulled from Sizzle
	rmultiselector = /,/,
	isSimple = /^.[^:#\[\.,]*$/,
	slice = Array.prototype.slice,
	POS = jQuery.expr.match.POS;

    jQuery.fn.extend({
        find: function (selector) {
            ///	<summary>
            ///     &#10;Searches for all elements that match the specified expression.
            ///     &#10;This method is a good way to find additional descendant
            ///     &#10;elements with which to process.
            ///     &#10;All searching is done using a jQuery expression. The expression can be
            ///     &#10;written using CSS 1-3 Selector syntax, or basic XPath.
            ///     &#10;Part of DOM/Traversing
            ///	</summary>
            ///	<returns type="jQuery" />
            ///	<param name="selector" type="String">
            ///     &#10;An expression to search with.
            ///	</param>
            ///	<returns type="jQuery" />

            var ret = this.pushStack("", "find", selector),
			length = 0;

            for (var i = 0, l = this.length; i < l; i++) {
                length = ret.length;
                jQuery.find(selector, this[i], ret);

                if (i > 0) {
                    // Make sure that the results are unique
                    for (var n = length; n < ret.length; n++) {
                        for (var r = 0; r < length; r++) {
                            if (ret[r] === ret[n]) {
                                ret.splice(n--, 1);
                                break;
                            }
                        }
                    }
                }
            }

            return ret;
        },

        has: function (target) {
            ///	<summary>
            ///     &#10;Reduce the set of matched elements to those that have a descendant that matches the
            ///     &#10;selector or DOM element.
            ///	</summary>
            ///	<param name="target" type="String">
            ///     &#10;A string containing a selector expression to match elements against.
            ///	</param>
            ///	<returns type="jQuery" />

            var targets = jQuery(target);
            return this.filter(function () {
                for (var i = 0, l = targets.length; i < l; i++) {
                    if (jQuery.contains(this, targets[i])) {
                        return true;
                    }
                }
            });
        },

        not: function (selector) {
            ///	<summary>
            ///     &#10;Removes any elements inside the array of elements from the set
            ///     &#10;of matched elements. This method is used to remove one or more
            ///     &#10;elements from a jQuery object.
            ///     &#10;Part of DOM/Traversing
            ///	</summary>
            ///	<param name="selector" type="jQuery">
            ///     &#10;A set of elements to remove from the jQuery set of matched elements.
            ///	</param>
            ///	<returns type="jQuery" />

            return this.pushStack(winnow(this, selector, false), "not", selector);
        },

        filter: function (selector) {
            ///	<summary>
            ///     &#10;Removes all elements from the set of matched elements that do not
            ///     &#10;pass the specified filter. This method is used to narrow down
            ///     &#10;the results of a search.
            ///     &#10;})
            ///     &#10;Part of DOM/Traversing
            ///	</summary>
            ///	<returns type="jQuery" />
            ///	<param name="selector" type="Function">
            ///     &#10;A function to use for filtering
            ///	</param>
            ///	<returns type="jQuery" />

            return this.pushStack(winnow(this, selector, true), "filter", selector);
        },

        is: function (selector) {
            ///	<summary>
            ///     &#10;Checks the current selection against an expression and returns true,
            ///     &#10;if at least one element of the selection fits the given expression.
            ///     &#10;Does return false, if no element fits or the expression is not valid.
            ///     &#10;filter(String) is used internally, therefore all rules that apply there
            ///     &#10;apply here, too.
            ///     &#10;Part of DOM/Traversing
            ///	</summary>
            ///	<returns type="Boolean" />
            ///	<param name="expr" type="String">
            ///     &#10; The expression with which to filter
            ///	</param>

            return !!selector && jQuery.filter(selector, this).length > 0;
        },

        closest: function (selectors, context) {
            ///	<summary>
            ///     &#10;Get a set of elements containing the closest parent element that matches the specified selector, the starting element included.
            ///	</summary>
            ///	<param name="selectors" type="String">
            ///     &#10;A string containing a selector expression to match elements against.
            ///	</param>
            ///	<param name="context" type="Element">
            ///     &#10;A DOM element within which a matching element may be found. If no context is passed
            ///     &#10;in then the context of the jQuery set will be used instead.
            ///	</param>
            ///	<returns type="jQuery" />

            var ret = [], i, l, cur = this[0];

            if (jQuery.isArray(selectors)) {
                var match, selector,
				matches = {},
				level = 1;

                if (cur && selectors.length) {
                    for (i = 0, l = selectors.length; i < l; i++) {
                        selector = selectors[i];

                        if (!matches[selector]) {
                            matches[selector] = jQuery.expr.match.POS.test(selector) ?
							jQuery(selector, context || this.context) :
							selector;
                        }
                    }

                    while (cur && cur.ownerDocument && cur !== context) {
                        for (selector in matches) {
                            match = matches[selector];

                            if (match.jquery ? match.index(cur) > -1 : jQuery(cur).is(match)) {
                                ret.push({ selector: selector, elem: cur, level: level });
                            }
                        }

                        cur = cur.parentNode;
                        level++;
                    }
                }

                return ret;
            }

            var pos = POS.test(selectors) ?
			jQuery(selectors, context || this.context) : null;

            for (i = 0, l = this.length; i < l; i++) {
                cur = this[i];

                while (cur) {
                    if (pos ? pos.index(cur) > -1 : jQuery.find.matchesSelector(cur, selectors)) {
                        ret.push(cur);
                        break;

                    } else {
                        cur = cur.parentNode;
                        if (!cur || !cur.ownerDocument || cur === context) {
                            break;
                        }
                    }
                }
            }

            ret = ret.length > 1 ? jQuery.unique(ret) : ret;

            return this.pushStack(ret, "closest", selectors);
        },

        // Determine the position of an element within
        // the matched set of elements
        index: function (elem) {
            ///	<summary>
            ///     &#10;Searches every matched element for the object and returns
            ///     &#10;the index of the element, if found, starting with zero. 
            ///     &#10;Returns -1 if the object wasn't found.
            ///     &#10;Part of Core
            ///	</summary>
            ///	<returns type="Number" />
            ///	<param name="elem" type="Element">
            ///     &#10;Object to search for
            ///	</param>

            if (!elem || typeof elem === "string") {
                return jQuery.inArray(this[0],
                // If it receives a string, the selector is used
                // If it receives nothing, the siblings are used
				elem ? jQuery(elem) : this.parent().children());
            }
            // Locate the position of the desired element
            return jQuery.inArray(
            // If it receives a jQuery object, the first element is used
			elem.jquery ? elem[0] : elem, this);
        },

        add: function (selector, context) {
            ///	<summary>
            ///     &#10;Adds one or more Elements to the set of matched elements.
            ///     &#10;Part of DOM/Traversing
            ///	</summary>
            ///	<param name="selector" type="String">
            ///     &#10;A string containing a selector expression to match additional elements against.
            ///	</param>
            ///	<param name="context" type="Element">
            ///     &#10;Add some elements rooted against the specified context.
            ///	</param>
            ///	<returns type="jQuery" />

            var set = typeof selector === "string" ?
				jQuery(selector, context || this.context) :
				jQuery.makeArray(selector),
			all = jQuery.merge(this.get(), set);

            return this.pushStack(isDisconnected(set[0]) || isDisconnected(all[0]) ?
			all :
			jQuery.unique(all));
        },

        andSelf: function () {
            ///	<summary>
            ///     &#10;Adds the previous selection to the current selection.
            ///	</summary>
            ///	<returns type="jQuery" />

            return this.add(this.prevObject);
        }
    });

    // A painfully simple check to see if an element is disconnected
    // from a document (should be improved, where feasible).
    function isDisconnected(node) {
        return !node || !node.parentNode || node.parentNode.nodeType === 11;
    }

    jQuery.fn.parents = function (until, selector) {
        /// <summary>
        ///     Get the ancestors of each element in the current set of matched elements, optionally filtered by a selector.
        /// </summary>
        /// <param name="until" type="String">
        ///     A string containing a selector expression to match elements against.
        /// </param>
        /// <returns type="jQuery" />
        return jQuery.dir(elem, "parentNode");
    };

    jQuery.fn.parentsUntil = function (until, selector) {
        /// <summary>
        ///     Get the ancestors of each element in the current set of matched elements, up to but not including the element matched by the selector.
        /// </summary>
        /// <param name="until" type="String">
        ///     A string containing a selector expression to indicate where to stop matching ancestor elements.
        /// </param>
        /// <returns type="jQuery" />
        return jQuery.dir(elem, "parentNode", until);
    };

    jQuery.each({
        parent: function (elem) {
            var parent = elem.parentNode;
            return parent && parent.nodeType !== 11 ? parent : null;
        },
        next: function (elem) {
            return jQuery.nth(elem, 2, "nextSibling");
        },
        prev: function (elem) {
            return jQuery.nth(elem, 2, "previousSibling");
        },
        nextAll: function (elem) {
            return jQuery.dir(elem, "nextSibling");
        },
        prevAll: function (elem) {
            return jQuery.dir(elem, "previousSibling");
        },
        nextUntil: function (elem, i, until) {
            ///	<summary>
            ///     &#10;Get all following siblings of each element up to but not including the element matched
            ///     &#10;by the selector.
            ///	</summary>
            ///	<param name="until" type="String">
            ///     &#10;A string containing a selector expression to indicate where to stop matching following
            ///     &#10;sibling elements.
            ///	</param>
            ///	<returns type="jQuery" />

            return jQuery.dir(elem, "nextSibling", until);
        },
        prevUntil: function (elem, i, until) {
            ///	<summary>
            ///     &#10;Get all preceding siblings of each element up to but not including the element matched
            ///     &#10;by the selector.
            ///	</summary>
            ///	<param name="until" type="String">
            ///     &#10;A string containing a selector expression to indicate where to stop matching preceding
            ///     &#10;sibling elements.
            ///	</param>
            ///	<returns type="jQuery" />

            return jQuery.dir(elem, "previousSibling", until);
        },
        siblings: function (elem) {
            return jQuery.sibling(elem.parentNode.firstChild, elem);
        },
        children: function (elem) {
            return jQuery.sibling(elem.firstChild);
        },
        contents: function (elem) {
            return jQuery.nodeName(elem, "iframe") ?
			elem.contentDocument || elem.contentWindow.document :
			jQuery.makeArray(elem.childNodes);
        }
    }, function (name, fn) {
        jQuery.fn[name] = function (until, selector) {
            var ret = jQuery.map(this, fn, until);

            if (!runtil.test(name)) {
                selector = until;
            }

            if (selector && typeof selector === "string") {
                ret = jQuery.filter(selector, ret);
            }

            ret = this.length > 1 ? jQuery.unique(ret) : ret;

            if ((this.length > 1 || rmultiselector.test(selector)) && rparentsprev.test(name)) {
                ret = ret.reverse();
            }

            return this.pushStack(ret, name, slice.call(arguments).join(","));
        };
    });

    jQuery.extend({
        filter: function (expr, elems, not) {
            if (not) {
                expr = ":not(" + expr + ")";
            }

            return elems.length === 1 ?
			jQuery.find.matchesSelector(elems[0], expr) ? [elems[0]] : [] :
			jQuery.find.matches(expr, elems);
        },

        dir: function (elem, dir, until) {
            ///	<summary>
            ///     &#10;This member is internal only.
            ///	</summary>
            ///	<private />

            var matched = [],
			cur = elem[dir];

            while (cur && cur.nodeType !== 9 && (until === undefined || cur.nodeType !== 1 || !jQuery(cur).is(until))) {
                if (cur.nodeType === 1) {
                    matched.push(cur);
                }
                cur = cur[dir];
            }
            return matched;
        },

        nth: function (cur, result, dir, elem) {
            ///	<summary>
            ///     &#10;This member is internal only.
            ///	</summary>
            ///	<private />

            result = result || 1;
            var num = 0;

            for (; cur; cur = cur[dir]) {
                if (cur.nodeType === 1 && ++num === result) {
                    break;
                }
            }

            return cur;
        },

        sibling: function (n, elem) {
            ///	<summary>
            ///     &#10;This member is internal only.
            ///	</summary>
            ///	<private />

            var r = [];

            for (; n; n = n.nextSibling) {
                if (n.nodeType === 1 && n !== elem) {
                    r.push(n);
                }
            }

            return r;
        }
    });

    // Implement the identical functionality for filter and not
    function winnow(elements, qualifier, keep) {
        if (jQuery.isFunction(qualifier)) {
            return jQuery.grep(elements, function (elem, i) {
                var retVal = !!qualifier.call(elem, i, elem);
                return retVal === keep;
            });

        } else if (qualifier.nodeType) {
            return jQuery.grep(elements, function (elem, i) {
                return (elem === qualifier) === keep;
            });

        } else if (typeof qualifier === "string") {
            var filtered = jQuery.grep(elements, function (elem) {
                return elem.nodeType === 1;
            });

            if (isSimple.test(qualifier)) {
                return jQuery.filter(qualifier, filtered, !keep);
            } else {
                qualifier = jQuery.filter(qualifier, filtered);
            }
        }

        return jQuery.grep(elements, function (elem, i) {
            return (jQuery.inArray(elem, qualifier) >= 0) === keep;
        });
    }




    var rinlinejQuery = / jQuery\d+="(?:\d+|null)"/g,
	rleadingWhitespace = /^\s+/,
	rxhtmlTag = /<(?!area|br|col|embed|hr|img|input|link|meta|param)(([\w:]+)[^>]*)\/>/ig,
	rtagName = /<([\w:]+)/,
	rtbody = /<tbody/i,
	rhtml = /<|&#?\w+;/,
	rnocache = /<(?:script|object|embed|option|style)/i,
    // checked="checked" or checked (html5)
	rchecked = /checked\s*(?:[^=]|=\s*.checked.)/i,
	raction = /\=([^="'>\s]+\/)>/g,
	wrapMap = {
	    option: [1, "<select multiple='multiple'>", "</select>"],
	    legend: [1, "<fieldset>", "</fieldset>"],
	    thead: [1, "<table>", "</table>"],
	    tr: [2, "<table><tbody>", "</tbody></table>"],
	    td: [3, "<table><tbody><tr>", "</tr></tbody></table>"],
	    col: [2, "<table><tbody></tbody><colgroup>", "</colgroup></table>"],
	    area: [1, "<map>", "</map>"],
	    _default: [0, "", ""]
	};

    wrapMap.optgroup = wrapMap.option;
    wrapMap.tbody = wrapMap.tfoot = wrapMap.colgroup = wrapMap.caption = wrapMap.thead;
    wrapMap.th = wrapMap.td;

    // IE can't serialize <link> and <script> tags normally
    if (!jQuery.support.htmlSerialize) {
        wrapMap._default = [1, "div<div>", "</div>"];
    }

    jQuery.fn.extend({
        text: function (text) {
            ///	<summary>
            ///     &#10;Set the text contents of all matched elements.
            ///     &#10;Similar to html(), but escapes HTML (replace &quot;&lt;&quot; and &quot;&gt;&quot; with their
            ///     &#10;HTML entities).
            ///     &#10;Part of DOM/Attributes
            ///	</summary>
            ///	<returns type="jQuery" />
            ///	<param name="text" type="String">
            ///     &#10;The text value to set the contents of the element to.
            ///	</param>

            if (jQuery.isFunction(text)) {
                return this.each(function (i) {
                    var self = jQuery(this);

                    self.text(text.call(this, i, self.text()));
                });
            }

            if (typeof text !== "object" && text !== undefined) {
                return this.empty().append((this[0] && this[0].ownerDocument || document).createTextNode(text));
            }

            return jQuery.text(this);
        },

        wrapAll: function (html) {
            ///	<summary>
            ///     &#10;Wrap all matched elements with a structure of other elements.
            ///     &#10;This wrapping process is most useful for injecting additional
            ///     &#10;stucture into a document, without ruining the original semantic
            ///     &#10;qualities of a document.
            ///     &#10;This works by going through the first element
            ///     &#10;provided and finding the deepest ancestor element within its
            ///     &#10;structure - it is that element that will en-wrap everything else.
            ///     &#10;This does not work with elements that contain text. Any necessary text
            ///     &#10;must be added after the wrapping is done.
            ///     &#10;Part of DOM/Manipulation
            ///	</summary>
            ///	<returns type="jQuery" />
            ///	<param name="html" type="Element">
            ///     &#10;A DOM element that will be wrapped around the target.
            ///	</param>

            if (jQuery.isFunction(html)) {
                return this.each(function (i) {
                    jQuery(this).wrapAll(html.call(this, i));
                });
            }

            if (this[0]) {
                // The elements to wrap the target around
                var wrap = jQuery(html, this[0].ownerDocument).eq(0).clone(true);

                if (this[0].parentNode) {
                    wrap.insertBefore(this[0]);
                }

                wrap.map(function () {
                    var elem = this;

                    while (elem.firstChild && elem.firstChild.nodeType === 1) {
                        elem = elem.firstChild;
                    }

                    return elem;
                }).append(this);
            }

            return this;
        },

        wrapInner: function (html) {
            ///	<summary>
            ///     &#10;Wraps the inner child contents of each matched elemenht (including text nodes) with an HTML structure.
            ///	</summary>
            ///	<param name="html" type="String">
            ///     &#10;A string of HTML or a DOM element that will be wrapped around the target contents.
            ///	</param>
            ///	<returns type="jQuery" />

            if (jQuery.isFunction(html)) {
                return this.each(function (i) {
                    jQuery(this).wrapInner(html.call(this, i));
                });
            }

            return this.each(function () {
                var self = jQuery(this),
				contents = self.contents();

                if (contents.length) {
                    contents.wrapAll(html);

                } else {
                    self.append(html);
                }
            });
        },

        wrap: function (html) {
            ///	<summary>
            ///     &#10;Wrap all matched elements with a structure of other elements.
            ///     &#10;This wrapping process is most useful for injecting additional
            ///     &#10;stucture into a document, without ruining the original semantic
            ///     &#10;qualities of a document.
            ///     &#10;This works by going through the first element
            ///     &#10;provided and finding the deepest ancestor element within its
            ///     &#10;structure - it is that element that will en-wrap everything else.
            ///     &#10;This does not work with elements that contain text. Any necessary text
            ///     &#10;must be added after the wrapping is done.
            ///     &#10;Part of DOM/Manipulation
            ///	</summary>
            ///	<returns type="jQuery" />
            ///	<param name="html" type="Element">
            ///     &#10;A DOM element that will be wrapped around the target.
            ///	</param>

            return this.each(function () {
                jQuery(this).wrapAll(html);
            });
        },

        unwrap: function () {
            ///	<summary>
            ///     &#10;Remove the parents of the set of matched elements from the DOM, leaving the matched
            ///     &#10;elements in their place.
            ///	</summary>
            ///	<returns type="jQuery" />
            return this.parent().each(function () {
                if (!jQuery.nodeName(this, "body")) {
                    jQuery(this).replaceWith(this.childNodes);
                }
            }).end();
        },

        append: function () {
            ///	<summary>
            ///     &#10;Append content to the inside of every matched element.
            ///     &#10;This operation is similar to doing an appendChild to all the
            ///     &#10;specified elements, adding them into the document.
            ///     &#10;Part of DOM/Manipulation
            ///	</summary>
            ///	<returns type="jQuery" />

            return this.domManip(arguments, true, function (elem) {
                if (this.nodeType === 1) {
                    this.appendChild(elem);
                }
            });
        },

        prepend: function () {
            ///	<summary>
            ///     &#10;Prepend content to the inside of every matched element.
            ///     &#10;This operation is the best way to insert elements
            ///     &#10;inside, at the beginning, of all matched elements.
            ///     &#10;Part of DOM/Manipulation
            ///	</summary>
            ///	<returns type="jQuery" />

            return this.domManip(arguments, true, function (elem) {
                if (this.nodeType === 1) {
                    this.insertBefore(elem, this.firstChild);
                }
            });
        },

        before: function () {
            ///	<summary>
            ///     &#10;Insert content before each of the matched elements.
            ///     &#10;Part of DOM/Manipulation
            ///	</summary>
            ///	<returns type="jQuery" />

            if (this[0] && this[0].parentNode) {
                return this.domManip(arguments, false, function (elem) {
                    this.parentNode.insertBefore(elem, this);
                });
            } else if (arguments.length) {
                var set = jQuery(arguments[0]);
                set.push.apply(set, this.toArray());
                return this.pushStack(set, "before", arguments);
            }
        },

        after: function () {
            ///	<summary>
            ///     &#10;Insert content after each of the matched elements.
            ///     &#10;Part of DOM/Manipulation
            ///	</summary>
            ///	<returns type="jQuery" />

            if (this[0] && this[0].parentNode) {
                return this.domManip(arguments, false, function (elem) {
                    this.parentNode.insertBefore(elem, this.nextSibling);
                });
            } else if (arguments.length) {
                var set = this.pushStack(this, "after", arguments);
                set.push.apply(set, jQuery(arguments[0]).toArray());
                return set;
            }
        },

        // keepData is for internal use only--do not document
        remove: function (selector, keepData) {
            for (var i = 0, elem; (elem = this[i]) != null; i++) {
                if (!selector || jQuery.filter(selector, [elem]).length) {
                    if (!keepData && elem.nodeType === 1) {
                        jQuery.cleanData(elem.getElementsByTagName("*"));
                        jQuery.cleanData([elem]);
                    }

                    if (elem.parentNode) {
                        elem.parentNode.removeChild(elem);
                    }
                }
            }

            return this;
        },

        empty: function () {
            for (var i = 0, elem; (elem = this[i]) != null; i++) {
                // Remove element nodes and prevent memory leaks
                if (elem.nodeType === 1) {
                    jQuery.cleanData(elem.getElementsByTagName("*"));
                }

                // Remove any remaining nodes
                while (elem.firstChild) {
                    elem.removeChild(elem.firstChild);
                }
            }

            return this;
        },

        clone: function (events) {
            ///	<summary>
            ///     &#10;Clone matched DOM Elements and select the clones. 
            ///     &#10;This is useful for moving copies of the elements to another
            ///     &#10;location in the DOM.
            ///     &#10;Part of DOM/Manipulation
            ///	</summary>
            ///	<returns type="jQuery" />
            ///	<param name="deep" type="Boolean" optional="true">
            ///     &#10;(Optional) Set to false if you don't want to clone all descendant nodes, in addition to the element itself.
            ///	</param>

            // Do the clone
            var ret = this.map(function () {
                if (!jQuery.support.noCloneEvent && !jQuery.isXMLDoc(this)) {
                    // IE copies events bound via attachEvent when
                    // using cloneNode. Calling detachEvent on the
                    // clone will also remove the events from the orignal
                    // In order to get around this, we use innerHTML.
                    // Unfortunately, this means some modifications to
                    // attributes in IE that are actually only stored
                    // as properties will not be copied (such as the
                    // the name attribute on an input).
                    var html = this.outerHTML,
					ownerDocument = this.ownerDocument;

                    if (!html) {
                        var div = ownerDocument.createElement("div");
                        div.appendChild(this.cloneNode(true));
                        html = div.innerHTML;
                    }

                    return jQuery.clean([html.replace(rinlinejQuery, "")
                    // Handle the case in IE 8 where action=/test/> self-closes a tag
					.replace(raction, '="$1">')
					.replace(rleadingWhitespace, "")], ownerDocument)[0];
                } else {
                    return this.cloneNode(true);
                }
            });

            // Copy the events from the original to the clone
            if (events === true) {
                cloneCopyEvent(this, ret);
                cloneCopyEvent(this.find("*"), ret.find("*"));
            }

            // Return the cloned set
            return ret;
        },

        html: function (value) {
            ///	<summary>
            ///     &#10;Set the html contents of every matched element.
            ///     &#10;This property is not available on XML documents.
            ///     &#10;Part of DOM/Attributes
            ///	</summary>
            ///	<returns type="jQuery" />
            ///	<param name="value" type="String">
            ///     &#10;A string of HTML to set as the content of each matched element.
            ///	</param>

            if (value === undefined) {
                return this[0] && this[0].nodeType === 1 ?
				this[0].innerHTML.replace(rinlinejQuery, "") :
				null;

                // See if we can take a shortcut and just use innerHTML
            } else if (typeof value === "string" && !rnocache.test(value) &&
			(jQuery.support.leadingWhitespace || !rleadingWhitespace.test(value)) &&
			!wrapMap[(rtagName.exec(value) || ["", ""])[1].toLowerCase()]) {

                value = value.replace(rxhtmlTag, "<$1></$2>");

                try {
                    for (var i = 0, l = this.length; i < l; i++) {
                        // Remove element nodes and prevent memory leaks
                        if (this[i].nodeType === 1) {
                            jQuery.cleanData(this[i].getElementsByTagName("*"));
                            this[i].innerHTML = value;
                        }
                    }

                    // If using innerHTML throws an exception, use the fallback method
                } catch (e) {
                    this.empty().append(value);
                }

            } else if (jQuery.isFunction(value)) {
                this.each(function (i) {
                    var self = jQuery(this);

                    self.html(value.call(this, i, self.html()));
                });

            } else {
                this.empty().append(value);
            }

            return this;
        },

        replaceWith: function (value) {
            ///	<summary>
            ///     &#10;Replaces all matched element with the specified HTML or DOM elements.
            ///	</summary>
            ///	<param name="value" type="Object">
            ///     &#10;The content to insert. May be an HTML string, DOM element, or jQuery object.
            ///	</param>
            ///	<returns type="jQuery">The element that was just replaced.</returns>

            if (this[0] && this[0].parentNode) {
                // Make sure that the elements are removed from the DOM before they are inserted
                // this can help fix replacing a parent with child elements
                if (jQuery.isFunction(value)) {
                    return this.each(function (i) {
                        var self = jQuery(this), old = self.html();
                        self.replaceWith(value.call(this, i, old));
                    });
                }

                if (typeof value !== "string") {
                    value = jQuery(value).detach();
                }

                return this.each(function () {
                    var next = this.nextSibling,
					parent = this.parentNode;

                    jQuery(this).remove();

                    if (next) {
                        jQuery(next).before(value);
                    } else {
                        jQuery(parent).append(value);
                    }
                });
            } else {
                return this.pushStack(jQuery(jQuery.isFunction(value) ? value() : value), "replaceWith", value);
            }
        },

        detach: function (selector) {
            ///	<summary>
            ///     &#10;Remove the set of matched elements from the DOM.
            ///	</summary>
            ///	<param name="selector" type="String">
            ///     &#10;A selector expression that filters the set of matched elements to be removed.
            ///	</param>
            ///	<returns type="jQuery" />

            return this.remove(selector, true);
        },

        domManip: function (args, table, callback) {
            ///	<param name="args" type="Array">
            ///     &#10; Args
            ///	</param>
            ///	<param name="table" type="Boolean">
            ///     &#10; Insert TBODY in TABLEs if one is not found.
            ///	</param>
            ///	<param name="dir" type="Number">
            ///     &#10; If dir&lt;0, process args in reverse order.
            ///	</param>
            ///	<param name="fn" type="Function">
            ///     &#10; The function doing the DOM manipulation.
            ///	</param>
            ///	<returns type="jQuery" />
            ///	<summary>
            ///     &#10;Part of Core
            ///	</summary>

            var results, first, fragment, parent,
			value = args[0],
			scripts = [];

            // We can't cloneNode fragments that contain checked, in WebKit
            if (!jQuery.support.checkClone && arguments.length === 3 && typeof value === "string" && rchecked.test(value)) {
                return this.each(function () {
                    jQuery(this).domManip(args, table, callback, true);
                });
            }

            if (jQuery.isFunction(value)) {
                return this.each(function (i) {
                    var self = jQuery(this);
                    args[0] = value.call(this, i, table ? self.html() : undefined);
                    self.domManip(args, table, callback);
                });
            }

            if (this[0]) {
                parent = value && value.parentNode;

                // If we're in a fragment, just use that instead of building a new one
                if (jQuery.support.parentNode && parent && parent.nodeType === 11 && parent.childNodes.length === this.length) {
                    results = { fragment: parent };

                } else {
                    results = jQuery.buildFragment(args, this, scripts);
                }

                fragment = results.fragment;

                if (fragment.childNodes.length === 1) {
                    first = fragment = fragment.firstChild;
                } else {
                    first = fragment.firstChild;
                }

                if (first) {
                    table = table && jQuery.nodeName(first, "tr");

                    for (var i = 0, l = this.length; i < l; i++) {
                        callback.call(
						table ?
							root(this[i], first) :
							this[i],
						i > 0 || results.cacheable || this.length > 1 ?
							fragment.cloneNode(true) :
							fragment
					);
                    }
                }

                if (scripts.length) {
                    jQuery.each(scripts, evalScript);
                }
            }

            return this;
        }
    });

    function root(elem, cur) {
        return jQuery.nodeName(elem, "table") ?
		(elem.getElementsByTagName("tbody")[0] ||
		elem.appendChild(elem.ownerDocument.createElement("tbody"))) :
		elem;
    }

    function cloneCopyEvent(orig, ret) {
        var i = 0;

        ret.each(function () {
            if (this.nodeName !== (orig[i] && orig[i].nodeName)) {
                return;
            }

            var oldData = jQuery.data(orig[i++]),
			curData = jQuery.data(this, oldData),
			events = oldData && oldData.events;

            if (events) {
                delete curData.handle;
                curData.events = {};

                for (var type in events) {
                    for (var handler in events[type]) {
                        jQuery.event.add(this, type, events[type][handler], events[type][handler].data);
                    }
                }
            }
        });
    }

    jQuery.buildFragment = function (args, nodes, scripts) {
        var fragment, cacheable, cacheresults,
		doc = (nodes && nodes[0] ? nodes[0].ownerDocument || nodes[0] : document);

        // Only cache "small" (1/2 KB) strings that are associated with the main document
        // Cloning options loses the selected state, so don't cache them
        // IE 6 doesn't like it when you put <object> or <embed> elements in a fragment
        // Also, WebKit does not clone 'checked' attributes on cloneNode, so don't cache
        if (args.length === 1 && typeof args[0] === "string" && args[0].length < 512 && doc === document &&
		!rnocache.test(args[0]) && (jQuery.support.checkClone || !rchecked.test(args[0]))) {

            cacheable = true;
            cacheresults = jQuery.fragments[args[0]];
            if (cacheresults) {
                if (cacheresults !== 1) {
                    fragment = cacheresults;
                }
            }
        }

        if (!fragment) {
            fragment = doc.createDocumentFragment();
            jQuery.clean(args, doc, fragment, scripts);
        }

        if (cacheable) {
            jQuery.fragments[args[0]] = cacheresults ? fragment : 1;
        }

        return { fragment: fragment, cacheable: cacheable };
    };

    jQuery.fragments = {};

    //	jQuery.each({
    //		appendTo: "append",
    //		prependTo: "prepend",
    //		insertBefore: "before",
    //		insertAfter: "after",
    //		replaceAll: "replaceWith"
    //	}, function( name, original ) {
    //		jQuery.fn[ name ] = function( selector ) {
    //			var ret = [],
    //				insert = jQuery( selector ),
    //				parent = this.length === 1 && this[0].parentNode;

    //			if ( parent && parent.nodeType === 11 && parent.childNodes.length === 1 && insert.length === 1 ) {
    //				insert[ original ]( this[0] );
    //				return this;

    //			} else {
    //				for ( var i = 0, l = insert.length; i < l; i++ ) {
    //					var elems = (i > 0 ? this.clone(true) : this).get();
    //					jQuery( insert[i] )[ original ]( elems );
    //					ret = ret.concat( elems );
    //				}
    //			
    //				return this.pushStack( ret, name, insert.selector );
    //			}
    //		};
    //	});
    jQuery.fn["appendTo"] = function (selector) {
        ///	<summary>
        ///     &#10;Append all of the matched elements to another, specified, set of elements.
        ///     &#10;As of jQuery 1.3.2, returns all of the inserted elements.
        ///     &#10;This operation is, essentially, the reverse of doing a regular
        ///     &#10;$(A).append(B), in that instead of appending B to A, you're appending
        ///     &#10;A to B.
        ///	</summary>
        ///	<param name="selector" type="Selector">
        ///     &#10; target to which the content will be appended.
        ///	</param>
        ///	<returns type="jQuery" />

        var ret = [], insert = jQuery(selector);

        for (var i = 0, l = insert.length; i < l; i++) {
            var elems = (i > 0 ? this.clone(true) : this).get();
            jQuery.fn["append"].apply(jQuery(insert[i]), elems);
            ret = ret.concat(elems);
        }
        return this.pushStack(ret, "appendTo", insert.selector);
    };

    jQuery.fn["prependTo"] = function (selector) {
        ///	<summary>
        ///     &#10;Prepend all of the matched elements to another, specified, set of elements.
        ///     &#10;As of jQuery 1.3.2, returns all of the inserted elements.
        ///     &#10;This operation is, essentially, the reverse of doing a regular
        ///     &#10;$(A).prepend(B), in that instead of prepending B to A, you're prepending
        ///     &#10;A to B.
        ///	</summary>
        ///	<param name="selector" type="Selector">
        ///     &#10; target to which the content will be appended.
        ///	</param>
        ///	<returns type="jQuery" />

        var ret = [], insert = jQuery(selector);

        for (var i = 0, l = insert.length; i < l; i++) {
            var elems = (i > 0 ? this.clone(true) : this).get();
            jQuery.fn["prepend"].apply(jQuery(insert[i]), elems);
            ret = ret.concat(elems);
        }
        return this.pushStack(ret, "prependTo", insert.selector);
    };

    jQuery.fn["insertBefore"] = function (selector) {
        ///	<summary>
        ///     &#10;Insert all of the matched elements before another, specified, set of elements.
        ///     &#10;As of jQuery 1.3.2, returns all of the inserted elements.
        ///     &#10;This operation is, essentially, the reverse of doing a regular
        ///     &#10;$(A).before(B), in that instead of inserting B before A, you're inserting
        ///     &#10;A before B.
        ///	</summary>
        ///	<param name="content" type="String">
        ///     &#10; Content after which the selected element(s) is inserted.
        ///	</param>
        ///	<returns type="jQuery" />

        var ret = [], insert = jQuery(selector);

        for (var i = 0, l = insert.length; i < l; i++) {
            var elems = (i > 0 ? this.clone(true) : this).get();
            jQuery.fn["before"].apply(jQuery(insert[i]), elems);
            ret = ret.concat(elems);
        }
        return this.pushStack(ret, "insertBefore", insert.selector);
    };

    jQuery.fn["insertAfter"] = function (selector) {
        ///	<summary>
        ///     &#10;Insert all of the matched elements after another, specified, set of elements.
        ///     &#10;As of jQuery 1.3.2, returns all of the inserted elements.
        ///     &#10;This operation is, essentially, the reverse of doing a regular
        ///     &#10;$(A).after(B), in that instead of inserting B after A, you're inserting
        ///     &#10;A after B.
        ///	</summary>
        ///	<param name="content" type="String">
        ///     &#10; Content after which the selected element(s) is inserted.
        ///	</param>
        ///	<returns type="jQuery" />

        var ret = [], insert = jQuery(selector);

        for (var i = 0, l = insert.length; i < l; i++) {
            var elems = (i > 0 ? this.clone(true) : this).get();
            jQuery.fn["after"].apply(jQuery(insert[i]), elems);
            ret = ret.concat(elems);
        }
        return this.pushStack(ret, "insertAfter", insert.selector);
    };

    jQuery.fn["replaceAll"] = function (selector) {
        ///	<summary>
        ///     &#10;Replaces the elements matched by the specified selector with the matched elements.
        ///     &#10;As of jQuery 1.3.2, returns all of the inserted elements.
        ///	</summary>
        ///	<param name="selector" type="Selector">The elements to find and replace the matched elements with.</param>
        ///	<returns type="jQuery" />

        var ret = [], insert = jQuery(selector);

        for (var i = 0, l = insert.length; i < l; i++) {
            var elems = (i > 0 ? this.clone(true) : this).get();
            jQuery.fn["replaceWith"].apply(jQuery(insert[i]), elems);
            ret = ret.concat(elems);
        }
        return this.pushStack(ret, "replaceAll", insert.selector);
    };

    jQuery.each({
        // keepData is for internal use only--do not document
        remove: function (selector, keepData) {
            if (!selector || jQuery.filter(selector, [this]).length) {
                if (!keepData && this.nodeType === 1) {
                    jQuery.cleanData(this.getElementsByTagName("*"));
                    jQuery.cleanData([this]);
                }

                if (this.parentNode) {
                    this.parentNode.removeChild(this);
                }
            }
        },

        empty: function () {
            ///	<summary>
            ///     &#10;Removes all child nodes from the set of matched elements.
            ///     &#10;Part of DOM/Manipulation
            ///	</summary>
            ///	<returns type="jQuery" />

            // Remove element nodes and prevent memory leaks
            if (this.nodeType === 1) {
                jQuery.cleanData(this.getElementsByTagName("*"));
            }

            // Remove any remaining nodes
            while (this.firstChild) {
                this.removeChild(this.firstChild);
            }
        }
    }, function (name, fn) {
        jQuery.fn[name] = function () {
            return this.each(fn, arguments);
        };
    });

    jQuery.extend({
        clean: function (elems, context, fragment, scripts) {
            ///	<summary>
            ///     &#10;This method is internal only.
            ///	</summary>
            ///	<private />

            context = context || document;

            // !context.createElement fails in IE with an error but returns typeof 'object'
            if (typeof context.createElement === "undefined") {
                context = context.ownerDocument || context[0] && context[0].ownerDocument || document;
            }

            var ret = [];

            for (var i = 0, elem; (elem = elems[i]) != null; i++) {
                if (typeof elem === "number") {
                    elem += "";
                }

                if (!elem) {
                    continue;
                }

                // Convert html string into DOM nodes
                if (typeof elem === "string" && !rhtml.test(elem)) {
                    elem = context.createTextNode(elem);

                } else if (typeof elem === "string") {
                    // Fix "XHTML"-style tags in all browsers
                    elem = elem.replace(rxhtmlTag, "<$1></$2>");

                    // Trim whitespace, otherwise indexOf won't work as expected
                    var tag = (rtagName.exec(elem) || ["", ""])[1].toLowerCase(),
					wrap = wrapMap[tag] || wrapMap._default,
					depth = wrap[0],
					div = context.createElement("div");

                    // Go to html and back, then peel off extra wrappers
                    div.innerHTML = wrap[1] + elem + wrap[2];

                    // Move to the right depth
                    while (depth--) {
                        div = div.lastChild;
                    }

                    // Remove IE's autoinserted <tbody> from table fragments
                    if (!jQuery.support.tbody) {

                        // String was a <table>, *may* have spurious <tbody>
                        var hasBody = rtbody.test(elem),
						tbody = tag === "table" && !hasBody ?
							div.firstChild && div.firstChild.childNodes :

                        // String was a bare <thead> or <tfoot>
							wrap[1] === "<table>" && !hasBody ?
								div.childNodes :
								[];

                        for (var j = tbody.length - 1; j >= 0; --j) {
                            if (jQuery.nodeName(tbody[j], "tbody") && !tbody[j].childNodes.length) {
                                tbody[j].parentNode.removeChild(tbody[j]);
                            }
                        }

                    }

                    // IE completely kills leading whitespace when innerHTML is used
                    if (!jQuery.support.leadingWhitespace && rleadingWhitespace.test(elem)) {
                        div.insertBefore(context.createTextNode(rleadingWhitespace.exec(elem)[0]), div.firstChild);
                    }

                    elem = div.childNodes;
                }

                if (elem.nodeType) {
                    ret.push(elem);
                } else {
                    ret = jQuery.merge(ret, elem);
                }
            }

            if (fragment) {
                for (i = 0; ret[i]; i++) {
                    if (scripts && jQuery.nodeName(ret[i], "script") && (!ret[i].type || ret[i].type.toLowerCase() === "text/javascript")) {
                        scripts.push(ret[i].parentNode ? ret[i].parentNode.removeChild(ret[i]) : ret[i]);

                    } else {
                        if (ret[i].nodeType === 1) {
                            ret.splice.apply(ret, [i + 1, 0].concat(jQuery.makeArray(ret[i].getElementsByTagName("script"))));
                        }
                        fragment.appendChild(ret[i]);
                    }
                }
            }

            return ret;
        },

        cleanData: function (elems) {
            var data, id, cache = jQuery.cache,
			special = jQuery.event.special,
			deleteExpando = jQuery.support.deleteExpando;

            for (var i = 0, elem; (elem = elems[i]) != null; i++) {
                if (elem.nodeName && jQuery.noData[elem.nodeName.toLowerCase()]) {
                    continue;
                }

                id = elem[jQuery.expando];

                if (id) {
                    data = cache[id];

                    if (data && data.events) {
                        for (var type in data.events) {
                            if (special[type]) {
                                jQuery.event.remove(elem, type);

                            } else {
                                jQuery.removeEvent(elem, type, data.handle);
                            }
                        }
                    }

                    if (deleteExpando) {
                        delete elem[jQuery.expando];

                    } else if (elem.removeAttribute) {
                        elem.removeAttribute(jQuery.expando);
                    }

                    delete cache[id];
                }
            }
        }
    });

    function evalScript(i, elem) {
        ///	<summary>
        ///     &#10;This method is internal.
        ///	</summary>
        /// <private />

        if (elem.src) {
            jQuery.ajax({
                url: elem.src,
                async: false,
                dataType: "script"
            });
        } else {
            jQuery.globalEval(elem.text || elem.textContent || elem.innerHTML || "");
        }

        if (elem.parentNode) {
            elem.parentNode.removeChild(elem);
        }
    }




    var ralpha = /alpha\([^)]*\)/i,
	ropacity = /opacity=([^)]*)/,
	rdashAlpha = /-([a-z])/ig,
	rupper = /([A-Z])/g,
	rnumpx = /^-?\d+(?:px)?$/i,
	rnum = /^-?\d/,

	cssShow = { position: "absolute", visibility: "hidden", display: "block" },
	cssWidth = ["Left", "Right"],
	cssHeight = ["Top", "Bottom"],
	curCSS,

	getComputedStyle,
	currentStyle,

	fcamelCase = function (all, letter) {
	    return letter.toUpperCase();
	};

    jQuery.fn.css = function (name, value) {
        ///	<summary>
        ///     &#10;Set a single style property to a value, on all matched elements.
        ///     &#10;If a number is provided, it is automatically converted into a pixel value.
        ///     &#10;Part of CSS
        ///	</summary>
        ///	<returns type="jQuery" />
        ///	<param name="name" type="String">
        ///     &#10;A CSS property name.
        ///	</param>
        ///	<param name="value" type="String">
        ///     &#10;A value to set for the property.
        ///	</param>

        // Setting 'undefined' is a no-op
        if (arguments.length === 2 && value === undefined) {
            return this;
        }

        return jQuery.access(this, name, value, true, function (elem, name, value) {
            return value !== undefined ?
			jQuery.style(elem, name, value) :
			jQuery.css(elem, name);
        });
    };

    jQuery.extend({
        // Add in style property hooks for overriding the default
        // behavior of getting and setting a style property
        cssHooks: {
            opacity: {
                get: function (elem, computed) {
                    if (computed) {
                        // We should always get a number back from opacity
                        var ret = curCSS(elem, "opacity", "opacity");
                        return ret === "" ? "1" : ret;

                    } else {
                        return elem.style.opacity;
                    }
                }
            }
        },

        // Exclude the following css properties to add px
        cssNumber: {
            "zIndex": true,
            "fontWeight": true,
            "opacity": true,
            "zoom": true,
            "lineHeight": true
        },

        // Add in properties whose names you wish to fix before
        // setting or getting the value
        cssProps: {
            // normalize float css property
            "float": jQuery.support.cssFloat ? "cssFloat" : "styleFloat"
        },

        // Get and set the style property on a DOM Node
        style: function (elem, name, value, extra) {
            // Don't set styles on text and comment nodes
            if (!elem || elem.nodeType === 3 || elem.nodeType === 8 || !elem.style) {
                return;
            }

            // Make sure that we're working with the right name
            var ret, origName = jQuery.camelCase(name),
			style = elem.style, hooks = jQuery.cssHooks[origName];

            name = jQuery.cssProps[origName] || origName;

            // Check if we're setting a value
            if (value !== undefined) {
                // Make sure that NaN and null values aren't set. See: #7116
                if (typeof value === "number" && isNaN(value) || value == null) {
                    return;
                }

                // If a number was passed in, add 'px' to the (except for certain CSS properties)
                if (typeof value === "number" && !jQuery.cssNumber[origName]) {
                    value += "px";
                }

                // If a hook was provided, use that value, otherwise just set the specified value
                if (!hooks || !("set" in hooks) || (value = hooks.set(elem, value)) !== undefined) {
                    // Wrapped to prevent IE from throwing errors when 'invalid' values are provided
                    // Fixes bug #5509
                    try {
                        style[name] = value;
                    } catch (e) { }
                }

            } else {
                // If a hook was provided get the non-computed value from there
                if (hooks && "get" in hooks && (ret = hooks.get(elem, false, extra)) !== undefined) {
                    return ret;
                }

                // Otherwise just get the value from the style object
                return style[name];
            }
        },

        css: function (elem, name, extra) {
            ///	<summary>
            ///     &#10;This method is internal only.
            ///	</summary>
            ///	<private />

            // Make sure that we're working with the right name
            var ret, origName = jQuery.camelCase(name),
			hooks = jQuery.cssHooks[origName];

            name = jQuery.cssProps[origName] || origName;

            // If a hook was provided get the computed value from there
            if (hooks && "get" in hooks && (ret = hooks.get(elem, true, extra)) !== undefined) {
                return ret;

                // Otherwise, if a way to get the computed value exists, use that
            } else if (curCSS) {
                return curCSS(elem, name, origName);
            }
        },

        // A method for quickly swapping in/out CSS properties to get correct calculations
        swap: function (elem, options, callback) {
            ///	<summary>
            ///     &#10;Swap in/out style options.
            ///	</summary>

            var old = {};

            // Remember the old values, and insert the new ones
            for (var name in options) {
                old[name] = elem.style[name];
                elem.style[name] = options[name];
            }

            callback.call(elem);

            // Revert the old values
            for (name in options) {
                elem.style[name] = old[name];
            }
        },

        camelCase: function (string) {
            return string.replace(rdashAlpha, fcamelCase);
        }
    });

    // DEPRECATED, Use jQuery.css() instead
    jQuery.curCSS = jQuery.css;

    jQuery.each(["height", "width"], function (i, name) {
        jQuery.cssHooks[name] = {
            get: function (elem, computed, extra) {
                var val;

                if (computed) {
                    if (elem.offsetWidth !== 0) {
                        val = getWH(elem, name, extra);

                    } else {
                        jQuery.swap(elem, cssShow, function () {
                            val = getWH(elem, name, extra);
                        });
                    }

                    if (val <= 0) {
                        val = curCSS(elem, name, name);

                        if (val === "0px" && currentStyle) {
                            val = currentStyle(elem, name, name);
                        }

                        if (val != null) {
                            // Should return "auto" instead of 0, use 0 for
                            // temporary backwards-compat
                            return val === "" || val === "auto" ? "0px" : val;
                        }
                    }

                    if (val < 0 || val == null) {
                        val = elem.style[name];

                        // Should return "auto" instead of 0, use 0 for
                        // temporary backwards-compat
                        return val === "" || val === "auto" ? "0px" : val;
                    }

                    return typeof val === "string" ? val : val + "px";
                }
            },

            set: function (elem, value) {
                if (rnumpx.test(value)) {
                    // ignore negative width and height values #1599
                    value = parseFloat(value);

                    if (value >= 0) {
                        return value + "px";
                    }

                } else {
                    return value;
                }
            }
        };
    });

    if (!jQuery.support.opacity) {
        jQuery.cssHooks.opacity = {
            get: function (elem, computed) {
                // IE uses filters for opacity
                return ropacity.test((computed && elem.currentStyle ? elem.currentStyle.filter : elem.style.filter) || "") ?
				(parseFloat(RegExp.$1) / 100) + "" :
				computed ? "1" : "";
            },

            set: function (elem, value) {
                var style = elem.style;

                // IE has trouble with opacity if it does not have layout
                // Force it by setting the zoom level
                style.zoom = 1;

                // Set the alpha filter to set the opacity
                var opacity = jQuery.isNaN(value) ?
				"" :
				"alpha(opacity=" + value * 100 + ")",
				filter = style.filter || "";

                style.filter = ralpha.test(filter) ?
				filter.replace(ralpha, opacity) :
				style.filter + ' ' + opacity;
            }
        };
    }

    if (document.defaultView && document.defaultView.getComputedStyle) {
        getComputedStyle = function (elem, newName, name) {
            var ret, defaultView, computedStyle;

            name = name.replace(rupper, "-$1").toLowerCase();

            if (!(defaultView = elem.ownerDocument.defaultView)) {
                return undefined;
            }

            if ((computedStyle = defaultView.getComputedStyle(elem, null))) {
                ret = computedStyle.getPropertyValue(name);
                if (ret === "" && !jQuery.contains(elem.ownerDocument.documentElement, elem)) {
                    ret = jQuery.style(elem, name);
                }
            }

            return ret;
        };
    }

    if (document.documentElement.currentStyle) {
        currentStyle = function (elem, name) {
            var left, rsLeft,
			ret = elem.currentStyle && elem.currentStyle[name],
			style = elem.style;

            // From the awesome hack by Dean Edwards
            // http://erik.eae.net/archives/2007/07/27/18.54.15/#comment-102291

            // If we're not dealing with a regular pixel number
            // but a number that has a weird ending, we need to convert it to pixels
            if (!rnumpx.test(ret) && rnum.test(ret)) {
                // Remember the original values
                left = style.left;
                rsLeft = elem.runtimeStyle.left;

                // Put in the new values to get a computed value out
                elem.runtimeStyle.left = elem.currentStyle.left;
                style.left = name === "fontSize" ? "1em" : (ret || 0);
                ret = style.pixelLeft + "px";

                // Revert the changed values
                style.left = left;
                elem.runtimeStyle.left = rsLeft;
            }

            return ret === "" ? "auto" : ret;
        };
    }

    curCSS = getComputedStyle || currentStyle;

    function getWH(elem, name, extra) {
        var which = name === "width" ? cssWidth : cssHeight,
		val = name === "width" ? elem.offsetWidth : elem.offsetHeight;

        if (extra === "border") {
            return val;
        }

        jQuery.each(which, function () {
            if (!extra) {
                val -= parseFloat(jQuery.css(elem, "padding" + this)) || 0;
            }

            if (extra === "margin") {
                val += parseFloat(jQuery.css(elem, "margin" + this)) || 0;

            } else {
                val -= parseFloat(jQuery.css(elem, "border" + this + "Width")) || 0;
            }
        });

        return val;
    }

    if (jQuery.expr && jQuery.expr.filters) {
        jQuery.expr.filters.hidden = function (elem) {
            var width = elem.offsetWidth,
			height = elem.offsetHeight;

            return (width === 0 && height === 0) || (!jQuery.support.reliableHiddenOffsets && (elem.style.display || jQuery.css(elem, "display")) === "none");
        };

        jQuery.expr.filters.visible = function (elem) {
            return !jQuery.expr.filters.hidden(elem);
        };
    }




    var jsc = jQuery.now(),
	rscript = /<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>/gi,
	rselectTextarea = /^(?:select|textarea)/i,
	rinput = /^(?:color|date|datetime|email|hidden|month|number|password|range|search|tel|text|time|url|week)$/i,
	rnoContent = /^(?:GET|HEAD)$/,
	rbracket = /\[\]$/,
	jsre = /\=\?(&|$)/,
	rquery = /\?/,
	rts = /([?&])_=[^&]*/,
	rurl = /^(\w+:)?\/\/([^\/?#]+)/,
	r20 = /%20/g,
	rhash = /#.*$/,

    // Keep a copy of the old load method
	_load = jQuery.fn.load;

    jQuery.fn.extend({
        load: function (url, params, callback) {
            ///	<summary>
            ///     &#10;Loads HTML from a remote file and injects it into the DOM.  By default performs a GET request, but if parameters are included
            ///     &#10;then a POST will be performed.
            ///	</summary>
            ///	<param name="url" type="String">The URL of the HTML page to load.</param>
            ///	<param name="data" optional="true" type="Map">Key/value pairs that will be sent to the server.</param>
            ///	<param name="callback" optional="true" type="Function">The function called when the AJAX request is complete.  It should map function(responseText, textStatus, XMLHttpRequest) such that this maps the injected DOM element.</param>
            ///	<returns type="jQuery" />

            if (typeof url !== "string" && _load) {
                return _load.apply(this, arguments);

                // Don't do a request if no elements are being requested
            } else if (!this.length) {
                return this;
            }

            var off = url.indexOf(" ");
            if (off >= 0) {
                var selector = url.slice(off, url.length);
                url = url.slice(0, off);
            }

            // Default to a GET request
            var type = "GET";

            // If the second parameter was provided
            if (params) {
                // If it's a function
                if (jQuery.isFunction(params)) {
                    // We assume that it's the callback
                    callback = params;
                    params = null;

                    // Otherwise, build a param string
                } else if (typeof params === "object") {
                    params = jQuery.param(params, jQuery.ajaxSettings.traditional);
                    type = "POST";
                }
            }

            var self = this;

            // Request the remote document
            jQuery.ajax({
                url: url,
                type: type,
                dataType: "html",
                data: params,
                complete: function (res, status) {
                    // If successful, inject the HTML into all the matched elements
                    if (status === "success" || status === "notmodified") {
                        // See if a selector was specified
                        self.html(selector ?
                        // Create a dummy div to hold the results
						jQuery("<div>")
                        // inject the contents of the document in, removing the scripts
                        // to avoid any 'Permission Denied' errors in IE
							.append(res.responseText.replace(rscript, ""))

                        // Locate the specified elements
							.find(selector) :

                        // If not, just inject the full result
						res.responseText);
                    }

                    if (callback) {
                        self.each(callback, [res.responseText, status, res]);
                    }
                }
            });

            return this;
        },

        serialize: function () {
            ///	<summary>
            ///     &#10;Serializes a set of input elements into a string of data.
            ///	</summary>
            ///	<returns type="String">The serialized result</returns>

            return jQuery.param(this.serializeArray());
        },

        serializeArray: function () {
            ///	<summary>
            ///     &#10;Serializes all forms and form elements but returns a JSON data structure.
            ///	</summary>
            ///	<returns type="String">A JSON data structure representing the serialized items.</returns>

            return this.map(function () {
                return this.elements ? jQuery.makeArray(this.elements) : this;
            })
		.filter(function () {
		    return this.name && !this.disabled &&
				(this.checked || rselectTextarea.test(this.nodeName) ||
					rinput.test(this.type));
		})
		.map(function (i, elem) {
		    var val = jQuery(this).val();

		    return val == null ?
				null :
				jQuery.isArray(val) ?
					jQuery.map(val, function (val, i) {
					    return { name: elem.name, value: val };
					}) :
					{ name: elem.name, value: val };
		}).get();
        }
    });

    // Attach a bunch of functions for handling common AJAX events
    //	jQuery.each( "ajaxStart ajaxStop ajaxComplete ajaxError ajaxSuccess ajaxSend".split(" "), function( i, o ) {
    //		jQuery.fn[o] = function( f ) {
    //			return this.bind(o, f);
    //		};
    //	});

    jQuery.fn["ajaxStart"] = function (f) {
        ///	<summary>
        ///     &#10;Attach a function to be executed whenever an AJAX request begins and there is none already active. This is an Ajax Event.
        ///	</summary>
        ///	<param name="f" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return this.bind("ajaxStart", f);
    };

    jQuery.fn["ajaxStop"] = function (f) {
        ///	<summary>
        ///     &#10;Attach a function to be executed whenever all AJAX requests have ended. This is an Ajax Event.
        ///	</summary>
        ///	<param name="f" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return this.bind("ajaxStop", f);
    };

    jQuery.fn["ajaxComplete"] = function (f) {
        ///	<summary>
        ///     &#10;Attach a function to be executed whenever an AJAX request completes. This is an Ajax Event.
        ///	</summary>
        ///	<param name="f" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return this.bind("ajaxComplete", f);
    };

    jQuery.fn["ajaxError"] = function (f) {
        ///	<summary>
        ///     &#10;Attach a function to be executed whenever an AJAX request fails. This is an Ajax Event.
        ///	</summary>
        ///	<param name="f" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return this.bind("ajaxError", f);
    };

    jQuery.fn["ajaxSuccess"] = function (f) {
        ///	<summary>
        ///     &#10;Attach a function to be executed whenever an AJAX request completes successfully. This is an Ajax Event.
        ///	</summary>
        ///	<param name="f" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return this.bind("ajaxSuccess", f);
    };

    jQuery.fn["ajaxSend"] = function (f) {
        ///	<summary>
        ///     &#10;Attach a function to be executed before an AJAX request is sent. This is an Ajax Event.
        ///	</summary>
        ///	<param name="f" type="Function">The function to execute.</param>
        ///	<returns type="jQuery" />

        return this.bind("ajaxSend", f);
    };

    jQuery.extend({
        get: function (url, data, callback, type) {
            ///	<summary>
            ///     &#10;Loads a remote page using an HTTP GET request.
            ///	</summary>
            ///	<param name="url" type="String">The URL of the HTML page to load.</param>
            ///	<param name="data" optional="true" type="Map">Key/value pairs that will be sent to the server.</param>
            ///	<param name="callback" optional="true" type="Function">The function called when the AJAX request is complete.  It should map function(responseText, textStatus) such that this maps the options for this AJAX request.</param>
            ///	<param name="type" optional="true" type="String">Type of data to be returned to callback function.  Valid valiues are xml, html, script, json, text, _default.</param>
            ///	<returns type="XMLHttpRequest" />

            // shift arguments if data argument was omited
            if (jQuery.isFunction(data)) {
                type = type || callback;
                callback = data;
                data = null;
            }

            return jQuery.ajax({
                type: "GET",
                url: url,
                data: data,
                success: callback,
                dataType: type
            });
        },

        getScript: function (url, callback) {
            ///	<summary>
            ///     &#10;Loads and executes a local JavaScript file using an HTTP GET request.
            ///	</summary>
            ///	<param name="url" type="String">The URL of the script to load.</param>
            ///	<param name="callback" optional="true" type="Function">The function called when the AJAX request is complete.  It should map function(data, textStatus) such that this maps the options for the AJAX request.</param>
            ///	<returns type="XMLHttpRequest" />

            return jQuery.get(url, null, callback, "script");
        },

        getJSON: function (url, data, callback) {
            ///	<summary>
            ///     &#10;Loads JSON data using an HTTP GET request.
            ///	</summary>
            ///	<param name="url" type="String">The URL of the JSON data to load.</param>
            ///	<param name="data" optional="true" type="Map">Key/value pairs that will be sent to the server.</param>
            ///	<param name="callback" optional="true" type="Function">The function called when the AJAX request is complete if the data is loaded successfully.  It should map function(data, textStatus) such that this maps the options for this AJAX request.</param>
            ///	<returns type="XMLHttpRequest" />

            return jQuery.get(url, data, callback, "json");
        },

        post: function (url, data, callback, type) {
            ///	<summary>
            ///     &#10;Loads a remote page using an HTTP POST request.
            ///	</summary>
            ///	<param name="url" type="String">The URL of the HTML page to load.</param>
            ///	<param name="data" optional="true" type="Map">Key/value pairs that will be sent to the server.</param>
            ///	<param name="callback" optional="true" type="Function">The function called when the AJAX request is complete.  It should map function(responseText, textStatus) such that this maps the options for this AJAX request.</param>
            ///	<param name="type" optional="true" type="String">Type of data to be returned to callback function.  Valid valiues are xml, html, script, json, text, _default.</param>
            ///	<returns type="XMLHttpRequest" />

            // shift arguments if data argument was omited
            if (jQuery.isFunction(data)) {
                type = type || callback;
                callback = data;
                data = {};
            }

            return jQuery.ajax({
                type: "POST",
                url: url,
                data: data,
                success: callback,
                dataType: type
            });
        },

        ajaxSetup: function (settings) {
            ///	<summary>
            ///     &#10;Sets up global settings for AJAX requests.
            ///	</summary>
            ///	<param name="settings" type="Options">A set of key/value pairs that configure the default Ajax request.</param>

            jQuery.extend(jQuery.ajaxSettings, settings);
        },

        ajaxSettings: {
            url: location.href,
            global: true,
            type: "GET",
            contentType: "application/x-www-form-urlencoded",
            processData: true,
            async: true,
            /*
            timeout: 0,
            data: null,
            username: null,
            password: null,
            traditional: false,
            */
            // This function can be overriden by calling jQuery.ajaxSetup
            xhr: function () {
                return new window.XMLHttpRequest();
            },
            accepts: {
                xml: "application/xml, text/xml",
                html: "text/html",
                script: "text/javascript, application/javascript",
                json: "application/json, text/javascript",
                text: "text/plain",
                _default: "*/*"
            }
        },

        ajax: function (url, options) {
            /// <summary>
            ///     Perform an asynchronous HTTP (Ajax) request.
            ///     &#10;1 - jQuery.ajax(url, settings) 
            ///     &#10;2 - jQuery.ajax(settings)
            /// </summary>
            /// <param name="url" type="String">
            ///     A string containing the URL to which the request is sent.
            /// </param>
            /// <param name="options" type="Object">
            ///     A set of key/value pairs that configure the Ajax request.
            /// </param>

            // If url is an object, simulate pre-1.5 signature
            if (typeof url === "object") {
                options = url;
                url = undefined;
            }

            // Force options to be an object
            options = options || {};

            var // Create the final options object
			    s = jQuery.ajaxSetup({}, options),
            // Callbacks context
			    callbackContext = s.context || s,
            // Context for global events
            // It's the callbackContext if one was provided in the options
            // and if it's a DOM node or a jQuery collection
			    globalEventContext = callbackContext !== s &&
				    (callbackContext.nodeType || callbackContext instanceof jQuery) ?
						    jQuery(callbackContext) : jQuery.event,
            // Deferreds
			    deferred = jQuery.Deferred(),
			    completeDeferred = jQuery._Deferred(),
            // Status-dependent callbacks
			    statusCode = s.statusCode || {},
            // ifModified key
			    ifModifiedKey,
            // Headers (they are sent all at once)
			    requestHeaders = {},
            // Response headers
			    responseHeadersString,
			    responseHeaders,
            // transport
			    transport,
            // timeout handle
			    timeoutTimer,
            // Cross-domain detection vars
			    parts,
            // The jqXHR state
			    state = 0,
            // To know if global events are to be dispatched
			    fireGlobals,
            // Loop variable
			    i,
            // Fake xhr
			    jqXHR = {

			        readyState: 0,

			        // Caches the header
			        setRequestHeader: function (name, value) {
			            if (!state) {
			                requestHeaders[name.toLowerCase().replace(rucHeaders, rucHeadersFunc)] = value;
			            }
			            return this;
			        },

			        // Raw string
			        getAllResponseHeaders: function () {
			            return state === 2 ? responseHeadersString : null;
			        },

			        // Builds headers hashtable if needed
			        getResponseHeader: function (key) {
			            var match;
			            if (state === 2) {
			                if (!responseHeaders) {
			                    responseHeaders = {};
			                    while ((match = rheaders.exec(responseHeadersString))) {
			                        responseHeaders[match[1].toLowerCase()] = match[2];
			                    }
			                }
			                match = responseHeaders[key.toLowerCase()];
			            }
			            return match === undefined ? null : match;
			        },

			        // Overrides response content-type header
			        overrideMimeType: function (type) {
			            if (!state) {
			                s.mimeType = type;
			            }
			            return this;
			        },

			        // Cancel the request
			        abort: function (statusText) {
			            statusText = statusText || "abort";
			            if (transport) {
			                transport.abort(statusText);
			            }
			            done(0, statusText);
			            return this;
			        }
			    };

            // Callback for when everything is done
            // It is defined here because jslint complains if it is declared
            // at the end of the function (which would be more logical and readable)
            function done(status, statusText, responses, headers) {

                // Called once
                if (state === 2) {
                    return;
                }

                // State is "done" now
                state = 2;

                // Clear timeout if it exists
                if (timeoutTimer) {
                    clearTimeout(timeoutTimer);
                }

                // Dereference transport for early garbage collection
                // (no matter how long the jqXHR object will be used)
                transport = undefined;

                // Cache response headers
                responseHeadersString = headers || "";

                // Set readyState
                jqXHR.readyState = status ? 4 : 0;

                var isSuccess,
				    success,
				    error,
				    response = responses ? ajaxHandleResponses(s, jqXHR, responses) : undefined,
				    lastModified,
				    etag;

                // If successful, handle type chaining
                if (status >= 200 && status < 300 || status === 304) {

                    // Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
                    if (s.ifModified) {

                        if ((lastModified = jqXHR.getResponseHeader("Last-Modified"))) {
                            jQuery.lastModified[ifModifiedKey] = lastModified;
                        }
                        if ((etag = jqXHR.getResponseHeader("Etag"))) {
                            jQuery.etag[ifModifiedKey] = etag;
                        }
                    }

                    // If not modified
                    if (status === 304) {

                        statusText = "notmodified";
                        isSuccess = true;

                        // If we have data
                    } else {

                        try {
                            success = ajaxConvert(s, response);
                            statusText = "success";
                            isSuccess = true;
                        } catch (e) {
                            // We have a parsererror
                            statusText = "parsererror";
                            error = e;
                        }
                    }
                } else {
                    // We extract error from statusText
                    // then normalize statusText and status for non-aborts
                    error = statusText;
                    if (!statusText || status) {
                        statusText = "error";
                        if (status < 0) {
                            status = 0;
                        }
                    }
                }

                // Set data for the fake xhr object
                jqXHR.status = status;
                jqXHR.statusText = statusText;

                // Success/Error
                if (isSuccess) {
                    deferred.resolveWith(callbackContext, [success, statusText, jqXHR]);
                } else {
                    deferred.rejectWith(callbackContext, [jqXHR, statusText, error]);
                }

                // Status-dependent callbacks
                jqXHR.statusCode(statusCode);
                statusCode = undefined;

                if (fireGlobals) {
                    globalEventContext.trigger("ajax" + (isSuccess ? "Success" : "Error"),
						    [jqXHR, s, isSuccess ? success : error]);
                }

                // Complete
                completeDeferred.resolveWith(callbackContext, [jqXHR, statusText]);

                if (fireGlobals) {
                    globalEventContext.trigger("ajaxComplete", [jqXHR, s]);
                    // Handle the global AJAX counter
                    if (!(--jQuery.active)) {
                        jQuery.event.trigger("ajaxStop");
                    }
                }
            }

            // Attach deferreds
            deferred.promise(jqXHR);
            jqXHR.success = jqXHR.done;
            jqXHR.error = jqXHR.fail;
            jqXHR.complete = completeDeferred.done;

            // Status-dependent callbacks
            jqXHR.statusCode = function (map) {
                if (map) {
                    var tmp;
                    if (state < 2) {
                        for (tmp in map) {
                            statusCode[tmp] = [statusCode[tmp], map[tmp]];
                        }
                    } else {
                        tmp = map[jqXHR.status];
                        jqXHR.then(tmp, tmp);
                    }
                }
                return this;
            };

            // Remove hash character (#7531: and string promotion)
            // Add protocol if not provided (#5866: IE7 issue with protocol-less urls)
            // We also use the url parameter if available
            s.url = ((url || s.url) + "").replace(rhash, "").replace(rprotocol, ajaxLocParts[1] + "//");

            // Extract dataTypes list
            s.dataTypes = jQuery.trim(s.dataType || "*").toLowerCase().split(rspacesAjax);

            // Determine if a cross-domain request is in order
            if (!s.crossDomain) {
                parts = rurl.exec(s.url.toLowerCase());
                s.crossDomain = !!(parts &&
				    (parts[1] != ajaxLocParts[1] || parts[2] != ajaxLocParts[2] ||
					    (parts[3] || (parts[1] === "http:" ? 80 : 443)) !=
						    (ajaxLocParts[3] || (ajaxLocParts[1] === "http:" ? 80 : 443)))
			    );
            }

            // Convert data if not already a string
            if (s.data && s.processData && typeof s.data !== "string") {
                s.data = jQuery.param(s.data, s.traditional);
            }

            // Apply prefilters
            inspectPrefiltersOrTransports(prefilters, s, options, jqXHR);

            // If request was aborted inside a prefiler, stop there
            if (state === 2) {
                return false;
            }

            // We can fire global events as of now if asked to
            fireGlobals = s.global;

            // Uppercase the type
            s.type = s.type.toUpperCase();

            // Determine if request has content
            s.hasContent = !rnoContent.test(s.type);

            // Watch for a new set of requests
            if (fireGlobals && jQuery.active++ === 0) {
                jQuery.event.trigger("ajaxStart");
            }

            // More options handling for requests with no content
            if (!s.hasContent) {

                // If data is available, append data to url
                if (s.data) {
                    s.url += (rquery.test(s.url) ? "&" : "?") + s.data;
                }

                // Get ifModifiedKey before adding the anti-cache parameter
                ifModifiedKey = s.url;

                // Add anti-cache in url if needed
                if (s.cache === false) {

                    var ts = jQuery.now(),
                    // try replacing _= if it is there
					    ret = s.url.replace(rts, "$1_=" + ts);

                    // if nothing was replaced, add timestamp to the end
                    s.url = ret + ((ret === s.url) ? (rquery.test(s.url) ? "&" : "?") + "_=" + ts : "");
                }
            }

            // Set the correct header, if data is being sent
            if (s.data && s.hasContent && s.contentType !== false || options.contentType) {
                requestHeaders["Content-Type"] = s.contentType;
            }

            // Set the If-Modified-Since and/or If-None-Match header, if in ifModified mode.
            if (s.ifModified) {
                ifModifiedKey = ifModifiedKey || s.url;
                if (jQuery.lastModified[ifModifiedKey]) {
                    requestHeaders["If-Modified-Since"] = jQuery.lastModified[ifModifiedKey];
                }
                if (jQuery.etag[ifModifiedKey]) {
                    requestHeaders["If-None-Match"] = jQuery.etag[ifModifiedKey];
                }
            }

            // Set the Accepts header for the server, depending on the dataType
            requestHeaders.Accept = s.dataTypes[0] && s.accepts[s.dataTypes[0]] ?
			    s.accepts[s.dataTypes[0]] + (s.dataTypes[0] !== "*" ? ", */*; q=0.01" : "") :
			    s.accepts["*"];

            // Check for headers option
            for (i in s.headers) {
                jqXHR.setRequestHeader(i, s.headers[i]);
            }

            // Allow custom headers/mimetypes and early abort
            if (s.beforeSend && (s.beforeSend.call(callbackContext, jqXHR, s) === false || state === 2)) {
                // Abort if not done already
                jqXHR.abort();
                return false;

            }

            // Install callbacks on deferreds
            for (i in { success: 1, error: 1, complete: 1 }) {
                jqXHR[i](s[i]);
            }

            // Get transport
            transport = inspectPrefiltersOrTransports(transports, s, options, jqXHR);

            // If no transport, we auto-abort
            if (!transport) {
                done(-1, "No Transport");
            } else {
                jqXHR.readyState = 1;
                // Send global event
                if (fireGlobals) {
                    globalEventContext.trigger("ajaxSend", [jqXHR, s]);
                }
                // Timeout
                if (s.async && s.timeout > 0) {
                    timeoutTimer = setTimeout(function () {
                        jqXHR.abort("timeout");
                    }, s.timeout);
                }

                try {
                    state = 1;
                    transport.send(requestHeaders, done);
                } catch (e) {
                    // Propagate exception as error if not done
                    if (status < 2) {
                        done(-1, e);
                        // Simply rethrow otherwise
                    } else {
                        jQuery.error(e);
                    }
                }
            }

            return jqXHR;
        },

        // Serialize an array of form elements or a set of
        // key/values into a query string
        param: function (a, traditional) {
            ///	<summary>
            ///     &#10;Create a serialized representation of an array or object, suitable for use in a URL
            ///     &#10;query string or Ajax request.
            ///	</summary>
            ///	<param name="a" type="Object">
            ///     &#10;An array or object to serialize.
            ///	</param>
            ///	<param name="traditional" type="Boolean">
            ///     &#10;A Boolean indicating whether to perform a traditional "shallow" serialization.
            ///	</param>
            ///	<returns type="String" />

            var s = [],
			add = function (key, value) {
			    // If value is a function, invoke it and return its value
			    value = jQuery.isFunction(value) ? value() : value;
			    s[s.length] = encodeURIComponent(key) + "=" + encodeURIComponent(value);
			};

            // Set traditional to true for jQuery <= 1.3.2 behavior.
            if (traditional === undefined) {
                traditional = jQuery.ajaxSettings.traditional;
            }

            // If an array was passed in, assume that it is an array of form elements.
            if (jQuery.isArray(a) || a.jquery) {
                // Serialize the form elements
                jQuery.each(a, function () {
                    add(this.name, this.value);
                });

            } else {
                // If traditional, encode the "old" way (the way 1.3.2 or older
                // did it), otherwise encode params recursively.
                for (var prefix in a) {
                    buildParams(prefix, a[prefix], traditional, add);
                }
            }

            // Return the resulting serialization
            return s.join("&").replace(r20, "+");
        }
    });

    function buildParams(prefix, obj, traditional, add) {
        if (jQuery.isArray(obj) && obj.length) {
            // Serialize array item.
            jQuery.each(obj, function (i, v) {
                if (traditional || rbracket.test(prefix)) {
                    // Treat each array item as a scalar.
                    add(prefix, v);

                } else {
                    // If array item is non-scalar (array or object), encode its
                    // numeric index to resolve deserialization ambiguity issues.
                    // Note that rack (as of 1.0.0) can't currently deserialize
                    // nested arrays properly, and attempting to do so may cause
                    // a server error. Possible fixes are to modify rack's
                    // deserialization algorithm or to provide an option or flag
                    // to force array serialization to be shallow.
                    buildParams(prefix + "[" + (typeof v === "object" || jQuery.isArray(v) ? i : "") + "]", v, traditional, add);
                }
            });

        } else if (!traditional && obj != null && typeof obj === "object") {
            if (jQuery.isEmptyObject(obj)) {
                add(prefix, "");

                // Serialize object item.
            } else {
                jQuery.each(obj, function (k, v) {
                    buildParams(prefix + "[" + k + "]", v, traditional, add);
                });
            }

        } else {
            // Serialize scalar item.
            add(prefix, obj);
        }
    }

    // This is still on the jQuery object... for now
    // Want to move this to jQuery.ajax some day
    jQuery.extend({

        // Counter for holding the number of active queries
        active: 0,

        // Last-Modified header cache for next request
        lastModified: {},
        etag: {},

        handleError: function (s, xhr, status, e) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            // If a local callback was specified, fire it
            if (s.error) {
                s.error.call(s.context, xhr, status, e);
            }

            // Fire the global callback
            if (s.global) {
                jQuery.triggerGlobal(s, "ajaxError", [xhr, s, e]);
            }
        },

        handleSuccess: function (s, xhr, status, data) {
            // If a local callback was specified, fire it and pass it the data
            if (s.success) {
                s.success.call(s.context, data, status, xhr);
            }

            // Fire the global callback
            if (s.global) {
                jQuery.triggerGlobal(s, "ajaxSuccess", [xhr, s]);
            }
        },

        handleComplete: function (s, xhr, status) {
            // Process result
            if (s.complete) {
                s.complete.call(s.context, xhr, status);
            }

            // The request was completed
            if (s.global) {
                jQuery.triggerGlobal(s, "ajaxComplete", [xhr, s]);
            }

            // Handle the global AJAX counter
            if (s.global && jQuery.active-- === 1) {
                jQuery.event.trigger("ajaxStop");
            }
        },

        triggerGlobal: function (s, type, args) {
            (s.context && s.context.url == null ? jQuery(s.context) : jQuery.event).trigger(type, args);
        },

        // Determines if an XMLHttpRequest was successful or not
        httpSuccess: function (xhr) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            try {
                // IE error sometimes returns 1223 when it should be 204 so treat it as success, see #1450
                return !xhr.status && location.protocol === "file:" ||
				xhr.status >= 200 && xhr.status < 300 ||
				xhr.status === 304 || xhr.status === 1223;
            } catch (e) { }

            return false;
        },

        // Determines if an XMLHttpRequest returns NotModified
        httpNotModified: function (xhr, url) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            var lastModified = xhr.getResponseHeader("Last-Modified"),
			etag = xhr.getResponseHeader("Etag");

            if (lastModified) {
                jQuery.lastModified[url] = lastModified;
            }

            if (etag) {
                jQuery.etag[url] = etag;
            }

            return xhr.status === 304;
        },

        httpData: function (xhr, type, s) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            var ct = xhr.getResponseHeader("content-type") || "",
			xml = type === "xml" || !type && ct.indexOf("xml") >= 0,
			data = xml ? xhr.responseXML : xhr.responseText;

            if (xml && data.documentElement.nodeName === "parsererror") {
                jQuery.error("parsererror");
            }

            // Allow a pre-filtering function to sanitize the response
            // s is checked to keep backwards compatibility
            if (s && s.dataFilter) {
                data = s.dataFilter(data, type);
            }

            // The filter can actually parse the response
            if (typeof data === "string") {
                // Get the JavaScript object, if JSON is used.
                if (type === "json" || !type && ct.indexOf("json") >= 0) {
                    data = jQuery.parseJSON(data);

                    // If the type is "script", eval it in global context
                } else if (type === "script" || !type && ct.indexOf("javascript") >= 0) {
                    jQuery.globalEval(data);
                }
            }

            return data;
        }

    });

    /*
    * Create the request object; Microsoft failed to properly
    * implement the XMLHttpRequest in IE7 (can't request local files),
    * so we use the ActiveXObject when it is available
    * Additionally XMLHttpRequest can be disabled in IE7/IE8 so
    * we need a fallback.
    */
    if (window.ActiveXObject) {
        jQuery.ajaxSettings.xhr = function () {
            if (window.location.protocol !== "file:") {
                try {
                    return new window.XMLHttpRequest();
                } catch (xhrError) { }
            }

            try {
                return new window.ActiveXObject("Microsoft.XMLHTTP");
            } catch (activeError) { }
        };
    }

    // Does this browser support XHR requests?
    jQuery.support.ajax = !!jQuery.ajaxSettings.xhr();




    var elemdisplay = {},
	rfxtypes = /^(?:toggle|show|hide)$/,
	rfxnum = /^([+\-]=)?([\d+.\-]+)(.*)$/,
	timerId,
	fxAttrs = [
    // height animations
		["height", "marginTop", "marginBottom", "paddingTop", "paddingBottom"],
    // width animations
		["width", "marginLeft", "marginRight", "paddingLeft", "paddingRight"],
    // opacity animations
		["opacity"]
	];

    jQuery.fn.extend({
        show: function (speed, easing, callback) {
            ///	<summary>
            ///     &#10;Show all matched elements using a graceful animation and firing an optional callback after completion.
            ///	</summary>
            ///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
            ///     &#10;the number of milliseconds to run the animation</param>
            ///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
            ///	<returns type="jQuery" />

            var elem, display;

            if (speed || speed === 0) {
                return this.animate(genFx("show", 3), speed, easing, callback);

            } else {
                for (var i = 0, j = this.length; i < j; i++) {
                    elem = this[i];
                    display = elem.style.display;

                    // Reset the inline display of this element to learn if it is
                    // being hidden by cascaded rules or not
                    if (!jQuery.data(elem, "olddisplay") && display === "none") {
                        display = elem.style.display = "";
                    }

                    // Set elements which have been overridden with display: none
                    // in a stylesheet to whatever the default browser style is
                    // for such an element
                    if (display === "" && jQuery.css(elem, "display") === "none") {
                        jQuery.data(elem, "olddisplay", defaultDisplay(elem.nodeName));
                    }
                }

                // Set the display of most of the elements in a second loop
                // to avoid the constant reflow
                for (i = 0; i < j; i++) {
                    elem = this[i];
                    display = elem.style.display;

                    if (display === "" || display === "none") {
                        elem.style.display = jQuery.data(elem, "olddisplay") || "";
                    }
                }

                return this;
            }
        },

        hide: function (speed, callback) {
            ///	<summary>
            ///     &#10;Hides all matched elements using a graceful animation and firing an optional callback after completion.
            ///	</summary>
            ///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
            ///     &#10;the number of milliseconds to run the animation</param>
            ///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
            ///	<returns type="jQuery" />

            if (speed || speed === 0) {
                return this.animate(genFx("hide", 3), speed, easing, callback);

            } else {
                for (var i = 0, j = this.length; i < j; i++) {
                    var display = jQuery.css(this[i], "display");

                    if (display !== "none") {
                        jQuery.data(this[i], "olddisplay", display);
                    }
                }

                // Set the display of the elements in a second loop
                // to avoid the constant reflow
                for (i = 0; i < j; i++) {
                    this[i].style.display = "none";
                }

                return this;
            }
        },

        // Save the old toggle function
        _toggle: jQuery.fn.toggle,

        toggle: function (fn, fn2, callback) {
            ///	<summary>
            ///     &#10;Toggles displaying each of the set of matched elements.
            ///	</summary>
            ///	<returns type="jQuery" />

            var bool = typeof fn === "boolean";

            if (jQuery.isFunction(fn) && jQuery.isFunction(fn2)) {
                this._toggle.apply(this, arguments);

            } else if (fn == null || bool) {
                this.each(function () {
                    var state = bool ? fn : jQuery(this).is(":hidden");
                    jQuery(this)[state ? "show" : "hide"]();
                });

            } else {
                this.animate(genFx("toggle", 3), fn, fn2, callback);
            }

            return this;
        },

        fadeTo: function (speed, to, easing, callback) {
            ///	<summary>
            ///     &#10;Fades the opacity of all matched elements to a specified opacity.
            ///	</summary>
            ///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
            ///     &#10;the number of milliseconds to run the animation</param>
            ///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
            ///	<returns type="jQuery" />

            return this.filter(":hidden").css("opacity", 0).show().end()
					.animate({ opacity: to }, speed, easing, callback);
        },

        animate: function (prop, speed, easing, callback) {
            ///	<summary>
            ///     &#10;A function for making custom animations.
            ///	</summary>
            ///	<param name="prop" type="Options">A set of style attributes that you wish to animate and to what end.</param>
            ///	<param name="speed" optional="true" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
            ///     &#10;the number of milliseconds to run the animation</param>
            ///	<param name="easing" optional="true" type="String">The name of the easing effect that you want to use.  There are two built-in values, 'linear' and 'swing'.</param>
            ///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
            ///	<returns type="jQuery" />

            var optall = jQuery.speed(speed, easing, callback);

            if (jQuery.isEmptyObject(prop)) {
                return this.each(optall.complete);
            }

            return this[optall.queue === false ? "each" : "queue"](function () {
                // XXX 'this' does not always have a nodeName when running the
                // test suite

                var opt = jQuery.extend({}, optall), p,
				isElement = this.nodeType === 1,
				hidden = isElement && jQuery(this).is(":hidden"),
				self = this;

                for (p in prop) {
                    var name = jQuery.camelCase(p);

                    if (p !== name) {
                        prop[name] = prop[p];
                        delete prop[p];
                        p = name;
                    }

                    if (prop[p] === "hide" && hidden || prop[p] === "show" && !hidden) {
                        return opt.complete.call(this);
                    }

                    if (isElement && (p === "height" || p === "width")) {
                        // Make sure that nothing sneaks out
                        // Record all 3 overflow attributes because IE does not
                        // change the overflow attribute when overflowX and
                        // overflowY are set to the same value
                        opt.overflow = [this.style.overflow, this.style.overflowX, this.style.overflowY];

                        // Set display property to inline-block for height/width
                        // animations on inline elements that are having width/height
                        // animated
                        if (jQuery.css(this, "display") === "inline" &&
							jQuery.css(this, "float") === "none") {
                            if (!jQuery.support.inlineBlockNeedsLayout) {
                                this.style.display = "inline-block";

                            } else {
                                var display = defaultDisplay(this.nodeName);

                                // inline-level elements accept inline-block;
                                // block-level elements need to be inline with layout
                                if (display === "inline") {
                                    this.style.display = "inline-block";

                                } else {
                                    this.style.display = "inline";
                                    this.style.zoom = 1;
                                }
                            }
                        }
                    }

                    if (jQuery.isArray(prop[p])) {
                        // Create (if needed) and add to specialEasing
                        (opt.specialEasing = opt.specialEasing || {})[p] = prop[p][1];
                        prop[p] = prop[p][0];
                    }
                }

                if (opt.overflow != null) {
                    this.style.overflow = "hidden";
                }

                opt.curAnim = jQuery.extend({}, prop);

                jQuery.each(prop, function (name, val) {
                    var e = new jQuery.fx(self, opt, name);

                    if (rfxtypes.test(val)) {
                        e[val === "toggle" ? hidden ? "show" : "hide" : val](prop);

                    } else {
                        var parts = rfxnum.exec(val),
						start = e.cur() || 0;

                        if (parts) {
                            var end = parseFloat(parts[2]),
							unit = parts[3] || "px";

                            // We need to compute starting value
                            if (unit !== "px") {
                                jQuery.style(self, name, (end || 1) + unit);
                                start = ((end || 1) / e.cur()) * start;
                                jQuery.style(self, name, start + unit);
                            }

                            // If a +=/-= token was provided, we're doing a relative animation
                            if (parts[1]) {
                                end = ((parts[1] === "-=" ? -1 : 1) * end) + start;
                            }

                            e.custom(start, end, unit);

                        } else {
                            e.custom(start, val, "");
                        }
                    }
                });

                // For JS strict compliance
                return true;
            });
        },

        stop: function (clearQueue, gotoEnd) {
            ///	<summary>
            ///     &#10;Stops all currently animations on the specified elements.
            ///	</summary>
            ///	<param name="clearQueue" optional="true" type="Boolean">True to clear animations that are queued to run.</param>
            ///	<param name="gotoEnd" optional="true" type="Boolean">True to move the element value to the end of its animation target.</param>
            ///	<returns type="jQuery" />

            var timers = jQuery.timers;

            if (clearQueue) {
                this.queue([]);
            }

            this.each(function () {
                // go in reverse order so anything added to the queue during the loop is ignored
                for (var i = timers.length - 1; i >= 0; i--) {
                    if (timers[i].elem === this) {
                        if (gotoEnd) {
                            // force the next step to be the last
                            timers[i](true);
                        }

                        timers.splice(i, 1);
                    }
                }
            });

            // start the next in the queue if the last step wasn't forced
            if (!gotoEnd) {
                this.dequeue();
            }

            return this;
        }

    });

    function genFx(type, num) {
        var obj = {};

        jQuery.each(fxAttrs.concat.apply([], fxAttrs.slice(0, num)), function () {
            obj[this] = type;
        });

        return obj;
    }

    // Generate shortcuts for custom animations
    //	jQuery.each({
    //		slideDown: genFx("show", 1),
    //		slideUp: genFx("hide", 1),
    //		slideToggle: genFx("toggle", 1),
    //		fadeIn: { opacity: "show" },
    //		fadeOut: { opacity: "hide" },
    //		fadeToggle: { opacity: "toggle" }
    //	}, function( name, props ) {
    //		jQuery.fn[ name ] = function( speed, easing, callback ) {
    //			return this.animate( props, speed, easing, callback );
    //		};
    //	});

    jQuery.fn["slideDown"] = function (speed, callback) {
        ///	<summary>
        ///     &#10;Reveal all matched elements by adjusting their height.
        ///	</summary>
        ///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
        ///     &#10;the number of milliseconds to run the animation</param>
        ///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
        ///	<returns type="jQuery" />

        return this.animate(genFx("show", 1), speed, callback);
    };

    jQuery.fn["slideUp"] = function (speed, callback) {
        ///	<summary>
        ///     &#10;Hiding all matched elements by adjusting their height.
        ///	</summary>
        ///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
        ///     &#10;the number of milliseconds to run the animation</param>
        ///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
        ///	<returns type="jQuery" />

        return this.animate(genFx("hide", 1), speed, callback);
    };

    jQuery.fn["slideToggle"] = function (speed, callback) {
        ///	<summary>
        ///     &#10;Toggles the visibility of all matched elements by adjusting their height.
        ///	</summary>
        ///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
        ///     &#10;the number of milliseconds to run the animation</param>
        ///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
        ///	<returns type="jQuery" />

        return this.animate(genFx("toggle", 1), speed, callback);
    };

    jQuery.fn["fadeIn"] = function (speed, callback) {
        ///	<summary>
        ///     &#10;Fades in all matched elements by adjusting their opacity.
        ///	</summary>
        ///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
        ///     &#10;the number of milliseconds to run the animation</param>
        ///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
        ///	<returns type="jQuery" />

        return this.animate({ opacity: "show" }, speed, callback);
    };

    jQuery.fn["fadeOut"] = function (speed, callback) {
        ///	<summary>
        ///     &#10;Fades the opacity of all matched elements to a specified opacity.
        ///	</summary>
        ///	<param name="speed" type="String">A string representing one of three predefined speeds ('slow', 'normal', or 'fast'), or
        ///     &#10;the number of milliseconds to run the animation</param>
        ///	<param name="callback" optional="true" type="Function">A function to be executed whenever the animation completes, once for each animated element.  It should map function callback() such that this is the DOM element being animated.</param>
        ///	<returns type="jQuery" />

        return this.animate({ opacity: "hide" }, speed, callback);
    };

    jQuery.extend({
        speed: function (speed, easing, fn) {
            ///	<summary>
            ///     &#10;This member is internal.
            ///	</summary>
            ///	<private />

            var opt = speed && typeof speed === "object" ? jQuery.extend({}, speed) : {
                complete: fn || !fn && easing ||
				jQuery.isFunction(speed) && speed,
                duration: speed,
                easing: fn && easing || easing && !jQuery.isFunction(easing) && easing
            };

            opt.duration = jQuery.fx.off ? 0 : typeof opt.duration === "number" ? opt.duration :
			opt.duration in jQuery.fx.speeds ? jQuery.fx.speeds[opt.duration] : jQuery.fx.speeds._default;

            // Queueing
            opt.old = opt.complete;
            opt.complete = function () {
                if (opt.queue !== false) {
                    jQuery(this).dequeue();
                }
                if (jQuery.isFunction(opt.old)) {
                    opt.old.call(this);
                }
            };

            return opt;
        },

        easing: {
            linear: function (p, n, firstNum, diff) {
                ///	<summary>
                ///     &#10;This member is internal.
                ///	</summary>
                ///	<private />

                return firstNum + diff * p;
            },
            swing: function (p, n, firstNum, diff) {
                ///	<summary>
                ///     &#10;This member is internal.
                ///	</summary>
                ///	<private />

                return ((-Math.cos(p * Math.PI) / 2) + 0.5) * diff + firstNum;
            }
        },

        timers: [],

        fx: function (elem, options, prop) {
            ///	<summary>
            ///     &#10;This member is internal.
            ///	</summary>
            ///	<private />

            this.options = options;
            this.elem = elem;
            this.prop = prop;

            if (!options.orig) {
                options.orig = {};
            }
        }

    });

    jQuery.fx.prototype = {
        // Simple function for setting a style value
        update: function () {
            ///	<summary>
            ///     &#10;This member is internal.
            ///	</summary>
            ///	<private />

            if (this.options.step) {
                this.options.step.call(this.elem, this.now, this);
            }

            (jQuery.fx.step[this.prop] || jQuery.fx.step._default)(this);
        },

        // Get the current size
        cur: function () {
            ///	<summary>
            ///     &#10;This member is internal.
            ///	</summary>
            ///	<private />

            if (this.elem[this.prop] != null && (!this.elem.style || this.elem.style[this.prop] == null)) {
                return this.elem[this.prop];
            }

            var r = parseFloat(jQuery.css(this.elem, this.prop));
            return r && r > -10000 ? r : 0;
        },

        // Start an animation from one number to another
        custom: function (from, to, unit) {
            var self = this,
			fx = jQuery.fx;

            this.startTime = jQuery.now();
            this.start = from;
            this.end = to;
            this.unit = unit || this.unit || "px";
            this.now = this.start;
            this.pos = this.state = 0;

            function t(gotoEnd) {
                return self.step(gotoEnd);
            }

            t.elem = this.elem;

            if (t() && jQuery.timers.push(t) && !timerId) {
                timerId = setInterval(fx.tick, fx.interval);
            }
        },

        // Simple 'show' function
        show: function () {
            ///	<summary>
            ///     &#10;Displays each of the set of matched elements if they are hidden.
            ///	</summary>

            // Remember where we started, so that we can go back to it later
            this.options.orig[this.prop] = jQuery.style(this.elem, this.prop);
            this.options.show = true;

            // Begin the animation
            // Make sure that we start at a small width/height to avoid any
            // flash of content
            this.custom(this.prop === "width" || this.prop === "height" ? 1 : 0, this.cur());

            // Start by showing the element
            jQuery(this.elem).show();
        },

        // Simple 'hide' function
        hide: function () {
            ///	<summary>
            ///     &#10;Hides each of the set of matched elements if they are shown.
            ///	</summary>

            // Remember where we started, so that we can go back to it later
            this.options.orig[this.prop] = jQuery.style(this.elem, this.prop);
            this.options.hide = true;

            // Begin the animation
            this.custom(this.cur(), 0);
        },

        // Each step of an animation
        step: function (gotoEnd) {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            var t = jQuery.now(), done = true;

            if (gotoEnd || t >= this.options.duration + this.startTime) {
                this.now = this.end;
                this.pos = this.state = 1;
                this.update();

                this.options.curAnim[this.prop] = true;

                for (var i in this.options.curAnim) {
                    if (this.options.curAnim[i] !== true) {
                        done = false;
                    }
                }

                if (done) {
                    // Reset the overflow
                    if (this.options.overflow != null && !jQuery.support.shrinkWrapBlocks) {
                        var elem = this.elem,
						options = this.options;

                        jQuery.each(["", "X", "Y"], function (index, value) {
                            elem.style["overflow" + value] = options.overflow[index];
                        });
                    }

                    // Hide the element if the "hide" operation was done
                    if (this.options.hide) {
                        jQuery(this.elem).hide();
                    }

                    // Reset the properties, if the item has been hidden or shown
                    if (this.options.hide || this.options.show) {
                        for (var p in this.options.curAnim) {
                            jQuery.style(this.elem, p, this.options.orig[p]);
                        }
                    }

                    // Execute the complete function
                    this.options.complete.call(this.elem);
                }

                return false;

            } else {
                var n = t - this.startTime;
                this.state = n / this.options.duration;

                // Perform the easing function, defaults to swing
                var specialEasing = this.options.specialEasing && this.options.specialEasing[this.prop];
                var defaultEasing = this.options.easing || (jQuery.easing.swing ? "swing" : "linear");
                this.pos = jQuery.easing[specialEasing || defaultEasing](this.state, n, 0, 1, this.options.duration);
                this.now = this.start + ((this.end - this.start) * this.pos);

                // Perform the next step of the animation
                this.update();
            }

            return true;
        }
    };

    jQuery.extend(jQuery.fx, {
        tick: function () {
            var timers = jQuery.timers;

            for (var i = 0; i < timers.length; i++) {
                if (!timers[i]()) {
                    timers.splice(i--, 1);
                }
            }

            if (!timers.length) {
                jQuery.fx.stop();
            }
        },

        interval: 13,

        stop: function () {
            clearInterval(timerId);
            timerId = null;
        },

        speeds: {
            slow: 600,
            fast: 200,
            // Default speed
            _default: 400
        },

        step: {
            opacity: function (fx) {
                jQuery.style(fx.elem, "opacity", fx.now);
            },

            _default: function (fx) {
                if (fx.elem.style && fx.elem.style[fx.prop] != null) {
                    fx.elem.style[fx.prop] = (fx.prop === "width" || fx.prop === "height" ? Math.max(0, fx.now) : fx.now) + fx.unit;
                } else {
                    fx.elem[fx.prop] = fx.now;
                }
            }
        }
    });

    if (jQuery.expr && jQuery.expr.filters) {
        jQuery.expr.filters.animated = function (elem) {
            return jQuery.grep(jQuery.timers, function (fn) {
                return elem === fn.elem;
            }).length;
        };
    }

    function defaultDisplay(nodeName) {
        if (!elemdisplay[nodeName]) {
            var elem = jQuery("<" + nodeName + ">").appendTo("body"),
			display = elem.css("display");

            elem.remove();

            if (display === "none" || display === "") {
                display = "block";
            }

            elemdisplay[nodeName] = display;
        }

        return elemdisplay[nodeName];
    }




    var rtable = /^t(?:able|d|h)$/i,
	rroot = /^(?:body|html)$/i;

    if ("getBoundingClientRect" in document.documentElement) {
        jQuery.fn.offset = function (options) {
            ///	<summary>
            ///     &#10;Set the current coordinates of every element in the set of matched elements,
            ///     &#10;relative to the document.
            ///	</summary>
            ///	<param name="options" type="Object">
            ///     &#10;An object containing the properties top and left, which are integers indicating the
            ///     &#10;new top and left coordinates for the elements.
            ///	</param>
            ///	<returns type="jQuery" />

            var elem = this[0], box;

            if (options) {
                return this.each(function (i) {
                    jQuery.offset.setOffset(this, options, i);
                });
            }

            if (!elem || !elem.ownerDocument) {
                return null;
            }

            if (elem === elem.ownerDocument.body) {
                return jQuery.offset.bodyOffset(elem);
            }

            try {
                box = elem.getBoundingClientRect();
            } catch (e) { }

            var doc = elem.ownerDocument,
			docElem = doc.documentElement;

            // Make sure we're not dealing with a disconnected DOM node
            if (!box || !jQuery.contains(docElem, elem)) {
                return box || { top: 0, left: 0 };
            }

            var body = doc.body,
			win = getWindow(doc),
			clientTop = docElem.clientTop || body.clientTop || 0,
			clientLeft = docElem.clientLeft || body.clientLeft || 0,
			scrollTop = (win.pageYOffset || jQuery.support.boxModel && docElem.scrollTop || body.scrollTop),
			scrollLeft = (win.pageXOffset || jQuery.support.boxModel && docElem.scrollLeft || body.scrollLeft),
			top = box.top + scrollTop - clientTop,
			left = box.left + scrollLeft - clientLeft;

            return { top: top, left: left };
        };

    } else {
        jQuery.fn.offset = function (options) {
            ///	<summary>
            ///     &#10;Set the current coordinates of every element in the set of matched elements,
            ///     &#10;relative to the document.
            ///	</summary>
            ///	<param name="options" type="Object">
            ///     &#10;An object containing the properties top and left, which are integers indicating the
            ///     &#10;new top and left coordinates for the elements.
            ///	</param>
            ///	<returns type="jQuery" />

            var elem = this[0];

            if (options) {
                return this.each(function (i) {
                    jQuery.offset.setOffset(this, options, i);
                });
            }

            if (!elem || !elem.ownerDocument) {
                return null;
            }

            if (elem === elem.ownerDocument.body) {
                return jQuery.offset.bodyOffset(elem);
            }

            jQuery.offset.initialize();

            var computedStyle,
			offsetParent = elem.offsetParent,
			prevOffsetParent = elem,
			doc = elem.ownerDocument,
			docElem = doc.documentElement,
			body = doc.body,
			defaultView = doc.defaultView,
			prevComputedStyle = defaultView ? defaultView.getComputedStyle(elem, null) : elem.currentStyle,
			top = elem.offsetTop,
			left = elem.offsetLeft;

            while ((elem = elem.parentNode) && elem !== body && elem !== docElem) {
                if (jQuery.offset.supportsFixedPosition && prevComputedStyle.position === "fixed") {
                    break;
                }

                computedStyle = defaultView ? defaultView.getComputedStyle(elem, null) : elem.currentStyle;
                top -= elem.scrollTop;
                left -= elem.scrollLeft;

                if (elem === offsetParent) {
                    top += elem.offsetTop;
                    left += elem.offsetLeft;

                    if (jQuery.offset.doesNotAddBorder && !(jQuery.offset.doesAddBorderForTableAndCells && rtable.test(elem.nodeName))) {
                        top += parseFloat(computedStyle.borderTopWidth) || 0;
                        left += parseFloat(computedStyle.borderLeftWidth) || 0;
                    }

                    prevOffsetParent = offsetParent;
                    offsetParent = elem.offsetParent;
                }

                if (jQuery.offset.subtractsBorderForOverflowNotVisible && computedStyle.overflow !== "visible") {
                    top += parseFloat(computedStyle.borderTopWidth) || 0;
                    left += parseFloat(computedStyle.borderLeftWidth) || 0;
                }

                prevComputedStyle = computedStyle;
            }

            if (prevComputedStyle.position === "relative" || prevComputedStyle.position === "static") {
                top += body.offsetTop;
                left += body.offsetLeft;
            }

            if (jQuery.offset.supportsFixedPosition && prevComputedStyle.position === "fixed") {
                top += Math.max(docElem.scrollTop, body.scrollTop);
                left += Math.max(docElem.scrollLeft, body.scrollLeft);
            }

            return { top: top, left: left };
        };
    }

    jQuery.offset = {
        initialize: function () {
            var body = document.body, container = document.createElement("div"), innerDiv, checkDiv, table, td, bodyMarginTop = parseFloat(jQuery.css(body, "marginTop")) || 0,
			html = "<div style='position:absolute;top:0;left:0;margin:0;border:5px solid #000;padding:0;width:1px;height:1px;'><div></div></div><table style='position:absolute;top:0;left:0;margin:0;border:5px solid #000;padding:0;width:1px;height:1px;' cellpadding='0' cellspacing='0'><tr><td></td></tr></table>";

            jQuery.extend(container.style, { position: "absolute", top: 0, left: 0, margin: 0, border: 0, width: "1px", height: "1px", visibility: "hidden" });

            container.innerHTML = html;
            body.insertBefore(container, body.firstChild);
            innerDiv = container.firstChild;
            checkDiv = innerDiv.firstChild;
            td = innerDiv.nextSibling.firstChild.firstChild;

            this.doesNotAddBorder = (checkDiv.offsetTop !== 5);
            this.doesAddBorderForTableAndCells = (td.offsetTop === 5);

            checkDiv.style.position = "fixed";
            checkDiv.style.top = "20px";

            // safari subtracts parent border width here which is 5px
            this.supportsFixedPosition = (checkDiv.offsetTop === 20 || checkDiv.offsetTop === 15);
            checkDiv.style.position = checkDiv.style.top = "";

            innerDiv.style.overflow = "hidden";
            innerDiv.style.position = "relative";

            this.subtractsBorderForOverflowNotVisible = (checkDiv.offsetTop === -5);

            this.doesNotIncludeMarginInBodyOffset = (body.offsetTop !== bodyMarginTop);

            body.removeChild(container);
            body = container = innerDiv = checkDiv = table = td = null;
            jQuery.offset.initialize = jQuery.noop;
        },

        bodyOffset: function (body) {
            var top = body.offsetTop,
			left = body.offsetLeft;

            jQuery.offset.initialize();

            if (jQuery.offset.doesNotIncludeMarginInBodyOffset) {
                top += parseFloat(jQuery.css(body, "marginTop")) || 0;
                left += parseFloat(jQuery.css(body, "marginLeft")) || 0;
            }

            return { top: top, left: left };
        },

        setOffset: function (elem, options, i) {
            var position = jQuery.css(elem, "position");

            // set position first, in-case top/left are set even on static elem
            if (position === "static") {
                elem.style.position = "relative";
            }

            var curElem = jQuery(elem),
			curOffset = curElem.offset(),
			curCSSTop = jQuery.css(elem, "top"),
			curCSSLeft = jQuery.css(elem, "left"),
			calculatePosition = (position === "absolute" && jQuery.inArray('auto', [curCSSTop, curCSSLeft]) > -1),
			props = {}, curPosition = {}, curTop, curLeft;

            // need to be able to calculate position if either top or left is auto and position is absolute
            if (calculatePosition) {
                curPosition = curElem.position();
            }

            curTop = calculatePosition ? curPosition.top : parseInt(curCSSTop, 10) || 0;
            curLeft = calculatePosition ? curPosition.left : parseInt(curCSSLeft, 10) || 0;

            if (jQuery.isFunction(options)) {
                options = options.call(elem, i, curOffset);
            }

            if (options.top != null) {
                props.top = (options.top - curOffset.top) + curTop;
            }
            if (options.left != null) {
                props.left = (options.left - curOffset.left) + curLeft;
            }

            if ("using" in options) {
                options.using.call(elem, props);
            } else {
                curElem.css(props);
            }
        }
    };


    jQuery.fn.extend({
        position: function () {
            ///	<summary>
            ///     &#10;Gets the top and left positions of an element relative to its offset parent.
            ///	</summary>
            ///	<returns type="Object">An object with two integer properties, 'top' and 'left'.</returns>

            if (!this[0]) {
                return null;
            }

            var elem = this[0],

            // Get *real* offsetParent
		offsetParent = this.offsetParent(),

            // Get correct offsets
		offset = this.offset(),
		parentOffset = rroot.test(offsetParent[0].nodeName) ? { top: 0, left: 0} : offsetParent.offset();

            // Subtract element margins
            // note: when an element has margin: auto the offsetLeft and marginLeft
            // are the same in Safari causing offset.left to incorrectly be 0
            offset.top -= parseFloat(jQuery.css(elem, "marginTop")) || 0;
            offset.left -= parseFloat(jQuery.css(elem, "marginLeft")) || 0;

            // Add offsetParent borders
            parentOffset.top += parseFloat(jQuery.css(offsetParent[0], "borderTopWidth")) || 0;
            parentOffset.left += parseFloat(jQuery.css(offsetParent[0], "borderLeftWidth")) || 0;

            // Subtract the two offsets
            return {
                top: offset.top - parentOffset.top,
                left: offset.left - parentOffset.left
            };
        },

        offsetParent: function () {
            ///	<summary>
            ///     &#10;This method is internal.
            ///	</summary>
            ///	<private />

            return this.map(function () {
                var offsetParent = this.offsetParent || document.body;
                while (offsetParent && (!rroot.test(offsetParent.nodeName) && jQuery.css(offsetParent, "position") === "static")) {
                    offsetParent = offsetParent.offsetParent;
                }
                return offsetParent;
            });
        }
    });


    // Create scrollLeft and scrollTop methods
    jQuery.each(["Left", "Top"], function (i, name) {
        var method = "scroll" + name;

        jQuery.fn[method] = function (val) {
            ///	<summary>
            ///     &#10;Gets and optionally sets the scroll left offset of the first matched element.
            ///	</summary>
            ///	<param name="val" type="Number" integer="true" optional="true">A positive number representing the desired scroll left offset.</param>
            ///	<returns type="Number" integer="true">The scroll left offset of the first matched element.</returns>

            var elem = this[0], win;

            if (!elem) {
                return null;
            }

            if (val !== undefined) {
                // Set the scroll offset
                return this.each(function () {
                    win = getWindow(this);

                    if (win) {
                        win.scrollTo(
						!i ? val : jQuery(win).scrollLeft(),
						 i ? val : jQuery(win).scrollTop()
					);

                    } else {
                        this[method] = val;
                    }
                });
            } else {
                win = getWindow(elem);

                // Return the scroll offset
                return win ? ("pageXOffset" in win) ? win[i ? "pageYOffset" : "pageXOffset"] :
				jQuery.support.boxModel && win.document.documentElement[method] ||
					win.document.body[method] :
				elem[method];
            }
        };
    });

    function getWindow(elem) {
        return jQuery.isWindow(elem) ?
		elem :
		elem.nodeType === 9 ?
			elem.defaultView || elem.parentWindow :
			false;
    }




    // Create innerHeight, innerWidth, outerHeight and outerWidth methods
    jQuery.each(["Height"], function (i, name) {

        var type = name.toLowerCase();

        // innerHeight and innerWidth
        jQuery.fn["inner" + name] = function () {
            ///	<summary>
            ///     &#10;Gets the inner height of the first matched element, excluding border but including padding.
            ///	</summary>
            ///	<returns type="Number" integer="true">The outer height of the first matched element.</returns>

            return this[0] ?
			parseFloat(jQuery.css(this[0], type, "padding")) :
			null;
        };

        // outerHeight and outerWidth
        jQuery.fn["outer" + name] = function (margin) {
            ///	<summary>
            ///     &#10;Gets the outer height of the first matched element, including border and padding by default.
            ///	</summary>
            ///	<param name="margins" type="Map">A set of key/value pairs that specify the options for the method.</param>
            ///	<returns type="Number" integer="true">The outer height of the first matched element.</returns>

            return this[0] ?
			parseFloat(jQuery.css(this[0], type, margin ? "margin" : "border")) :
			null;
        };

        jQuery.fn[type] = function (size) {
            ///	<summary>
            ///     &#10;Set the CSS height of every matched element. If no explicit unit
            ///     &#10;was specified (like 'em' or '%') then &quot;px&quot; is added to the width.  If no parameter is specified, it gets
            ///     &#10;the current computed pixel height of the first matched element.
            ///     &#10;Part of CSS
            ///	</summary>
            ///	<returns type="jQuery" type="jQuery" />
            ///	<param name="cssProperty" type="String">
            ///     &#10;Set the CSS property to the specified value. Omit to get the value of the first matched element.
            ///	</param>

            // Get window width or height
            var elem = this[0];
            if (!elem) {
                return size == null ? null : this;
            }

            if (jQuery.isFunction(size)) {
                return this.each(function (i) {
                    var self = jQuery(this);
                    self[type](size.call(this, i, self[type]()));
                });
            }

            if (jQuery.isWindow(elem)) {
                // Everyone else use document.documentElement or document.body depending on Quirks vs Standards mode
                return elem.document.compatMode === "CSS1Compat" && elem.document.documentElement["client" + name] ||
				elem.document.body["client" + name];

                // Get document width or height
            } else if (elem.nodeType === 9) {
                // Either scroll[Width/Height] or offset[Width/Height], whichever is greater
                return Math.max(
				elem.documentElement["client" + name],
				elem.body["scroll" + name], elem.documentElement["scroll" + name],
				elem.body["offset" + name], elem.documentElement["offset" + name]
			);

                // Get or set width or height on the element
            } else if (size === undefined) {
                var orig = jQuery.css(elem, type),
				ret = parseFloat(orig);

                return jQuery.isNaN(ret) ? orig : ret;

                // Set the width or height on the element (default to pixels if value is unitless)
            } else {
                return this.css(type, typeof size === "string" ? size : size + "px");
            }
        };

    });

    // Create innerHeight, innerWidth, outerHeight and outerWidth methods
    jQuery.each(["Width"], function (i, name) {

        var type = name.toLowerCase();

        // innerHeight and innerWidth
        jQuery.fn["inner" + name] = function () {
            ///	<summary>
            ///     &#10;Gets the inner width of the first matched element, excluding border but including padding.
            ///	</summary>
            ///	<returns type="Number" integer="true">The outer width of the first matched element.</returns>

            return this[0] ?
			parseFloat(jQuery.css(this[0], type, "padding")) :
			null;
        };

        // outerHeight and outerWidth
        jQuery.fn["outer" + name] = function (margin) {
            ///	<summary>
            ///     &#10;Gets the outer width of the first matched element, including border and padding by default.
            ///	</summary>
            ///	<param name="margin" type="Map">A set of key/value pairs that specify the options for the method.</param>
            ///	<returns type="Number" integer="true">The outer width of the first matched element.</returns>

            return this[0] ?
			parseFloat(jQuery.css(this[0], type, margin ? "margin" : "border")) :
			null;
        };

        jQuery.fn[type] = function (size) {
            ///	<summary>
            ///     &#10;Set the CSS width of every matched element. If no explicit unit
            ///     &#10;was specified (like 'em' or '%') then &quot;px&quot; is added to the width.  If no parameter is specified, it gets
            ///     &#10;the current computed pixel width of the first matched element.
            ///     &#10;Part of CSS
            ///	</summary>
            ///	<returns type="jQuery" type="jQuery" />
            ///	<param name="cssProperty" type="String">
            ///     &#10;Set the CSS property to the specified value. Omit to get the value of the first matched element.
            ///	</param>

            // Get window width or height
            var elem = this[0];
            if (!elem) {
                return size == null ? null : this;
            }

            if (jQuery.isFunction(size)) {
                return this.each(function (i) {
                    var self = jQuery(this);
                    self[type](size.call(this, i, self[type]()));
                });
            }

            if (jQuery.isWindow(elem)) {
                // Everyone else use document.documentElement or document.body depending on Quirks vs Standards mode
                return elem.document.compatMode === "CSS1Compat" && elem.document.documentElement["client" + name] ||
				elem.document.body["client" + name];

                // Get document width or height
            } else if (elem.nodeType === 9) {
                // Either scroll[Width/Height] or offset[Width/Height], whichever is greater
                return Math.max(
				elem.documentElement["client" + name],
				elem.body["scroll" + name], elem.documentElement["scroll" + name],
				elem.body["offset" + name], elem.documentElement["offset" + name]
			);

                // Get or set width or height on the element
            } else if (size === undefined) {
                var orig = jQuery.css(elem, type),
				ret = parseFloat(orig);

                return jQuery.isNaN(ret) ? orig : ret;

                // Set the width or height on the element (default to pixels if value is unitless)
            } else {
                return this.css(type, typeof size === "string" ? size : size + "px");
            }
        };

    });


})(window);
